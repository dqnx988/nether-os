const main = document.getElementById("main");
const body = document.body;

function showHome() {
    document.title = "NetherOS";

    body.innerHTML = `
        <header>
            <button onclick="showHome()">
                <img src="img/logo.svg" alt="">
            </button>
        </header>

        <main>
            <div class="box vertical">
                <button class="sub-box button" onclick="showProductPages()">Products</button>
                <br>
                <button class="sub-box button" onclick="showHelp()">Help</button>
            </div>
        </main>
    `;
}

function showProducts() {
    document.title = "Products - NetherOS";

    body.innerHTML = `
        <header>
            <button onclick="showHome()">
                <img src="img/logo.svg" alt="">
            </button>
        </header>

        <main>
            <div class="box vertical">

                <button class="sub-box button" onclick="showHome()">
                    <img src="img/go-back.svg" alt="">
                </button>
                <br>

                <button class="sub-box button" onclick="showProductPage('Calculator', 'calculator')">
                    <img src="source-codes/calculator/img/logo.svg" alt="">
                    Calculator
                </button>
                <br>

                <button class="sub-box button" onclick="showProductPage('Clocks', 'clocks')">
                    <img src="source-codes/clocks/img/logo.svg" alt="">
                    Clocks
                </button>
                <br>

                <button class="sub-box button" onclick="showProductPage('Explorer', 'explorer')">
                    <img src="source-codes/explorer/img/logo.svg" alt="">
                    Explorer
                </button>
                <br>

                <button class="sub-box button" onclick="showProductPage('Music Player', 'music-player')">
                    <img src="source-codes/music-player/img/logo.svg" alt="">
                    Music Player
                </button>
                <br>

                <!--<button class="sub-box button" onclick="showProductPage('Photo Viewer', 'photo-viewer')">
                    <img src="source-codes/photo-viewer/img/logo.svg" alt="">
                    Photo Viewer
                </button>
                <br>-->

                <button class="sub-box button" onclick="showProductPage('Server', 'server')">
                    <img src="source-codes/server/img/logo.svg" alt="">
                    Server
                </button>
                <br>

                <!--<button class="sub-box button" onclick="showProductPage('System Monitor', 'system-monitor')">
                    <img src="source-codes/system-monitor/img/logo.svg" alt="">
                    System Monitor
                </button>
                <br>-->

                <button class="sub-box button" onclick="showProductPage('Terminal', 'terminal')">
                    <img src="source-codes/terminal/img/logo.svg" alt="">
                    Terminal
                </button>
                <br>

                <button class="sub-box button" onclick="showProductPage('UI', 'ui')">
                    <img src="source-codes/ui/img/logo.svg" alt="">
                    UI
                </button>

                <!--<br>
                <button class="sub-box button" onclick="showProductPage('Video Player', 'video-player')">
                    <img src="source-codes/video-player/img/logo.svg" alt="">
                    Video Player
                </button>-->

            </div>
        </main>
    `;
}

function showProductPage(nameUpperFirstLetter, nameLowerFirstLetter) {
    document.title = `${nameUpperFirstLetter} - Products - NetherOS`;

    body.innerHTML = `
        <header>
            <button onclick="showHome()">
                <img src="img/logo.svg" alt="">
            </button>
        </header>

        <main>
            <div class="box horizontal">

                <button class="sub-box button" onclick="showProductPages()">
                    <img src="img/go-back.svg" alt="">
                </button>
                <br>

                <div class="sub-box vertical">
                    <h2>Windows 64-bit (x64)</h2>
                    <a class="button" href="downloads/nether-os-${nameLowerFirstLetter}-win-x64-portable.zip">Download Portable</a>
                    <a class="button" href="downloads/nether-os-${nameLowerFirstLetter}-win-x64-install.zip">Download Installer</a>
                </div>
                <br>

                <div class="sub-box vertical">
                    <h2>Windows 32-bit (x86)</h2>
                    <a class="button" href="downloads/nether-os-${nameLowerFirstLetter}-win-x86-portable.zip">Download Portable</a>
                    <a class="button" href="downloads/nether-os-${nameLowerFirstLetter}-win-x86-install.zip">Download Installer</a>
                </div>
                <br>

                <div class="sub-box vertical">
                    <h2>Windows ARM64 (arm-64)</h2>
                    <a class="button" href="downloads/nether-os-${nameLowerFirstLetter}-win-arm64-portable.zip">Download Portable</a>
                    <a class="button" href="downloads/nether-os-${nameLowerFirstLetter}-win-arm64-install.zip">Download Installer</a>
                </div>

            </div>
        </main>
    `;
}

function showHelp() {
    document.title = "Help - NetherOS"

    body.innerHTML = `
        <header>
            <button onclick="showHome()">
                <img src="img/logo.svg" alt="">
            </button>
        </header>
        <main>

        </main>
    `
}

function showHelpPage(nameUpperFirstLetter) {
    document.title = `${nameUpperFirstLetter}`

    body.innerHTML = `
        <header>
            <button onclick="showHome()">
                <img src="img/logo.svg" alt="">
            </button>
        </header>
        <main>

        </main>
    `
}

showHome();