Remove-Item .\api -Recurse
Remove-Item .\_site\. -Recurse
docfx metadata
docfx build .\docfx.json 

$Url = "http://localhost:8080"
$ProcId = (Get-Process -Name '*edge*' | Where-Object MainWindowTitle -like "*Summary - Coverage Report*").Id

if ($ProcId) {
    Stop-Process -Id $ProcId
}

Start-Process msedge $($Url)

docfx serve _site