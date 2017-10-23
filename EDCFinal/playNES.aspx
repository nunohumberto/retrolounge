<%@ Page Title="Playing" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="playNES.aspx.cs" Inherits="EDCFinal.playNES" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <h3><i class="fa fa-gamepad"></i>&nbsp;&nbsp;Playing NES</h3>
    <hr />
    <center><div id="emulator"></div><br />
        <img src="/Content/nesctr.png" style="width: 400px" />
    </center>
    

    <script src="/Scripts/lib/jquery-1.4.2.min.js" type="text/javascript" charset="utf-8"></script>
    <script src="/Scripts/lib/dynamicaudio-min.js" type="text/javascript" charset="utf-8"></script>
    <script src="/Scripts/source/nes.js" type="text/javascript" charset="utf-8"></script>
    <script src="/Scripts/source/utils.js" type="text/javascript" charset="utf-8"></script>
    <script src="/Scripts/source/cpu.js" type="text/javascript" charset="utf-8"></script>
    <script src="/Scripts/source/keyboard.js" type="text/javascript" charset="utf-8"></script>
    <script src="/Scripts/source/mappers.js" type="text/javascript" charset="utf-8"></script>
    <script src="/Scripts/source/papu.js" type="text/javascript" charset="utf-8"></script>
    <script src="/Scripts/source/ppu.js" type="text/javascript" charset="utf-8"></script>
    <script src="/Scripts/source/rom.js" type="text/javascript" charset="utf-8"></script>
    <script src="/Scripts/source/ui.js" type="text/javascript" charset="utf-8"></script>
    <script type="text/javascript" charset="utf-8">
        var nes;
        $(function() {
            nes = new JSNES({
                'ui': $('#emulator').JSNESUI()                  // As alterações no emulador para possibilitar a automatização do carregamento no ROM
            });                                                 // Foram feitas directamente nas bibliotecas JS do emulador de NES.
            
        });


    </script>
</asp:Content>
