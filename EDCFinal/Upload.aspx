<%@ Page Title="Upload" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="EDCFinal.Upload" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <h3><i class="fa fa-cloud-upload"></i>&nbsp;&nbsp;Upload a ROM</h3>
    <hr />
    <div id="gamecontainer" runat="server" class="row" > 
    </div>
    <br/>
    <div class="row" style="text-align: center;">
        <center>
            <asp:FileUpload id="upload_element"  runat="server" CssClass="fileinput btn btn-default" />&nbsp;&nbsp;&nbsp;
            <asp:Button runat="server" id="upload_button" CssClass="btn btn-primary" text="Upload" OnClick="upload_button_Click" />
        </center>
    </div>
</asp:Content>
