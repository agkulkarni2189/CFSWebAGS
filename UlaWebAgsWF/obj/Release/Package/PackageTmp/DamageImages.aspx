<%@ Page Title="Container Transaction" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DamageImages.aspx.cs" Inherits="UlaWebAgsWF.DamageImages" EnableSessionState="True" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {

            if ($('#MainContent_isPreview').text() != "")
            {
                if ($("#MainContent_isPreview").text() == "1") {
                    $("#btnApprove").hide();
                    $("#btnReject").hide();
                    $("#btnBack").show();
                    $("#btnSubmitAll").hide();
                }
                else
                {
                    $("#btnApprove").show();
                    $("#btnReject").show();
                    $("#btnBack").hide();
                    $("#btnSubmitAll").show();
                }
            }

            $('input[type=image]').click(function (e) {
                console.log($('#ContImgMagPop').find('.modal-body'));
                $('#ContImgMagPop').find('.modal-body').html('<img src="' + this.getAttribute('src') + '" class="img-responsive img-fluid" id="ContMagImg" style="width:100%;">');
                $('#ContImgMagPop').show('slow');
                e.preventDefault();
            });

            $('#btnClose').click(function () {
                $('#ContMagImg').attr('src', '');
                $('#ContImgMagPop').hide('fast');
            });

            $('#btnBack').click(function (e) {
                e.preventDefault();
                window.location.replace(document.referrer);
            });

            $("#MainContent_btn_Submit").click(function (e) {
                e.preventDefault();
            });

            $("#btnApprove").click(function () {
                if ($("#MainContent_CB_Damaged").prop("checked") == true)
                {
                    $("#TransApprove").find(".text_center").find("#MainContent_mod_Cont_Num").text($("Cont_Num").text());
                    $("#TransApprove").find(".text_center").find("#MainContent_mod_Cont_Num").show();
                    $("#TransApprove").find(".text_center").find("#MainContent_mod_lbl_Cont_Num").show();
                    $("#TransApprove").find(".text_center").find("#MainContent_mod_Cont_Type").text($("Cont_Type").text());
                    $("#TransApprove").find(".text_center").find("#MainContent_mod_Cont_Type").show();
                    $("#TransApprove").find(".text_center").find("#MainContent_mod_lbl_Cont_Type").show();
                }
            });

            $('.table-hover th:nth-child(1)').css('display', 'none');
            $('.table-hover td:nth-child(1)').css('display', 'none');

        });
    </script>
    <div class="page-header">
        <h2>Container Transaction</h2>
    </div>
    <div class="text-left">
        <asp:Label ID="lbl_Cont_Num" CssClass="h4" style="float:left;margin-right:113px;" runat="server" Text="Container Number: " Visible="false"></asp:Label>
        <asp:Label ID="Cont_Num" CssClass="h4" runat="server" Visible="false" />
    </div>
    <br clear="all" />
    <div class="text-left">
        <asp:Label ID="lbl_Cont_Type" CssClass="text-left h4" style="float:left;margin-right:137px;" runat="server" Text="Container Type: " Visible="false" />
        <asp:Label ID="Cont_Type" CssClass="text-left h4" runat="server" Visible="false" />
    </div>
    <br clear="all" />
    <div class="text-left">
        <asp:Label ID="lbl_Cont_Dmgd" CssClass="text-left h4" style="float:left;margin-right:20px;" runat="server" Text="Container Damaged?(Yes/No): " Visible="false" />
        <asp:Label ID="Cont_Dmgd" CssClass="text-left h4" runat="server" Visible="false" >No</asp:Label>
    </div>
    <br />
    <div style="float:left;margin:auto;height:auto;width:auto">
        <asp:ImageButton ID="imgFromDB1" runat="server" Height="350px" Width="390px" CssClass="img-responsive" Visible="False" />
        <br />
        <div class="text-center">
            <asp:Label ID="Pos1" runat="server" Visible="False" CssClass="h4" />
        </div>
        <br />
        <asp:Panel ID="DmgOps1" runat="server" CssClass="panel-default" style="float:left;margin-top:20px;" VerticalAlign="Justify" Width="390px" Visible="False">
            <asp:TextBox ID="DmgImgCmnt1" runat="server" CssClass="form-control" placeholder="Enter damage comments..."></asp:TextBox>
            <asp:CheckBoxList ID="DmgImgCBL1" runat="server" CssClass="form-control">
            </asp:CheckBoxList>
            <br />
            <br />
        </asp:Panel>
    </div>
    <div style="float:left;margin:auto;height:auto;width:auto">
        <asp:ImageButton ID="imgFromDB2" runat="server" Height="350px" Width="390px" style="max-width:100%;" CssClass="img-responsive" Visible="False" />
        <br />
        <div class="text-center">
            <asp:Label ID="Pos2" runat="server" Visible="False" CssClass="h4" />
        </div>
        <br />
        <asp:Panel ID="DmgOps2" runat="server" CssClass="panel-default" style="float:left;margin-top:20px;" VerticalAlign="Justify" Width="390px" Visible="False">
            <asp:TextBox ID="DmgImgCmnt2" runat="server" CssClass="form-control" placeholder="Enter damage comments..."></asp:TextBox>
            <asp:CheckBoxList ID="DmgImgCBL2" runat="server" CssClass="form-control">
            </asp:CheckBoxList>
            <br />
            <br />
        </asp:Panel>
    </div>
    <div style="float:left;margin:auto;height:auto;width:auto">
        <asp:ImageButton ID="imgFromDB3" runat="server" Height="350px" Width="390px" style="max-width:100%" CssClass="img-responsive" Visible="False" />
        <br />
        <div class="text-center">
            <asp:Label ID="Pos3" runat="server" Visible="False" CssClass="h4" />
        </div>
        <br />
        <asp:Panel ID="DmgOps3" runat="server" CssClass="panel-default" style="float:left;margin-top:20px;" VerticalAlign="Justify" Width="390px" Visible="False">
            <asp:TextBox ID="DmgImgCmnt3" runat="server" CssClass="form-control" placeholder="Enter damage comments..."></asp:TextBox>
            <asp:CheckBoxList ID="DmgImgCBL3" runat="server" CssClass="form-control">
            </asp:CheckBoxList>
            <br />
            <br />
        </asp:Panel>
    </div>
    <div style="float:left;margin:auto;height:auto;width:auto">
        <asp:ImageButton ID="imgFromDB4" runat="server" Height="350px" Width="390px" CssClass="img-responsive" Visible="False" />
        <br />
        <div class="text-center">
            <asp:Label ID="Pos4" runat="server" Visible="False" CssClass="h4" />
        </div>
        <br />
        <asp:Panel ID="DmgOps4" runat="server" CssClass="panel-default" style="float:left;margin-top:20px;" VerticalAlign="Justify" Width="390px" Visible="False">
            <asp:TextBox ID="DmgImgCmnt4" runat="server" CssClass="form-control" placeholder="Enter damage comments..."></asp:TextBox>
            <asp:CheckBoxList ID="DmgImgCBL4" runat="server" CssClass="form-control">
            </asp:CheckBoxList>
            <br />
            <br />
        </asp:Panel>
    </div>
    <div style="float:left;margin:auto;height:auto;width:auto">
        <asp:ImageButton ID="imgFromDB5" runat="server" Height="350px" Width="390px" style="max-width:100%;" CssClass="img-responsive" Visible="False" />
        <br />
        <div class="text-center">
            <asp:Label ID="Pos5" runat="server" Visible="False" CssClass="h4" />
        </div>
        <br />
        <asp:Panel ID="DmgOps5" runat="server" CssClass="panel-default" style="float:left;margin-top:20px;" VerticalAlign="Justify" Width="390px" Visible="False">
            <asp:TextBox ID="DmgImgCmnt5" runat="server" CssClass="form-control" placeholder="Enter damage comments..."></asp:TextBox>
            <asp:CheckBoxList ID="DmgImgCBL5" runat="server" CssClass="form-control">
            </asp:CheckBoxList>
            <br />
            <br />
        </asp:Panel>
    </div>
    <div style="float:left;margin:auto;height:auto;width:auto">
        <asp:ImageButton ID="imgFromDB6" runat="server" Height="350px" Width="390px" style="max-width:100%;" CssClass="img-responsive" Visible="False" />
        <br />
        <div class="text-center">
            <asp:Label ID="Pos6" runat="server" Visible="False" CssClass="h4" />
        </div>
        <br />
        <asp:Panel ID="DmgOps6" runat="server" CssClass="panel-default" style="float:left;margin-top:20px;" VerticalAlign="Justify" Width="390px" Visible="False">
            <asp:TextBox ID="DmgImgCmnt6" runat="server" CssClass="form-control" placeholder="Enter damage comments..."></asp:TextBox>
            <asp:CheckBoxList ID="DmgImgCBL6" runat="server" CssClass="form-control">
            </asp:CheckBoxList>
            <br />
            <br />
        </asp:Panel>
    </div>
    <div style="float:left;margin: auto; height: auto; width: auto">
        <asp:ImageButton ID="imgFromDB7" runat="server" Height="350px" Width="390px" CssClass="img-responsive" Visible="False" />
        <br />
        <div class="text-center">
            <asp:Label ID="Pos7" runat="server" Visible="False" CssClass="h4" />
        </div>
        <br />
        <asp:Panel ID="DmgOps7" runat="server" CssClass="panel-default" Style="float: left; margin-top: 20px;" VerticalAlign="Justify" Width="390px" Visible="False">
            <asp:TextBox ID="DmgImgCmnt7" runat="server" CssClass="form-control" placeholder="Enter damage comments..."></asp:TextBox>
            <asp:CheckBoxList ID="DmgImgCBL7" runat="server" CssClass="form-control">
            </asp:CheckBoxList>
            <br />
            <br />
        </asp:Panel>
    </div>
    <div style="float: left; margin: auto; height: auto; width: auto">
        <div class="text-center">
            <asp:Label ID="CommonRemark" runat="server" Visible="False" CssClass="h4" >Common Remark</asp:Label>
        </div>
        <br />
        <asp:Panel ID="CmnRemPnl" runat="server" CssClass="panel-default" style="float:left;margin-top:20px;" VerticalAlign="Justify" Width="390px" Visible="False">
            <asp:TextBox ID="DmgImgCmnt8" runat="server" CssClass="form-control" placeholder="Enter damage comments..."></asp:TextBox>
            <asp:CheckBoxList ID="DmgImgCBL8" runat="server" CssClass="form-control">
            </asp:CheckBoxList>
        </asp:Panel>
    </div>
    <br clear="all" />
    <div class="text-center">
        <asp:Label ID="noPreviewTxt" runat="server" CssClass="text-danger h3" Text="" Visible="false"></asp:Label>
    </div>
    <br />

    <asp:CheckBox ID="CB_Damaged" runat="server" CssClass="text-danger" Text="Damaged" AutoPostBack="True" OnCheckedChanged="CB_Damaged_CheckedChanged" />
    <div class="text-center">
        <button type="button" id="btnApprove" class="btn btn-primary" data-toggle="modal" data-target="#TransApprove">Submit</button>
        <button type="button" id="btnBack" class="btn btn-primary">Back</button>
        <button type="button" id="btnReject" class="btn btn-primary" data-toggle="modal" data-target="#TransReject">Cancel</button>
        <button type="button" id="btnSubmitAll" class="btn btn-primary" data-toggle="modal" data-target="#TransSubmitAll">Submit All</button>
        <asp:Button ID="btnDownloadImgs" runat="server" Text="Download Images" CssClass="btn btn-primary" OnClick="btnDownloadImgs_Click" />
    </div>
    <br />
    <div class="modal" id="ContImgMagPop" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog" role="document" style="width:1250px;">
            <div class="modal-content">
                <div class="modal-body">
                
                </div>
                <div class="modal-footer">
                    <button id="btnClose" type="button" class="btn btn-secondary" data-dismiss="modal" style ="display:block;margin-left:auto;margin-right:auto;">Close</button>
                 </div>
            </div>
            <br />
        </div>
    </div>
    <div class="modal fade" id="TransApprove" tabindex="-1" role="dialog" aria-hidden="true">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="modal-body">
              <div class="text-center">
                  Are you sure you want to submit the transaction ?
                  <br clear="all" />
                <asp:Label ID="mod_lbl_Cont_Num" runat="server" Text="Container Number: " CssClass="h4" style="float:left;display:none;margin-right:20px;" />
                <asp:Label ID="mod_Cont_Num" runat="server" style="display:none;" CssClass="h4" />
                <br clear="all" />
                <asp:Label ID="mod_lbl_Cont_Type" runat="server" Text="Container Type: " CssClass="h4" style="float:left;margin-right:20px;display:none;" />
                <asp:Label ID="mod_Cont_Type" runat="server" CssClass="h4" style="display:none;" />
                <br clear="all" />
              </div>
          </div>
          <div class="modal-footer">
              <div class="text-center">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                <asp:Button ID="SubmitConfirm" runat="server" type="button" CssClass="btn btn-primary" Text="Yes" OnClick="btn_Submit_Click" />
              </div>
          </div>
        </div>
      </div>
    </div>
    <div class="modal fade" id="TransReject" tabindex="-1" role="dialog" aria-hidden="true">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="modal-body">
            <div class="text-center">
                Are you sure you want to reject the transaction ?
            </div>
          </div>
          <div class="modal-footer">
              <div class="text-center">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                <asp:Button ID="Button1" runat="server" type="button" CssClass="btn btn-primary" Text="Yes" OnClick="btn_Cancel_Click" />
              </div>
          </div>
        </div>
      </div>
    </div>
    <div class="modal fade" id="TransSubmitAll" tabindex="-1" role="dialog" aria-hidden="true">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="modal-body">
            <div class="text-center">
                Are you sure you want clear all previous pending transactions?
            </div>
          </div>
          <div class="modal-footer">
              <div class="text-center">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                <asp:Button ID="btnSubmitAllCnf" runat="server" type="button" CssClass="btn btn-primary" Text="Yes" OnClick="btnSubmitAllCnf_Click" />
              </div>
          </div>
        </div>
      </div>
    </div>
    <asp:Label ID="isPreview" runat="server" style="display:none;" Text ="0" />
    <asp:Label ID="RolID" runat="server" style="display:none;" Text="0" />
</asp:Content>
