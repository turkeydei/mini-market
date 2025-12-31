# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["MiniMarket.sln", "./"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Persistence/Persistence.csproj", "Persistence/"]
COPY ["WebShop/WebShop.csproj", "WebShop/"]

# Restore dependencies
RUN dotnet restore "MiniMarket.sln"

# Copy all source files
COPY . .

# Build and publish the application
WORKDIR "/src/WebShop"
RUN dotnet build "WebShop.csproj" -c Release -o /app/build
RUN dotnet publish "WebShop.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

# Entry point
ENTRYPOINT ["dotnet", "WebShop.dll"]

