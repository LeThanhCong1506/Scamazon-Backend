using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MV.PresentationLayer.Middlewares;

/// <summary>
/// Attribute để kiểm tra user có phải admin không
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireAdminAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        // Kiểm tra user đã được authenticate chưa
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                success = false,
                message = "Token required"
            });
            return;
        }

        // Kiểm tra role = admin
        var roleClaim = user.FindFirst("role");
        if (roleClaim == null || roleClaim.Value != "admin")
        {
            context.Result = new ObjectResult(new
            {
                success = false,
                message = "Admin access required"
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            return;
        }
    }
}
