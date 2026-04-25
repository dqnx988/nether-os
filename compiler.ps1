$VerbosePreference = "SilentlyContinue"

# Setting variables
$apps = @("calculator", "clocks", "explorer", "music", "photos", "server", "terminal", "ui")

$runtimes = @("win-x64", "win-x86", "win-arm64")

# Compiling (build)
foreach ($app in $apps) {
    foreach ($runtime in $runtimes) {
        dotnet.exe publish ".\source-codes\$app" -c release -r $runtime --self-contained false -p:publishsinglefile=false --output .\downloads\$app
    }
}

# Compressing portable version
foreach ($app in $apps) {
    foreach ($runtime in $runtimes) {
        Compress-Archive -Path .\downloads\$app\$runtime\publish -DestinationPath .\downloads\nether-os-$app-$runtime-portable.zip
    }
}

# Removing unnecessary folders in downloads folder
foreach ($app in $apps) {
    Remove-Item .\downloads\$app -Recurse -Force
}