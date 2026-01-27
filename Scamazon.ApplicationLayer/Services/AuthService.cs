using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MV.ApplicationLayer.Interfaces;
using MV.DomainLayer.DTO.RequestModels;
using MV.DomainLayer.DTO.ResponseModels;
using MV.DomainLayer.Entities;
using MV.InfrastructureLayer.Interfaces;

namespace MV.ApplicationLayer.Services;

/// <summary>
/// Service xử lý Authentication
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    /// <summary>
    /// Đăng ký tài khoản mới
    /// </summary>
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var errors = new List<ValidationErrorDto>();

        // 1. Kiểm tra username đã tồn tại chưa
        if (await _userRepository.UsernameExistsAsync(request.Username))
        {
            errors.Add(new ValidationErrorDto
            {
                Field = "username",
                Message = "Username đã được sử dụng"
            });
        }

        // 2. Kiểm tra email đã tồn tại chưa (nếu có nhập email)
        if (!string.IsNullOrEmpty(request.Email) && await _userRepository.EmailExistsAsync(request.Email))
        {
            errors.Add(new ValidationErrorDto
            {
                Field = "email",
                Message = "Email đã được sử dụng"
            });
        }

        // Nếu có lỗi validation, trả về error response
        if (errors.Count > 0)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = errors[0].Field == "username" ? "Username đã tồn tại" : "Email đã tồn tại",
                Errors = errors
            };
        }

        // 3. Hash password bằng bcrypt (cost factor = 10)
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 10);

        // 4. Tạo user entity
        var user = new User
        {
            Username = request.Username,
            PasswordHash = passwordHash,
            Email = request.Email,
            Phone = request.Phone,
            FullName = request.FullName,
            Address = request.Address,
            City = request.City,
            District = request.District,
            Ward = request.Ward
        };

        // 5. INSERT user mới vào table users
        var createdUser = await _userRepository.CreateUserAsync(user);

        // 6. Generate JWT token (payload: user_id, username, exp: 7 ngày)
        var token = GenerateJwtToken(createdUser);

        // 7. Return user info + token
        return new AuthResponseDto
        {
            Success = true,
            Message = "Đăng ký thành công",
            Data = new AuthDataDto
            {
                User = new UserResponseDto
                {
                    Id = createdUser.Id,
                    Username = createdUser.Username,
                    Email = createdUser.Email,
                    Phone = createdUser.Phone,
                    FullName = createdUser.FullName,
                    Role = createdUser.Role,
                    AvatarUrl = createdUser.AvatarUrl,
                    CreatedAt = createdUser.CreatedAt
                },
                Token = token
            }
        };
    }

    /// <summary>
    /// Đăng nhập
    /// </summary>
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress)
    {
        // 1. Tìm user theo username
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        if (user == null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Username hoặc mật khẩu không đúng"
            };
        }

        // 2. Verify password với bcrypt
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Username hoặc mật khẩu không đúng"
            };
        }

        // 3. Check is_active = true
        if (user.IsActive != true)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Tài khoản đã bị vô hiệu hóa"
            };
        }

        // 4. Generate JWT (payload có role)
        var token = GenerateJwtToken(user);

        // 5. Nếu role = admin → log vào admin_activity_logs
        if (user.Role == "admin")
        {
            var activityLog = new AdminActivityLog
            {
                AdminId = user.Id,
                Action = "LOGIN",
                EntityType = "user",
                EntityId = user.Id,
                IpAddress = ipAddress
            };
            await _userRepository.AddAdminActivityLogAsync(activityLog);
        }

        // 6. Return user (có role) + token
        return new AuthResponseDto
        {
            Success = true,
            Message = "Đăng nhập thành công",
            Data = new AuthDataDto
            {
                User = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Phone = user.Phone,
                    FullName = user.FullName,
                    Role = user.Role,
                    AvatarUrl = user.AvatarUrl,
                    CreatedAt = user.CreatedAt
                },
                Token = token
            }
        };
    }

    /// <summary>
    /// Lưu FCM token
    /// </summary>
    public async Task<BaseResponseDto> SaveFcmTokenAsync(int userId, FcmTokenRequestDto request)
    {
        await _userRepository.SaveDeviceTokenAsync(userId, request.Token, request.DeviceType);

        return new BaseResponseDto
        {
            Success = true,
            Message = "Lưu token thành công"
        };
    }

    /// <summary>
    /// Đăng xuất
    /// </summary>
    public async Task<BaseResponseDto> LogoutAsync(int userId, LogoutRequestDto? request)
    {
        if (!string.IsNullOrEmpty(request?.FcmToken))
        {
            await _userRepository.RemoveDeviceTokenAsync(userId, request.FcmToken);
        }

        return new BaseResponseDto
        {
            Success = true,
            Message = "Đăng xuất thành công"
        };
    }

    /// <summary>
    /// Generate JWT token
    /// </summary>
    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "ScamazonSecretKey2025VeryLongSecretKeyForJWT";
        var issuer = jwtSettings["Issuer"] ?? "Scamazon";
        var audience = jwtSettings["Audience"] ?? "ScamazonApp";
        var expirationDays = int.Parse(jwtSettings["ExpirationDays"] ?? "7");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim("user_id", user.Id.ToString()),
            new Claim("username", user.Username),
            new Claim("role", user.Role ?? "customer"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (!string.IsNullOrEmpty(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(expirationDays),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
