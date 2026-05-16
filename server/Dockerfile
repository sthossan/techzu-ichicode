# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy files relative to the root context (.)
COPY ["server/Server.sln", "server/"]
COPY ["server/API/Server.API.csproj", "server/API/"]
COPY ["server/Application/Server.Application.csproj", "server/Application/"]
COPY ["server/Domain/Server.Domain.csproj", "server/Domain/"]
COPY ["server/Infrastructure/Server.Infrastructure.csproj", "server/Infrastructure/"]
COPY ["server/Shared/Shared.csproj", "server/Shared/"]

RUN dotnet restore "server/Server.sln"

# Copy everything and build
COPY . .
WORKDIR "/src/server/API"
RUN dotnet publish "Server.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render dynamic port
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "Server.API.dll"]
