<%@ Page Title="User Management" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="userManagement.aspx.cs" Inherits="EDCFinal.userManagement" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <br />
        <h3><i class="fa fa-user"></i>&nbsp;&nbsp;User Management</h3>
    <hr />
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="table table-striped" DataSourceID="SqlDataSource2" DataKeyNames="Id">
        <Columns>
                        <asp:TemplateField ShowHeader="False">
                <EditItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update"><span class="glyphicon glyphicon-floppy-saved"></span></asp:LinkButton>
                    &nbsp;<asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel"><span class="glyphicon glyphicon-remove"></span></asp:LinkButton>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit"><span class="glyphicon glyphicon-pencil"></span></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Id" HeaderText="Identificador do utilizador" SortExpression="Id" ReadOnly="True" />
            <asp:BoundField DataField="Email" HeaderText="Endereço de e-mail" SortExpression="Email" />
            <asp:BoundField DataField="UserName" HeaderText="Nome de utilizador" SortExpression="UserName" />
        </Columns>
    </asp:GridView>

 

    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="SELECT [Id], [Email], [UserName] FROM [AspNetUsers]" UpdateCommand="UPDATE [AspNetUsers] SET [Email] = @Email, [UserName] = @UserName WHERE [Id] = @Id"></asp:SqlDataSource>

 

</asp:Content>
