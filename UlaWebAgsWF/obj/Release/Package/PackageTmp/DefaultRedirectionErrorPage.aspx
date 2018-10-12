<%@ Page Title="" Language="C#" MasterPageFile="~/SiteLogin.Master" AutoEventWireup="true" CodeBehind="DefaultRedirectionErrorPage.aspx.cs" Inherits="UlaWebAgsWF.DefaultRedirectionErrorPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2>Unknown Exception</h2>
        <p>
            An application is crashed due to unknown exception.<br />
            Go to the <a href="Default.aspx">Default Page</a>.
        </p>
    </div>
</asp:Content>
