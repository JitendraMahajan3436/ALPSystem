$(document).ready(function (e) {





    $(".srlno-input").keydown(function (event) {

        clearSelectBox("Checklist_ID");
        clearQualityErrorMessage();
        $(".quality_select_checklist").hide();
        $("#added_defect_list").html("");
        if (event.keyCode == 13) {
            loadSerialDetail();
        }
    });

    if ($("#hdnIsTablet").val() == "True")
    {
        $(".srlno-input").attr("readonly", true);
        setTimeout(function ()
        {
            loadSerialNumberForTablet();

            

        }, 2000);
        
    }
    else
    {
        $(".srlno-input").removeAttr("readonly");
    }

    function loadSerialNumberForTablet()
    {
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var stationId = $("#Captured_Station_ID").val();
        
        var url = "/QualityCaptures/GetSerialNumber";
        ajaxpack.postAjaxRequest(url, "stationId=" + stationId + "&lineId=" + lineId + "&shopId=" + shopId, loadSerialNumberForTabletDetail, "json");       
    }

    function loadSerialNumberForTabletDetail()
    {
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
        if (serialNo) {
            var url = "/QualityCaptures/GetOrderDetails";
            ajaxpack.getAjaxRequest(url, "serialNo=" + $(".srlno-input").val() + "", showSerialDetail, "json");
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

                    if (jsonRes[0].Is_Take_Out > 0) {
                        clearQualityErrorMessage();
                        $("#srlmessage-block").html("This vehicle is taken out.");
                        $("#srlmessage-block").addClass("bg-error");
                        return true;
                    }

                    $("#hdnSeries").val(jsonRes[0].Series);
                    $("#hdnFamily").val(jsonRes[0].Family);


                    // show quality checklist
                    var stationId = $("#Captured_Station_ID").val();
                    var family = $("#hdnFamily").val();
                    var shopId = $("#Shop_ID").val();
                    var isFinalQualityOK = $("#isFinalQualityStation").val();
                    var isRework = $("#isReworkStation").val();
                    if (isFinalQualityOK == "True" || isRework == "True") {
                        var url = "/QualityStationChecklist/getAllChecklistByFamily";
                        ajaxpack.getAjaxRequest(url, "family=" + family, showQualityChecklist, "json");
                    }
                    else {
                        var url = "/QualityStationChecklist/getAddedChecklistToStationId";

                        ajaxpack.getAjaxRequest(url, "shopId=" + shopId + "&stationId=" + stationId + "&family=" + family, showQualityChecklist, "json");
                    }

                }
                else {
                    // no record found, process to clear things
                    clearQualityErrorMessage();
                    $("#srlmessage-block").html("Order Number is invalid.");

                    $("#srlmessage-block").addClass("bg-error");
                }
                //SelectOptionHTML(jsonRes, "Shop_ID");
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
                SelectOptionHTML(jsonRes, "Checklist_ID");






                if ($("select#Checklist_ID").length == 1) {
                    $("select#Checklist_ID option").eq(1).attr("selected", "selected");
                    $("#Checklist_ID").trigger("change");
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


    $("#Checklist_ID").change(function (e) {
        var checklistId = $(this).val();
        $("#added_defect_list").html("");
        if (checklistId) {
            var url = "/QualityCaptures/isQualityLoggedForSerialNumberWithChecklistId";
            ajaxpack.getAjaxRequest(url, "serialNo=" + $(".srlno-input").val() + "&checklistId=" + checklistId, isQualityLoggedForSerial, "json");
        }
    });

    function isQualityLoggedForSerial() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes == true) {
                    // show history
                    showChecklistHistory();
                }
                if (jsonRes == false) {

                    // process to get the list of checkpoint assigned to the checklist

                    var url = "/QualityCaptures/getCheckpointByChecklist";
                    ajaxpack.getAjaxRequest(url, "checklist=" + $("#Checklist_ID").val(), showChecklistDetail, "json");
                }

            }
        }
    }

    function showChecklistDetail() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                $(".checklistId").val($("#Checklist_ID").val());
                $("#frm_show_defect_list").submit();

            }
        }
    }


    $(".quality_checklist_radio").change(function (e) {
        alert("ok");
    });

    $("#added_defect_list").on("click", ".checkpoint_radio_click i", function (e) {

        var checkpointId = 0;
        checkpointId = $(this).parent().find('.quality_checklist_radio').attr("id");
        checkpointId = checkpointId.split("_");
        checkpointId = checkpointId[1];

        // process to check checkpoint is value based or not
        var isValueBased = false;
        if ($("#" + checkpointId + " .hdnIsValueBased").val() == "" || $("#" + checkpointId + " .hdnIsValueBased").val() == null) {
            isValueBased = false;
        }
        else {
            isValueBased = true;
        }


        if ($(this).parent().find('.quality_checklist_radio').prop('checked') === true) {
            if (isValueBased) {
                // process to show the text item                
                $("#" + checkpointId + " .defect_value").show("slow");
                $("#" + checkpointId + " .quality_remarks").show("slow");

            }
            else {
                // process to show the defect item
                $("#" + checkpointId + " .quality_defect").show("slow");

                $("#" + checkpointId + " .quality_remarks").show("slow");
                $("#" + checkpointId + " .defect_resolved").show("slow");
                // process to load the defect for the selected checkpoint

                $("#hdnSelectedCheckpoint").val(checkpointId);
                var url = "/QualityDefects/getAddedDefectToCheckpoint";
                ajaxpack.getAjaxRequest(url, "checkpointId=" + checkpointId, showDefectDetail, "json");
            }
        }
        else {

            // process to hide all the inputs
            $("#" + checkpointId + " .quality_defect").hide("slow");

            $("#" + checkpointId + " .defect_value").hide("slow");
            $("#" + checkpointId + " .quality_remarks").hide("slow");

            $("#" + checkpointId + " .defect_resolved").hide("slow");
            $("#" + checkpointId + " .quality_corrective_actions").hide("slow");
            uncheckQualityResolved(checkpointId);
        }
    });

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


        if ($(this).parent().find('.defect_resolved_radio').prop('checked') === true) {
            $("#" + checkpointId + " .quality_corrective_actions").show("slow");
        }
        else {
            $("#" + checkpointId + " .quality_corrective_actions").hide("slow");
        }
    });


    $("#added_defect_list").on("click", "#check_quality_checkpoint", function (e) {
        var isValid = true;
        var qualityCheckpointRes = new Array();
        $(".checkpoint_message").html("");
        $("#hdnNotSelectedUnValueBasedCheckpoint").val("");
        $("#hdnSelectedCheckpoint").val("");
        $(".table tr").each(function (e) {

            var checkpointId = $(this).attr("id");
            var isValueBased = false;
            var isUserValuebased = false;
            var isValueBasedCorrect = true;
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
                                }
                            }
                            else {
                                if ((lowerLimit <= value) && (value <= upperLimit)) {

                                }
                                else {
                                    $("#" + checkpointId + " .checkpoint_message").html("Entered value is not between checkpoint value. It will be added as defect.");
                                    isValueBasedCorrect = false;
                                }
                            }
                        }

                    }
                }
                else {
                    // check the checkbox is selected or not
                    if ($("#checklist_" + checkpointId).prop('checked') === true) {
                        // process to check the defect is selected or not
                        if ($("#checkpoint_defect_" + checkpointId).val() == "-1" || $("#checkpoint_defect_" + checkpointId).val() == "" || $("#checkpoint_defect_" + checkpointId).val() == null) {
                            $("#" + checkpointId + " .checkpoint_message").html("Please select defect");
                            isValid = false;
                        }

                        // process to check the defect is resolved or not

                        if ($("#resolved_checklist_" + checkpointId).prop('checked') === true) {
                            // process to check the corrective action is selected
                            if ($("#quality_corrective_actions_" + checkpointId).val() == "-1" || $("#quality_corrective_actions_" + checkpointId).val() == null) {
                                $("#" + checkpointId + " .checkpoint_message").html("Please select corrective actions.");
                                isValid = false;
                            }

                            // process to check the defect category is selected or not
                            if ($("#Defect_Category_ID_" + checkpointId).val() == "-1" || $("#Defect_Category_ID_" + checkpointId).val() == "" || $("#Defect_Category_ID_" + checkpointId).val() == null) {
                                $("#" + checkpointId + " .checkpoint_message").html("Please select defect category.");
                                isValid = false;
                            }
                        }
                    }
                    else {
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
                    }
                }


                var Defect_Category_ID = $("#Defect_Category_ID_" + checkpointId).val();
                var item = { Checklist_ID: $("#Checklist_ID").val(), Checkpoint_ID: checkpointId, Is_Value_Based: isValueBased, Lower_Limit: $("#" + checkpointId + " .hdnLowerLimit").val(), Upper_Limit: $("#" + checkpointId + " .hdnUpperLimit").val(), User_Value: $("#checkpoint_value_" + checkpointId).val(), Remark: $("#remark_" + checkpointId).val(), Defect_ID: $("#checkpoint_defect_" + checkpointId).val(), Is_Value_Based_Correct: isValueBasedCorrect, Corrective_Actions_ID: $("#quality_corrective_actions_" + checkpointId).val(), Is_Rework_Done: $("#resolved_checklist_" + checkpointId).prop("checked"), Defect_Name: $("#checkpoint_defect_" + checkpointId).text(), Corrective_Actions_Name: $("#quality_corrective_actions_" + checkpointId).val(), Is_User_Value_Based: isUserValuebased, Defect_Category_ID: Defect_Category_ID };



                if (qualityCheckpointRes.length == 0) {
                    qualityCheckpointRes[0] = item;
                }
                else {
                    qualityCheckpointRes[qualityCheckpointRes.length + 1] = item;
                }
            }

        });
        $("#hdnQualityCheckpointRes").val(JSON.stringify(qualityCheckpointRes));
        if (isValid) {
            var unselectedUnValueBasedCheckpoint = $("#hdnNotSelectedUnValueBasedCheckpoint").val();
            if (unselectedUnValueBasedCheckpoint) {
                // process to get the list of checkpoint
                var url = "/QualityCheckpoint/getCheckpointById";
                ajaxpack.getAjaxRequest(url, "checkpointId=" + unselectedUnValueBasedCheckpoint, showCheckpointDetails, "json");

            }
            else {
                submitValue();
            }
        }

    });

    function submitValue() {
        var plantId = $("#Plant_ID").val();
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var stationId = $("#Captured_Station_ID").val();
        var serialNumber = $(".srlno-input").val();
        var qualityRes = $("#hdnQualityCheckpointRes").val();
        //var userId = $("#Captured_User_ID").val();
        var userId = $("#Captured_User_ID").val();
        var url = "/QualityCaptures/saveQualityData";
        ajaxpack.postAjaxRequest(url, "plantId=" + plantId + "&shopId=" + shopId + "&lineId=" + lineId + "&stationId=" + stationId + "&serialNumber=" + serialNumber + "&userId=" + userId + "&qualityData=" + qualityRes, showQualityLog, "json");
    }

    function showQualityLog() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);


                // process to show the checklist history
                showChecklistHistory();

                closePopup();


                if ($("#hdnIsTablet").val() == "True") {
                    $(".srlno-input").attr("readonly", true);
                    setTimeout(function () {
                        loadSerialNumberForTablet();



                    }, 2000);

                }
            }
        }
    }

    function showChecklistHistory() {
        $(".checklistId").val($("#Checklist_ID").val());
        $("#stationId").val($("#Captured_Station_ID").val());
        $("#serialNo").val($(".srlno-input").val());
        $("#frm_show_defect_history_list").submit();
    }

    $(".save_quality_data").click(function (e) {
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

    $("#added_defect_list").on("click", ".quality_edit_popup", function (e) {

        var defectId = $(this).attr("id");
        $("#hdnQualityId").val(defectId);
        var url = "/QualityCaptures/getDefectDetailById";
        clearQualityEditErrorPopup();
        ajaxpack.postAjaxRequest(url, "defectId=" + defectId, ShowQualityEditDetails, "json");

        clearQualityEditValuePopup();
    });

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
            var url = "/QualityCaptures/getCorrectiveActionByDefectId";
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

    $(".edit_quality_data").click(function (e) {

        if ($("#hdnQualityIsFirstShotClear").val() == "true") {
            return;
        }
        var isValid = true;
        clearQualityEditErrorPopup();

        var isResolved = true;

        var isValueBased = $("#hdnIsValuedBased").val();
        var userCheckpointValue = 0;
        if (isValueBased == "true") {

            if ($("#edit_user_checkpoint_value").val() == "" || $("#edit_user_checkpoint_value").val() == null) {
                isValid = false;
                $("#errorQualityEditCheckpointUserValue").html("Required");
            }
            else
                if (!$.isNumeric($("#edit_user_checkpoint_value").val())) {
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

        // process to update the defect
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

            var userId = $("#Captured_User_ID").val();

            var defectCategory = $("#Defect_Category_ID").val();
            if (defectCategory == "" || defectCategory == null || (!$.isNumeric(defectCategory))) {
                //qualityDefectId = 0;


                $("#errorDefectCategory").html("Please select");
                return;
            }
            else {
                $("#errorQualityDefectCategoryId").html("");
            }

            if (checkReworkManPowerAndReworkManTime() == false) {
                return;
            }

            var plantId = $("#Plant_ID").val();
            var url = "/QualityCaptures/updateQualityDefect";
            ajaxpack.postAjaxRequest(url, "defectId=" + defectId + "&plantId=" + plantId + "&isValueBased=" + isValueBased + "&isCheckpointValueBasedCorrect=" + isCheckpointValueBasedCorrect + "&userValue=" + userValue + "&reworkManPower=" + reworkManPower + "&reworkManHours=" + reworkManHours + "&reworkManMinutes=" + reworkManMinutes + "&qualityDefectId=" + qualityDefectId + "&correctiveActions=" + correctiveActions + "&userId=" + userId + "&stationId=" + resolvedStationId + "&isResolved=" + isResolved + "&defectName=" + $("#edit_quality_defect").val() + "&correctiveActionName=" + $("#edit_quality_corrective_actions").val() + "&defectCategory=" + defectCategory, showQualityResolveDetails, "json");
        }

    });

    function showQualityResolveDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                closePopup();
                $("#Checklist_ID").trigger("change");
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
        }
    });

    $(".final_quality_ok").click(function (e) {
        var serialNo = $(".srlno-input").val();
        var plantId = $("#Plant_ID").val();
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var stationId = $("#Captured_Station_ID").val();
        if (serialNo) {
            var url = "/QualityCaptures/isQualityOK";
            ajaxpack.postAjaxRequest(url, "serialNo=" + serialNo + "&plantId=" + plantId + "&shopId=" + shopId + "&lineId=" + lineId + "&stationId=" + stationId, showFinalQualityOKDetails, "json");
        }
    });

    function showFinalQualityOKDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes == true) {
                    // barcode is printed
                    clearQualityErrorMessage();
                    $("#srlmessage-block").html("Final quality done.");
                    $("#srlmessage-block").addClass("bg-correct");
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

    // process to show the defect corrective actions
    $("#added_defect_list").on("change", ".checkpoint_defect", function (e) {
        var checkpointId = $(this).attr("id");
        checkpointId = checkpointId.split("_");
        checkpointId = checkpointId[2];

        var defectId = $("#checkpoint_defect_" + checkpointId).val();
        if (defectId) {

            clearSelectBox("quality_corrective_actions_" + checkpointId);
            var url = "/QualityCaptures/getCorrectiveActionByDefectId";
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
    }

});