$VerbosePreference = "SilentlyContinue"

$apps = @("explorer", "terminal", "calculator", "ui", "clocks")
$runtimes = @("win-x64", "win-x86", "win-arm64")

Set-Location products

# Compiling (build)
foreach ($app in $apps) {
    foreach ($runtime in $runtimes) {
        dotnet publish $app -c Release -r $runtime --self-contained false -p:PublishSingleFile=false
    }
}


foreach ($app in $apps) {
    foreach ($runtime in $runtimes) {
        $publishPath = ".\$app\bin\Release\net8.0-windows\$runtime\publish"
        $zipPath = ".\$app\nether-os-$app-$runtime-portable.zip"
        Compress-Archive -Path $publishPath -DestinationPath $zipPath -Force
    }
}

foreach ($app in $apps) {
    foreach ($runtime in $runtimes) {
        $publishPath = ".\$app\bin\Release\net8.0-windows\$runtime\publish"
        $zipPath = ".\$app\nether-os-$app-$runtime-install.zip"
        Compress-Archive -Path @($publishPath, ".\$app\installuser.ps1", ".\$app\installsystem.ps1") -DestinationPath $zipPath -Force
    }
}

foreach ($app in $apps) {
    Remove-Item ".\$app\bin" -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item ".\$app\obj" -Recurse -Force -ErrorAction SilentlyContinue
}

Pause