# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore as distinct layers
COPY ["server/src/API/Server.API.csproj", "server/src/API/"]
COPY ["server/src/Application/Server.Application.csproj", "server/src/Application/"]
COPY ["server/src/Domain/Server.Domain.csproj", "server/src/Domain/"]
COPY ["server/src/Infrastructure/Server.Infrastructure.csproj", "server/src/Infrastructure/"]
COPY ["server/src/Shared/Server.Shared.csproj", "server/src/Shared/"]

RUN dotnet restore "server/src/API/Server.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/server/src/API"
RUN dotnet build "Server.API.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "Server.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Render assigns a dynamic port via the PORT env var
# ASP.NET Core 8+ uses ASPNETCORE_HTTP_PORTS
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "Server.API.dll"]
