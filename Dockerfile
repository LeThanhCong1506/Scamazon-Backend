# ============================================================
# Stage 1: Base runtime image
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# ============================================================
# Stage 2: Build
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files trước để tận dụng Docker layer cache
COPY ["Scamazon.PresentationLayer/MV.PresentationLayer.csproj", "Scamazon.PresentationLayer/"]
COPY ["Scamazon.ApplicationLayer/MV.ApplicationLayer.csproj", "Scamazon.ApplicationLayer/"]
COPY ["Scamazon.InfrastructureLayer/MV.InfrastructureLayer.csproj", "Scamazon.InfrastructureLayer/"]
COPY ["Scamazon.DomainLayer/MV.DomainLayer.csproj", "Scamazon.DomainLayer/"]

# Restore dependencies
RUN dotnet restore "Scamazon.PresentationLayer/MV.PresentationLayer.csproj"

# Copy toàn bộ source code
COPY . .

# Build
WORKDIR "/src/Scamazon.PresentationLayer"
RUN dotnet build "MV.PresentationLayer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ============================================================
# Stage 3: Publish
# ============================================================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MV.PresentationLayer.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ============================================================
# Stage 4: Final runtime image
# ============================================================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Render inject PORT env variable; ASP.NET Core đọc từ ASPNETCORE_URLS
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "MV.PresentationLayer.dll"]
