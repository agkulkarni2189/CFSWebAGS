<%@ Page Title="User Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableViewState="true" EnableViewStateMac="true" CodeBehind="Login.aspx.cs" Inherits="UlaWebAgsWF.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .container {
            height: 100%;
            justify-content: center;
            align-items: center;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#SuccessMsgClose').click(function (e) {
                e.preventDefault();
                $('#<%= txtSuccessMsg.ClientID %>').text('');
                $('#<%= PnlSuccessMsg.ClientID %>').hide();
            });

            $('#FailureMsgClose').click(function (e) {
                e.preventDefault();
                $('#<%= txtFailureMsg.ClientID %>').text('');
                $('#<%= PnlFailureMsg.ClientID %>').hide();
            });
        });
    </script>
    <asp:Panel ID="PnlLogin" CssClass="container" style="width:500px" runat="server">
        <asp:Panel ID="PnlSuccessMsg" CssClass="alert alert-success alert-dismissible" runat="server" Visible="False">
            <a href="#" class="close" data-dismiss="alert" aria-label="close" id="SuccessMsgClose">&times;</a>
            <asp:Label ID="txtSuccessMsg" runat="server"></asp:Label>
        </asp:Panel>
        <asp:Panel ID="PnlFailureMsg" CssClass="alert alert-danger alert-dismissible" runat="server" Visible="False">
            <a href="#" class="close" data-dismiss="alert" aria-label="close" id="FailureMsgClose">&times;</a>
            <asp:Label ID="txtFailureMsg" runat="server"></asp:Label>
        </asp:Panel>
        <form>
            <div class="page-header text-center">
                <h3>Login</h3>
            </div>
            <div class="form-group">
                <label for="txtUserName">User Name:</label>
                <asp:TextBox ID="txtUserName" CssClass="form-control" runat="server" style="max-width:none"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidatortxtUserName" ControlToValidate="txtUserName" runat="server" CssClass="text text-danger" ErrorMessage="Please enter User Name"></asp:RequiredFieldValidator>
            </div>
            <div class="form-group">
                <label for="txtPassword">Password:</label>
                <asp:TextBox ID="txtPassword" TextMode="Password" CssClass="form-control" style="max-width:none" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidatortxtPassword" runat="server" ControlToValidate="txtPassword" CssClass="text text-danger" ErrorMessage="Please enter Password"></asp:RequiredFieldValidator>
            </div>
            <div class="text-center" style="margin-bottom:20px">
                <asp:Button ID="btnLogin" CssClass="btn btn-default" runat="server" Text="Login" OnClick="btnLogin_Click" />
            </div>
        </form>
    </asp:Panel>
</asp:Content>
