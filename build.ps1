dotnet restore
dotnet build --configuration Release --no-restore --no-incremental
dotnet xx
dotnet test --configuration Release --no-build
dotnet pack --configuration Release --no-build