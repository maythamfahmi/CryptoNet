FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
COPY . .

RUN apt-get update && apt-get install libxml2

RUN dotnet restore
# Unit ans Integration Tests
RUN dotnet test
# Functional Tests of the Examples
RUN dotnet run --project ./CryptoNet.Cli/CryptoNet.Cli.csproj