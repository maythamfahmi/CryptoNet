# Define file paths
$indexFilePath = "index.md"
$readmeFilePath = "README.md"

# Check if README.md exists
if (-Not (Test-Path $readmeFilePath)) {
    Write-Host "README.md not found. Please ensure it exists in the current directory." -ForegroundColor Red
    exit
}

# Content to add at the beginning of index.md
$frontMatter = @"
---
_layout: landing
---

"@

# Write the front matter to index.md
Set-Content -Path $indexFilePath -Value $frontMatter

# Append the content of README.md to index.md
Get-Content -Path $readmeFilePath | Add-Content -Path $indexFilePath

Write-Host "index.md has been successfully created with the content of README.md appended." -ForegroundColor Green
