[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, HelpMessage = "Provide the version number in the format 'X.Y.Z', e.g., '3.0.0'.")]
    [string]$VersionNumber,
    [Parameter(Mandatory = $true)]
    [bool]$IsPreview
)

# Validate the input format
if ($VersionNumber -notmatch '^\d+\.\d+\.\d+$') {
    Write-Host "Error: Version number must be in the format 'X.Y.Z', e.g., '3.0.0'." -ForegroundColor Red
    exit 1
}

# Generate PreviewVersion dynamically
$Timestamp = (Get-Date -Format "ddMMyy")
$Time = (Get-Date -Format "HHmm")
$PreviewVersion = "$Timestamp.$Time"

# Construct the tag and message dynamically
if ($IsPreview) {
    $TagName = "v$VersionNumber-preview$PreviewVersion"
} else {
    $TagName = "v$VersionNumber"
}
$Message = "Release version $TagName"

# Display confirmation prompt
Write-Host "You are about to release:" -ForegroundColor Yellow
Write-Host "Tag: $TagName" -ForegroundColor Cyan
Write-Host "Message: $Message" -ForegroundColor Cyan
$response = Read-Host "Are you sure you want to proceed? (yes/no)"

if ($response -ne "yes") {
    Write-Host "Release canceled by the user." -ForegroundColor Red
    exit 0
}

# Execute the git commands
Write-Host "Creating tag $TagName with message: $Message"
git tag -a $TagName -m $Message

Write-Host "Pushing tag $TagName to origin"
git push origin $TagName

Write-Host "Tag $TagName pushed successfully." -ForegroundColor Green
