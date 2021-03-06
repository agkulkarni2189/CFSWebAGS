﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ScreensMaster.aspx.cs" EnableEventValidation="false" Async="true" Inherits="UlaWebAgsWF.ScreensMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%= CerateNewScreenLink.ClientID %>').click(function (e) {
                e.preventDefault();
                $('#<%= CreateNewScreenContainer.ClientID %>').toggle("slow", "swing");
            });
        });
    </script>
    <div class="container">
        <div class="page-header">
            <h2>Screens</h2>
        </div>
        <asp:Panel ID="SuccessMsg" runat="server" Visible="false" CssClass="alert alert-success alert-dismissible">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="SuccessMsgText" runat="server" Text="" />
        </asp:Panel>
        <asp:Panel ID="FailureMsg" runat="server" Visible="false" CssClass="alert alert-danger alert-dismissible">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="FailureMsgText" runat="server" Text="" />
        </asp:Panel>
        <asp:Panel ID="CreateNewScreenLinkContainer" CssClass="text-left" runat="server">
            <asp:LinkButton ID="CerateNewScreenLink" style="padding: 0px;" CssClass="btn btn-link" Text="Create screen" runat="server" />
        </asp:Panel>
        <asp:Panel ID="CreateNewScreenContainer" runat="server" style="display:none;">
            <form class="form-inline">
                <div class="form-group">
                    <label for="NewScreenName">Screen Name:</label>
                    <asp:TextBox ID="NewScreenName" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="RequiredFieldNewScreenName" ValidationGroup="NewScreenValidate" ControlToValidate="NewScreenName" CssClass="text text-danger validator-field" runat="server" ErrorMessage="Please screen Lane Name" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label for="ScreenUrl">Screen URL:</label>
                    <asp:TextBox ID="ScreenUrl" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="RequiredFieldScreenUrl" ValidationGroup="NewScreenValidate" ControlToValidate="ScreenUrl" CssClass="text text-danger validator-field" runat="server" ErrorMessage="Please enter screen URL"></asp:RequiredFieldValidator>
                </div>
                <asp:Button ID="NewScreenBtnSubmit" UseSubmitBehavior="true" CausesValidation="true" ValidationGroup="NewScreenValidate" runat="server" CssClass="btn btn-default" Text="Add Screen" OnClick="NewScreenBtnSubmit_Click" />
            </form>
        </asp:Panel>
        <br clear="all" />
        <asp:GridView ID="ScreensDGV" AutoGenerateColumns="true" AutoGenerateEditButton="false" AutoGenerateDeleteButton="false" OnRowEditing="ScreensDGV_RowEditing" OnRowUpdating="ScreensDGV_RowUpdating" OnRowDeleting="ScreensDGV_RowDeleting" OnRowCancelingEdit="ScreensDGV_RowCancelingEdit" CssClass="table table-hover text-center" DataKeyNames="ID" runat="server" >
            <Columns>
                <asp:TemplateField HeaderText="Screen Name" HeaderStyle-CssClass="text-center" SortExpression="ScreenName">
                    <ItemTemplate>
                        <asp:Label ID="ScreenNameTxt" runat="server" Text='<%# Bind("ScreenName") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="ScreenNameTBX" runat="server" CssClass="form-control" Text='<%# Bind("ScreenName") %>' />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorScreenNameTBX" ControlToValidate="ScreenNameTBX" ValidationGroup="ScreenUpdate" runat="server" ErrorMessage="Screen name is required" CssClass="text text-danger validator-field"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Screen URL" HeaderStyle-CssClass="text-center" SortExpression="ScreenUrl">
                    <ItemTemplate>
                        <asp:Label ID="ScreenUrlText" runat="server" Text='<%# Bind("ScreenUrl") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="ScreenUrlTBX" runat="server" CssClass="form-control" Text='<%# Bind("ScreenUrl") %>' />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorScreenUrlTBX" ControlToValidate="ScreenUrlTBX" ValidationGroup="ScreenUpdate" runat="server" ErrorMessage="Screen URL is required" CssClass="text text-danger validator-field"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:CommandField CausesValidation="true" ValidationGroup="ScreenUpdate" ShowEditButton="true" ShowDeleteButton="true" DeleteText="Delete" EditText="Edit" />
            </Columns>
        </asp:GridView> 
    </div>
</asp:Content>
