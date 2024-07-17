FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
COPY . .

RUN dotnet restore
RUN dotnet test

RUN dotnet run --project ./CryptoNet.Cli/CryptoNet.Cli.csproj

RUN dotnet tool install --global dotnet-reportgenerator-globaltool
RUN dotnet test --collect:"XPlat Code Coverage"
RUN reportgenerator -reports:"./CryptoNet.UnitTests/TestResults/*/coverage.cobertura.xml" -targetdir:"./CoverageReport" -reporttypes:"Html;Badges"
