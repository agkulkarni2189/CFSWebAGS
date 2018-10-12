<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" EnableViewState="true" ViewStateEncryptionMode="Auto" AutoEventWireup="true" CodeBehind="DamageImages_temp.aspx.cs" Inherits="UlaWebAgsWF.DamageImages_temp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
    $(document).ready(function () {

        if ($('#<%= isPreview.ClientID %>').text() != "")
        {
                if ($("#<%= isPreview.ClientID %>").text() == "1") {
                    $("#btnApprove").hide();
                    $("#btnReject").hide();
                    //$("#btnBack").show();
                    $("#btnSubmitAll").hide();
                }
                else
                {
                    $("#btnApprove").show();
                    $("#btnReject").show();
                    //$("#btnBack").hide();
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

            //$('#btnBack').click(function (e) {
            //    e.preventDefault();
            //    window.location.replace(document.referrer);
            //});

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
    <div class="container">
        <div class="page-header">
            <h2>
                Container Transaction
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
        <asp:Literal ID="Mainliteral" runat="server"></asp:Literal>
        <br clear="all" />
        <div class="text-center">
            <asp:Label ID="noPreviewTxt" runat="server" CssClass="text-danger h3" Text="" Visible="false"></asp:Label>
        </div>
        <br />

        <asp:CheckBox ID="CB_Damaged" runat="server" CssClass="text-danger" Text="Damaged" AutoPostBack="True" OnCheckedChanged="CB_Damaged_CheckedChanged" />
        <div class="text-center">
            <button type="button" id="btnApprove" class="btn btn-primary" data-toggle="modal" data-target="#TransApprove">Submit</button>
            <%--<button type="button" id="btnBack" class="btn btn-primary">Back</button>--%>
            <asp:Button ID="BtnBack" runat="server" Text="Back" CssClass="btn btn-primary" OnClick="BtnBack_Click" />
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
                    <asp:Button ID="SubmitConfirm" runat="server" type="button" CssClass="btn btn-primary" Text="Yes" OnClick="SubmitConfirm_Click" />
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
                    <asp:Button ID="SubmitCancel" runat="server" type="button" CssClass="btn btn-primary" Text="Yes" OnClick="SubmitCancel_Click" />
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
    </div>
</asp:Content>
