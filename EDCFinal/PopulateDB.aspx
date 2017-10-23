<%@ Page Title="Rebuild Database" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PopulateDB.aspx.cs" Inherits="EDCFinal.PopulateDB" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <h3><i class="fa fa-database"></i>&nbsp;&nbsp;Rebuild database</h3>
    <hr />
    <div class="row">
        <div class="col-md-6 col-md-offset-3" style="border-style: solid; border-color: #596a7b;">
            <div class="row" style="padding: 10px">
                <div class="col-md-12" style="padding-left: 30px; padding-right: 30px; padding-top: 10px; padding-bottom: 30px">
                    <h3>Warning!&nbsp;&nbsp;<small>Please ensure the game database is empty.</small></h3>
                    <h5>Proceeding without clearing the Games table will result in duplicate data.</h5>
                    <br />
                    <div style="text-align: center;">
                        <a href="/Populate?c=NES" class="btn btn-default" role="button">NES&nbsp;&nbsp;&nbsp;&nbsp;<img src="/Content/NES.png" /></a>&nbsp;&nbsp;&nbsp;
                        <a href="/Populate?c=GB" class="btn btn-default" role="button">GB&nbsp;&nbsp;<img src="/Content/GB.png" /></a>&nbsp;&nbsp;&nbsp;
                        <a href="/Populate?c=GBC" class="btn btn-default" role="button">GBC&nbsp;&nbsp;<img src="/Content/GBC.png" /></a>&nbsp;&nbsp;&nbsp;
                        <a href="/Populate?c=GBA" class="btn btn-default" role="button">GBA&nbsp;&nbsp;&nbsp;&nbsp;<img src="/Content/GBA.png" /></a>
                    </div>
                    <br />
                    <h5>The console game indexes may be re-downloaded individually.</h5>
                    <h5>This process may take a while.</h5>
                    <div style="padding-left: 40px">
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
