$ProductName = "NetherOS Explorer"
$InstallFolder = "$env:ProgramFiles\NetherOS\$ProductName"
Remove-Item -Path $InstallFolder -Recurse -Force -ErrorAction SilentlyContinue
New-Item -Path $InstallFolder -ItemType Directory -Force | Out-Null
Copy-Item -Path "publish\*" -Destination $InstallFolder -Recurse -Force -ErrorAction SilentlyContinue