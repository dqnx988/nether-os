$VerbosePreference = "SilentlyContinue"

# Setting variables
$apps = @("calculator", "clocks", "explorer", "music-player", "photo-viewer", "server", "terminal", "ui")
$runtimes = @("win-x64", "win-x86", "win-arm64")

# Compilation (Build)
foreach ($app in $apps) {
    foreach ($runtime in $runtimes) {
        dotnet publish .\source-codes\$app -c Release -r $runtime --self-contained false -p:PublishSingleFile=false
    }
}

# Compressing full folder 
foreach ($app in $apps) {
    $sourcePathFullDir = @(".\source-codes\$app\bin", ".\source-codes\$app\obj")
    $zipPathFullDir = ".\downloads\nether-os-$app-full-folder.zip"
    Compress-Archive -Path $sourcePathFullDir -DestinationPath $zipPathFullDir -Force
}

# Compressing portable version
foreach ($app in $apps) {
    foreach ($runtime in $runtimes) {
        Compress-Archive -Path ".\source-codes\$app\bin\Release\net10.0-windows\$runtime\publish" -DestinationPath ".\downloads\nether-os-$app-$runtime-portable.zip" -Force
    }
}

# Cleaning source-codes dirs from obj and bin folders
foreach ($app in $apps) {
    Remove-Item .\source-codes\$app\bin -Recurse -Force
    Remove-Item .\source-codes\$app\obj -Recurse -Force
}