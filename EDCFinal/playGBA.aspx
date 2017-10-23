<%@ Page Title="Playing" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="playGBA.aspx.cs" Inherits="EDCFinal.playGBA" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
        <script src="/Scripts/IodineGBA/includes/TypedArrayShim.js"></script>
        <script src="/Scripts/IodineGBA/core/Cartridge.js"></script>
        <script src="/Scripts/IodineGBA/core/DMA.js"></script>
        <script src="/Scripts/IodineGBA/core/Emulator.js"></script>
        <script src="/Scripts/IodineGBA/core/Graphics.js"></script>
        <script src="/Scripts/IodineGBA/core/RunLoop.js"></script>
        <script src="/Scripts/IodineGBA/core/Memory.js"></script>
        <script src="/Scripts/IodineGBA/core/IRQ.js"></script>
        <script src="/Scripts/IodineGBA/core/JoyPad.js"></script>
        <script src="/Scripts/IodineGBA/core/Serial.js"></script>
        <script src="/Scripts/IodineGBA/core/Sound.js"></script>
        <script src="/Scripts/IodineGBA/core/Timer.js"></script>
        <script src="/Scripts/IodineGBA/core/Wait.js"></script>
        <script src="/Scripts/IodineGBA/core/CPU.js"></script>
        <script src="/Scripts/IodineGBA/core/Saves.js"></script>
        <script src="/Scripts/IodineGBA/core/sound/FIFO.js"></script>
        <script src="/Scripts/IodineGBA/core/sound/Channel1.js"></script>
        <script src="/Scripts/IodineGBA/core/sound/Channel2.js"></script>
        <script src="/Scripts/IodineGBA/core/sound/Channel3.js"></script>
        <script src="/Scripts/IodineGBA/core/sound/Channel4.js"></script>
        <script src="/Scripts/IodineGBA/core/CPU/ARM.js"></script>
        <script src="/Scripts/IodineGBA/core/CPU/THUMB.js"></script>
        <script src="/Scripts/IodineGBA/core/CPU/CPSR.js"></script>
        <script src="/Scripts/IodineGBA/core/graphics/Renderer.js"></script>
        <script src="/Scripts/IodineGBA/core/graphics/RendererShim.js"></script>
        <script src="/Scripts/IodineGBA/core/graphics/RendererProxy.js"></script>
        <script src="/Scripts/IodineGBA/core/graphics/BGTEXT.js"></script>
        <script src="/Scripts/IodineGBA/core/graphics/BG2FrameBuffer.js"></script>
        <script src="/Scripts/IodineGBA/core/graphics/BGMatrix.js"></script>
        <script src="/Scripts/IodineGBA/core/graphics/AffineBG.js"></script>
        <script src="/Scripts/IodineGBA/core/graphics/ColorEffects.js"></script>
        <script src="/Scripts/IodineGBA/core/graphics/Mosaic.js"></script>
        <script src="/Scripts/IodineGBA/core/graphics/OBJ.js"></script>
        <script src="/Scripts/IodineGBA/core/graphics/OBJWindow.js"></script>
        <script src="/Scripts/IodineGBA/core/graphics/Window.js"></script>
        <script src="/Scripts/IodineGBA/core/graphics/Compositor.js"></script>
        <script src="/Scripts/IodineGBA/core/memory/DMA0.js"></script>
        <script src="/Scripts/IodineGBA/core/memory/DMA1.js"></script>
        <script src="/Scripts/IodineGBA/core/memory/DMA2.js"></script>
        <script src="/Scripts/IodineGBA/core/memory/DMA3.js"></script>
        <script src="/Scripts/IodineGBA/core/cartridge/SaveDeterminer.js"></script>
        <script src="/Scripts/IodineGBA/core/cartridge/SRAM.js"></script>
        <script src="/Scripts/IodineGBA/core/cartridge/FLASH.js"></script>
        <script src="/Scripts/IodineGBA/core/cartridge/EEPROM.js"></script>
        <script src="/Scripts/IodineGBA/core/cartridge/GPIO.js"></script>
        <!--Add your webpage scripts below-->
        <script src="/Scripts/user_scripts/AudioGlueCode.js"></script>
        <script src="/Scripts/user_scripts/base64.js"></script>
        <script src="/Scripts/user_scripts/CoreGlueCode.js"></script>
        <script src="/Scripts/user_scripts/GfxGlueCode.js"></script>
        <script src="/Scripts/user_scripts/GUIGlueCode.js"></script>
        <script src="/Scripts/user_scripts/JoyPadGlueCode.js"></script>
        <script src="/Scripts/user_scripts/ROMLoadGlueCode.js"></script>
        <script src="/Scripts/user_scripts/SavesGlueCode.js"></script>
        <script src="/Scripts/user_scripts/WorkerGfxGlueCode.js"></script>
        <script src="/Scripts/user_scripts/WorkerGlueCode.js"></script>
        <script src="/Scripts/user_scripts/XAudioJS/swfobject.js"></script>
        <script src="/Scripts/user_scripts/XAudioJS/resampler.js"></script>
        <script src="/Scripts/user_scripts/XAudioJS/XAudioServer.js"></script>
        <script src="/Scripts/user_scripts/VoiceController.js"></script>
        
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    
    <br />
    <h3><i class="fa fa-gamepad"></i>&nbsp;&nbsp;Playing GameBoy Advance</h3>
    <hr />

     <script type="text/javascript">
    function loadBIOS() {
        var binaryHandle = new FileReader();
        binaryHandle.onload = function () {
            if (this.readyState == 2) {
                try {
                    attachBIOS(this.result);
                }
                catch (error) {
                }
            }
            else {
            }
        }
        var xhr = new XMLHttpRequest();
        xhr.open("GET", "/ROMS/gba_bios.bin");  // Descarregar a BIOS da consola
        xhr.responseType = "blob";

        xhr.onload = function () {
            binaryHandle.readAsArrayBuffer(xhr.response);
        };
        xhr.send();
    }

    function loadROM(romname) {
        var binaryHandle = new FileReader();
        binaryHandle.onload = function () {
            if (this.readyState == 2) {
                try {
                    attachROM(this.result);
                }
                catch (error) {
                }
            }
            else {
            }
        }
        var xhr = new XMLHttpRequest();
        xhr.open("GET", "/ROMS/" + romname + ".GBA");  // Descarregar o ROM selecionado
        xhr.responseType = "blob";

        xhr.onload = function () {
            binaryHandle.readAsArrayBuffer(xhr.response);
        };
        xhr.send();
    }

    function wait(romname) { // O processo de boot do emulador de GBA deve ser sequencial.
        if (iAmReady) {
            loadBIOS();         // Fase 1 - Carregar a BIOS
            setTimeout(function () { wait2(romname); }, 300); 
        } else {
            setTimeout(function () { wait(romname); }, 300 );
        }
    };

    function wait2(romname) {
        loadROM(romname);       // Fase 2 - Carregar o ROM
        setTimeout(function () { wait3(); }, 300);
    };

    function wait3() {
        IodineGUI.Iodine.play();  // Fase 3 - Iniciar o ROM (o jogo inicia)
    };


    </script>
           <center>
            <div id="main">
                <canvas class="canvas" id="emulator_target" width="240" height="160" style="width: 720px; height: 480px;"></canvas>
            </div><br />
        <img src="/Content/gbactr.png" style="height: 250px" />
           </center>
            <div id="container" style="display: none;"> <!--- Esconder os controlos do emulador --->
            <div id="menu" class="paused">
                <ul class="menu" id="menu_top">
                    <li>
                        File
                        <ul>
                            <li><span>BIOS: </span> <input type="file" id="bios_load" class="files"></li>
                            <li><span>Game: </span> <input type="file" id="rom_load" class="files"></li>
                        </ul>
                    </li>
                    <li id="play" class="show">Play</li>
                    <li id="pause" class="hide">Pause</li>
                    <li id="restart">Restart</li>
                    <li>
                        Settings
                        <ul>
                            <li>
                                <input type="checkbox" id="skip_boot"> Skip Boot Intro
                            </li>
                            <li>
                                <input type="checkbox" id="toggleSmoothScaling" checked="checked"> Smooth Scaling
                            </li>
                            <li>
                                <input type="checkbox" id="toggleDynamicSpeed"> Dynamic Speed
                            </li>
                            <li>
                                <input type="checkbox" id="offthread-cpu" checked="checked"> CPU off-thread
                            </li>
                            <li>
                                <input type="checkbox" id="offthread-gpu" checked="checked"> GPU off-thread
                            </li>
                            <li>
                                <input type="checkbox" id="sound"> Sound
                            </li>
                            <li>
                                GBA Bindings
                                <ul>
                                    <li id="key_a">
                                        <span>A</span>
                                    </li>
                                    <li id="key_b">
                                        <span>B</span>
                                    </li>
                                    <li id="key_l">
                                        <span>L</span>
                                    </li>
                                    <li id="key_r">
                                        <span>R</span>
                                    </li>
                                    <li id="key_start">
                                        <span>Start</span>
                                    </li>
                                    <li id="key_select">
                                        <span>Select</span>
                                    </li>
                                    <li id="key_up">
                                        <span>↑</span>
                                    </li>
                                    <li id="key_down">
                                        <span>↓</span>
                                    </li>
                                    <li id="key_left">
                                        <span>←</span>
                                    </li>
                                    <li id="key_right">
                                        <span>→</span>
                                    </li>
                                </ul>
                            </li>
                            <li>
                                Emulator Bindings
                                <ul>
                                    <li id="key_volumeup">
                                        <span>Volume Up</span>
                                    </li>
                                    <li id="key_volumedown">
                                        <span>Volume Down</span>
                                    </li>
                                    <li id="key_speedup">
                                        <span>Speed Up</span>
                                    </li>
                                    <li id="key_slowdown">
                                        <span>Slow Down</span>
                                    </li>
                                    <li id="key_speedreset">
                                        <span>Speed Reset</span>
                                    </li>
                                    <li id="key_fullscreen">
                                        <span>Fullscreen</span>
                                    </li>
                                    <li id="key_playpause">
                                        <span>Play/Pause</span>
                                    </li>
                                    <li id="key_restart">
                                        <span>Restart</span>
                                    </li>
                                </ul>
                            </li>
                        </ul>
                    </li>
                    <li>
                        Volume
                        <ul>
                            <li>
                                <input type="range" id="volume">
                            </li>
                        </ul>
                    </li>
                    <li id="saves_menu">
                        Saves
                        <ul id="saves_menu_container">
                            <li>
                                <span>Import:</span><input type="file" id="import" class="files">
                            </li>
                            <li id="existing_saves">
                                <span>Existing Saves</span>
                                <ul id="existing_saves_list">

                                </ul>
                            </li>
                            <li>
                                <a href="./" id="export" target="_new">Export All Saves</a>
                            </li>
                        </ul>
                    </li>
                    <li id="fullscreen">Fullscreen</li>
                    <li>
                        <span id="speed">Speed</span>
                        <ul>
                            <li id="speedup">
                                <span>+5%</span>
                            </li>
                            <li id="speedreset">
                                <span>100%</span>
                            </li>
                            <li id="speeddown">
                                <span>-5%</span>
                            </li>
                        </ul>
                    </li>
                </ul>
            </div>

            <div class="touch-controls">
                <div class="touch-dpad">
                    <button id="touch-up">↑</button><br>
                    <button id="touch-left">←</button>
                    <button id="touch-right">→</button><br>
                    <button id="touch-down">↓</button>
                </div>
                <div class="touch-buttons">
                    <button id="touch-select">SELECT</button> 
                    <button id="touch-start">START</button>
                </div>
                <div class="touch-buttons">
                    <button id="touch-a">A</button>
                    <button id="touch-b">B</button><br>
                    <button id="touch-l">L</button>
                    <button id="touch-r">R</button>
                </div>
            </div>
            <span class="message" id="tempMessage"></span>
        </div>

</asp:Content>
