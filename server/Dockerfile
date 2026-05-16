# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj files and restore as distinct layers
COPY Server.sln ./
COPY API/Server.API.csproj ./API/
COPY Domain/Server.Domain.csproj ./Domain/
COPY Application/Server.Application.csproj ./Application/
COPY Infrastructure/Server.Infrastructure.csproj ./Infrastructure/
COPY Shared/Shared.csproj ./Shared/

RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish API/Server.API.csproj -c Release -o out

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/out .

# Render assigns a dynamic port via the PORT env var
# ASP.NET Core 8+ uses ASPNETCORE_HTTP_PORTS
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "Server.API.dll"]
