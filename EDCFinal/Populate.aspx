<%@ Page Title="Scrap Results" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Populate.aspx.cs" Inherits="EDCFinal.Populate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <h3><i class="fa fa-database"></i>&nbsp;&nbsp;Obtained game list</h3>
    <hr />
    <table class="table table-striped">
        <thead>
          <tr>
            <th>Title</th>
            <th>Path</th>
            <th>Genre</th>
            <th>Developers</th>
            <th>Publishers</th>
            <th>Release</th>
            <th>Image</th>
          </tr>
        </thead>
        <tbody runat="server" id="datacontainer">
        </tbody>
      </table>
</asp:Content>
