<%@ Page Title="Container Transactions History" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ContainerTransactionsReport.aspx.cs" Inherits="UlaWebAgsWF.ContainerTransactionsReport" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {

            $(".table-hover").delegate('tr', 'click', function () {
                if ($(this).hasClass("ContTransPagination") != true)
                {
                    var transid = $(this).find($('td')).first().text();
                    if (transid != "")
                    {
                        window.location.replace("DamageImages.aspx?TransactionID=" + transid);
                    }
                }
            });
        });
    </script>
    <style type="text/css">
        .element-form {
            width:auto;
            float:left;
            margin-right:10px;
        }
    </style>
    <div class="container container-fluid">
        <div class="page-header">
            <h2>
                Container Transactions Report
            </h2>
        </div>
        <div class="navbar-nav">
            <div class="form-group element-form">
                <label for="MainContent_dmgtypedd">Damage Type:</label>
                <asp:DropDownList ID="dmgtypedd" CssClass="form-control" runat="server">
                    <asp:ListItem Selected="True" Value="0">Both</asp:ListItem>
                    <asp:ListItem Value="1">Damaged</asp:ListItem>
                    <asp:ListItem Value="2">Undamaged</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group element-form">
                <label for="MainContent_ContCode">Container Code:</label>
                <asp:TextBox ID="ContCode" placeholder="Container Code" CssClass="form-control" runat="server"></asp:TextBox>
            </div>
            <div class="form-group date element-form">
                <label for="MainContent_FromDate">From Date:</label>
                <asp:TextBox ID="FromDate" CssClass="form-control" runat="server"></asp:TextBox>
                <cc1:CalendarExtender Animated="true" runat="server" ID="FromDateCal" PopupButtonID="FromDate" TargetControlID="FromDate" Format="dd-MM-yyyy" />
             </div>
            <div class="form-group date element-form">
                <label for="MainContent_FromDate">To Date:</label>
                <asp:TextBox ID="ToDate" CssClass="form-control" runat="server"></asp:TextBox>
                <cc1:CalendarExtender Animated="true" runat="server" ID="ToDateCal" PopupButtonID="ToDate" TargetControlID="ToDate" Format="dd-MM-yyyy" />
            </div>
            <div class="form-group element-form">
                <label for="MainContent_FromDate">Container Type:</label>
                <asp:TextBox ID="ContainerType" CssClass="form-control" runat="server"></asp:TextBox>
            </div>
            <div class="form-group element-form">
                <label for="MainContent_FromDate">Lane:</label>
                <asp:DropDownList ID="LaneDD" CssClass="form-control" runat="server" DataSourceID="DIMContainerDBEDS" DataTextField="LaneName" DataValueField="LaneID"></asp:DropDownList>
                <asp:EntityDataSource ID="DIMContainerDBEDS" runat="server" ConnectionString="name=DIMContainerDB_RevisedEntities" DefaultContainerName="DIMContainerDB_RevisedEntities" EnableFlattening="False" EntitySetName="LaneMasters" Select="it.[LaneID], it.[LaneName]">
                </asp:EntityDataSource>
            </div>
        </div>
        <div class="text-center">
            <asp:Button ID="btnSubmit" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
        </div>
        <br clear="all" />
        <asp:GridView ID="ContainerTransDGV" CssClass="table table-hover" runat="server" AutoGenerateColumns="False" AllowPaging="True" OnPageIndexChanging="ContainerTransDGV_PageIndexChanging">
            <PagerStyle CssClass="ContTransPagination" />
        </asp:GridView>
        <br clear="all" />
        <div class="text-center">
            <asp:Label ID="lbl_Error" runat="server" CssClass="text-danger h3" Text="" Visible="false"></asp:Label>
        </div>
        <br clear="all" />
    </div>
</asp:Content>
