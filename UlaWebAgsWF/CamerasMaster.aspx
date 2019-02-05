<%@ Page Title="Camera Master" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CamerasMaster.aspx.cs" Inherits="UlaWebAgsWF.CamerasMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%= CreateNewCamLink.ClientID %>').click(function (e) {
                e.preventDefault();
                $('#<%= CreateNewCamPnl.ClientID %>').toggle("slow", "swing");
            });
            $('#<%= CreateNewPosLink.ClientID %>').click(function (e) {
                e.preventDefault();
                $('#<%= CreateNewPosPnl.ClientID %>').toggle("slow", "swing");
            });
        });
    </script>
    <div class="container">
        <div class="page-header">
            <h2>Cameras</h2>
        </div>
        <br clear="all" />
        <asp:Panel ID="CreateNewCamLinkPnl" CssClass="text-left" runat="server">
            <asp:LinkButton ID="CreateNewCamLink" CssClass="btn btn-link" style="padding:0px;" Text="Add Camera" runat="server"></asp:LinkButton>
            <asp:LinkButton ID="CreateNewPosLink" runat="server" CssClass="btn btn-link" Text="Add Camera Position" style="padding:0px;margin-left:10px;"></asp:LinkButton>
        </asp:Panel>
        <br clear="all" />
        <asp:Panel ID="CreateNewCamPnl" style="display:none;" CssClass="text-left" runat="server">
            <form>
                <div class="form-group">
                    <label for="txtCamIP">Camera IP</label>
                    <asp:TextBox ID="txtCamIP" CssClass="form-control" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatortxtCamIP" ValidationGroup="CamDetailsVal" runat="server" CssClass="text text-danger" ControlToValidate="txtCamIP" ErrorMessage="Please enter Camera IP"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label for="CamLaneDD">Camera Lane:</label>
                    <asp:DropDownList ID="CamLaneDD" CssClass="form-control" runat="server" DataSourceID="CamLaneDDDS" DataTextField="LaneName" DataValueField="LaneID"></asp:DropDownList>
                    <asp:EntityDataSource ID="CamLaneDDDS" runat="server" ConnectionString="name=DIMContainerDB_Revised_DevEntities" DefaultContainerName="DIMContainerDB_Revised_DevEntities" EnableFlattening="False" EntitySetName="LaneMasters" Select="it.[LaneID], it.[LaneName]">
                    </asp:EntityDataSource>
                </div>
                <div class="form-group">
                    <label for="CamPosDD">Camera Position:</label>
                    <asp:DropDownList ID="CamPosDD" CssClass="form-control" runat="server" DataSourceID="CamPosDDDS" DataTextField="PositionName" DataValueField="PositionID"></asp:DropDownList>
                    <asp:EntityDataSource ID="CamPosDDDS" runat="server" ConnectionString="name=DIMContainerDB_Revised_DevEntities" DefaultContainerName="DIMContainerDB_Revised_DevEntities" EnableFlattening="False" EntitySetName="CameraPositionMasterProxys" Select="it.[PositionID], it.[PositionName]">
                    </asp:EntityDataSource>
                </div>
                <div class="checkbox">
                    <label for="IsCamActive"><asp:CheckBox ID="IsCamActive" Text="Camera Active?" runat="server" Checked="false" /></label>
                </div>
                <asp:Button ID="NewCamBtnSubmit" runat="server" CssClass="btn btn-default btn-large" ValidationGroup="CamDetailsVal" Text="Add Camera" CausesValidation="true" OnClick="NewCamBtnSubmit_Click1" />
            </form>
            <br clear="all" />
            <br />
        </asp:Panel>
        <asp:Panel ID="CreateNewPosPnl" style="display:none;" CssClass="text-left" runat="server">
            <form>
                <div class="form-group">
                <label for="txtPosName">Position Name:</label>
                <asp:TextBox ID="txtPosName" CssClass="form-control" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidatortxtPosName" ValidationGroup="NewPosDetailsVal" ControlToValidate="txtPosName" CssClass="text text-danger" runat="server" ErrorMessage="Please enter Camera Position Name"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label for="txtPosDesc">Position Description:</label>
                    <asp:TextBox ID="txtPosDesc" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label for="txtContVis">Container Visible:</label>
                    <asp:TextBox ID="txtContVis" CssClass="form-control" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatortxtContVis" ValidationGroup="NewPosDetailsVal" ControlToValidate="txtContVis" runat="server" CssClass="text text-danger" ErrorMessage="Please enter Container Visible number"></asp:RequiredFieldValidator>
                    <asp:RangeValidator ID="RangeValidatortxtContVis" runat="server" ValidationGroup="NewPosDetailsVal" ControlToValidate="txtContVis" CssClass="text text-danger" MinimumValue="1" MaximumValue="3" ErrorMessage="Container visibility number must be between 1 to 3"></asp:RangeValidator>
                </div>
                <asp:Button ID="NewPosCreateBtn" CssClass="btn btn-default btn-large" runat="server" ValidationGroup="NewPosDetailsVal" CausesValidation="true" Text="Create Camera Position" OnClick="NewPosCreateBtn_Click1" />
            </form>
            <br clear="all" />
            <br />
        </asp:Panel>
        <div class="col-lg-4 col-sm-6 col-xs-12" style="padding:0px;">
            <div class="col-lg-4 col-sm-4 col-xs-12" style="padding:0px;">
                <asp:Label ID="lblSelectLane" runat="server" Font-Bold="True">Select Lane:</asp:Label>
            </div>
            <div class="col-lg-6 col-sm-6 col-xs-12">
                <asp:DropDownList ID="SelectLaneDDL" CssClass="form-control" runat="server"></asp:DropDownList>
            </div>
            <div class="col-lg-2 col-sm-2 col-xs-12">
                <asp:Button ID="BtnLaneDataSubmit" runat="server" CssClass="btn btn-default" Text="Submit" OnClick="BtnLaneDataSubmit_Click" />
            </div>
        </div>
        <br clear="all" />
        <br />
        <asp:Panel ID="CamsViewPnl" runat="server" HorizontalAlign="Center">
            <asp:Panel ID="CamViewPnl1" runat="server" Width="900px" Height="500px" Visible="False" HorizontalAlign="Center">
                <asp:Image ID="CamViewImg1" runat="server" Height="500px" ImageAlign="Left" Visible="False" Width="900px" />
            </asp:Panel>
            <br clear="all" />
            <br />
            <asp:Panel ID="CamPosLblPnl1" runat="server" Width="900px" HorizontalAlign="Center">
                <asp:Label ID="CamPosLbl1" runat="server" Visible="false" Font-Bold="True" Font-Overline="False"></asp:Label>
            </asp:Panel>
            <br clear="all" />
            <br />
            <asp:Panel ID="CamViewPnl2" runat="server" Width="900px" Height="500px" Visible="False" HorizontalAlign="Center">
                <asp:Image ID="CamViewImg2" runat="server" Height="500px" ImageAlign="Left" Visible="False" Width="900px" />
            </asp:Panel>
            <br clear="all" />
            <br />
            <asp:Panel ID="CamPosLblPnl2" runat="server" Width="900px" HorizontalAlign="Center">
                <asp:Label ID="CamPosLbl2" runat="server" Visible="false" Font-Bold="True"></asp:Label>
            </asp:Panel>
            <br clear="all" />
            <br />
            <asp:Panel ID="CamViewPnl3" runat="server" Width="900px" Height="500px" Visible="False" HorizontalAlign="Center">
                <asp:Image ID="CamViewImg3" runat="server" Height="500px" ImageAlign="Left" Visible="False" Width="900px" />
            </asp:Panel>
            <br clear="all" />
            <br />
            <asp:Panel ID="CamPosLblPnl3" runat="server" Width="900px" HorizontalAlign="Center">
                <asp:Label ID="CamPosLbl3" runat="server" Visible="false" Font-Bold="True"></asp:Label>
            </asp:Panel>
            <br clear="all" />
            <br />
            <asp:Panel ID="CamViewPnl4" runat="server" Width="900px" Height="500px" Visible="False" HorizontalAlign="Center">
                <asp:Image ID="CamViewImg4" runat="server" Height="500px" ImageAlign="Left" Visible="False" Width="900px" />
            </asp:Panel>
            <br clear="all" />
            <br />
            <asp:Panel ID="CamPosLblPnl4" runat="server" Width="900px" HorizontalAlign="Center">
                <asp:Label ID="CamPosLbl4" runat="server" Visible="false" Font-Bold="True"></asp:Label>
            </asp:Panel>
            <br clear="all" />
            <br />
            <asp:Panel ID="CamViewPnl5" runat="server" Width="900px" Height="500px" Visible="False" HorizontalAlign="Center">
                <asp:Image ID="CamViewImg5" runat="server" Height="500px" Visible="False" Width="900px" />
            </asp:Panel>
            <br clear="all" />
            <br />
            <asp:Panel ID="CamPosLblPnl5" runat="server" Width="900px" HorizontalAlign="Center">
                <asp:Label ID="CamPosLbl5" runat="server" Visible="false" Font-Bold="True"></asp:Label>
            </asp:Panel>
            <br clear="all" />
            <br />
            <asp:Panel ID="CamViewPnl6" runat="server" Width="900px" Height="500px" Visible="False" HorizontalAlign="Center">
                <asp:Image ID="CamViewImg6" runat="server" Height="500px" ImageAlign="Left" Visible="False" Width="900px" />
            </asp:Panel>
            <br clear="all" />
            <br />
            <asp:Panel ID="CamPosLblPnl6" runat="server" Width="900px" HorizontalAlign="Center">
                <asp:Label ID="CamPosLbl6" runat="server" Visible="false" Font-Bold="True"></asp:Label>
            </asp:Panel>
            
            <br clear="all" />
            <br />
            <asp:Panel ID="CamViewPnl7" runat="server" Width="900px" Height="500px" Visible="False" HorizontalAlign="Center">
                <asp:Image ID="CamViewImg7" runat="server" Height="500px" ImageAlign="Left" Visible="False" Width="900px" />
            </asp:Panel>
            <br clear="all" />
            <br />
            <asp:Panel ID="CamPosLblPnl7" runat="server" Width="900px" HorizontalAlign="Center">
                <asp:Label ID="CamPosLbl7" runat="server" Visible="false" Font-Bold="True"></asp:Label>
            </asp:Panel>
            <br clear="all" />
            <br />
        </asp:Panel>      
    </div>
</asp:Content>
