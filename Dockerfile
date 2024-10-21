FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
COPY . .

RUN apt-get update && apt-get install libxml2

# RUN dotnet build
RUN dotnet restore
# Unit ans Integration Tests
RUN dotnet run --project ./Examples/AESExample/AESExample.csproj
RUN dotnet run --project ./Examples/RSAExample/RSAExample.csproj
# Unit ans Integration Tests
RUN dotnet test --verbosity normal