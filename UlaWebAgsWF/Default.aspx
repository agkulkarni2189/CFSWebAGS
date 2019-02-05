<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="UlaWebAgsWF._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <%--<script type="text/javascript">
        $(document).ready(function () {
            window.setInterval(function () {
                $.ajax({
                    type: 'POST',
                    url: '<%= ResolveUrl("Default.aspx/Trans_Check_Timer_Tick") %>',
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    async: 'true',
                    success: function (response) {
                        if (response.d != "")
                            window.location.replace('DamageImages.aspx?TransactionID=' + response.d);
                    }
                });
            }, 2000);
        });
    </script>--%>
    <asp:Panel ID="ErrorMsgPanel" Visible="false" CssClass="alert alert-danger alert-dismissible" runat="server">
        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
        <asp:Literal ID="ErrorMsgLiteral" runat="server"></asp:Literal>
    </asp:Panel>
    <asp:Panel ID="SuccessMsgPanel" Visible="false" CssClass="alert alert-success alert-dismissible" runat="server">
        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
        <asp:Literal ID="SuccessMsgLiteral" runat="server"></asp:Literal>
    </asp:Panel>
    <asp:Panel ID="PageLinksContainer" runat="server">
        <%--<asp:UpdatePanel ID="TransCheckUpdatePanel" runat="server">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="TransCheckTimer" EventName="Tick" />
            </Triggers>
            <ContentTemplate>
                <asp:Timer ID="TransCheckTimer" runat="server" OnTick="TransCheckTimer_Tick" Interval="2000" Enabled="False"></asp:Timer>
            </ContentTemplate>
        </asp:UpdatePanel>--%>
        <asp:Literal ID="PageLinksLiteral" runat="server">
        </asp:Literal>
    </asp:Panel>

</asp:Content>
