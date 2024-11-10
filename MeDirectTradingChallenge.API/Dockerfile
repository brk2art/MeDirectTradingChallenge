# Use the .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the API project files, including appsettings
COPY MeDirectTradingChallenge.API/MeDirectTradingChallenge.API.csproj MeDirectTradingChallenge.API/
COPY MeDirectTradingChallenge.API/appsettings.json MeDirectTradingChallenge.API/
COPY MeDirectTradingChallenge.API/appsettings.Development.json MeDirectTradingChallenge.API/

# Copy all source files for the API project and its dependencies
COPY MeDirectTradingChallenge.API/ MeDirectTradingChallenge.API/
COPY MeDirectTradingChallenge.Domain/ MeDirectTradingChallenge.Domain/
COPY MeDirectTradingChallenge.Infrastructure/ MeDirectTradingChallenge.Infrastructure/

# Restore and build the API project
RUN dotnet restore "MeDirectTradingChallenge.API/MeDirectTradingChallenge.API.csproj"
RUN dotnet build "MeDirectTradingChallenge.API/MeDirectTradingChallenge.API.csproj" -c Release -o /app/build

# Publish the API project
RUN dotnet publish "MeDirectTradingChallenge.API/MeDirectTradingChallenge.API.csproj" -c Release -o /app/publish

# Use the runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose API port
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "MeDirectTradingChallenge.API.dll", "&&", "dotnet", "ef", "database", "update"]
