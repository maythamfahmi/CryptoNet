FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app
COPY . .

RUN dotnet restore
RUN dotnet test
#RUN dotnet publish -c Release -o out