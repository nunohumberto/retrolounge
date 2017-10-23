<%@ Page Title="Editing ROM" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="editROM.aspx.cs" Inherits="EDCFinal.editROM" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <h3><i class="fa fa-pencil-square-o"></i>&nbsp;&nbsp;Editing a ROM</h3>
    <hr />
    <div id="gamecontainer" runat="server" class="row" >
        <div class="col-md-8 col-md-offset-2" style="border-style: solid; border-color: #596a7b;">
            <div class="row ext" style="padding: 10px">
                <div class="col-md-4 int" style="text-align: center; padding-top: 10px;">
                    <img id="image" runat="server" style="max-height: 300px; max-width:100%" src="http://vignette1.wikia.nocookie.net/nintendo/images/3/39/Super_Mario_Bros_3_%28NA%29.png/revision/latest/scale-to-width-down/250?cb=20120517034717&path-prefix=en" />
                    <br />
                    <div style="padding-top: 10px;">
                     <asp:Button runat="server" CssClass="btn btn-success" Text="Save" OnClick="Unnamed_Click"/>&nbsp;&nbsp;&nbsp;&nbsp;
                     <a style="" href="/Library" class="btn btn-danger" role="button">Cancel</a>
                    </div>
                </div>
                <div class="col-md-8" style="text-align:center; padding-left: 30px; padding-right: 30px; padding-top: 0px; padding-bottom: 0px">
                            <h5>Title&nbsp;&nbsp;<small><asp:TextBox ID="title" runat="server" Width="100%" CssClass="editinput">Super Mario</asp:TextBox></small></h5>
                            <h5>Release Date&nbsp;&nbsp;<small><asp:TextBox ID="release" runat="server" Width="100%" CssClass="editinput"></asp:TextBox></small></h5>
                            <h5>Genre&nbsp;&nbsp;<small><asp:TextBox ID="genre" runat="server" Width="100%" CssClass="editinput"></asp:TextBox></small></h5>
                            <h5>Developer&nbsp;&nbsp;<small><asp:TextBox ID="developer" runat="server" Width="100%" CssClass="editinput"></asp:TextBox></small></h5>
                            <h5>Publisher&nbsp;&nbsp;<small><asp:TextBox ID="publisher" runat="server" Width="100%" CssClass="editinput"></asp:TextBox></small></h5>
                            <h5>Image URL&nbsp;&nbsp;<small><asp:TextBox ID="imageurl" runat="server" Width="100%" CssClass="editinput"></asp:TextBox></small></h5>
                </div>
            </div>
            <div class="row" style="padding: 10px">
                <div class="col-md-12" style="text-align:center;">
           
                </div>
            </div>
       </div>
    </div>
    <br/>
</asp:Content>
