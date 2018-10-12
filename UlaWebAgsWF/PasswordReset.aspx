<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="true" CodeBehind="PasswordReset.aspx.cs" Inherits="UlaWebAgsWF.PasswordReset" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="page-header">
            <h2>Password Reset</h2>
        </div>
        <asp:Panel runat="server" CssClass="alert alert-success alert-dismissible" ID="SuccessMsg" Visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close" id="SuccessMsgClose">&times;</a>
            <asp:Label ID="SuccessMsgTxt" runat="server" Visible ="false" Text="" />
        </asp:Panel>
        <asp:Panel runat="server" ID="FailureMsg" CssClass="alert alert-danger alert-dismissible" Visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close" id="FailMsgClose">&times;</a>
            <asp:Label ID="FailureMsgTxt" runat="server" Visible ="false" Text="" />
        </asp:Panel>
        <asp:Panel runat="server" ID="InfoMessage" CssClass="alert alert-info alert-dismissible" Visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close" id="InfoMessageClose">&times;</a>
            <asp:Label ID="InfoMessageText" runat="server" Visible ="false" Text="" />
        </asp:Panel>
        <asp:Panel ID="PasswordResetPanel" runat="server">
            <form>
                <div class="form-group">
                    <label for="LblUserName">User Name:</label>
                    <asp:Label ID="LblUserName" runat="server" Text=""></asp:Label>
                </div>
                <div class="form-group">
                    <label for="NewPassword">New Password:</label>
                    <asp:TextBox ID="NewPassword" runat="server" CssClass="form-control" Placeholder="Enter Password..." />
                    <asp:RequiredFieldValidator ID="RequiredFieldNewPassword" ValidationGroup="PasswordReset" ValidateRequestMode="Enabled" ControlToValidate="NewPassword" CssClass="text text-danger validator-field" runat="server" ErrorMessage="Please enter new password"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="CustomValidatorNewPassword" runat="server" ControlToValidate="NewPassword" OnServerValidate="CustomValidatorNewPassword_ServerValidate" CssClass="text text-danger" ErrorMessage="Password must be different from previous password"></asp:CustomValidator>
                </div>
                <div class="form-group">
                    <label for="PasswordAgain">Re enter Password:</label>
                    <asp:TextBox ID="PasswordAgain" runat="server" CssClass="form-control" Placeholder="Re enter Password..." />
                    <asp:RequiredFieldValidator ID="RequiredFieldPasswordAgain" ValidationGroup="PasswordReset" ValidateRequestMode="Enabled" ControlToValidate="PasswordAgain" CssClass="text text-danger validator-field" runat="server" ErrorMessage="Please re enter password"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="CompareValidatorPasswordAgain" ValidationGroup="PasswordReset" ValidateRequestMode="Enabled" ControlToValidate="PasswordAgain" CssClass="text text-danger validator-field" ControlToCompare="NewPassword" runat="server" ErrorMessage="Re entered password does not match with original passowrd"></asp:CompareValidator>
                </div>
                <asp:Button ID="PasswordResetSubmit" CausesValidation="true" ValidateRequestMode="Enabled" ValidationGroup="PasswordReset" UseSubmitBehavior="false" runat="server" CssClass="btn btn-default" Text="Reset Password" OnClick="PasswordResetSubmit_Click" />
            </form>
        </asp:Panel>
    </div>
</asp:Content>
