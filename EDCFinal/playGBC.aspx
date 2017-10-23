﻿<%@ Page Title="Playing" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="playGBC.aspx.cs" Inherits="EDCFinal.playGBC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <script type="text/javascript">
    var DEBUG_MESSAGES = false;
    var DEBUG_WINDOWING = false;
    window.onload = function () {
        windowingInitialize();
       


        

    }
    function loadROM(rom) {                     // Automatizar o processo de carregamento do ROM, para o jogo iniciar assim que a página abrir
        var binaryHandle = new FileReader();
        binaryHandle.onload = function () {
            if (this.readyState == 2) {
                cout("file loaded.", 0);
                try {
                    initPlayer();
                    start(mainCanvas, this.result);
                }
                catch (error) {
                    alert(error.message + " file: " + error.fileName + " line: " + error.lineNumber);
                }
            }
            else {
                cout("loading file, please wait...", 0);
            }
        }
        var xhr = new XMLHttpRequest();
        xhr.open("GET", "/ROMS/" + rom + ".GBC");           // Executado pelo cliente para descarregar o ROM do nosso servidor
        xhr.responseType = "blob";

        xhr.onload = function () {
            binaryHandle.readAsBinaryString(xhr.response);
        };
        xhr.send();
    }
    </script>
    <script type="text/javascript" src="/Scripts/other/windowStack.js"></script>
    <script type="text/javascript" src="/Scripts/other/terminal.js"></script>
    <script type="text/javascript" src="/Scripts/other/gui.js"></script>
    <script type="text/javascript" src="/Scripts/other/base64.js"></script>
    <script type="text/javascript" src="/Scripts/other/json2.js"></script>
    <script type="text/javascript" src="/Scripts/other/swfobject.js"></script>
    <script type="text/javascript" src="/Scripts/other/resampler.js"></script>
    <script type="text/javascript" src="/Scripts/other/XAudioServer.js"></script>
    <script type="text/javascript" src="/Scripts/other/resize.js"></script>
    <script type="text/javascript" src="/Scripts/GameBoyCore.js"></script>
    <script type="text/javascript" src="/Scripts/GameBoyIO.js"></script>
    <br />
    <h3><i class="fa fa-gamepad"></i>&nbsp;&nbsp;Playing Gameboy Color</h3>
    <hr />
        <div id="GameBoy" class="window">
            <div style="display: none" class="menubar">
                <span id="GameBoy_file_menu">File</span>
                <span id="GameBoy_settings_menu">Settings</span>
                <span id="GameBoy_view_menu">View</span>
                <span id="GameBoy_about_menu">About</span>
            </div>
            <center>
                <div id="gfx">
                    <canvas id="mainCanvas" style="height: 30vw; width: 30vw;" />
                    <span id="title">GameBoy</span>
                    <span id="port_title">Online</span>
                </div>
                <br />
        <img src="/Content/gbcctr.png" style="width: 400px" />
            </center>
        </div>
        <div style="display: none">   <!--- Esconder os controlos do emulador ---->
        <div id="terminal" class="window">
            <div id="terminal_output"/>
            <div class="button_rack">
                <button id="terminal_clear_button" class="left">Clear Messages</button>
                <button id="terminal_close_button" class="right">Close Terminal</button>
            </div>
        </div>
        <div id="about" class="window">
            <div id="about_message">
                <h1>GameBoy Online</h1>
                <p>This is a GameBoy Color emulator written purely in JavaScript.</p><p>The graphics blitting is done through HTML5 canvas, with the putImageData and drawImage functions.</p><p>Save states are implemented through the window.localStorage object, and are serialized/deserialized through JSON. SRAM saving is also implemented through the window.localStorage object, and are serialized/deserialized through JSON. In order for save states to work properly on most browsers, you need set the maximum size limit for DOM storage higher, to meet the needs of the emulator's save data size.</p><p>For more information about this emulator and its source code, visit the GIT repository at: <a href="https://github.com/taisel/GameBoy-Online" target="_blank">https://github.com/taisel/GameBoy-Online</a>.
                </p>
            </div>
            <div class="button_rack">
                <button id="about_close_button" class="center">Close Popup</button>
            </div>
        </div>
        <div class="window" id="settings">
            <div id="toggle_settings">
                <div class="setting">
                    <span>Enable Sound:</span>
                    <input type="checkbox" checked="checked" id="enable_sound"/>
                </div>
                <div class="setting">
                    <span>GB mode has priority over GBC mode:</span>
                    <input type="checkbox" id="disable_colors"/>
                </div>
                <div class="setting">
                    <span>Use the BIOS ROM:</span>
                    <input type="checkbox" checked="checked" id="enable_gbc_bios"/>
                </div>
                <div class="setting">
                    <span>Override ROM only cartridge typing to MBC1:</span>
                    <input type="checkbox" checked="checked" id="rom_only_override"/>
                </div>
                <div class="setting">
                    <span>Always allow reading and writing to the MBC banks:</span>
                    <input type="checkbox" checked="checked" id="mbc_enable_override"/>
                </div>
                <div class="setting">
                    <span>Colorize Classic GameBoy Palettes:</span>
                    <input type="checkbox" checked="checked" id="enable_colorization"/>
                </div>
                <div class="setting">
                    <span>Minimal view on fullscreen:</span>
                    <input type="checkbox" checked="checked" id="do_minimal"/>
                </div>
                <div class="setting">
                    <span>Resize canvas directly in JavaScript:</span>
                    <input type="checkbox" id="software_resizing"/>
                </div>
                <div class="setting">
                    <span>Disallow typed arrays to be used:</span>
                    <input type="checkbox" id="typed_arrays_disallow"/>
                </div>
                <div class="setting">
                    <span>Use the DMG boot ROM instead of CGB:</span>
                    <input type="checkbox" id="gb_boot_rom_utilized"/>
                </div>
                <div class="setting">
                    <span>Smooth upon resizing canvas:</span>
                    <input type="checkbox" id="resize_smoothing"/>
                </div>
                <div class="setting">
                    <span>Enable Channel 1 Audio:</span>
                    <input type="checkbox" checked="checked" id="channel1"/>
                </div>
                <div class="setting">
                    <span>Enable Channel 2 Audio:</span>
                    <input type="checkbox" checked="checked" id="channel2"/>
                </div>
                <div class="setting">
                    <span>Enable Channel 3 Audio:</span>
                    <input type="checkbox" checked="checked" id="channel3"/>
                </div>
                <div class="setting">
                    <span>Enable Channel 4 Audio:</span>
                    <input type="checkbox" checked="checked" id="channel4"/>
                </div>
            </div>
            <div class="button_rack">
                <button id="settings_close_button" class="center">Close Settings</button>
            </div>
        </div>
        <div id="instructions" class="window">
            <div id="keycodes">
                <h1>Keyboard Controls:</h1>
                <ul>
                    <li>X/J are A.</li>
                    <li>Z/Y/Q are B.</li>
                    <li>Shift is Select.</li>
                    <li>Enter is Start.</li>
                    <li>The d-pad is the control pad.</li>
                    <li>The escape key (esc) allows you to get in and out of fullscreen mode.</li>
                </ul>
            </div>
            <div class="button_rack">
                <button id="instructions_close_button" class="center">Close Instructions</button>
            </div>
        </div>
        <div id="input_select" class="window">
            <form>
                <input type="file" id="local_file_open"/>
            </form>
            <div class="button_rack">
                <button id="input_select_close_button" class="center">Close File Input</button>
            </div>
        </div>
        <div id="save_importer" class="window">
            <form>
                <input type="file" id="save_open"/>
            </form>
            <div class="button_rack">
                <button id="save_importer_close_button" class="center">Close Save Importer</button>
            </div>
        </div>
        <div class="window" id="local_storage_listing">
            <div id="storageListingMasterContainer" class="storageList">
                <div id="storageListingMasterContainerSub"/>
            </div>
            <div id="download_all_storage">
                <a href="about:blank" id="download_local_storage_dba">Export all saved data.</a>
            </div>
            <div class="button_rack">
                <button id="local_storage_list_refresh_button" class="left">Refresh List</button>
                <button id="local_storage_list_close_button" class="right">Close Storage List</button>
            </div>
        </div>
        <div class="window" id="local_storage_popup">
            <div id="storagePopupMasterParent" class="storageList">
                <div id="storagePopupMasterContainer"/>
            </div>
            <div class="button_rack">
                <button id="local_storage_popup_close_button" class="center">Close Storage Popup</button>
            </div>
        </div>
        <div class="window" id="freeze_listing">
            <div id="freezeListingMasterContainer" class="storageList">
                <div id="freezeListingMasterContainerSub"/>
            </div>
            <div class="button_rack">
                <button id="freeze_list_refresh_button" class="left">Refresh List</button>
                <button id="freeze_list_close_button" class="right">Close Freeze State List</button>
            </div>
        </div>
        <ul class="menu" id="GameBoy_file_popup">
            <li>Open As<ul class="menu">
                <li id="data_uri_clicker">Base 64 Encoding</li>
                <li id="internal_file_clicker">Local File</li>
            </ul></li>
            <li id="save_SRAM_state_clicker">Save Game Memory</li>
            <li id="save_state_clicker">Save Freeze State</li>
            <li id="set_volume">Set Volume</li>
            <li id="set_speed">Set Speed</li>
            <li id="restart_cpu_clicker">Restart</li>
            <li id="run_cpu_clicker">Resume</li>
            <li id="kill_cpu_clicker">Pause</li>
        </ul>
        <ul class="menu" id="GameBoy_view_popup">
            <li id="view_terminal">Terminal</li>
            <li id="view_instructions">Instructions</li>
            <li id="view_importer">Save Importer</li>
            <li id="local_storage_list_menu">Save Manager</li>
            <li id="freeze_list_menu">Freeze State Manager</li>
            <li id="view_fullscreen">Fullscreen Mode</li>
        </ul>
        <div id="fullscreenContainer">
            <canvas id="fullscreen" class="maximum"/>
        </div>
        </div>
</asp:Content>
