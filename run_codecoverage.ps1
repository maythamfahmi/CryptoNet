dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"./CryptoNet.UnitTests/TestResults/*/coverage.cobertura.xml" -targetdir:"./CoverageReport" -reporttypes:"Html;Badges"

$Url = "$(Get-Location)\CoverageReport\index.html"
$ProcId = (Get-Process -Name '*edge*' | Where-Object MainWindowTitle -like "*Summary - Coverage Report*").Id

if ($ProcId) {
    Stop-Process -Id $ProcId
}

Start-Process msedge $($Url)
