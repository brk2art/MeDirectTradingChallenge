# Use the .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the Console project files, including appsettings and log4net configuration
COPY MeDirectTradingChallenge.Console/MeDirectTradingChallenge.Console.csproj MeDirectTradingChallenge.Console/
COPY MeDirectTradingChallenge.Console/appsettings.json MeDirectTradingChallenge.Console/
COPY MeDirectTradingChallenge.Console/appsettings.Development.json MeDirectTradingChallenge.Console/
COPY MeDirectTradingChallenge.Console/log4net.config MeDirectTradingChallenge.Console/

# Copy all source files for the Console project and its dependencies
COPY MeDirectTradingChallenge.Console/ MeDirectTradingChallenge.Console/
COPY MeDirectTradingChallenge.Domain/ MeDirectTradingChallenge.Domain/
COPY MeDirectTradingChallenge.Infrastructure/ MeDirectTradingChallenge.Infrastructure/

# Restore and build the Console project
RUN dotnet restore "MeDirectTradingChallenge.Console/MeDirectTradingChallenge.Console.csproj"
RUN dotnet build "MeDirectTradingChallenge.Console/MeDirectTradingChallenge.Console.csproj" -c Release -o /app/build

# Publish the Console project
RUN dotnet publish "MeDirectTradingChallenge.Console/MeDirectTradingChallenge.Console.csproj" -c Release -o /app/publish

# Use the runtime image to run the app
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Ensure log4net configuration is recognized at runtime
ENV DOTNET_log4net__configurationFile=log4net.config

# Run the console application
ENTRYPOINT ["dotnet", "MeDirectTradingChallenge.Console.dll"]
