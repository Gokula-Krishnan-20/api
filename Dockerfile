# Build stage: use .NET 9 SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and publish
COPY . ./
RUN dotnet publish -c Release -o out

# Runtime stage: use ASP.NET Core runtime 9
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy published output
COPY --from=build /app/out .

# Expose port 5123
EXPOSE 5123

# Run your app on port 5123
ENTRYPOINT ["dotnet", "TimeAPI.dll", "--urls", "http://0.0.0.0:5123"]
