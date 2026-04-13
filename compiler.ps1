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

# Compressing portable version
foreach ($app in $apps) {
    foreach ($runtime in $runtimes) {
        Compress-Archive -Path ".\source-codes\$app\bin\Release\net10.0-windows\$runtime\publish" -DestinationPath ".\downloads\nether-os-$app-$runtime-portable.zip" -Force
    }
    
}

foreach ($runtime in $runtimes) {
    Compress-Archive -Path ".\source-codes\ui\bin\Release\net10.0-windows10.0.19041.0\$runtime\publish" -DestinationPath ".\downloads\nether-os-ui-$runtime-portable.zip" -Force
}

# Cleaning source-codes dirs from obj and bin folders
foreach ($app in $apps) {
    Remove-Item .\source-codes\$app\bin -Recurse -Force
    Remove-Item .\source-codes\$app\obj -Recurse -Force
}