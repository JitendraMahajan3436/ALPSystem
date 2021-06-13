$(document).ready(function (e) {
    $("#added_defect_list").on("change", ".img_Upload", function (e) {
        var imageUploadID = $(this).parent().find('.img_Upload').attr("id");
        var AuditLogID = $("#hdnAudit_Log_ID").val();
        var checklistID = $("#Audit_Checklist_ID").val();
        var checkpointID = imageUploadID.split('_');
        var data = new FormData();
        var files = $("#" + imageUploadID).get(0).files;

        var ext = $("#" + imageUploadID).val().split('.').pop().toLowerCase();
        if ($.inArray(ext, ['gif', 'png', 'jpg', 'jpeg']) == -1) {
            alert('invalid file selected!');
            return false;
        }

        if (files.length > 0) {
            data.append("ImageName", files[0]);
            data.append("AuditLogID", AuditLogID);
            data.append("CheckpointID", checkpointID[1]);
            data.append("checklistID", checklistID);
        }

        $.ajax({
            url: "/QualityAuditCaptures/Upload",
            type: "POST",
            processData: false,
            contentType: false,
            data: data,
            success: function (response) {
                if (response != "") {
                    //code after success    
                    $("#dropzonePreview_" + checkpointID[1]).show();
                    $("#imgPreview_" + checkpointID[1]).attr('href', "/Content/Images/AuditImages/" + response);
                }
                //$("#dropzonePreview_" + checkpointID[1]).hide();

            },
            error: function (er) {
                alert(er);
            }

        });

    });

    var corrID = 0;
    $(".nav-tabs-custom").css("display", "none");
    $("#ddlAnalytics").select2({
        tags: true
    });
    //$("#Analytics_ID").select2({
    //    tags: true
    //});
    var shopId = $("#Shop_ID").val();
    var isProductAudit = $("#hdnAuditCategoryID").val();
    var AuditTypeID = $("#hdnAuditTypeID").val();
    if (isProductAudit == 0) {
        var url = "/QualityAuditCaptures/getAddedChecklistToStationIdShop";
        ajaxpack.getAjaxRequest(url, "shopId=" + shopId + "&stationId=" + 0 + "&family=" + 0 + "&isProductAudit=" + 0 + "&model_code=" + 0 + "&AuditTypeID=" + AuditTypeID, showQualityChecklist, "json");
    }
    var defectId;
    $('.CT_quality_ok').css('display', 'none');
    $(".srlno-input").keydown(function (event) {
        $(".quality_Rejection").hide();
        clearSelectBox("Audit_Checklist_ID");
        clearQualityErrorMessage();
        $(".quality_select_checklist").hide();
        $(".qualitySaveAsDraft").hide();
        $("#added_defect_list").html("");
        $(".nav-tabs-custom").css("display", "none");
        var Line_ID = $("#Line_ID").val();
        var Station_ID = $("#Captured_Station_ID").val();
        var AuditType = $("#hdnAuditCategoryID").val();
        if (event.keyCode == 13) {
            if (AuditType == 1) {
                //if (Line_ID == "" || Station_ID == "") {
                //    if (Line_ID == "" && Station_ID == "") {
                //        $("#Line_Validation").html("Select Line");
                //        $("#Station_Validation").html("Select Station");

                //    }
                //    else if (Line_ID == "") {
                //        $("#Line_Validation").html("Select Line");
                //    } else if (Station_ID == "") {
                //        $("#Line_Validation").html("");
                //        $("#Station_Validation").html("Select Station");
                //    }
                //}
                //    else {
                var shopID = $("#Shop_ID").val();
                if (shopID == 7 || shopID == 8) {
                    $("#Line_Validation").html("");
                    $("#Station_Validation").html("");
                    $("#saveChanges").prop('disabled', false);
                    var serialNo = $(".srlno-input").val();
                    var url = "/QualityAuditCaptures/isValidPartNumber";
                    ajaxpack.postAjaxRequest(url, "partNumber=" + serialNo + "&Shop_ID=" + shopID, chkValidPartNumber, "json");
                }
                else {
                    $("#Line_Validation").html("");
                    $("#Station_Validation").html("");
                    $("#saveChanges").prop('disabled', false);
                    var serialNo = $(".srlno-input").val();
                    var url = "/QualityAuditCaptures/isValidSerialNumber";
                    ajaxpack.postAjaxRequest(url, "SerialNo=" + serialNo, ChkValidSerial, "json");
                }
                //}
            }
            else {
                $("#Line_Validation").html("");
                $("#Station_Validation").html("");
                $("#saveChanges").prop('disabled', false);
                var serialNo = $(".srlno-input").val();
                var model_code = $("#hdnModel_Code").val();
                var shopId = $("#Shop_ID").val();
                var isProductAudit = $("#hdnAuditCategoryID").val();
                var AuditTypeID = $("#hdnAuditTypeID").val();
                var url = "/QualityAuditCaptures/getAddedChecklistToStationIdShop";
                ajaxpack.getAjaxRequest(url, "shopId=" + shopId + "&stationId=" + 0 + "&family=" + 0 + "&isProductAudit=" + isProductAudit + "&model_code=" + 0 + "&AuditTypeID=" + AuditTypeID, showQualityChecklist, "json");

            }
        }

    });

    function chkValidPartNumber() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);

                if (jsonRes.Success == true) {
                    $("#hdnFamily").val(jsonRes.Family);
                    $("#shop-series").html(jsonRes.Series);
                    $("#shop-family").html(jsonRes.FamilyName);
                    var serialNo = $(".srlno-input").val();
                    var AuditTypeID = $("#hdnAuditTypeID").val();
                    var stationId = $("#Captured_Station_ID").val();
                    var family = $("#hdnFamily").val();
                    var url = "/QualityAuditCaptures/getAddedChecklistToStationIdShop";
                    ajaxpack.getAjaxRequest(url, "shopId=" + shopId + "&stationId=" + stationId + "&family=" + family + "&isProductAudit=" + $("#hdnAuditCategoryID").val() + "&model_code=" + serialNo + "&AuditTypeID=" + AuditTypeID, showQualityChecklist, "json");
                }
                else {
                    $('#divCapa').css('display', 'block');
                    $("#srlmessage-block").html("Invalid part number !.");
                    $("#srlmessage-block").addClass("bg-error");
                    $('#FQP_Transmission').css('display', 'none');
                }
            }
        }
    }


    function ChkValidSerial() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);

                if (jsonRes == true) {
                    var serialNo = $(".srlno-input").val();
                    var url = "/QualityAuditCaptures/isAlreadyRejected";
                    ajaxpack.getAjaxRequest(url, "serialNo=" + serialNo, showAlreadyRejection, "json");
                }
                else {
                    $('#divCapa').css('display', 'block');
                    $("#srlmessage-block").html("Invalid Sr.No !.");
                    $("#srlmessage-block").addClass("bg-error");
                    $('#FQP_Transmission').css('display', 'none');
                }
            }
        }
    }
    //Changed On 8th May
    //6 May

    // GetQualityCount();



    if ($("#hdnIsTablet").val() == "True") {
        $(".srlno-input").attr("readonly", true);
        setTimeout(function () {
            loadSerialNumberForTablet();
        }, 2000);

    }
    else {
        $(".srlno-input").removeAttr("readonly");
    }

    function loadSerialNumberForTablet() {
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var stationId = $("#Captured_Station_ID").val();

        var url = "/QualityAuditCaptures/GetSerialNumber";
        ajaxpack.postAjaxRequest(url, "stationId=" + stationId + "&lineId=" + lineId + "&shopId=" + shopId, loadSerialNumberForTabletDetail, "json");
    }

    function loadSerialNumberForTabletDetail() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes.length > 0) {

                    var serialNo = jsonRes[0].Serial_No;
                    $(".srlno-input").val(serialNo);

                    var e = $.Event("keydown");
                    e.keyCode = 13; // # Some key code value
                    $('.srlno-input').trigger(e);

                }
                else {

                }
            }
        }
    }

    function loadSerialDetail() {

        var serialNo = $(".srlno-input").val();
        var Is_Domestic = $('#Is_Domestic').val();

        if (Is_Domestic == 1 || Is_Domestic == 2) {
            if (serialNo) {
                $.ajax({
                    url: '/QualityAuditCaptures/UpdateQualityStatus/',
                    //data: { 'SerialNo': serialNo },
                    data: { 'SerialNo': serialNo, 'Is_Domestic': Is_Domestic },
                    type: 'POST',
                    success: function (result) {
                        if (result == true) {
                            // alert("Updated");
                            $('#frm_Quality_Shead').submit();
                            $("#srlmessage-block").html("Order Closed Successfully.");
                            $("#srlmessage-block").addClass("bg-correct");
                        }
                        else {
                            // alert("Not Updated");
                            $("#srlmessage-block").html("Invalid Sr.No or Order Is Allready closed.");
                            $("#srlmessage-block").addClass("bg-error");
                        }
                    }
                });
            }
            return false;
        }//Is_Domestic
        if (serialNo) {
            //Added by sandy 22 April 2017
            var stationID = $('#hdnStationID').val();
            if (stationID == 90) {
                //alert("Final QP Transmission Line");
                $('#FQP_Transmission').css('display', 'block');
                $('#txtCH').focus();
            }
            else {

                var url = "/QualityAuditCaptures/GetOrderDetails";
                ajaxpack.getAjaxRequest(url, "serialNo=" + $(".srlno-input").val() + "", showSerialDetail, "json");
            }
        }
        return false;
    }

    function showSerialDetail() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes.length > 0) {
                    if (jsonRes[0].AlredyScan == "NULL") {
                        if (jsonRes[0].isPDI == "1") {
                            // alert("isPDI OK");
                            if (jsonRes[0].isPTOResult != "1") {
                                //    alert("isPTOResult Not OK");
                                clearQualityErrorMessage();
                                $("#srlmessage-block").html("PTO Station not ok.");
                                $("#srlmessage-block").addClass("bg-error");
                                return 0;
                            }
                        }
                        //ispdi
                        //alert("Null");
                        if (jsonRes[0].Is_Take_Out > 0) {
                            clearQualityErrorMessage();

                            var isRework = $("#isReworkStation").val();
                            if (isRework == "True") {

                            }
                            else {
                                $("#srlmessage-block").html("This vehicle is taken out.");
                                $("#srlmessage-block").addClass("bg-error");
                                return true;
                            }
                        }
                        $(".re_final_quality_ok").css('display', 'none');
                        $("#shop-series").html(jsonRes[0].Series);
                        $("#shop-family").html(jsonRes[0].Family_Name);
                        $("#hdnSeries").val(jsonRes[0].Series);
                        $("#hdnFamily").val(jsonRes[0].Family);
                        $("#hdnModel_Code").val(jsonRes[0].Model_Code);
                        //$(".quality_remark").show();
                        //$("#Remark").val(jsonRes[0].Remark);
                        // show quality checklist
                        var stationId = $("#Captured_Station_ID").val();
                        var family = $("#hdnFamily").val();
                        //28
                        //$('#shop-series').text(jsonRes[0].Series);
                        //$('#shop-family').text(jsonRes[0].Family);
                        var shopId = $("#Shop_ID").val();
                        var isFinalQualityOK = $("#isFinalQualityStation").val();
                        var isRework = $("#isReworkStation").val();
                        if (isFinalQualityOK == "True" || isRework == "True") {
                            //var url = "/QualityStationChecklist/getAllChecklistByFamily";
                            var url = "/QualityAuditCaptures/getAllChecklistByFamily";
                            ajaxpack.getAjaxRequest(url, "family=" + family, showQualityChecklist, "json");

                        }
                        else {
                            //  if ($("#Line_ID").val() == jsonRes[0].LineID) {

                            $("#Line_ID").val(jsonRes[0].LineID);
                            LineChange();
                            setTimeout(function () {
                                $("#Captured_Station_ID").val(jsonRes[0].Station_ID);
                                var AuditTypeID = $("#hdnAuditTypeID").val();
                                var url = "/QualityAuditCaptures/getAddedChecklistToStationIdShop";
                                ajaxpack.getAjaxRequest(url, "shopId=" + shopId + "&stationId=" + stationId + "&family=" + family + "&isProductAudit=" + $("#hdnAuditCategoryID").val() + "&model_code=" + $("#hdnModel_Code").val() + "&AuditTypeID=" + AuditTypeID, showQualityChecklist, "json");
                            }, 1000);
                            //  }
                            //else {
                            //    $("#srlmessage-block").removeClass("bg-correct");
                            //    $("#srlmessage-block").html("Selected line is not valid for this serial number.");
                            //    $("#srlmessage-block").addClass("bg-error");
                            //}
                        }
                        $(".final_quality_ok").css('display', 'block');
                        $(".re_final_quality_ok").css('display', 'none');
                    }//Allready Scan
                    else {

                        clearQualityErrorMessage();
                        $(".re_final_quality_ok").css('display', 'block');
                        $(".final_quality_ok").css('display', 'none');
                        $("#srlmessage-block").html(jsonRes[0].AlredyScan);
                        $("#srlmessage-block").addClass("bg-error");
                    }
                }
                else {
                    // no record found, process to clear things
                    clearQualityErrorMessage();
                    $("#srlmessage-block").html("Serial Number is invalid.");

                    $("#srlmessage-block").addClass("bg-error");
                }
                //SelectOptionHTML(jsonRes, "Shop_ID");
            }
        }
    }
    $("#Captured_Station_ID").change(function () {
        var stationId = $("#Captured_Station_ID").val();
        if (stationId > 0) {
            $("#Station_Validation").html("");
            //var isProductAudit = parseInt($("#hdnAuditCategoryID").val());
            //if (isProductAudit == 1) {
            //    var family = $("#hdnFamily").val();
            //    var shopId = $("#Shop_ID").val();
            //    var url = "/QualityAuditCaptures/getAddedChecklistToStationIdShop";
            //    ajaxpack.getAjaxRequest(url, "shopId=" + shopId + "&stationId=" + stationId + "&family=" + 0 + "&isProductAudit=" + isProductAudit + "&model_code=" + $("#hdnModel_Code").val(), showQualityChecklist, "json");
            //}
        }
    });

    $(".Rejection").click(function (e) {

        var serialNo = $(".srlno-input").val();
        var result = confirm("Are you Sure Want to Reject Order?");
        if (result) {
            var url = "/QualityAuditCaptures/saveRejectionOrderData";
            ajaxpack.getAjaxRequest(url, "serialNo=" + serialNo, showRejectionDetails, "json");
        }
    });

    function showRejectionDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                $("#srlmessage-block").html("Order is Rejected successfully.");
                $("#srlmessage-block").addClass("bg-correct");
                //refresh
                //clearSelectBox("Audit_Checklist_ID");
                //clearQualityErrorMessage();
                $(".nav-tabs-custom").hide();
                $(".quality_select_checklist").hide();
                $(".qualitySaveAsDraft").hide();
                //$(".quality_remark").hide();
                //$(".quality_remark_edit").hide();
                $(".rework_quality_ok").hide();
                $(".final_quality_ok").hide();
                $(".quality_Rejection").hide();
                $("#added_defect_list").html("");
                $("#added_defect_list1").html("");
            }
        }
    }

    function showQualityChecklist() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                $(".quality_select_checklist").show();
                $(".qualitySaveAsDraft").show();
                SelectOptionHTML(jsonRes, "Audit_Checklist_ID");
                if ($("select#Audit_Checklist_ID").length == 1) {
                    $("select#Audit_Checklist_ID option").eq(1).attr("selected", "selected");
                    $("#Audit_Checklist_ID").trigger("change");
                }
            }
        }
    }

    function showAlreadyRejection() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes == true) {
                    $("#srlmessage-block").html("Order is Rejected.");
                    $("#srlmessage-block").addClass("bg-error");
                }
                else {
                    clearQualityErrorMessage();
                    $(".nav-tabs-custom").show();
                    $(".quality_Rejection").show();
                    $('.CT_quality_ok').css('display', 'block');

                    //$(".quality_Rejection").show();                   
                    //$(".quality_select_checklist").hide();
                    //$(".quality_remark").hide();
                    //$(".quality_remark_edit").hide();                    
                    //$("#added_defect_list").html("");



                    //   loadSerialDetail();

                    var serialNo = $(".srlno-input").val();
                    loadSerialDetail();
                    //setTimeout(function () {
                    //    var url = "/QualityAuditCaptures/getReferenceParts";
                    //    ajaxpack.getAjaxRequest(url, "serialNo=" + serialNo, loadReferencePart, "json");
                    //}, 5000);

                    //setTimeout(function () {
                    //    var url = "/QualityAuditCaptures/getUsers";
                    //    ajaxpack.getAjaxRequest(url, "serialNo=" + serialNo, loadUsers, "json");
                    //}, 12000);
                }

            }

        }
    }

    function SelectOptionHTML(jsonRes, targetId) {
        //var jsonRes = $.parseJSON(myajax.responseText);
        var res = "";
        for (var i = 0; i < jsonRes.length; i++) {
            res += "<option value='" + jsonRes[i].Id + "'>" + jsonRes[i].Value + "</option>";
        }

        res = "<option value=''>" + $("#" + targetId + " option:first").html() + "</option>" + res;
        $("#" + targetId).html(res);
    }

    function clearSelectBox(targetId) {
        var res = "";
        res = "<option value=''>" + $("#" + targetId + " option:first").html() + "</option>";
        $("#" + targetId).html(res);
    }


    $("#Audit_Checklist_ID").change(function (e) {
        $("#saveChanges").prop('disabled', false);
        var checklistId = $(this).val();
        $("#added_defect_list").html("");
        if (checklistId) {
            var url = "/QualityAuditCaptures/isQualityLoggedForSerialNumberWithChecklistId";
            ajaxpack.getAjaxRequest(url, "serialNo=" + $(".srlno-input").val() + "&checklistId=" + checklistId + "&Station_ID=" + $("#Captured_Station_ID").val() + "&Shop_ID=" + $("#Shop_ID").val() + "&Audit_Log_ID=" + $("#hdnAudit_Log_ID").val(), isQualityLoggedForSerial, "json");
        }
    });

    function isQualityLoggedForSerial() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                //alert(jsonRes);
                if (jsonRes == true) {
                    // show history
                    //alert("true");
                    showChecklistHistory();
                    $(".nav-tabs-custom").css("display", "block");

                    //$(".quality_remark_edit").show();
                }
                if (jsonRes == false) {

                    // process to get the list of checkpoint assigned to the checklist
                    setTimeout(function (e) {
                        var url = "/QualityAuditCaptures/getCheckpointByChecklist";
                        ajaxpack.getAjaxRequest(url, "checklist=" + $("#Audit_Checklist_ID").val(), showChecklistDetail, "json");
                    }, 2000);

                    $(".checklistId1").val($("#Audit_Checklist_ID").val());
                    $(".stationId1").val($("#Captured_Station_ID").val());
                    $(".serialNo1").val($(".srlno-input").val());

                    setTimeout(function (e) {
                        $("#frm_show_defect_history_list").submit();
                    }, 1000);



                }

            }
        }
    }

    $(".quality_remark_edit").click(function (e) {

        var stationId = $("#Captured_Station_ID").val();
        var serialNo = $(".srlno-input").val();
        var remark = $("#Remark").val();
        if (remark.length > 0) {

            var url = "/QualityAuditCaptures/editQualityRemark";
            ajaxpack.getAjaxRequest(url, "stationId=" + stationId + "&serialNo=" + serialNo + "&remark=" + remark, showRemarkDetails, "json");
        }
        else {
            $("#srlmessage-block").removeClass("bg-correct");
            $("#srlmessage-block").html("Please enter remark.");
            $("#srlmessage-block").addClass("bg-error");
        }
    });

    function showRemarkDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                $("#srlmessage-block").removeClass("bg-error");
                $("#srlmessage-block").html("Remark is saved successfully.");
                $("#srlmessage-block").addClass("bg-correct");
            }
        }
    }

    function showChecklistDetail() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                $(".checklistId").val($("#Audit_Checklist_ID").val());

                $("#frm_show_defect_list").submit();
                //$("#frm_show_defect_historyWithoutAction_list").submit();
                $(".nav-tabs-custom").css("display", "block");
            }
        }
    }


    $(".quality_checklist_radio").change(function (e) {
        // alert("ok");
    });

    $("#added_defect_list").on("click", ".checkpoint_radio_click i", function (e) {
        debugger;
        var checkpointId = 0;
        checkpointId = $(this).parent().find('.quality_checklist_radio').attr("id");
        checkpointId = checkpointId.split("_");
        checkpointId = checkpointId[1];
        var isValueBased = false;
        if ($("#" + checkpointId + " .hdnIsValueBased").val() == "" || $("#" + checkpointId + " .hdnIsValueBased").val() == null) {
            isValueBased = false;
        }
        else {
            isValueBased = true;
        }
        if ($(this).parent().find('.quality_checklist_radio').prop('checked') === true) {
            if (isValueBased) {
                $("#" + checkpointId + " .defect_value").show("slow");
            }
            else {
                $("#" + checkpointId + " .quality_defect").show("slow");
                $("#fallback_" + checkpointId).show("slow");
                $("#" + checkpointId + " .defect_resolved").show("slow");
                $("#hdnSelectedCheckpoint").val(checkpointId);
                var url = "/AuditDefects/getAddedDefectToCheckpoint";
                ajaxpack.getAjaxRequest(url, "checkpointId=" + checkpointId, showDefectDetail, "json");
                loadStationUsers();
                $("#" + checkpointId + " .quality_defect_category").show("slow");
            }
        }
        else {
            $("#fallback_" + checkpointId).hide("slow");
            $("#" + checkpointId + " .quality_defect").hide("slow");
            $("#" + checkpointId + " .defect_value").hide("slow");
            $("#" + checkpointId + " .defect_resolved").hide("slow");
            $("#" + checkpointId + " .quality_corrective_actions").hide("slow");
            uncheckQualityResolved(checkpointId);
            clearSelectBox("checkpoint_defect_" + checkpointId);
        }
    });

    function loadStationUsers() {
        setTimeout(function (e) {
            var stationId = $("#Captured_Station_ID").val();
            var checkpointId = $("#hdnSelectedCheckpoint").val();
            var shopId = $("#Shop_ID").val();
            if (checkpointId) {
                var url = "/QualityAuditCaptures/GetEmployeeListByCheckpointStation";
                ajaxpack.getAjaxRequest(url, "shopId=" + shopId + "&checkpointId=" + checkpointId, showEmployeeDetails, "json");
            }
        }, 1000);
    }

    function showEmployeeDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "quality_checkpoint_station_users_" + $("#hdnSelectedCheckpoint").val());

                //$("#quality_checkpoint_station_users_" + $("#hdnSelectedCheckpoint").val()).select2({
                //    //tags: true
                //});

            }
        }
    }

    function showDefectDetail() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "checkpoint_defect_" + $("#hdnSelectedCheckpoint").val());
            }
        }
    }

    $("#added_defect_list").on("click", ".defect_resolved i", function (e) {
        var checkpointId = 0;
        checkpointId = $(this).parent().find('.defect_resolved_radio').attr("id");
        checkpointId = checkpointId.split("_");
        checkpointId = checkpointId[2];
        // alert("main");
        if ($(this).parent().find('.defect_resolved_radio').prop('checked') === true) {
            //$("#Lines_ID").val(null).trigger("change");
            //   alert("in");

            $("#" + checkpointId + " .quality_corrective_actions").show("slow");
            $("#" + checkpointId + " .quality_analytics").show("slow");
            $("#fallback_" + checkpointId).show("slow");
            $("#" + checkpointId + " .quality_corrective_actions").val(null).trigger("change");
            $("#" + checkpointId + " .quality_analytics").val(null).trigger("change");

        }
        else {
            $("#" + checkpointId + " .quality_corrective_actions").hide();
            $("#" + checkpointId + " .quality_analytics").hide();
            $("#fallback_" + checkpointId).show("slow");

        }
        $("#" + checkpointId + " .quality_defect_category").show();
    });


    $("#added_defect_list").on("click", "#check_quality_checkpoint", function (e) {
        var isValid = true;
        var qualityCheckpointRes = new Array();
        $(".checkpoint_message").html("");
        $("#hdnNotSelectedUnValueBasedCheckpoint").val("");
        $("#hdnSelectedCheckpoint").val("");
        var isLineRequired = $("#hdnIsLineRequired").val();
        var isStationRequired = $("#hdnIsStationRequired").val();
        isValid = true;
        debugger;
        var Line_ID = $("#Line_ID").val();
        var Station_ID = $("#Captured_Station_ID").val();

        if (isLineRequired == "1" && isStationRequired == "1") {
            if (Line_ID == "") {
                $("#Line_Validation").html("Select Line");
                //  $("#Station_Validation").html("Select Station");
                isValid = false;
            }
            if (Station_ID == "") {
                //$("#Line_Validation").html("Select Line");
                $("#Station_Validation").html("Select Station");
                isValid = false;
            }
        }
        else if (isLineRequired == "1" && Line_ID == "") {
            // if (Line_ID == "") {
            $("#Line_Validation").html("Select Line");
            //  $("#Station_Validation").html("Select Station");
            isValid = false;
            // }
        }
        else if (isStationRequired == "1" && Station_ID == "") {
            //if (Station_ID == "") {
            //  $("#Line_Validation").html("Select Line");
            $("#Station_Validation").html("Select Station");
            isValid = false;
            // }
        }

        //else if ((Line_ID == "" && isLineRequired=="1")) {
        //    $("#Line_Validation").html("Select Line");
        //    isValid = false;
        //} else if ((Station_ID == "" && isStationRequired=="1")) {
        //    $("#Line_Validation").html("");
        //    $("#Station_Validation").html("Select Station");
        //    isValid = false;
        //}
        //}

        //if (Line_ID == "" || Station_ID == "") {
        //    if (Line_ID == "" && Station_ID == "") {
        //        $("#Line_Validation").html("Select Line");
        //        $("#Station_Validation").html("Select Station");
        //        isValid = false;
        //    }
        //    else if (Line_ID == "") {
        //        $("#Line_Validation").html("Select Line");
        //        isValid = false;
        //    } else if (Station_ID == "") {
        //        $("#Line_Validation").html("");
        //        $("#Station_Validation").html("Select Station");
        //        isValid = false;
        //    }
        //}
        //$("#Line_Validation").html("");
        //$("#Station_Validation").html("");

        $(".table tr").each(function (e) {
            var checkpointId = $(this).attr("id");
            var isValueBased = false;
            var isUserValuebased = false;
            var isValueBasedCorrect = true;
            var stationUserId = 0;
            if (checkpointId) {

                if ($("#" + checkpointId + " .hdnIsValueBased").val() == "" || $("#" + checkpointId + " .hdnIsValueBased").val() == null) {
                    isValueBased = false;
                }
                else {
                    isValueBased = true;
                }

                if ($("#" + checkpointId + " .hdnIsUserValueBased").val() == "" || $("#" + checkpointId + " .hdnIsUserValueBased").val() == null) {
                    isUserValuebased = false;
                }
                else {
                    isUserValuebased = true;
                    isValueBased = false;
                }

                if (isUserValuebased) {
                    if ($("#" + checkpointId + " .checkpoint_value").val() == "" || $("#" + checkpointId + " .checkpoint_value").val() == null) {
                        $("#" + checkpointId + " .checkpoint_message").html("Please enter the user value");
                        isValid = false;
                    }
                    else {
                        $("#" + checkpointId + " .checkpoint_message").html(null);
                    }
                }

                if (isValueBased) {
                    // process to check the user enter the value in text box
                    if ($("#" + checkpointId + " .checkpoint_value").val() == "" || $("#" + checkpointId + " .checkpoint_value").val() == null) {
                        $("#" + checkpointId + " .checkpoint_message").html("Please enter the checkpoint value");
                        isValid = false;
                    }
                    else {


                        var value = $("#" + checkpointId + " .checkpoint_value").val();

                        if (!$.isNumeric(value)) {

                            $("#" + checkpointId + " .checkpoint_message").html("Please enter the correct checkpoint value");
                            isValid = false;
                            stationUserId = $("#" + checkpointId + " .quality_station_users").val();
                        }
                        else {

                            // process to check the value is valid or not
                            value = parseFloat(value);
                            var lowerLimit = parseFloat($("#" + checkpointId + " .hdnLowerLimit").val());

                            var upperLimit = parseFloat($("#" + checkpointId + " .hdnUpperLimit").val());

                            if (upperLimit == null || upperLimit == 0 || isNaN(upperLimit)) {
                                // check only with exact lower limit
                                if (value != lowerLimit) {
                                    $("#" + checkpointId + " .checkpoint_message").html("Entered value is not matched with checkpoint value. It will be added as defect.");
                                    isValueBasedCorrect = false;
                                    stationUserId = $("#" + checkpointId + " .quality_station_users").val();
                                }
                            }
                            else {
                                if ((lowerLimit <= value) && (value <= upperLimit)) {
                                    $("#" + checkpointId + " .checkpoint_message").html(null);
                                }
                                else {
                                    $("#" + checkpointId + " .checkpoint_message").html("Entered value is not between checkpoint value. It will be added as defect.");
                                    isValueBasedCorrect = false;
                                    stationUserId = $("#" + checkpointId + " .quality_station_users").val();
                                }
                            }
                        }


                        // process to check the user defect
                        if (isValueBasedCorrect == false) {

                            //if ($("#quality_checkpoint_station_users_" + checkpointId).val() == "-1" || $("#quality_checkpoint_station_users_" + checkpointId).val() == null || $("#quality_checkpoint_station_users_" + checkpointId).val() == "") {
                            //    $("#" + checkpointId + " .checkpoint_message").html("Please select defect user.");
                            //    isValid = false;
                            //}

                            // process to check the defect category is selected or not
                            if ($("#Defect_Category_ID_" + checkpointId).val() == "-1" || $("#Defect_Category_ID_" + checkpointId).val() == "" || $("#Defect_Category_ID_" + checkpointId).val() == null) {
                                $("#" + checkpointId + " .checkpoint_message").html("Please select defect category.");
                                isValid = false;
                            }
                        }

                    }
                }
                else {
                    // check the checkbox is selected or not
                    if ($("#checklist_" + checkpointId).prop('checked') === true) {

                        //alert("not checked");
                        var unselectedCheckpoint = $("#hdnNotSelectedUnValueBasedCheckpoint").val();
                        if (isUserValuebased) { }
                        else
                        {
                            if (unselectedCheckpoint == "" || unselectedCheckpoint == null) {
                                $("#hdnNotSelectedUnValueBasedCheckpoint").val(checkpointId);
                            }
                            else {
                                unselectedCheckpoint = unselectedCheckpoint + "," + checkpointId;
                                $("#hdnNotSelectedUnValueBasedCheckpoint").val(unselectedCheckpoint);
                            }
                        }

                        //alert("checked");
                        // process to check the defect is selected or not
                        if ($("#checkpoint_defect_" + checkpointId).val() == "-1" || $("#checkpoint_defect_" + checkpointId).val() == "" || $("#checkpoint_defect_" + checkpointId).val() == null) {
                            $("#" + checkpointId + " .checkpoint_message").html("Please select defect");
                            isValid = false;
                        }


                        else {

                        }

                        // process to check the defect user is added or not

                        //if ($("#quality_checkpoint_station_users_" + checkpointId).val() == "-1" || $("#quality_checkpoint_station_users_" + checkpointId).val() == null || $("#quality_checkpoint_station_users_" + checkpointId).val() == "") {
                        //    $("#" + checkpointId + " .checkpoint_message").html("Please select defect user.");
                        //    isValid = false;
                        //}
                        // process to check the defect is resolved or not

                        //if ($("#resolved_checklist_" + checkpointId).prop('checked') === true) {
                        //    // process to check the corrective action is selected
                        //    if ($("#quality_corrective_actions_" + checkpointId).val() == "-1" || $("#quality_corrective_actions_" + checkpointId).val() == null || $("#quality_corrective_actions_" + checkpointId).val() == "") {
                        //        $("#" + checkpointId + " .checkpoint_message").html("Please select corrective actions.");
                        //        isValid = false;
                        //    }
                        //    if ($("#Analytics_ID_" + checkpointId).val() == "-1" || $("#Analytics_ID_" + checkpointId).val() == "" || $("#Analytics_ID_" + checkpointId).val() == null) {
                        //        $("#" + checkpointId + " .checkpoint_message").html("Please select analytics.");
                        //        isValid = false;
                        //    }
                        //}
                        //else {
                        //    //alert("checked");
                        //}

                        // process to check the defect category is selected or not

                        if ($("#Defect_Category_ID_" + checkpointId).val() == "-1" || $("#Defect_Category_ID_" + checkpointId).val() == "" || $("#Defect_Category_ID_" + checkpointId).val() == null) {
                            $("#" + checkpointId + " .checkpoint_message").html("Please select defect category.");
                            isValid = false;
                        }

                        //Process to check Analytic is selected or not



                        stationUserId = $("#" + checkpointId + " .quality_station_users").val();
                    }
                    else {
                        ////alert("not checked");
                        //var unselectedCheckpoint = $("#hdnNotSelectedUnValueBasedCheckpoint").val();
                        //if (isUserValuebased) { }
                        //else
                        //{
                        //    if (unselectedCheckpoint == "" || unselectedCheckpoint == null) {
                        //        $("#hdnNotSelectedUnValueBasedCheckpoint").val(checkpointId);
                        //    }
                        //    else {
                        //        unselectedCheckpoint = unselectedCheckpoint + "," + checkpointId;
                        //        $("#hdnNotSelectedUnValueBasedCheckpoint").val(unselectedCheckpoint);
                        //    }
                        //}
                    }
                }

                var Defect_Category_ID = $("#Defect_Category_ID_" + checkpointId).val();
                var Analytics_ID = $("#Analytics_ID_" + checkpointId).val();

                var quality_corrective_actions_ID = $("#quality_corrective_actions_" + checkpointId).val();
                if (quality_corrective_actions_ID != "-1") {
                    corrID = quality_corrective_actions_ID;
                }
                var Checkpoint_Remark = $("#remark_value_" + checkpointId).val();
                var chklistID = $("#Audit_Checklist_ID").val();

                var item = {
                    Audit_Checklist_ID: chklistID,
                    Checkpoint_ID: checkpointId,
                    Is_Value_Based: isValueBased,
                    Lower_Limit: $("#" + checkpointId + " .hdnLowerLimit").val(),
                    Upper_Limit: $("#" + checkpointId + " .hdnUpperLimit").val(),
                    User_Value: $("#checkpoint_value_" + checkpointId).val(),
                    Remark: $("#remark_" + checkpointId).val(),
                    Defect_ID: $("#checkpoint_defect_" + checkpointId).val(),
                    Is_Value_Based_Correct: isValueBasedCorrect,
                    Corrective_Actions_ID: corrID,
                    Is_Rework_Done: $("#resolved_checklist_" + checkpointId).prop("checked"),
                    Defect_Name: $("#checkpoint_defect_" + checkpointId + " option:selected").text(),
                    Corrective_Actions_Name: $("#quality_corrective_actions_" + checkpointId).val(),
                    Is_User_Value_Based: isUserValuebased,
                    Defect_Category_ID: Defect_Category_ID,
                    Checkpoint_Station_User_ID: stationUserId,
                    Analytics_ID: Analytics_ID,
                    Checkpoint_Remark: Checkpoint_Remark,
                    Mean_Value: $("#" + checkpointId + " .hdnMeanValue").val(),
                    Multiplication_Factor: $("#" + checkpointId + " .hdnMultiplicationFactor").val(),
                    Master_Ring_Value: $("#" + checkpointId + " .hdnMasterRingValue").val(),
                    Checkpoint_Category_ID: $("#" + checkpointId + " .hdnCategory").val(),
                    Is_Checkpoint_Applicable: $('#checkpointApplicable_' + checkpointId).prop('checked'),
                    Family_ID: $("#hdnFamily").val(),
                    Model_ID: $("#hdnModel_Code").val()
                };




                if (qualityCheckpointRes.length == 0) {
                    qualityCheckpointRes[0] = item;
                }
                else {
                    qualityCheckpointRes[qualityCheckpointRes.length + 1] = item;
                }
            }

        });//s



        $("#hdnQualityCheckpointRes").val(JSON.stringify(qualityCheckpointRes));
        if (isValid) {
            var unselectedUnValueBasedCheckpoint = $("#hdnNotSelectedUnValueBasedCheckpoint").val();
            if (unselectedUnValueBasedCheckpoint) {
                // process to get the list of checkpoint
                var url = "/AuditCheckpoint/getCheckpointById";
                ajaxpack.getAjaxRequest(url, "checkpointId=" + unselectedUnValueBasedCheckpoint, showCheckpointDetails, "json");

            }
            else {

                submitValue();
            }
        }

    });

    $("#btn_model_not_availble").click(function (e) {
        var isValid = true;
        debugger;
        var Line_ID = $("#Line_ID").val();
        var Station_ID = $("#Captured_Station_ID").val();
        if (Line_ID == "" || Station_ID == "") {
            if (Line_ID == "" && Station_ID == "") {
                $("#Line_Validation").html("Select Line");
                $("#Station_Validation").html("Select Station");
                isValid = false;
            }
            else if (Line_ID == "") {
                $("#Line_Validation").html("Select Line");
                isValid = false;
            } else if (Station_ID == "") {
                $("#Line_Validation").html("");
                $("#Station_Validation").html("Select Station");
                isValid = false;
            }
        }

        if (isValid == true) {
            var plantId = $("#Plant_ID").val();
            var shopId = $("#Shop_ID").val();
            var lineId = $("#Line_ID").val();
            var stationId = $("#Captured_Station_ID").val();
            var userId = $("#Captured_User_ID").val();
            var Audit_Log_ID = $("#hdnAudit_Log_ID").val();

            var url = "/QualityAuditCaptures/saveAuditModelNotavailble";
            $("#loading_global_spinner_image").show();
            ajaxpack.postAjaxRequest(url, "plantId=" + plantId + "&shopId=" + shopId + "&lineId=" + lineId + "&stationId=" + stationId + "&userId=" + userId + "&Audit_Log_ID=" + Audit_Log_ID, showAuditModelNotAvailable, "json");
        }
    });

    function showAuditModelNotAvailable() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                $("#loading_global_spinner_image").hide();
                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes == true) {
                    $("#srlmessage-block").removeClass("bg-error");
                    $("#srlmessage-block").addClass("bg-correct");
                    $("#srlmessage-block").html("Audit Captured Successfully.");
                }
                else {
                    $("#srlmessage-block").removeClass("bg-correct");
                    $("#srlmessage-block").html("Failed to Save");
                    $("#srlmessage-block").addClass("bg-error");
                }
            }
        }
    }

    $("#btn_Save_Audit_Draft").click(function (e) {
        var isValid = true;
        var qualityCheckpointRes = new Array();
        $(".checkpoint_message").html("");
        $("#hdnNotSelectedUnValueBasedCheckpoint").val("");
        $("#hdnSelectedCheckpoint").val("");
        isValid = true;
        $(".table tr").each(function (e) {

            var checkpointId = $(this).attr("id");
            var isValueBased = false;
            var isUserValuebased = false;
            var isValueBasedCorrect = true;
            var stationUserId = 0;
            if (checkpointId) {

                if ($("#" + checkpointId + " .hdnIsValueBased").val() == "" || $("#" + checkpointId + " .hdnIsValueBased").val() == null) {
                    isValueBased = false;
                }
                else {
                    isValueBased = true;
                }

                if ($("#" + checkpointId + " .hdnIsUserValueBased").val() == "" || $("#" + checkpointId + " .hdnIsUserValueBased").val() == null) {
                    isUserValuebased = false;
                }
                else {
                    isUserValuebased = true;
                    isValueBased = false;
                }

                var Defect_Category_ID = $("#Defect_Category_ID_" + checkpointId).val();
                var Analytics_ID = $("#Analytics_ID_" + checkpointId).val();
                var quality_corrective_actions_ID = $("#quality_corrective_actions_" + checkpointId).val();
                if (quality_corrective_actions_ID != "-1") {
                    corrID = quality_corrective_actions_ID;
                }
                var Checkpoint_Remark = $("#remark_value_" + checkpointId).val();
                var chklistID = $("#Audit_Checklist_ID").val();
                var item = {
                    Audit_Checklist_ID: chklistID,
                    Checkpoint_ID: checkpointId,
                    Is_Value_Based: isValueBased,
                    Lower_Limit: $("#" + checkpointId + " .hdnLowerLimit").val(),
                    Upper_Limit: $("#" + checkpointId + " .hdnUpperLimit").val(),
                    User_Value: $("#checkpoint_value_" + checkpointId).val(),
                    Remark: $("#remark_" + checkpointId).val(),
                    Defect_ID: $("#checkpoint_defect_" + checkpointId).val(),
                    Is_Value_Based_Correct: isValueBasedCorrect,
                    Corrective_Actions_ID: corrID,
                    Is_Rework_Done: $("#resolved_checklist_" + checkpointId).prop("checked"),
                    Defect_Name: $("#checkpoint_defect_" + checkpointId + " option:selected").text(),
                    Corrective_Actions_Name: $("#quality_corrective_actions_" + checkpointId).val(),
                    Is_User_Value_Based: isUserValuebased,
                    Defect_Category_ID: Defect_Category_ID,
                    Checkpoint_Station_User_ID: stationUserId,
                    Analytics_ID: Analytics_ID,
                    Checkpoint_Remark: Checkpoint_Remark,
                    Is_Value_BasedChecked: $('#checklist_' + checkpointId).prop('checked'),
                    Is_Checkpoint_Applicable: $('#checkpointApplicable_' + checkpointId).prop('checked'),
                    Family_ID: $("#hdnFamily").val(),
                    Model_ID: $("#hdnModel_Code").val()
                };
                if (qualityCheckpointRes.length == 0) {
                    qualityCheckpointRes[0] = item;
                }
                else {
                    qualityCheckpointRes[qualityCheckpointRes.length + 1] = item;
                }
            }

        });//s



        $("#hdnQualityCheckpointRes").val(JSON.stringify(qualityCheckpointRes));
        submitDraft();

    });

    function submitDraft() {
        // $("#btnAuditSubmit").hide();
        var plantId = $("#Plant_ID").val();
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var stationId = $("#Captured_Station_ID").val();
        var serialNumber = $(".srlno-input").val();
        var qualityRes = $("#hdnQualityCheckpointRes").val();
        //var userId = $("#Captured_User_ID").val();
        var userId = $("#Captured_User_ID").val();
        //check capa available for that station
        var remark = $("#Remark").val();
        var Audit_Log_ID = $("#hdnAudit_Log_ID").val();
        var url = "/QualityAuditCaptures/saveAuditDraft";
        $("#loading_global_spinner_image").show();
        ajaxpack.postAjaxRequest(url, "plantId=" + plantId + "&shopId=" + shopId + "&lineId=" + lineId + "&stationId=" + stationId + "&serialNumber=" + serialNumber + "&userId=" + userId + "&qualityData=" + qualityRes + "&remark=" + remark + "&Audit_Log_ID=" + Audit_Log_ID, showDraft, "json");
    }


    function showDraft() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                $("#loading_global_spinner_image").hide();
                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes.msg == "Success") {
                    $("#srlmessage-block").removeClass("bg-error");
                    $("#srlmessage-block").addClass("bg-correct");
                    $("#srlmessage-block").html("Draft Saved Successfully.");
                    $("#btnAuditSubmit").show();
                }
                else {
                    $("#srlmessage-block").removeClass("bg-correct");
                    $("#srlmessage-block").html("Failed to Save");
                    $("#srlmessage-block").addClass("bg-error");
                }
            }
        }
    }
    function submitValue() {
        $("#btnAuditSubmit").hide();
        var plantId = $("#Plant_ID").val();
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var stationId = $("#Captured_Station_ID").val();
        var serialNumber = $(".srlno-input").val();
        var qualityRes = $("#hdnQualityCheckpointRes").val();
        //var userId = $("#Captured_User_ID").val();
        var userId = $("#Captured_User_ID").val();
        $("#qualitySaveAsDraft").hide();
        //check capa available for that station
        var remark = $("#Remark").val();
        var Audit_Log_ID = $("#hdnAudit_Log_ID").val();
        var url = "/QualityAuditCaptures/saveQualityData";
        $("#loading_global_spinner_image").show();
        //alert("plantId=" + plantId + "&shopId=" + shopId + "&lineId=" + lineId + "&stationId=" + stationId + "&serialNumber=" + serialNumber + "&userId=" + userId + "&remark=" + remark + "&Audit_Log_ID=" + Audit_Log_ID + " fileData: " + fileData);
        //$.ajax({
        //    type: "POST",
        //    url: "/QualityAuditCaptures/saveQualityData",
        //    dataType: "json",
        //    contentType: false, // Not to set any content header
        //    processData: false, // Not to process data
        //    data: { fileData: fileData ,plantId:plantId ,shopId:shopId ,lineId:lineId ,stationId:stationId ,serialNumber:serialNumber,userId: userId ,qualityData: qualityRes,remark:remark , Audit_Log_ID: Audit_Log_ID},
        //    success: function (result, status, xhr) {
        //        alert(result);
        //    },
        //    error: function (xhr, status, error) {
        //        alert(status);
        //    }
        //});

        ajaxpack.postAjaxRequest(url, "plantId=" + plantId + "&shopId=" + shopId + "&lineId=" + lineId + "&stationId=" + stationId + "&serialNumber=" + serialNumber + "&userId=" + userId + "&qualityData=" + qualityRes + "&remark=" + remark + "&Audit_Log_ID=" + Audit_Log_ID, showQualityLog, "json");
    }

    function UploadFile(checkpointID, AuditLogID) {
        alert(checkpointID + " " + AuditLogID);
    }

    function showQualityLog() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                $("#loading_global_spinner_image").hide();
                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes.msg == "OTHER") {
                    $('.datatable_completes').empty();
                    $('#shop-Quality').html(jsonRes.qualitycount);
                    showChecklistHistory();
                    closePopup();
                    if ($("#hdnIsTablet").val() == "True") {
                        $(".srlno-input").attr("readonly", true);
                        setTimeout(function () {
                            loadSerialNumberForTablet();
                        }, 2000);
                    }
                    $("#srlmessage-block").removeClass("bg-error");
                    $("#srlmessage-block").addClass("bg-correct");
                    $("#srlmessage-block").html("Audit captured successfully.");

                }
                else if (jsonRes.msg == "CT") {
                    $("#srlmessage-block").removeClass("bg-error");
                    $("#srlmessage-block").addClass("bg-correct");
                    $("#srlmessage-block").html("CT Quality captured successfully.");
                    showChecklistHistory();
                    closePopup();
                    $('.CT_quality_ok').css('display', 'block');

                    if (jsonRes.is_road_test == 1) {
                        //alert("Popup");22
                        $('#quality_road_test').modal("show");
                    }

                }
                else if (jsonRes.msg == "CT_Error") {
                    $("#btnAuditSubmit").show();
                    $("#srlmessage-block").removeClass("bg-correct");
                    $("#srlmessage-block").html("Error occured while capturing audit");
                    $("#srlmessage-block").addClass("bg-error");
                    $('#quality_not_checked_checkpoint').modal("hide");
                    showChecklistHistory();
                    closePopup();
                }
                    //

                else if (jsonRes.msg == "ROAD") {
                    $("#srlmessage-block").removeClass("bg-error");
                    $("#srlmessage-block").addClass("bg-correct");
                    $("#srlmessage-block").html("Audit captured successfully.");
                    showChecklistHistory();
                    closePopup();
                }

                else if (jsonRes.msg == "ROAD_Error") {
                    $("#btnAuditSubmit").show();
                    $("#srlmessage-block").removeClass("bg-correct");
                    $("#srlmessage-block").html("Error occured while capturing audit.");
                    $("#srlmessage-block").addClass("bg-error");
                    $('#quality_not_checked_checkpoint').modal("hide");
                    showChecklistHistory();
                    closePopup();
                }

                else if (jsonRes.msg == "PDI") {
                    $("#srlmessage-block").removeClass("bg-error");
                    $("#srlmessage-block").addClass("bg-correct");
                    $("#srlmessage-block").html("Audit captured successfully.");
                    showChecklistHistory();
                    closePopup();
                }

                else if (jsonRes.msg == "PDI_Error") {
                    $("#btnAuditSubmit").show();
                    $("#srlmessage-block").removeClass("bg-correct");
                    $("#srlmessage-block").html("Error occured while capturing audit.");
                    $("#srlmessage-block").addClass("bg-error");
                    $('#quality_not_checked_checkpoint').modal("hide");
                    showChecklistHistory();
                    closePopup();
                }


                else if (jsonRes.msg == "Error") {
                    $("#btnAuditSubmit").show();
                    $("#srlmessage-block").removeClass("bg-correct");
                    $("#srlmessage-block").html("Error occured while capturing audit.");
                    $("#srlmessage-block").addClass("bg-error");
                    $('#quality_not_checked_checkpoint').modal("hide");
                }
                    //
                else {
                    $("#srlmessage-block").removeClass("bg-correct");
                    $("#srlmessage-block").html("Please close capa");
                    $("#srlmessage-block").addClass("bg-error");
                    $('#quality_not_checked_checkpoint').modal("hide");
                }

            }
        }
    }

    function showChecklistHistory() {
        //alert("in");
        $(".checklistId1").val($("#Audit_Checklist_ID").val());
        $(".stationId1").val($("#Captured_Station_ID").val());
        $(".serialNo1").val($(".srlno-input").val());
        //$('.datatable_completes').empty();
        //$("#frm_show_defect_history_list").submit($("#Audit_Checklist_ID").val(), $("#Captured_Station_ID").val(), $(".srlno-input").val());
        $("#frm_show_defect_history_list").submit();

        setTimeout(function () {
            if ($('#rowCount').val() > 0) {
                $(".quality_pint").css("display", "block");
            }
            else {
                //alert("else");
                $(".quality_pint").css("display", "none");
            }
        }, 3000);


        $("#frm_show_defect_historyWithoutAction_list").submit();
        $(".qualitySaveAsDraft").hide();
        //getChecklistHistoryHTMLWithoutAction
    }

    $(".save_quality_data").click(function (e) {
        $("#saveChanges").prop('disabled', true);
        submitValue();
    });

    function showCheckpointDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                var res = "";
                $("#quality_checkpoint_table").html("");
                for (var i = 0; i < jsonRes.length; i++) {
                    res = res + "<tr><td>" + jsonRes[i].Value + "</td></tr>";
                }
                $("#quality_checkpoint_table").html(res);
                $("#quality_not_checked_checkpoint_popup").trigger("click");
            }
        }
    }

    $("#added_defect_list1").on("click", ".quality_edit_popup", function (e) {
        //check capa is close or not
        //$('#lblMSg').html(null);

        defectId = $(this).attr("id");
        $("#hdnQualityId").val(defectId);
        var url = "/QualityAuditCaptures/getDefectDetailById";
        clearQualityEditErrorPopup();
        ajaxpack.postAjaxRequest(url, "defectId=" + defectId, ShowQualityEditDetails, "json");
        clearQualityEditValuePopup();
        $('#serialNo_part').val($(".srlno-input").val());
        $('#defectID_part').val(defectId);
        $('#frm_show_defectPartList').submit();

    });

    function checkCapaStatusDefectId() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) {
                var jsonRes = $.parseJSON(myajax.responseText);
                if (!jsonRes) {

                    $("#srlmessage-block").removeClass("bg-correct");
                    $("#srlmessage-block").html("Please close capa");
                    $("#srlmessage-block").addClass("bg-error");
                    //$('#quality_not_checked_checkpoint').modal("hide");
                }
                else {
                    //$("#hdnQualityId").val(defectId);
                    ////alert(defectId);
                    //var url = "/QualityAuditCaptures/getDefectDetailById";
                    //clearQualityEditErrorPopup();
                    //ajaxpack.postAjaxRequest(url, "defectId=" + defectId, ShowQualityEditDetails, "json");

                    //clearQualityEditValuePopup();
                    //$('#serialNo_part').val($(".srlno-input").val());
                    //$('#defectID_part').val(defectId);
                    //$('#frm_show_defectPartList').submit();

                    var qualityId = $('#hdnQualityId').val();
                    var url = "/QualityAuditCaptures/updateConfirmDetails";
                    ajaxpack.postAjaxRequest(url, "qualityId=" + qualityId, ShowConfirmDetails, "json");
                }
            }

        }
    }


    function ShowQualityEditDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally



                $(".quality_value_based_edit_popup").hide();

                $(".quality_defect_edit_popup").hide();
                $(".quality_corrective_actions_popup").hide();
                $(".quality_edit_rework_man_power_time_popup").hide();
                var jsonRes = $.parseJSON(myajax.responseText);
                //alert(jsonRes[0].Checkpoint_Name);
                $("#edit_checkpoint_name").val(jsonRes[0].Checkpoint_Name);
                $("#edit_is_checkpoint_value_based").val(jsonRes[0].Is_Value_Based);
                if (jsonRes[0].Is_Value_Based == true) {
                    // process to show the user entered value
                    $(".quality_value_based_edit_popup").show();
                    $("#edit_user_logged_checkpoint_value").val(jsonRes[0].User_Checkpoint_Value);
                    $("#edit_user_checkpoint_value").val(jsonRes[0].User_Checkpoint_Corrective_Value);

                    // check if it is first shot clear
                    if (jsonRes[0].Is_First_Shot_Clear == true) {
                        // process to hide button and fields                        
                        $(".quality_edit_rework_man_power_time_popup").hide();
                    }
                    else
                        if (jsonRes[0].Is_Clear == true) {
                            checkQualityEditClear();
                            $(".quality_edit_rework_man_power_time_popup").show();
                        }
                        else {
                            uncheckQualityEditClear();
                        }

                    $(".quality_clear_defect_popup").hide();

                    if (jsonRes[0].Is_Clear == true) {
                        $("#hdnIsCheckpointValueCorrect").val("true");
                    }

                    SelectOptionHTML(jsonRes[0].Checkpoint_Station_Users, "edit_quality_defect_user");
                    $("select#edit_quality_defect_user").val(jsonRes[0].Checkpoint_Station_User_ID);
                }
                else {
                    // check if it is first shot clear
                    if (jsonRes[0].Is_First_Shot_Clear == true) {
                        // process to hide button and fields
                        $(".quality_clear_defect_popup").hide();
                        $(".quality_edit_rework_man_power_time_popup").hide();
                        $(".quality_defect_edit_popup").hide();
                    }
                    else
                        if (jsonRes[0].Is_Clear == true) {
                            checkQualityEditClear();
                            $(".quality_defect_edit_popup").show();
                            $(".quality_edit_rework_man_power_time_popup").show();
                            $(".quality_corrective_actions_popup").show();
                            $(".quality_clear_defect_popup").show();
                        }
                        else {
                            uncheckQualityEditClear();
                            $(".quality_defect_edit_popup").show();
                            $(".quality_clear_defect_popup").show();
                        }

                    //$(".quality_corrective_actions_popup").show();

                    SelectOptionHTML(jsonRes[0].Defect_ID, "edit_quality_defect");
                    $("select#edit_quality_defect").val(jsonRes[0].Selected_Defect_ID);

                    SelectOptionHTML(jsonRes[0].Corrective_Action_ID, "edit_quality_corrective_actions");
                    $("#edit_quality_defect").select2({
                        tags: true
                    });

                    $("select#edit_quality_corrective_actions").val(jsonRes[0].Selected_Corrective_Actions_ID);
                    $("#edit_quality_corrective_actions").select2({
                        tags: true
                    });

                    SelectOptionHTML(jsonRes[0].Checkpoint_Station_Users, "edit_quality_defect_user");
                    $("select#edit_quality_defect_user").val(jsonRes[0].Checkpoint_Station_User_ID);

                    //$("select#edit_quality_corrective_actions").val(jsonRes[0].Selected_Corrective_Actions_ID);
                }


                if (jsonRes[0].Is_First_Shot_Clear == true) {
                    // process to hide corrective actions, rework man power and rework man time
                    $(".quality_corrective_actions_popup").hide();
                    $(".quality_edit_rework_man_power_time_popup").hide();

                }
                else
                    if (jsonRes[0].Is_Clear == true) {

                    }

                $("#edit_rework_man_power").val(jsonRes[0].Rework_Man_Power);
                if (jsonRes[0].Rework_Man_Minutes != null) {
                    $("#edit_rework_man_hours").val(jsonRes[0].Rework_Man_Minutes["Hours"]);
                    $("#edit_rework_man_minute").val(jsonRes[0].Rework_Man_Minutes["Minutes"]);
                }
                $("#Defect_Category_ID").val(jsonRes[0].Defect_Category_ID);
                $("#quality_edit_checkpoint_popup").trigger("click");

                $("#hdnIsValuedBased").val(jsonRes[0].Is_Value_Based);
                $("#hdnQualityIsFirstShotClear").val(jsonRes[0].Is_First_Shot_Clear);
                $("#hdnQualityIsClear").val(jsonRes[0].Is_Clear);
                $("#hdnLowerLimit").val(jsonRes[0].Lower_Limit);
                $("#hdnUpperLimit").val(jsonRes[0].Upper_Limit);
            }
        }
    }

    $("#edit_quality_defect").change(function (e) {


        var defectId = $(this).val();
        clearSelectBox("edit_quality_corrective_actions");
        if (defectId) {
            var url = "/QualityAuditCaptures/getCorrectiveActionByDefectId";
            ajaxpack.postAjaxRequest(url, "defectId=" + defectId, ShowQualityEditCorrectiveActions, "json");
        }

    });

    function ShowQualityEditCorrectiveActions() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);

                SelectOptionHTML(jsonRes, "edit_quality_corrective_actions");

            }
        }
    }

    //Jitendra Mahajan
    $("#added_defect_list1").on("click", ".quality_confirm", function (e) {
        var defectId = $(this).attr("id");
        $("#hdnQualityId").val(defectId);
        var url = "/QualityAuditCaptures/checkCapaStatusDefectId";
        ajaxpack.postAjaxRequest(url, "defectId=" + defectId, checkCapaStatusDefectId, "json");
    });

    function ShowConfirmDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes) {
                    $("#srlmessage-block").removeClass("bg-error");
                    $("#srlmessage-block").html("Confirmed data Sucessfully.");
                    $("#srlmessage-block").addClass("bg-correct");
                    showChecklisqualitySaveAsDrafttHistory();
                }
                else {
                    $("#srlmessage-block").addClass("bg-error");
                    $("#srlmessage-block").html("Please clear the Defect.");
                    $("#srlmessage-block").removeClass("bg-correct");
                    showChecklistHistory();
                }

            }
        }
    }

    function clearEditQualityPopup() {
        clearSelectBox("edit_quality_defect");
        clearSelectBox("edit_quality_corrective_actions");
        $("#edit_rework_man_power").val("");
        $("#edit_rework_man_minute").val("");
    }

    $("#quality_edit_checkpoint").on("click", ".checkpoint_radio_edit_click i", function (e) {
        var isValuedBased = $("#hdnIsValuedBased").val();
        if ($(this).parent().find('#edit_quality_clear_defect').prop('checked') === true) {
            // show rework man power and rework man time

            if (isValuedBased == "false") {

                $(".quality_corrective_actions_popup").show();
            }
            else {
                $(".quality_defect_edit_popup").hide();
            }

            $(".quality_non_value_based_edit_popup").show();
            $(".quality_edit_rework_man_power_time_popup").show();

        }
        else {

            $(".quality_corrective_actions_popup").hide();
            $(".quality_non_value_based_edit_popup").hide();
            $(".quality_corrective_actions_popup").hide();
            $(".quality_edit_rework_man_power_time_popup").hide();
        }
        //checkpointId = checkpointId.split("_");
        //checkpointId = checkpointId[1];
    });

    function uncheckQualityEditClear() {
        $("#edit_quality_clear_defect").removeAttr("checked");
        $(".checkpoint_radio_edit_click i").removeClass("tt-switch-on");
        $(".checkpoint_radio_edit_click i").addClass("tt-switch-off");
        $(".checkpoint_radio_edit_click i").css("color", "#999999");
    }

    function checkQualityEditClear() {
        $("#edit_quality_clear_defect").attr("checked", true);
        $(".checkpoint_radio_edit_click i").removeClass("tt-switch-off");
        $(".checkpoint_radio_edit_click i").addClass("tt-switch-on");
        $(".checkpoint_radio_edit_click i").css("color", "#009933");
    }

    function uncheckQualityResolved(checkpointId) {
        $("#resolved_checklist_" + checkpointId).removeAttr("checked");
        $("#" + checkpointId + " .defect_resolved i").removeClass("tt-switch-on");
        $("#" + checkpointId + " .defect_resolved i").addClass("tt-switch-off");
        $("#" + checkpointId + " .defect_resolved i").css("color", "#999999");
    }

    $('.CT_quality_ok').click(function () {
        var serialNo = $(".srlno-input").val();
        var plantId = $("#Plant_ID").val();
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var stationId = $("#Captured_Station_ID").val();
        var reprint = "print";
        if (serialNo != null) {
            // alert(serialNo);
            var url = "/QualityAuditCaptures/CTQualityOkPrint";
            //ajaxpack.getAjaxRequest(url, "serialNo=" + serialNo, CTQualityOk, "json");
            ajaxpack.postAjaxRequest(url, "serialNo=" + serialNo + "&plantId=" + plantId + "&shopId=" + shopId + "&lineId=" + lineId + "&stationId=" + stationId + "&isReprint=" + reprint, CTQualityOk, "json");
        }

    })




    function CTQualityOk() {//6
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes == true) {
                    // show error message
                    clearQualityErrorMessage();
                    $("#srlmessage-block").html("Final quality done.");
                    $("#srlmessage-block").addClass("bg-correct");

                    $('#FQP_Transmission').css('display', 'none');
                    $(".final_quality_ok").css('display', 'none');//2 may
                    //$(".re_final_quality_ok").css('display', 'block');
                }
                else {
                    // show error message
                    clearQualityErrorMessage();
                    $("#srlmessage-block").html("Please clear the defects and try again.");
                    $("#srlmessage-block").addClass("bg-error");
                }
            }
        }

    }

    $(".edit_quality_data").click(function (e) {
        var serialNo = $(".srlno-input").val();
        var capapartReferencePiece = $("#Refrence_Piece").val();
        var capapartReferencePeiceDesc = $("#Refrence_Piece_Desc").val();
        var capapartQuantity = $("#Quantity").val();
        var defectId = $("#hdnQualityId").val();
        //alert(capapartReferencePiece);
        if (capapartReferencePiece != null) {
            //alert(serialNo);
            var url = "/QualityAuditCaptures/isDuplicateRefrencePart";
            ajaxpack.getAjaxRequest(url, "capapartReferencePiece=" + capapartReferencePiece + "&serialNo=" + serialNo + "&defectId=" + defectId, duplicateRefrencePart, "json");
        }
        else {

            //alert("hello");
            if ($("#hdnQualityIsFirstShotClear").val() == "true") {
                return;
            }
            var isValid = true;
            clearQualityEditErrorPopup();

            var isResolved = true;

            if ($('#ddluser').val().length > 0 && $('#ddluser').val() != "") {

            }
            else {
                $('#lblUsers').html('Required');
                isValid = false;
            }
            if ($('#Analytics').val().length > 0 && $('#Analytics').val() != "") {
                $('#lblAnalytics').html(null);
            }
            else {
                $('#lblAnalytics').html('Required');
                isValid = false;
            }

            var isValueBased = $("#hdnIsValuedBased").val();
            var userCheckpointValue = 0;
            if (isValueBased == "true") {

                if ($("#edit_user_checkpoint_value").val() == "" || $("#edit_user_checkpoint_value").val() == null) {
                    isValid = false;
                    $("#errorQualityEditCheckpointUserValue").html("Required");
                }
                else if (!$.isNumeric($("#edit_user_checkpoint_value").val())) {
                    isValid = false;
                    $("#errorQualityEditCheckpointUserValue").html("Invalid");
                }
                else {
                    $("#errorQualityEditCheckpointUserValue").html("");
                }

                if ($("#hdnIsCheckpointValueCorrect").val() == "true") {

                }
                else {
                    isValid = false;
                    //$("#errorQualityEditCheckpointValue").html("Invalid");
                }
            }
            else {
                // process need to check whether user checked the clear defect
                if ($("#edit_quality_clear_defect").prop("checked") === true) {
                    $("#errorQualityEditCorrectiveActions").html("");
                    if ($("#edit_quality_corrective_actions").val() == "" || $("#edit_quality_corrective_actions").val() == null) {
                        isValid = false;
                        $("#errorQualityEditCorrectiveActions").html("Please select");
                        return;
                    }

                    if (checkReworkManPowerAndReworkManTime() == false) {
                        isValid = false;
                    }
                }
                else {
                    isResolved = false;
                    //isValid = false;
                }

            }

            if (isValid) {
                var userValue = $("#edit_user_checkpoint_value").val();
                var reworkManPower = $("#edit_rework_man_power").val();
                var reworkManHours = $("#edit_rework_man_hours").val();
                var reworkManMinutes = $("#edit_rework_man_minute").val();
                var correctiveActions = $("#edit_quality_corrective_actions").val();
                if (correctiveActions == "" || correctiveActions == null || (!$.isNumeric(correctiveActions))) {
                    correctiveActions = 0;
                }

                var defectId = $("#hdnQualityId").val();
                var isCheckpointValueBasedCorrect = $("#hdnIsCheckpointValueCorrect").val();
                var qualityDefectId = $("#edit_quality_defect").val();
                if (qualityDefectId == "" || qualityDefectId == null || (!$.isNumeric(qualityDefectId))) {
                    qualityDefectId = 0;
                }
                var resolvedStationId = $("#Captured_Station_ID").val();

                // var userId = $("#Captured_User_ID").val();
                var userId = $("#ddluser").val();

                var defectCategory = $("#Defect_Category_ID").val();
                if ($("#Defect_Category_ID").val() == "" || $("#Defect_Category_ID").val() == null) {
                    //qualityDefectId = 0;
                    //$("#errorDefectCategory").html("Please select");
                    //return;
                }
                else {
                    $("#errorQualityDefectCategoryId").html("");
                }

                if (checkReworkManPowerAndReworkManTime() == false) {
                    alert("Please clear the defect and save");
                    return;
                }

                var plantId = $("#Plant_ID").val();
                var checkpointUserId = $("#edit_quality_defect_user").val();
                checkpointUserId = 0;
                var url = "/QualityAuditCaptures/updateQualityDefect";
                ajaxpack.postAjaxRequest(url, "defectId=" + defectId + "&plantId=" + plantId + "&isValueBased=" + isValueBased + "&isCheckpointValueBasedCorrect=" + isCheckpointValueBasedCorrect + "&userValue=" + userValue + "&reworkManPower=" + reworkManPower + "&reworkManHours=" + reworkManHours + "&reworkManMinutes=" + reworkManMinutes + "&qualityDefectId=" + qualityDefectId + "&correctiveActions=" + correctiveActions + "&userId=" + userId + "&stationId=" + resolvedStationId + "&isResolved=" + isResolved + "&defectName=" + $("#edit_quality_defect").val() + "&correctiveActionName=" + $("#edit_quality_corrective_actions").val() + "&defectCategory=" + defectCategory + "&checkpointUserId=" + checkpointUserId + "&analytics=" + $('#Analytics').val(), showQualityResolveDetails, "json");
            }
        }

    });

    function showQualityResolveDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                closePopup();
                $('#frm_show_defect_history_list').submit();
                $("#Audit_Checklist_ID").trigger("change");
                $("#srlmessage-block").removeClass("bg-error");
                $("#srlmessage-block").html("Defect close successfully.");
                $("#srlmessage-block").addClass("bg-correct");
            }
        }
    }

    //Jitendra Mahajan
    function duplicateRefrencePart() {
        //alert("jhu");
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype

        if (myajax.readyState == 4) {
            //alert(myajax.status);
            //alert(window.location.href.indexOf("http"));
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                // alert(myajax.responseText);
                var jsonRes = $.parseJSON(myajax.responseText);
                //alert(jsonRes);
                if (jsonRes == true) {
                    //alert("Referece Part is Already prescent Against the Defect");
                    $("#lblMSg").html("Referece Part is Already prescent Against the Defect.");
                    //$(".modal-title").focus();
                    //$("#quality_edit_checkpoint").on('shown.bs.modal', function () {
                    //    $(this).find('#lblMSg').focus();
                    //});
                    $('#testy').toastee({
                        type: 'error',
                        width: '400px',
                        height: '80px',
                        message: 'Referece Part is Already prescent Against the Defect...',
                    });
                    //$("#srlmessage-block").html("Scanned serial no. is wrong");
                    //$("#srlmessage-block").addClass("bg-error");
                    // isValid = false;
                    return false;
                }
                else {
                    //Jitendra Mahajan
                    //  alert("hi");
                    var serialNo = $(".srlno-input").val();
                    var capapartReferencePiece = $("#Refrence_Piece").val();
                    var capapartReferencePeiceDesc = $("#Refrence_Piece_Desc").val();
                    // alert(capapartReferencePeiceDesc);
                    var capapartQuantity = $("#Quantity").val();

                    // alert("Save");
                    if ($("#hdnQualityIsFirstShotClear").val() == "true") {
                        return;
                    }
                    var isValid = true;
                    clearQualityEditErrorPopup();

                    var isResolved = true;

                    if ($('#ddluser').val().length > 0 && $('#ddluser').val() != "") {
                        $('#lblUsers').html(null);
                    }
                    else {
                        $('#lblUsers').html('Required');
                        isValid = false;
                    }

                    if ($('#Analytics').val().length > 0 && $('#Analytics').val() != "") {
                        $('#lblAnalytics').html(null);
                    }
                    else {
                        $('#lblAnalytics').html('Required');
                        isValid = false;
                    }

                    var isValueBased = $("#hdnIsValuedBased").val();
                    var userCheckpointValue = 0;
                    if (isValueBased == "true") {

                        if ($("#edit_user_checkpoint_value").val() == "" || $("#edit_user_checkpoint_value").val() == null) {
                            isValid = false;
                            $("#errorQualityEditCheckpointUserValue").html("Please enter User Value");
                        }
                        else
                            if (!$.isNumeric($("#edit_user_checkpoint_value").val())) {
                                isValid = false;
                                $("#errorQualityEditCheckpointUserValue").html("Invalid User Value");
                            }
                            else {
                                $("#errorQualityEditCheckpointUserValue").html("");
                            }

                        if ($("#hdnIsCheckpointValueCorrect").val() == "true") {

                        }
                        else {
                            isValid = false;
                            //$("#errorQualityEditCheckpointValue").html("Invalid");
                        }
                    }
                    else {
                        // process need to check whether user checked the clear defect
                        if ($("#edit_quality_clear_defect").prop("checked") === true) {
                            $("#errorQualityEditCorrectiveActions").html("");
                            if ($("#edit_quality_corrective_actions").val() == "" || $("#edit_quality_corrective_actions").val() == null) {
                                isValid = false;
                                $("#errorQualityEditCorrectiveActions").html("Please select CorrectiveActions");
                                return;
                            }

                            if (checkReworkManPowerAndReworkManTime() == false) {
                                isValid = false;
                            }
                        }
                        else {
                            isResolved = false;
                            //isValid = false;
                        }


                    }

                    // process to check the defect user is selected or not
                    //if ($("#edit_quality_defect_user").val() == "" || $("#edit_quality_defect_user").val() == null)
                    //{
                    //    isValid = false;
                    //    $("#errorQualityDefectUser").html("Please select defect user");           
                    //}
                    //else
                    //{
                    //    $("#errorQualityDefectUser").html("");
                    //}


                    // process to update the defect
                    //alert("before isvalid");
                    if (isValid) {

                        var userValue = $("#edit_user_checkpoint_value").val();
                        var reworkManPower = $("#edit_rework_man_power").val();
                        var reworkManHours = $("#edit_rework_man_hours").val();
                        var reworkManMinutes = $("#edit_rework_man_minute").val();
                        var correctiveActions = $("#edit_quality_corrective_actions").val();
                        if (correctiveActions == "" || correctiveActions == null || (!$.isNumeric(correctiveActions))) {
                            correctiveActions = 0;
                        }

                        var defectId = $("#hdnQualityId").val();

                        var isCheckpointValueBasedCorrect = $("#hdnIsCheckpointValueCorrect").val();
                        var qualityDefectId = $("#edit_quality_defect").val();
                        if (qualityDefectId == "" || qualityDefectId == null || (!$.isNumeric(qualityDefectId))) {
                            qualityDefectId = 0;
                        }
                        var resolvedStationId = $("#Captured_Station_ID").val();

                        //var userId = $("#Captured_User_ID").val();
                        var userId = $("#ddluser").val();

                        var defectCategory = $("#Defect_Category_ID").val();
                        if ($("#Defect_Category_ID").val() == "" || $("#Defect_Category_ID").val() == null) {
                            //qualityDefectId = 0;
                            $("#errorDefectCategory").html("Please select DefectCategory");
                            return;
                        }
                        else {
                            $("#errorQualityDefectCategoryId").html("");
                        }

                        if (checkReworkManPowerAndReworkManTime() == false) {
                            //alert("Please clear the defect and save");
                            $("#lblMSg").html("Please clear the defect and save.");
                            return;
                        }

                        var plantId = $("#Plant_ID").val();
                        var checkpointUserId = $("#edit_quality_defect_user").val();
                        checkpointUserId = 0;

                        //Jitendra Mahajan
                        if (capapartReferencePiece != null) {
                            var istrue = true;
                            if (capapartReferencePiece) {
                                if (capapartQuantity == "") {
                                    // clearCAPAErrorMessage();
                                    $("#lblQuantity").html("Please Enter Quantity of causal Part.");
                                    //$("#lblQuantity").addClass("bg-error");
                                    // return false;
                                    istrue = false;
                                }
                                if (!/^[0-9]+$/.test(capapartQuantity)) {
                                    // clearCAPAErrorMessage();
                                    $("#lblQuantity").html("Please Enter Numeric value.");
                                    //$("#lblQuantity").addClass("bg-error");
                                    //return false;
                                    istrue = false;
                                }

                                if ($("#Quantity").val().length > 5) {
                                    //clearCAPAErrorMessage();
                                    $("#lblQuantity").html("Length Excceds, Enter text upto 5 digit.");
                                    // $("#lblQuantity").addClass("bg-error");
                                    // return false;
                                    istrue = false;
                                }
                                if (capapartReferencePeiceDesc == "") {
                                    //clearCAPAErrorMessage();
                                    $("#lblRefPieceDesc").html("Please Enter Refrence Part Desc.");
                                    //$("#lblRefPieceDesc").addClass("bg-error");
                                    //return false;
                                    istrue = false;
                                }
                                if ($("#Refrence_Piece_Desc").val().length > 100) {
                                    //clearCAPAErrorMessage();
                                    $("#lblRefPieceDesc").html("Length Excceds, Enter text upto 100 characters.");
                                    //$("#lblRefPieceDesc").addClass("bg-error");
                                    //return false;
                                    istrue = false;
                                }
                                if ($("#Refrence_Piece").val().length > 100) {
                                    //clearCAPAErrorMessage();
                                    $("#lblRefrencePiece").html("Length Excceds, Enter text upto 100 characters.");
                                    //$("#lblRefPieceDesc").addClass("bg-error");
                                    //return false;
                                    istrue = false;
                                }
                            }
                        }
                        //alert(istrue);
                        if (istrue == true) {
                            var url = "/QualityAuditCaptures/updateQualityDefect";
                            ajaxpack.postAjaxRequest(url, "defectId=" + defectId + "&plantId=" + plantId + "&isValueBased=" + isValueBased + "&isCheckpointValueBasedCorrect=" + isCheckpointValueBasedCorrect + "&userValue=" + userValue + "&reworkManPower=" + reworkManPower + "&reworkManHours=" + reworkManHours + "&reworkManMinutes=" + reworkManMinutes + "&qualityDefectId=" + qualityDefectId + "&correctiveActions=" + correctiveActions + "&userId=" + userId + "&stationId=" + resolvedStationId + "&isResolved=" + isResolved + "&defectName=" + $("#edit_quality_defect").val() + "&correctiveActionName=" + $("#edit_quality_corrective_actions").val() + "&defectCategory=" + defectCategory + "&checkpointUserId=" + checkpointUserId + "&serialNo=" + serialNo + "&capapartReferencePiece=" + capapartReferencePiece + "&capapartReferencePeiceDesc=" + capapartReferencePeiceDesc + "&capapartQuantity=" + capapartQuantity + "&analytics=" + $('#Analytics').val(), showQualityResolveDetails, "json");
                        }

                    }
                }
            }
        }
    }

    $("#edit_user_checkpoint_value").keyup(function (event) {
        $("#hdnIsCheckpointValueCorrect").val("false");
        var value = $(this).val();
        if (!$.isNumeric(value)) {

            $("#errorQualityEditCheckpointValue").html("Please enter the correct checkpoint value");
            return;
        }
        else {
            value = parseFloat(value);
            $("#errorQualityEditCheckpointUserValue").html("");
            var lowerLimit = parseFloat($("#hdnLowerLimit").val());

            var upperLimit = parseFloat($("#hdnUpperLimit").val());

            if (upperLimit == null || upperLimit == 0 || isNaN(upperLimit)) {
                // check only with exact lower limit
                if (value != lowerLimit) {
                    $("#errorQualityEditCheckpointValue").html("Entered value is not matched with checkpoint value. It will be added as defect.");
                    $(".quality_edit_rework_man_power_time_popup").hide();
                    return;
                }
            }
            else {
                if ((lowerLimit <= value) && (value <= upperLimit)) {

                    // process to show th rework man power and man minutes
                    $("#errorQualityEditCheckpointValue").html("");
                    $(".quality_edit_rework_man_power_time_popup").show();
                    $("#hdnIsCheckpointValueCorrect").val("true");
                    return;
                }
                else {
                    $("#errorQualityEditCheckpointValue").html("Entered value is not between checkpoint value. It will be added as defect.");
                    $(".quality_edit_rework_man_power_time_popup").hide();
                    return;
                }
            }
        }

    });

    function checkReworkManPowerAndReworkManTime() {
        var res = true;
        clearQualityEditErrorPopup();
        var reworkManPower = $("#edit_rework_man_power").val();
        if (!$.isNumeric(reworkManPower)) {
            $("#errorQualityEditReworkManPower").html("Invalid");

            res = false;
        }

        var reworkManHours = $("#edit_rework_man_hours").val();
        if (!$.isNumeric(reworkManHours)) {
            $("#errorQualityEditReworkManHours").html("Invalid");
            res = false;

        }

        var reworkManMinutes = $("#edit_rework_man_minute").val();
        if (!$.isNumeric(reworkManMinutes)) {
            $("#errorQualityEditReworkManMinute").html("Invalid");
            res = false;
        }
        else {
            if (reworkManMinutes > 60) {
                $("#errorQualityEditReworkManMinute").html("Invalid");
                res = false;
            }
        }

        return res;

    }

    function clearQualityEditErrorPopup() {
        $("#errorQualityEditCorrectiveActions").html("");
        $("#errorQualityEditReworkManPower").html("");
        $("#errorQualityEditReworkManHours").html("");
        $("#errorQualityEditReworkManMinute").html("");
        $("#errorQualityEditCheckpointNewValue").html("");
        $("#errorDefectCategory").html("");
        $("#errorQualityDefectUser").html("");
        //$("#errorQualityEditCheckpointValue").html("");
    }

    function clearQualityEditValuePopup() {
        $("#edit_user_checkpoint_value").val("");
        $("#edit_rework_man_power").val("");
        $("#edit_rework_man_hours").val("");
        $("#edit_rework_man_minute").val("");
    }

    function closePopup() {
        $(".modal-header .close").trigger("click");
    }


    $("#added_defect_list").on("keyup", ".checkpoint_value", function (e) {
        var value = $(this).val();
        var checkpointId = $(this).attr("id");
        checkpointId = checkpointId.split("_");
        checkpointId = checkpointId[2];
        $("#hdnSelectedCheckpoint").val(checkpointId);
        // alert(value.length);
        if (value.length == 0) {
            $("#" + checkpointId + " .quality_defect_category").hide("hide");
            $("#fallback_" + checkpointId).hide("slow");
            $("#" + checkpointId + " .checkpoint_message").html(null);
        }

        var isUserValueBased = $("#" + checkpointId + " .hdnIsUserValueBased").val();
        if (isUserValueBased) {
            return;
        }
        if (!$.isNumeric(value)) {

            $("#" + checkpointId + " .checkpoint_message").html("Please enter the correct checkpoint value");
            isValid = false;
        }
        else {

            // process to check the value is valid or not
            value = parseFloat(value);
            var lowerLimit = parseFloat($("#" + checkpointId + " .hdnLowerLimit").val());
            var upperLimit = parseFloat($("#" + checkpointId + " .hdnUpperLimit").val());
            var MultiplicationFactor = parseFloat($("#" + checkpointId + " .hdnMultiplicationFactor").val());
            var MasterRingValue = parseFloat($("#" + checkpointId + " .hdnMasterRingValue").val());
            var isRingValue = parseFloat($("#" + checkpointId + " .hdnRingValue").val());
            //added for calculations on 14042018
            if (isRingValue == 1) {
                value = (value * MultiplicationFactor) + MasterRingValue;
            }
            //end
            var isValueBasedCorrect = true;
            if (upperLimit == null || upperLimit == 0 || isNaN(upperLimit)) {
                // check only with exact lower limit
                if (value != lowerLimit) {
                    $("#" + checkpointId + " .checkpoint_message").html("Entered value is not matched with checkpoint value. It will be added as defect.");
                    isValueBasedCorrect = false;
                }
            }
            else {
                if ((lowerLimit <= value) && (value <= upperLimit)) {
                    $("#" + checkpointId + " .checkpoint_message").html("");
                }
                else {
                    $("#" + checkpointId + " .checkpoint_message").html("Entered value is not between checkpoint value. It will be added as defect.");
                    isValueBasedCorrect = false;
                }
            }


            if (isValueBasedCorrect == false) {
                //$("#" + checkpointId + " .quality_checkpoint_station_users").show("slow");
                $("#" + checkpointId + " .quality_defect_category").show("slow");
                $("#fallback_" + checkpointId).show("slow");

                loadStationUsers();
            }
            else {
                //$("#" + checkpointId + " .quality_checkpoint_station_users").hide("slow");


                $("#fallback_" + checkpointId).hide("slow");

                $("#" + checkpointId + " .quality_defect_category").hide("slow");
            }
        }
    });

    $(".final_quality_ok").click(function (e) {
        var serialNo = $(".srlno-input").val();
        var plantId = $("#Plant_ID").val();
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var stationId = $("#Captured_Station_ID").val();
        var reprint = "print";
        if (serialNo) {
            var url = "/QualityAuditCaptures/isQualityOK";
            ajaxpack.postAjaxRequest(url, "serialNo=" + serialNo + "&plantId=" + plantId + "&shopId=" + shopId + "&lineId=" + lineId + "&stationId=" + stationId + "&isReprint=" + reprint, showFinalQualityOKDetails, "json");
        }
    });

    $(".re_final_quality_ok").click(function (e) {
        var serialNo = $(".srlno-input").val();
        var plantId = $("#Plant_ID").val();
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var stationId = $("#Captured_Station_ID").val();
        var reprint = "reprint";
        if (stationId == "37") {//engine quality want multiple password so we are not checking password here
            if (serialNo) {
                var url = "/QualityAuditCaptures/isQualityOK";
                ajaxpack.postAjaxRequest(url, "serialNo=" + serialNo + "&plantId=" + plantId + "&shopId=" + shopId + "&lineId=" + lineId + "&stationId=" + stationId + "&isReprint=" + reprint, showFinalQualityOKDetails, "json");
            }
        }
        else {
            $('#My_Modal_TokenNo').modal('show');             //uncode when you want password prorection
        }

    });
    $("#EnterToken").click(function () {
        var tokenno = $("#TokenNo").val();
        var password = $("#Password").val();
        var plantId = $("#Plant_ID").val();
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        if (password == "@123") {
            var url = "/QualityAuditCaptures/ReprintWithLineInchargeTokenNo";
            ajaxpack.postAjaxRequest(url, "TokenNo=" + tokenno + "&plantId=" + plantId + "&shopId=" + shopId + "&lineId=" + lineId, IsValidLineIncharge, "json");

        }
        else {
            $("#lblpass").html("Invalid Token No OR Password !.");
        }

    });
    function IsValidLineIncharge() {
        var tokenno = $("#TokenNo").val();
        var serialNo = $(".srlno-input").val();
        var plantId = $("#Plant_ID").val();
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var stationId = $("#Captured_Station_ID").val();
        var reprint = "reprint";
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);

                if (jsonRes == true) {
                    if (serialNo) {
                        var url = "/QualityAuditCaptures/isQualityOK";
                        ajaxpack.postAjaxRequest(url, "serialNo=" + serialNo + "&plantId=" + plantId + "&shopId=" + shopId + "&lineId=" + lineId + "&stationId=" + stationId + "&isReprint=" + reprint + "&tokenNo=" + tokenno, showFinalQualityOKDetails, "json");
                    }
                    $('#My_Modal_TokenNo').modal('hide');
                    $("#TokenNo").val("");
                    $("#Password").val("");
                }
                else {
                    $("#lblpass").html("Invalid Token No!.");
                    $("#srlmessage-block").addClass("bg-error");
                    $('#My_Modal_TokenNo').modal('show');
                    $("#TokenNo").val("");
                    $("#Password").val("");
                }
            }
        }

        //Changed On 8th May
        //6 May

        // GetQualityCount();



    }

    function showFinalQualityOKDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);

                if (jsonRes == "Reprint") {
                    // barcode is printed
                    clearQualityErrorMessage();

                    $("#srlmessage-block").html("Final Quality Sticker Reprinted.");
                    $("#srlmessage-block").addClass("bg-correct");
                    //27 sandip
                    $('#txtCH').val('');
                    $('#txtVTU').val('');
                    $('#FQP_Transmission').css('display', 'none');
                    $(".final_quality_ok").css('display', 'none');//2 may
                    $('.quality_Rejection').css('display', 'none');
                    //
                }
                else if (jsonRes == true) {
                    // show error message
                    clearQualityErrorMessage();
                    $("#srlmessage-block").html("Final quality done.");//6
                    $("#srlmessage-block").addClass("bg-correct");
                    //27 sandip
                    $('#txtCH').val('');
                    $('#txtVTU').val('');
                    $('#FQP_Transmission').css('display', 'none');
                    $(".final_quality_ok").css('display', 'none');//2 may
                    $(".re_final_quality_ok").css('display', 'block');
                    $('.quality_Rejection').css('display', 'none');
                }
                else if (jsonRes == false) {
                    // show error message
                    clearQualityErrorMessage();
                    $("#srlmessage-block").html("Please clear the defects and try again.");
                    $("#srlmessage-block").addClass("bg-error");
                }
            }
        }
    }

    // process to show the defect corrective actions
    $("#added_defect_list").on("change", ".checkpoint_defect", function (e) {
        var checkpointId = $(this).attr("id");
        checkpointId = checkpointId.split("_");
        checkpointId = checkpointId[2];

        var defectId = $("#checkpoint_defect_" + checkpointId).val();
        if (defectId) {

            clearSelectBox("quality_corrective_actions_" + checkpointId);
            var url = "/QualityAuditCaptures/getCorrectiveActionByDefectId";
            $("#hdnSelectedCheckpoint").val(checkpointId);
            ajaxpack.postAjaxRequest(url, "defectId=" + defectId, showQualityCheckpointDefects, "json");
        }

    });

    function showQualityCheckpointDefects() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                var checkpointId = $("#hdnSelectedCheckpoint").val();
                SelectOptionHTML(jsonRes, "quality_corrective_actions_" + checkpointId);
                //correctiveActionsSelect(checkpointId);
            }
        }
    }


    function correctiveActionsSelect(checkpointId) {
        $("#" + checkpointId + " .quality_corrective_actions").select2();
    }



    function clearQualityErrorMessage() {
        $("#srlmessage-block").removeClass("bg-correct");
        $("#srlmessage-block").removeClass("bg-error");

        $("#srlmessage-block").html("");
        $("#Remark").val("");
    }

    //Added by sandy 22 April 2017
    $("#txtCH").keydown(function (event) {
        if (event.keyCode == 13) {
            $('#txtVTU').focus();
        }
    });

    $("#txtVTU").keydown(function (event) {
        if (event.keyCode == 13) {
            ErrorProffingOfAllParts();
        }
    });


    function ErrorProffingOfAllParts() {

        var serialNo = $(".srlno-input").val();
        var CH = $('#txtCH').val();
        var VTU = $('#txtVTU').val();
        var url = "/QualityAuditCaptures/ErrorProffingOfAllParts";
        ajaxpack.getAjaxRequest(url, "serialNo=" + $(".srlno-input").val() + "&cluchHousing=" + $('#txtCH').val() + "&hydrolic=" + $('#txtVTU').val(), showCheckListDetail, "json");

    }


    function showCheckListDetail() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                var EP_Status = jsonRes.EP_Status;
                //var VTUmodel_Code = jsonRes.VTUmodel_Code;
                if (EP_Status == true) {

                    var url = "/QualityAuditCaptures/GetOrderDetails";
                    ajaxpack.getAjaxRequest(url, "serialNo=" + $(".srlno-input").val() + "", showSerialDetail, "json");
                }
                    //else if (VTUmodel_Code == false) {
                    //    $("#srlmessage-block").show();
                    //    $("#srlmessage-block").html("Hydrolic Allready Attach To Transmission");
                    //    $("#srlmessage-block").addClass("bg-error");
                    //}
                else {
                    clearQualityErrorMessage();
                    $("#srlmessage-block").show();
                    $("#srlmessage-block").html("Clutch Housing Allready Attach To Transmission");
                    $("#srlmessage-block").addClass("bg-error");
                }
            }
        }
    }

    function GetQualityCount() {
        setTimeout(function () {
            var url = "/QualityAuditCaptures/GetQualityCount";
            ajaxpack.getAjaxRequest(url, "", showQualityCount, "json");
        }, 3000);
    }

    function showQualityCount() {

        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                //alert(myajax.responseText);
                var jsonRes = $.parseJSON(myajax.responseText);
                $('#shop-Quality').text(myajax.responseText);
            }
        }

    }

    $('.close_quality_data').click(function () {
        //$('#quality_edit_checkpoint').model('hide');
        $('#quality_edit_checkpoint').modal('hide');
    })

    $("#Refrence_Piece").change(function (e) {
        // alert("inn");
        $("#lblMSg").html("");
        var serialNo = $(".srlno-input").val();
        var partID = $(this).val();
        $("#Refrence_Piece_Desc").prop('disabled', false);
        $('#Refrence_Piece_Desc').val("");
        $('#Quantity').val("");
        $("#lblQuantity").html("");
        $("#lblRefPieceDesc").html("");
        $("#lblRefrencePiece").html("");

        //clearSelectBox("edit_quality_corrective_actions");
        if (partID) {
            var url = "/QualityAuditCaptures/getQuantityPartDesc";
            ajaxpack.getAjaxRequest(url, "partID=" + partID + "&serialNo=" + serialNo, bindQuantityPartDesc, "json");
            // ajaxpack.getAjaxRequest(url, "defectId=" + defectId + "&capaStatus=" + capaStatus, showCAPAData, "json");
        }

    });

    function loadReferencePart() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                // alert("ok");
                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Refrence_Piece");
                $("#Refrence_Piece").select2({
                    tags: true
                });
            }
        }
    }
    function loadUsers() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                // alert("ok");
                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "ddluser");
                $("#ddluser").select2({
                    tags: true
                });
            }
        }
    }

    function bindQuantityPartDesc() {
        // alert("hiii");
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        //alert(myajax.readyState);
        if (myajax.readyState == 4) {
            //alert(myajax.readyState);
            //alert(myajax.status);
            //alert(window.location.href.indexOf("http"));
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                //alert(capaList);
                //var jsonRes = $.parseJSON(myajax.responseText);
                var jsonRes = $.parseJSON(myajax.responseText);
                //alert(capaList);
                // alert(jsonRes.partDesc);
                // alert(jsonRes.quantity);                       
                $("#Refrence_Piece_Desc").val(jsonRes.partDesc);
                $("#Quantity").val(jsonRes.quantity);
                $("#Refrence_Piece_Desc").prop('disabled', true);
            }
        }
    }

    //end

    $('#btnPrint').click(function () {
        //alert($(".srlno-input").val());
        //var url = "/QualityAuditCaptures/GenerateBillSheet";
        //ajaxpack.getAjaxRequest(url, "serialNumber=" + $(".srlno-input").val(), generateBillSheet, "json");
        //BillSheet.html?serialNumber=NXH00049RD
        window.open("/Content/prnfiles/BillSheet.html?serialNumber=" + $(".srlno-input").val());
    })

    $('.road_test_yes').click(function () {
        var serialNo = $(".srlno-input").val();
        var url = "/QualityAuditCaptures/SendTo_Road_Test";
        ajaxpack.getAjaxRequest(url, "Road_Test=YES" + "&serialNo=" + serialNo, SendToRoadTest, "json");

    })

    $('.road_test_no').click(function () {
        var serialNo = $(".srlno-input").val();
        var url = "/QualityAuditCaptures/SendTo_Road_Test";
        ajaxpack.getAjaxRequest(url, "Road_Test=NO" + "&serialNo=" + serialNo, SendToRoadTest, "json");

    })

    function SendToRoadTest() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally             
                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes) {
                    $('#quality_road_test').modal("hide");
                }
                else {
                    $('#quality_road_test').modal("show");
                }
            }
        }
    }
    function LineChange() {
        clearSelectBox("Captured_Station_ID");
        if ($("#Line_ID").val()) {
            $("#Line_Validation").html("");
            var url = "/QualityAuditCaptures/GetStationByLineID";
            ajaxpack.getAjaxRequest(url, "Line_ID=" + $("#Line_ID").val(), loadStationByLine, "json");
        }
    }

    $("#Line_ID").change(function (e) {
        LineChange();
    });

    function loadStationByLine() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                // alert("ok");
                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Captured_Station_ID");
            }
        }
    }
});