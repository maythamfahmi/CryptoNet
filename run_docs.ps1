## Once
# dotnet tool install -g docfx      // install this tool once.
# docfx init -y                     // already done and configured once.

## Each time
Remove-Item .\api -Recurse
Remove-Item .\_site\. -Recurse
docfx metadata
docfx build .\docfx.json 
docfx serve _site