<%@ Page Title="Users Master" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UsersMaster.aspx.cs" Inherits="UlaWebAgsWF.UsersMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .visible-none {
            display:none;
        }
    </style>
    <%--<script type="text/javascript">
        $(document).ready(function () {
            $('#<%= UserRolesDD.ClientID %>').change(function () {
                //console.log($("#<%= UserRolesDD.ClientID %> option:selected").val());
                console.log($("#<%= UserRolesDD.ClientID %> option:selected").val() == 1);
                if ($("#<%= UserRolesDD.ClientID %> option:selected").val() != 1) {

                    $("#UserActiveCBCont").hide();
                    $("#UserActiveCB").prop("checked", false);
                }
                else {
                    $("#UserActiveCBCont").show();
                }
            });
        });
    </script>--%>
    <div class="container">
        <div class="page-header">
            <h2>
                Users
            </h2>
        </div>
        <asp:Panel runat="server" CssClass="alert alert-success alert-dismissible" ID="SuccessMsg" Visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close" id="SuccessMsgClose">&times;</a>
            <asp:Label ID="SuccessMsgTxt" runat="server" Visible ="false" Text="" />
        </asp:Panel>
        <asp:Panel runat="server" ID="FailureMsg" CssClass="alert alert-danger alert-dismissible" Visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close" id="FailMsgClose">&times;</a>
            <asp:Label ID="FailureMsgTxt" runat="server" Visible ="false" Text="" />
        </asp:Panel>
        <asp:Panel ID="CreateNewUserLinkContainer" CssClass="text-left" runat="server">
            <asp:LinkButton ID="CerateNewUserLink" style="padding: 0px;" CssClass="btn btn-link" Text="Create User" runat="server" />
        </asp:Panel>

        <asp:Panel ID="CreateNewUserPanel" runat="server" CssClass="text-left" style="display:none;">
            <form>
                <div class="form-group">
                    <label for="NewUserFN">First Name:</label>
                    <asp:TextBox ID="NewUserFN" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="RequiredFieldNewUserFN" ControlToValidate="NewUserFN" CssClass="text text-danger validator-field" runat="server" ErrorMessage="Please enter First Name"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidatorNewUserFN" ControlToValidate="NewUserFN" ValidationExpression="^[a-zA-Z]+$" runat="server" CssClass="text text-danger" ErrorMessage="Only letters allowed"></asp:RegularExpressionValidator>
                </div>
                <div class="form-group">
                    <label for="NewUserLN">Last Name:</label>
                    <asp:TextBox ID="NewUserLN" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="RequiredFieldNewUserLN" ControlToValidate="NewUserLN" CssClass="text text-danger validator-field" runat="server" ErrorMessage="Please enter Last Name"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidatorNewUserLN" ControlToValidate="NewUserLN" ValidationExpression="^[a-zA-Z]+$" runat="server" CssClass="text text-danger" ErrorMessage="Only letters allowed"></asp:RegularExpressionValidator>
                </div>
                <div class="form-group">
                    <label for="NewUserCN">Contact Number:</label>
                    <asp:TextBox ID="NewUserCN" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="RequiredFieldNewUserCN" ControlToValidate="NewUserCN" CssClass="text text-danger validator-field" runat="server" ErrorMessage="Please enter contact number"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionNewUserCN" ValidationExpression="^[0-9]{10}$" ControlToValidate="NewUserCN" runat="server" ErrorMessage="Contact number must be numeric and 10 digits only"></asp:RegularExpressionValidator>
                </div>
                <div class="form-group">
                    <label for="NewUserEmail">E-Mail:</label>
                    <asp:TextBox ID="NewUserEmail" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="RequiredFieldNewUserEmail" CssClass="text text-danger validator-field" ControlToValidate="NewUserEmail" runat="server" ErrorMessage="Please enter E-Mail"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionNewUserEmail" ControlToValidate="NewUserEmail" ValidationExpression="^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$" runat="server" ErrorMessage="Please enter a valid E-Mail"></asp:RegularExpressionValidator>
                </div>
                <div class="form-group">
                    <label for="NewUserDesignation">Designation:</label>
                    <br />
                    <asp:DropDownList ID="UserDesignationDD" runat="server" CssClass="form-control">
                    </asp:DropDownList>
                    <br />
                </div>
                <div class="form-group">
                    <label for="NewUserUserName">User Name:</label>
                    <asp:TextBox ID="NewUserUserName" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="RequiredFieldNewUserUserName" runat="server" CssClass="text text-danger validator-field" ControlToValidate="NewUserUserName" ErrorMessage="Please enter User Name"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="CustomNewUserUserName" CssClass="text text-danger validator-field" ValidateEmptyText="true" runat="server" ControlToValidate="NewUserUserName" ErrorMessage="User name must be unique." OnServerValidate="CustomValidator1_ServerValidate"></asp:CustomValidator>
                </div>
                <div class="form-group">
                    <label for="NewUserPassword">Password:</label>
                    <asp:TextBox ID="NewUserPassword" runat="server" TextMode="Password" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="RequiredFieldNewUserPassword" CssClass="text text-danger validator-field" ControlToValidate="NewUserPassword" runat="server" ErrorMessage="Please enter password"></asp:RequiredFieldValidator>
                </div>
                <div class="checkbox" id="UserActiveCBCont">
                    <asp:CheckBox ID="UserActiveCB" runat="server" Text="User Active?" />
                </div>
                <asp:Button ID="NewUserBtnSubmit" UseSubmitBehavior="false" runat="server" CssClass="btn btn-default" Text="Add User" OnClick="NewUserBtnSubmit_Click" />
            </form>
         </asp:Panel>
        <br clear="all" />
        <asp:GridView ID="UsersDGV" runat="server" AutoGenerateColumns="False" CssClass="table table-hover" DataKeyNames="UserId" EmptyDataText="&quot;&lt;h4&gt;No Users available&lt;/h4&gt;&quot;" OnRowCancelingEdit="UsersDGV_RowCancelingEdit" OnRowDeleted="UsersDGV_RowDeleted" OnRowDeleting="UsersDGV_RowDeleting" OnRowEditing="UsersDGV_RowEditing" OnRowUpdated="UsersDGV_RowUpdated" OnRowUpdating="UsersDGV_RowUpdating" EnablePersistedSelection="True">
            <Columns>
                <asp:TemplateField HeaderText="First Name" SortExpression="FirstName">
                    <ItemTemplate>
                        <asp:Label ID="FirstNameTxt" runat="server" Text='<%# Bind("FirstName") %>'></asp:Label> 
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="FirstNameTextBox" CssClass="form-control" runat="server" Text='<%# Bind("FirstName") %>' />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Last Name" SortExpression="LastName">
                    <ItemTemplate>
                        <asp:Label ID="LastNameTxt" runat="server" Text='<%# Bind("LastName") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="LastNameTextBox" CssClass="form-control" runat="server" Text='<%# Bind("LastName") %>' />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Contact Number" SortExpression="ContactNo">
                    <ItemTemplate>
                        <asp:Label ID="ContactNoTxt" runat="server" Text='<%# Bind("ContactNo") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="ContactNoTextBox" CssClass="form-control" runat="server" Text='<%# Bind("ContactNo") %>' />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Email ID" SortExpression="EmailId">
                    <ItemTemplate>
                        <asp:Label ID="EmailIdTxt" runat="server" Text='<%# Bind("EmailId") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="EmailIdTextBox" CssClass="form-control" runat="server" Text='<%# Bind("EmailId") %>' />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Designation" SortExpression="Designation">
                    <ItemTemplate>
                        <asp:Label ID="DesignationTxt" runat="server" Text='<%# Bind("Designation") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="UserDGVDesignationDD" DataSourceID="UserDesignationEDS" DataTextField="DesignationName" DataValueField="ID" CssClass="form-control" runat="server"></asp:DropDownList>
                        <asp:EntityDataSource ID="UserDesignationEDS" runat="server" ConnectionString="name=DIMContainerDB_Revised_DevEntities" DefaultContainerName="DIMContainerDB_Revised_DevEntities" EnableFlattening="False" EntitySetName="DIMSContainerDBEFDLL.EntityProxies.RoleMasterProxys" Select="it.[ID], it.[DesignationName]">
                        </asp:EntityDataSource> 
                    </EditItemTemplate>
                </asp:TemplateField>
                <%--<asp:TemplateField HeaderText="Role" SortExpression="Role">
                    <ItemTemplate>
                        <asp:Label ID="RoleText" runat="server" Text='<%# Bind("Role") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="UserDGVRoleDD" DataSourceID="UserRolesEDS" DataTextField="RoleName" DataValueField="ID" CssClass="form-control" runat="server"></asp:DropDownList>
                        <asp:EntityDataSource ID="UserRolesEDS" runat="server" ConnectionString="name=DIMContainerDB_Revised_DevEntities" DefaultContainerName="DIMContainerDB_Revised_DevEntities" EnableFlattening="False" EntitySetName="DIMSContainerDBEFDLL.EntityProxies.RoleMasterProxys" Select="it.[ID], it.[RoleName]">
                        </asp:EntityDataSource> 
                    </EditItemTemplate>
                </asp:TemplateField>--%>
                <asp:TemplateField HeaderText="User Active ?" SortExpression="isUserActive">
                    <ItemTemplate>
                        <asp:CheckBox ID="ActiveStatusCBDis" runat="server" Checked='<%# Eval("isUserActive") %>' Enabled="false" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox ID="UserAcitve" runat="server" Enabled="true" Checked='<%# Eval("isUserActive") %>' />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:CommandField ButtonType="Link" EditText="Edit" DeleteText="Delete" ShowEditButton ="true" ShowDeleteButton="true" CausesValidation="false" />
            </Columns>
        </asp:GridView>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%= CerateNewUserLink.ClientID %>').click(function (e) {
                e.preventDefault();
                $('#<%= CreateNewUserPanel.ClientID %>').toggle("slow", "swing");
            });

            $('#SuccessMsgClose').click(function (e) {
                e.preventDefault();

                $('#<%= SuccessMsgTxt.ClientID %>').text("");
                $('#<%= SuccessMsg.ClientID %>').hide();
            });

            $('#FailMsgClose').click(function (e) {
                e.preventDefault();

                $('#<%= FailureMsgTxt.ClientID %>').text("");
                $('#<%= FailureMsg.ClientID %>').hide();
            });
        });
    </script>
</asp:Content>
