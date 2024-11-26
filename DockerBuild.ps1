$StartDocker = $true

## Fire Docker
while ($True) {
    $currentStatus = $(docker ps)
    if ([string]::IsNullOrEmpty($currentStatus)) {
        if ($StartDocker) {
            Write-Output("Start docker")
            Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe"
            $StartDocker = $false
        }
        Write-Output "Docker is starting..."
        # Wait for a specified period before checking again
        Start-Sleep -Seconds 10
    }
    else {
        Write-Output "Docker is started."
        break
    }
}

docker build . --file .\Dockerfile --tag cryptonet-service:latest