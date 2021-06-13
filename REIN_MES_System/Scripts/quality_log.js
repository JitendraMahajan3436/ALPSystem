
$(document).ready(function (e) {

    clearAllInputBox();
    hideButton();
    $('#Serial_No').bind('keydown', function (e) {

        //clearAddedDefectList();

        clearAllInputBox();
        hideButton();

        if (e.keyCode == 13) { //if this is enter key
            e.preventDefault();

            loadSerialDetail();

        }
        else {
            return true;
        }

    });

    function loadSerialDetail() {
        $("#added_defect_list").html("");
        $("#no_defect_image").hide();
        var serialNo = $("#Serial_No").val();
        if (serialNo) {
            var url = "/QualityLog/GetOrderDetails";
            ajaxpack.getAjaxRequest(url, "serialNo=" + $("#Serial_No").val() + "", showSerialDetail, "json");
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
                        alert("This vehicle is taken out.");
                        return true;
                    }
                    $("#add_deviation").removeClass("disabled");
                    $("#add_quality_take_out").removeClass("disabled");
                    $("#add_quality").removeClass("disabled");
                    $("#other_quality_station_defect").removeClass("disabled");
                    $("#same_quality_station_defect").removeClass("disabled");
                    $("#Model_Code").val(jsonRes[0].Model_Code);
                    $("#modelDescription").val(jsonRes[0].Model_Description);
                    $("#show_other_station_assigned_defect").removeClass("disabled");

                    // get the complete quality log details

                    var captureStationId = $("#Capture_Station_ID").val();
                    var url = "/QualityLog/IsFSOK";
                    ajaxpack.getAjaxRequest(url, "serialNo=" + $("#Serial_No").val() + "&stationId=" + captureStationId, showIsFSOK, "json");

                }
                else {
                    // no record found, process to clear things

                    $("#Model_Code").val("");
                    $("#modelDescription").val("");
                    clearSelectBox("defectList");
                    clearSelectBox("Defect_ID");
                    $("#show_other_station_assigned_defect").removeClass("disabled");
                    alert("Order Number is invalid.");
                }
                //SelectOptionHTML(jsonRes, "Shop_ID");
            }
        }
    }

    function showIsFSOK() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes.length > 0) {

                    if (jsonRes[0].Is_FSOK == true) {
                        // clear the defect list
                        clearSelectBox("defectList");
                        clearSelectBox("Defect_ID");

                        $("#add_quality_no_concern").addClass("disabled");
                        $("#no_defect_image").css("display", "block");
                        loadNoAddedDefect();
                        setTimeout(function () {
                            showAddedDefectMaster();
                        }, 2000);
                    }
                    else {
                        showQualityDefect(1);

                        $("#add_quality_no_concern").addClass("disabled");
                        var stationId = $("#Capture_Station_ID").val();
                        // process to load defect added and not added
                        //var url = "/QualityLog/GetAddedDefectListDetails";
                        //ajaxpack.getAjaxRequest(url, "serialNo=" + $("#Serial_No").val() + "&stationId=" + stationId + "&isAdded=1", showAddedDefect, "json");

                        //setTimeout(function () {
                        //    url = "/QualityLog/GetAddedDefectListDetails";
                        //    ajaxpack.getAjaxRequest(url, "serialNo=" + $("#Serial_No").val() + "&stationId=" + stationId + "&isAdded=0", showNoAddedDefect, "json");
                        //}, 500);



                        //var url = "/QualityLog/GetAddedDefectListDetails";
                        //ajaxpack.getAjaxRequest(url, "serialNo=" + $("#Serial_No").val() + "&stationId=" + stationId + "&isAdded=0", showNoAddedDefect, "json");
                        loadNoAddedDefect();
                        setTimeout(function () {
                            showAddedDefectMaster();
                        }, 2000);
                    }

                    if (jsonRes.length > 1) {
                        // more than one defect, can not add as no concern
                        $("#add_quality_no_concern").addClass("disabled");
                    }
                    else {

                    }
                }
                else {
                    if (jsonRes.length == 0) {
                        $("#add_quality_no_concern").removeClass("disabled");
                    }
                    else {
                        $("#add_quality_no_concern").addClass("disabled");
                    }
                    showQualityDefect(1);


                    //var stationId = $("#Capture_Station_ID").val();
                    //var url = "/QualityLog/GetAddedDefectListDetails";
                    //ajaxpack.getAjaxRequest(url, "serialNo=" + $("#Serial_No").val() + "&stationId=" + stationId + "&isAdded=0", showNoAddedDefect, "json");

                    loadNoAddedDefect();
                }


            }
        }
    }

    //function showAddedDefect()
    //{
    //    var myajax = ajaxpack.ajaxobj
    //    var myfiletype = ajaxpack.filetype
    //    if (myajax.readyState == 4) {

    //        if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

    //            var jsonRes = $.parseJSON(myajax.responseText);
    //            SelectOptionHTML(jsonRes, "Defect_ID");
    //            //SelectOptionHTML(jsonRes, "Shop_ID");
    //        }
    //    }
    //}

    function showNoAddedDefect() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "defectList");

                // process to check only one defect list, if yes show selected
                if ($('select#defectList option').length == 2) {
                    // show selected option 1
                    //alert("inside");
                    $("select#defectList option").eq(1).attr("selected", "selected");
                    $("#defectList").trigger("change");
                }
                //SelectOptionHTML(jsonRes, "Shop_ID");
            }
        }
    }

    function loadNoAddedDefect() {
        var stationId = $("#Capture_Station_ID").val();
        var lineId = $("#Line_ID").val();
        var modelCode = $("#Model_Code").val();
        var url = "/QualityLog/GetAddedDefectListDetails";
        $("#same_station").prop("checked", true);
        ajaxpack.getAjaxRequest(url, "serialNo=" + $("#Serial_No").val() + "&modelCode=" + modelCode + "&stationId=" + stationId + "&lineId=" + lineId + "&isAdded=1", showNoAddedDefect, "json");
    }

    function loadNoAddedCheckpoint() {
        var stationId = $("#Capture_Station_ID").val();
        var defectId = $("#defectList").val();
        if (defectId) {


            var url = "/QualityLog/GetAddedCheckpointListDetails";
            ajaxpack.getAjaxRequest(url, "serialNo=" + $("#Serial_No").val() + "&stationId=" + stationId + "&isAdded=0&defectId=" + defectId, showNoAddedCheckpoint, "json");
        }
    }

    function showNoAddedCheckpoint() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "defectCheckpoint");
                //SelectOptionHTML(jsonRes, "Shop_ID");
            }
        }
    }

    function clearAddedDefectList() {
        $("#added_defect_list").html("");
    }

    function clearAllInputBox() {
        clearAddedDefectList();
        $("#Model_Code").val("");
        $("#modelDescription").val("");
        clearSelectBox("defectList");
        clearSelectBox("Defect_ID");
        clearSelectBox("defectCheckpoint");
        $("#add_quality_no_concern").addClass("disabled");
        //$("#add_quality_no_concern").removeClass("disabled");
        $("#add_quality").removeClass("disabled");
        $("#other_quality_station_defect").removeClass("disabled");
        $("#no_defect_image").css("display", "none");
        $("#add_quality_take_out").addClass("disabled");
        $("#same_quality_station_defect").addClass("disabled");
        $("#add_deviation").addClass("disabled");
        clearQualityExtraDetailFields();

        $("#Defect_Category option[value='']").prop('selected', 'selected');

        $("#same_station").prop("checked", false);
        $("#other_station").prop("checked", false);

        //clear defect master
        clearSelectBox("Quality_Defect_ID");
        clearSelectBox("Quality_Defect_Item");

        // show default checklist
        enableQualityChecklist();
        $("#quality_checklist_select").prop("checked", "true");
    }

    function hideButton() {
        $("#add_quality_no_concern").addClass("disabled");
        $("#add_quality").addClass("disabled");
        $("#other_quality_station_defect").addClass("disabled");
    }

    $("#refresh_link").click(function (e) {
        clearAllInputBox();
        hideButton();

        $("#Serial_No").val("");
    });

    $("#add_quality").click(function (e) {

        //alert($('select#Defect_ID option').length);
        if ($('select#defectList option:selected').length > 0) {
            // process to log record
            var defectId = new Array();
            var cnt = 0;
            
            var defectItemId = new Array();
            var cnt1 = 0;
            var isCheckList = true;

            if ($("#quality_checklist_select").is(":checked")) {

                // process to check the validation for cheklist
                if ($("#defectList").val() == "" || $("#defectList").val() == null) {
                    alert("Please select the quality checklist");
                    return false;
                }
                if ($("#defectCheckpoint").val() == "" || $("#defectCheckpoint").val() == null) {
                    alert("Please select the quality checkpoint");
                    return false;
                }

                $("select#defectList option:selected").each(function () {
                    //alert(this.text + ' ' + this.value);
                    if (this.value != "" || this.value != null) {
                        if (cnt == 0) {
                            defectId[cnt] = this.value;
                        }
                        else {
                            defectId[cnt] = this.value;
                            cnt = cnt + 1;
                        }
                        cnt = cnt + 1;
                    }
                });
            }
            else
            {
                // process to check the defect
                if ($("#Quality_Defect_ID").val() == "" || $("#Quality_Defect_ID").val() == null) {
                    alert("Please select the quality defect");
                    return false;
                }
                if ($("#Quality_Defect_Item").val() == "" || $("#Quality_Defect_Item").val() == null) {
                    alert("Please select the quality defect item");
                    return false;
                }

                $("select#Quality_Defect_Item option:selected").each(function () {
                    //alert(this.text + ' ' + this.value);
                    if (this.value != "" || this.value != null) {
                        if (cnt1 == 0) {
                            defectItemId[cnt1] = this.value;
                        }
                        else {
                            defectItemId[cnt1] = this.value;
                            cnt1 = cnt1 + 1;
                        }
                        cnt1 = cnt1 + 1;
                    }
                });

                isCheckList = false;
            }

            var defectCategory = $("#Defect_Category").val();
            if (reworkStatusIsOK()) {

                defectId = defectId.join();
                $.ajaxSettings.traditional = true;
                var stationId = $("#Capture_Station_ID").val();

                var plantId = $("#Plant_ID").val();
                var shopId = $("#Shop_ID").val();
                var lineId = $("#Line_ID").val();
                var captureStationId = $("#Capture_Station_ID").val();
                var modelCode = $("#Model_Code").val();
                var serialNo = $("#Serial_No").val();
                var captureUserId = $("#Capture_User_ID").val();
                var insertedHost = $("#Inserted_Host").val();
                //var severity = $("#Quality_Severity_ID").val();
                //var location = $("#Quality_Location_ID").val();
                var checkpointId = ($("#defectCheckpoint").val() == null) ? 0 : $("#defectCheckpoint").val();
                var remark = $("#Remark").val();
                var reworkTime = parseInt($("#Rework_Time").val());
                var reworkManPower = parseInt($("#Rework_Man_Power").val());
                var correctiveAction = $("#Corrective_Action_ID").val();

                var defectMasterId = ($("#Quality_Defect_ID").val() == null) ? 0 : $("#Quality_Defect_ID").val();
                if (defectMasterId == "" || defectMasterId == null) { defectMasterId = 0;}

                var res = "plantId=" + plantId + "&shopId=" + shopId + "&lineId=" + lineId + "&stationId=" + captureStationId + "&modelCode=" + modelCode + "&serialNo=" + serialNo + "&captureUserId=" + captureUserId + "&insertedHost=" + insertedHost + "&defectList=" + defectId + "&checkpointId=" + checkpointId + "&defectCategory=" + defectCategory + "&remark=" + remark + "&reworkTime=" + reworkTime + "&reworkManPower=" + reworkManPower + "&correctiveAction=" + correctiveAction + "&isChecklist=" + isCheckList + "&defectItemId=" + defectItemId + "&defectMasterId=" + defectMasterId;
                //var res = "plantId=" + plantId+"&shopId=" + shopId ;
                var url = "/QualityLog/addDefectToStation";
                ajaxpack.postAjaxRequest(url, res, saveDefectToStation, "json");
            }
        }
        else {
            alert("Please select defect.");
        }
    });
    function reworkStatusIsOK() {

        var defectCategory = $("#Defect_Category").val();
        if (defectCategory) {
            if (defectCategory == "N") {
                //
            }
            else

                if (defectCategory == "Y" || defectCategory == "A") {

                    // check for corrective action
                    if ($("#Corrective_Action_ID").val() == "" || $("#Corrective_Action_ID").val() == null) {
                        alert("Please select corrective action");
                        return false;
                    }

                    // check for rework duration and rework people
                    if ($("#Rework_Time").val() == "" || $("#Rework_Time").val() == null) {
                        alert("Please enter rework time");
                        return false;
                    }

                    var reworkTime = parseInt($("#Rework_Time").val());
                    if (isNaN(reworkTime)) {
                        alert("Please enter valid rework time");
                        return false;
                    }

                    if ($("#Rework_Man_Power").val() == "" || $("#Rework_Man_Power").val() == null) {

                        alert("Please enter the rework man power");
                        return false;
                    }


                    var reworkManPower = parseInt($("#Rework_Man_Power").val());
                    if (isNaN(reworkManPower)) {
                        alert("Please enter valid rework man power");
                        return false;
                    }

                }
        }
        else {
            alert("Please select defect category");
            return false;
        }

        return true;
        /* else
            if ($("#Quality_Severity_ID").val() == "" || $("#Quality_Severity_ID").val() == null) {
                alert("Please select the severity");
                return false;
            }
            else
                if ($("#Quality_Location_ID").val() == "" || $("#Quality_Location_ID").val() == null) {
                    alert("Please select the location");
                    return false;
                } */
    }


    function saveDefectToStation() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                showQualityDefect(1);
                //loadNoAddedDefect();
                //loadNoAddedCheckpoint();
                $("#no_defect_image").css("display", "none");

                clearQualityExtraDetailFields();

                $("#add_quality_no_concern").addClass("disabled");

                $("#Defect_Category option[value='']").prop("selected", "selected");

                // process to reload the defects
                if ($("#quality_defect_select").is(":checked")) {
                    // process to trigger selected defect master

                    $("#Quality_Defect_ID").trigger("change");
                    
                }
                else
                {
                    // process to trigger selected checklist
                    $("#defectList").trigger("change");
                }
            }
        }
    }

    function enableControls() {

    }

    function showQualityDefect(i) {
        if (i == 1) {
            // same station
            $("#isSameStation").val("1");
        }
        else {
            $("#isSameStation").val("0");
        }

        $("#defectSerialNumber").val($("#Serial_No").val());

        $("#frm_show_defect_list").submit();
    }


    function triggerSerialNumber() {
        var e = $.Event('keydown');
        e.which = 13; // Character 'A'
        $('#Serial_No').trigger(e);
    }

    $("#add_quality_no_concern").click(function (e) {
        //alert("ok");
        // process to add no defect vehicle
        var plantId = $("#Plant_ID").val();
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var captureStationId = $("#Capture_Station_ID").val();
        var modelCode = $("#Model_Code").val();
        var serialNo = $("#Serial_No").val();
        var captureUserId = $("#Capture_User_ID").val();
        var insertedHost = $("#Inserted_Host").val();

        var res = "plantId=" + plantId + "&shopId=" + shopId + "&lineId=" + lineId + "&captureStationId=" + captureStationId + "&modelCode=" + modelCode + "&serialNo=" + serialNo + "&captureUserId=" + captureUserId + "&insertedHost=" + insertedHost;
        var url = "/QualityLog/addNoDefectStation";
        ajaxpack.getAjaxRequest(url, res, showNoDefectSuccess, "json");

    });

    $("#other_quality_station_defect").click(function (e) {
        // process to show the list of defect captured on other station
        showQualityDefect(0);
        $("#no_defect_image").css("display", "none");

    });

    $("#same_quality_station_defect").click(function (e) {
        loadSerialDetail();
    });

    $("#take_out_vehicle").click(function (e) {
        //var confirmResult = confirm("DO you want to take out this item ?");
        //if (confirmResult) {
        //    //alert("ok");

        //}
        //else {
        //    alert("not ok");
        //}

        //alert("ok");

        var stationId = $("#Capture_Station_ID").val();

        var plantId = $("#Plant_ID").val();
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var captureStationId = $("#Capture_Station_ID").val();
        var modelCode = $("#Model_Code").val();
        var serialNo = $("#Serial_No").val();
        var captureUserId = $("#Capture_User_ID").val();
        var insertedHost = $("#Inserted_Host").val();

        var takeoutStationId = $("#takeOutStationId").val();
        if (takeoutStationId == "" || takeoutStationId == null) {
            $("#errorQualityTakeOutStation").html("Required");
            return false;
        }

        var reasonId = $("#Quality_Take_Out_Reason").val();
        if (reasonId == "" || reasonId == null) {
            $("#errorQualityTakeOutReason").html("Required");
            return false;
        }

        var remark = $("#take_out_remark").val();

        var takeoutLineId = $("#takeOutLineId").val();
        var takeoutStationId = $("#takeOutStationId").val();

        var res = "plantId=" + plantId + "&shopId=" + shopId + "&lineId=" + lineId + "&stationId=" + captureStationId + "&modelCode=" + modelCode + "&serialNo=" + serialNo + "&captureUserId=" + captureUserId + "&insertedHost=" + insertedHost + "&reasonId=" + reasonId + "&remark=" + remark + "&takeoutLineId=" + takeoutLineId + "&takeoutStationId=" + takeoutStationId;
        //var res = "plantId=" + plantId+"&shopId=" + shopId ;
        var url = "/QualityLog/TakeOutVehicle";
        ajaxpack.postAjaxRequest(url, res, showTakeOutVehicleDetail, "json");
    });

    function showTakeOutVehicleDetail() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes == true) {
                    $("#quality_take_out .close").trigger("click");
                    $("#Quality_Take_Out_Reason").val("");
                    $("#take_out_remark").val("");
                    alert("Vehicle is take out successfully.");
                    clearAllInputBox();

                }
            }
        }
    }

    function showNoDefectSuccess() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes == true) {
                    alert("Serial is added with no concern successfully.");
                    $("#no_defect_image").show();
                    $("#add_quality_no_concern").addClass("disabled");
                }
                else {
                    // issue occur
                }
            }
        }
    }

    $("#added_defect_list").on("click", ".remove_defect", function () {
        // process to delete the defect for the same station
        var rowId = $(this).attr("id");
        rowId = rowId.split("_");
        rowId = rowId[1];
        rowId = parseInt(rowId);
        if (rowId) {
            var res = "rowId=" + rowId;
            var url = "/QualityLog/RemoveDefectById";
            ajaxpack.getAjaxRequest(url, res, showRemoveDefect, "json");
        }
    });

    function showRemoveDefect() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes == true) {
                    // process to load same station defect list
                    showQualityDefect(1);
                    //loadNoAddedDefect();
                    loadNoAddedCheckpoint();
                }
                else {

                }
            }
        }
    }

    $("#show_other_station_assigned_defect").click(function (e) {
        // process to load the defect assigned to other station
        var stationId = $("#Capture_Station_ID").val();
        var lineId = $("#Line_ID").val();
        var modelCode = $("#Model_Code").val();
        var url = "/QualityLog/GetAddedDefectListDetails";
        ajaxpack.getAjaxRequest(url, "serialNo=" + $("#Serial_No").val() + "&modelCode=" + modelCode + "&stationId=" + stationId + "&lineId=" + lineId + "&isAdded=0", showNoAddedDefect, "json");
    });

    $("#added_defect_list").on("click", ".clear_defect", function () {
        // process to show the rework time
        //alert("sample");
        var rowId = $(this).attr("id");
        rowId = rowId.split("_");
        rowId = rowId[1];
        rowId = parseInt(rowId);
        if (rowId) {
            $("#hdnQualityClearDefect").val(rowId);
        }
        //$("#quality_defect_clear").attr("disabled", "true");

        // load defect details
        var url = "/QualityLog/getDefectDetailByID";
        ajaxpack.getAjaxRequest(url, "defectId=" + rowId, showDefectDetailOnEdit, "json");

        // clear the fields
        clearQualityEditExtraFields();
        //$("#quality_rework_model_code").val("");
        //$("#quality_rework_serial_number").val("");
        //$("#quality_rework_defect_category option[value='']").prop('selected', 'selected');
        //$("#quality_rework_remark").val("");
        //$("#quality_rework_duration").val("");
        //$("#quality_rework_man_power").val("");
    });




    // process to load the defect detail
    function showDefectDetailOnEdit() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);

                // model code

                $("#quality_rework_model_code").val(jsonRes.Model_Code);

                $("#quality_rework_serial_number").val(jsonRes.Serial_No);


                // set defect category
                $("#quality_rework_defect_category option[value=" + jsonRes.Defect_Category + "]").prop('selected', 'selected');
                $("#quality_rework_corrective_action option[value=" + jsonRes.Corrective_Action_ID + "]").prop('selected', 'selected');
                // remark
                $("#quality_rework_remark").val(jsonRes.Remark);

                // duration
                $("#quality_rework_duration").val(jsonRes.Rework_Time);

                // rework man power
                $("#quality_rework_man_power").val(jsonRes.Rework_Manpower);


                // input
                if (jsonRes.Defect_Category == "N") {
                    // hide hours and manpower
                    $("#quality_rework_duration").attr("disabled", "true");
                    $("#quality_rework_man_power").attr("disabled", "true");
                    $("#quality_rework_corrective_action").attr("disabled", "true");
                }
                else if (jsonRes.Defect_Category == "Y" || jsonRes.Defect_Category == "A") {
                    // show hours and manpower
                    $("#quality_rework_duration").removeAttr("disabled");
                    $("#quality_rework_man_power").removeAttr("disabled");
                    $("#quality_rework_corrective_action").removeAttr("disabled");
                }
            }
        }
    }
    $("#quality_rework_defect_category").change(function (e) {

        // check the defect category is changed on edit form
        var defectCatgory = $("#quality_rework_defect_category").val();

        if (defectCatgory == "N") {
            // hide hours and manpower
            $("#quality_rework_duration").attr("disabled", "true");
            $("#quality_rework_man_power").attr("disabled", "true");
            $("#quality_rework_corrective_action").attr("disabled", "true");

            $("#quality_rework_corrective_action option[value='']").prop('selected', 'selected');
            $("#quality_rework_duration").val("");
            $("#quality_rework_man_power").val("");
        }
        else if (defectCatgory == "Y" || defectCatgory == "A") {
            // show hours and manpower
            $("#quality_rework_duration").removeAttr("disabled");
            $("#quality_rework_man_power").removeAttr("disabled");
            $("#quality_rework_corrective_action").removeAttr("disabled");
        }
    });
    $("#quality_defect_clear").click(function (e) {
        var rowId = $("#hdnQualityClearDefect").val();
        var reworkTime = 0, reworkManPower = 0;
        var defectCategory = "";
        if (rowId) {
            $("#errorQualityReworkRemark").html("");

            defectCategory = $("#quality_rework_defect_category").val();
            if (defectCategory) {
                if (defectCategory == "N") {
                    //
                }
                else
                    if (defectCategory == "Y" || defectCategory == "A") {
                        // check for rework duration and rework people
                        if ($("#quality_rework_duration").val() == "" || $("#quality_rework_duration").val() == null) {
                            //alert("Please enter rework time");
                            $("#errorQualityReworkDuration").html("Required");
                            $("#errorQualityReworkDuration").show();
                            return false;
                        }
                        else
                        {
                            $("#errorQualityReworkDuration").html("");
                        }

                        reworkTime = parseInt($("#quality_rework_duration").val());
                        if (isNaN(reworkTime)) {
                            //alert("Please enter valid rework time");
                            $("#errorQualityReworkDuration").html("Invalid");
                            $("#errorQualityReworkDuration").show();
                            return false;
                        }
                        else
                        {
                            $("#errorQualityReworkDuration").html("");
                        }

                        if ($("#quality_rework_man_power").val() == "" || $("#quality_rework_man_power").val() == null) {
                            //alert("Please enter the rework man power");

                            $("#errorQualityReworkManPower").html("Required");
                            $("#errorQualityReworkManPower").show();
                            return false;
                        }
                        else
                        {
                            $("#errorQualityReworkManPower").html("");
                        }

                        reworkManPower = parseInt($("#quality_rework_man_power").val());
                        if (isNaN(reworkManPower)) {
                            $("#errorQualityReworkManPower").html("Invalid");
                            $("#errorQualityReworkManPower").show();
                            return false;
                        }
                        else
                        {

                            $("#errorQualityReworkManPower").html("");
                        }

                        if ($("#quality_rework_corrective_action").val() == "" || $("#quality_rework_corrective_action").val() == null) {
                            //alert("Please enter the rework man power");

                            $("#errorQualityReworkCorrectiveAction").html("Required");

                            $("#errorQualityReworkCorrectiveAction").show();
                            return false;
                        }
                        else
                        {
                            $("#errorQualityReworkCorrectiveAction").html("");
                        }

                    }
            }
            else {
                $("#errorQualityDefectCategory").html("Required");
                return false;
            }

            var remark = $("#quality_rework_remark").val();
            //if (remark == "" || remark == null) {
            //    $("#errorQualityReworkRemark").html("Required");
            //    return false;
            //}

            //var duration = parseInt($("#quality_rework_duration").val());
            //if (duration == "NaN") {
            //    $("#errorQualityReworkDuration").html("Required");
            //    return false;
            //}

            // process to clear the defect
            var stationId = $("#Capture_Station_ID").val();

            var plantId = $("#Plant_ID").val();
            var shopId = $("#Shop_ID").val();
            var lineId = $("#Line_ID").val();
            var captureStationId = $("#Capture_Station_ID").val();
            var modelCode = $("#Model_Code").val();
            var serialNo = $("#Serial_No").val();
            var captureUserId = $("#Capture_User_ID").val();
            var insertedHost = $("#Inserted_Host").val();
            var correctiveAction = $("#quality_rework_corrective_action").val();

            var res = "rowId=" + rowId + "&stationId=" + captureStationId + "&captureUserId=" + captureUserId + "&insertedHost=" + insertedHost + "&remark=" + remark + "&duration=" + reworkTime + "&defectCategory=" + defectCategory + "&reworkManPower=" + reworkManPower + "&correctiveAction=" + correctiveAction;
            //var res = "plantId=" + plantId+"&shopId=" + shopId ;
            var url = "/QualityLog/QualityClearVehicle";
            ajaxpack.postAjaxRequest(url, res, showReworkDetail, "json");

        }
    });

    function showReworkDetail() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes == true) {
                    // process to load other station defect
                    var station = $("#isSameStation").val();
                    showQualityDefect(station);
                    closePopup();

                }
                else {

                }
            }
        }
    }

    function clearSelectBox(targetId) {
        var res = "";
        res = "<option value=''>" + $("#" + targetId + " option:first").html() + "</option>";
        $("#" + targetId).html(res);
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

    this.searchSelectBox = function (textBoxId, targetId) {
        //alert(targetId);
        if ($("#" + textBoxId).val() == "" || $("#" + textBoxId).val() == null) {
            $("#" + targetId + " option").show();
        }
        else {
            var searchString = $("#" + textBoxId).val().toUpperCase().trim();
            $("#" + targetId + " option").each(function () {
                var inputString = $(this).text().toUpperCase();
                if (inputString.indexOf(searchString) > -1) {
                    $(this).show();
                }
                else {
                    $(this).hide();
                }

            });
        }

    }

    // process to load the checkpoint added for the checklist on checklist change

    $(".quality_captures").on("change", "#defectList", function (e) {
        var defectId = $(this).val();
        if (defectId) {
            /* var res = "defectId=" + defectId;
            //var res = "plantId=" + plantId+"&shopId=" + shopId ;
            var url = "/QualityCheckpoint/getCheckpointByDefectID";
            ajaxpack.postAjaxRequest(url, res, showQualityCheckpointDetail, "json"); */

            loadNoAddedCheckpoint();
        }
        else {

            clearSelectBox("defectCheckpoint");
        }
    });

    // process to show the checkpoint list
    function showQualityCheckpointDetail() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "defectCheckpoint");
            }
        }
    }

    // defect category change, show and hide the required things
    $("#Defect_Category").change(function (e) {
        var defectCategory = $(this).val();
        //alert("Okok");
        clearQualityExtraDetailFields();
        if (defectCategory) {
            if (defectCategory == "N") {
                // show remark only
                $("#Rework_Time").attr("disabled", "true");
                $("#Rework_Man_Power").attr("disabled", "true");
                $("#Corrective_Action_ID").attr("disabled", "true");
            }
            else if (defectCategory == "Y" || defectCategory == "A") {
                // show remark, rework time and rework duration
                $("#Rework_Time").removeAttr("disabled");
                $("#Rework_Man_Power").removeAttr("disabled");

                $("#Corrective_Action_ID").removeAttr("disabled");
                $("#Corrective_Action_ID option[value='']").prop('selected', 'selected');
            }
            else if (defectCategory == "A") {
                // show remark, rework time and rework duration
            }
        }
        else {
            $("#Rework_Time").hide("slow");
            $("#Rework_Man_Power").hide("slow");
        }
    });

    function clearQualityExtraDetailFields() {
        $("#Remark").val("");
        $("#Rework_Time").val("");
        $("#Rework_Man_Power").val("");

        $("#Rework_Time").attr("disabled", "true");
        $("#Rework_Man_Power").attr("disabled", "true");

        $("#Corrective_Action_ID").attr("disabled", "true");
        $("#Corrective_Action_ID option[value='']").prop('selected', 'selected');

    }






    function closePopup() {
        $(".modal-header .close").trigger("click");
    }





    function clearQualityEditExtraFields() {
        $("#quality_rework_model_code").val("");
        $("#quality_rework_serial_number").val("");
        $("#quality_rework_defect_category option[value='']").prop('selected', 'selected');
        $("#quality_rework_remark").val("");
        $("#quality_rework_duration").val("");
        $("#quality_rework_man_power").val("");

        $("#errorQualityDefectCategory").html("");
        $("#errorQualityReworkRemark").html("");
        $("#errorQualityReworkDuration").html("");
        $("#quality_rework_corrective_action option[value='']").prop('selected', 'selected');

        $("#errorQualityReworkCorrectiveAction").html("");
    }



    $("#add_log_quality_ok").click(function (e) {
        // process to check all the defect is resolved or not
        var serialNo = $("#Serial_No").val();
        if (serialNo) {
            var res = "serialNo=" + serialNo;
            //var res = "plantId=" + plantId+"&shopId=" + shopId ;
            var url = "/QualityLog/isOrderIsReadyForQualityOK";
            ajaxpack.postAjaxRequest(url, res, isOrderReadyForQualityOK, "json");
        }
        else {
            alert("Please enter the serial number");
        }
    });

    function isOrderReadyForQualityOK() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes == true) {
                    // process to print barcode
                    // process to add quality ok result

                    var plantId = $("#Plant_ID").val();
                    var shopId = $("#Shop_ID").val();
                    var lineId = $("#Line_ID").val();
                    var captureStationId = $("#Capture_Station_ID").val();
                    var modelCode = $("#Model_Code").val();
                    var serialNo = $("#Serial_No").val();
                    var captureUserId = $("#Capture_User_ID").val();
                    var insertedHost = $("#Inserted_Host").val();
                    var res = "plantId=" + plantId + "&shopId=" + shopId + "&lineId=" + lineId + "&stationId=" + captureStationId + "&modelCode=" + modelCode + "&serialNo=" + serialNo;
                    var url = "/QualityLog/saveQualityOKDetail";
                    ajaxpack.postAjaxRequest(url, res, qualityOkOrderDetails, "json");
                }
                else {
                    // no defects are cleared
                    alert("Label will print for this order when you resolve all the defects.")
                }
            }
        }
    }

    function qualityOkOrderDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes == true) {
                    alert("Order is logged successfully");
                    clearAllInputBox();
                }

            }
        }
    }

    $(".show_defect_list_detail").change(function (e) {
        //alert("ok");

        var id = $(this).attr("id");
        if (id == "other_station") {
            // load other station
            $("#show_other_station_assigned_defect").trigger("click");
        }
        else {
            // load same station
            loadNoAddedDefect();
        }

        clearSelectBox("defectCheckpoint");
    });


    function showAddedDefectMaster() {
        // process to show the defect master added to station
        var stationId = $("#Capture_Station_ID").val();
        var lineId = $("#Line_ID").val();
        var modelCode = $("#Model_Code").val();
        var serialNo = $("#Serial_No").val();
        if (serialNo) {


            var url = "/QualityLog/GetAddedDefectMasterListDetails";
            ajaxpack.getAjaxRequest(url, "serialNo=" + $("#Serial_No").val() + "&modelCode=" + modelCode + "&stationId=" + stationId + "&lineId=" + lineId + "&isAdded=1", showDefectMasterDetail, "json");
        }
    }

    function showDefectMasterDetail() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Quality_Defect_ID");
                //SelectOptionHTML(jsonRes, "Shop_ID");
            }
        }
    }

    function showNoAddedDefectMaster() {
        // process to show the defect master added to station
        var stationId = $("#Capture_Station_ID").val();
        var lineId = $("#Line_ID").val();
        var modelCode = $("#Model_Code").val();
        var serialNo = $("#Serial_No").val();
        if (serialNo) {
            var url = "/QualityLog/GetAddedDefectMasterListDetails";
            ajaxpack.getAjaxRequest(url, "serialNo=" + $("#Serial_No").val() + "&modelCode=" + modelCode + "&stationId=" + stationId + "&lineId=" + lineId + "&isAdded=0", showDefectMasterDetail, "json");
        }
    }

    $(".show_defect_master_list_detail").change(function (e) {
        //alert("ok");

        var id = $(this).attr("id");
        if (id == "other_station_defect_details") {
            // load other station
            showNoAddedDefectMaster();
        }
        else {
            // load same station
            showAddedDefectMaster();
        }

        clearSelectBox("Quality_Defect_Item");
    });

    // process to enable or disable checklist and defects
    $(".select_defect_options").change(function (e) {
        //alert("ok");

        var id = $(this).attr("id");
        //alert(id);
        if (id == "quality_checklist_select") {
            // show quality checklist
            enableQualityChecklist();
        }
        else {
            // load same station
            enableQualityDefect();
        }

    });

    function enableQualityChecklist() {
        $("#defectList").prop("disabled", "");
        $("#defectCheckpoint").prop("disabled", "");
        $("#search_defect").prop("disabled", "");

        $("#Quality_Defect_ID").prop("disabled", "true");
        $("#Quality_Defect_Item").prop("disabled", "true");
        $("#search_quality_defect").prop("disabled", "true");

        $("#same_station").trigger("click");
    }

    function enableQualityDefect() {

        $("#Quality_Defect_ID").prop("disabled", "");
        $("#Quality_Defect_Item").prop("disabled", "");
        $("#search_quality_defect").prop("disabled", "");

        $("#defectList").prop("disabled", "true");
        $("#defectCheckpoint").prop("disabled", "true");
        $("#search_defect").prop("disabled", "true");
        $("#same_station_defect_details").trigger("click");
    }

    $("#Quality_Defect_ID").change(function (e) {
        var defectId = $(this).val();
        var serialNo = $("#Serial_No").val();
        var stationId=$("#Capture_Station_ID").val();
        if (serialNo && defectId) {
            var url = "/QualityLog/getQualityDefectItemDetails";
            ajaxpack.getAjaxRequest(url, "serialNo=" + serialNo + "&qualityDefectId=" + defectId + "&stationId=" + stationId, showDefectItemDetail, "json");
        }
        else {
            clearSelectBox("Quality_Defect_Item");
        }
    });

    function showDefectItemDetail() {

        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Quality_Defect_Item");
                //SelectOptionHTML(jsonRes, "Shop_ID");
            }
        }
    }

    // process to add the defect as deviation
    $("#add_deviation").click(function (e) {
        // check it is checklist or defect
        var isChecklist = false;

        var serialNo = "";
        var modelCode = $("#Model_Code").val();
        var qualityCheckListId = 0;
        var qualityCheckpointId = "";

        var qualityDefectId = 0;
        var qualityDefectItem = 0;
        if (modelCode == "" || modelCode == null)
        {
            alert("Please enter valid serial number");
            return false;
        }

        if ($("#quality_checklist_select").is(":checked"))
        {
            isChecklist = true;
            qualityCheckListId = $("#defectList").val();
            if (qualityCheckListId == "" || qualityCheckListId == null)
            {
                alert("Please select checklist");
                return false;
            }


            qualityCheckpointId = new Array();
            var cnt = 0;
            
            $("select#defectCheckpoint option:selected").each(function () {
                //alert(this.text + ' ' + this.value);
                if (this.value != "" || this.value != null) {
                    if (cnt == 0) {
                        qualityCheckpointId[cnt] = this.value;
                    }
                    else {
                        qualityCheckpointId[cnt] = this.value;
                        cnt = cnt + 1;
                    }
                    cnt = cnt + 1;
                }
            });


            if(cnt==0)
            {
                alert("Please select checkpoint");
                return false;
            }
        }
        else
        {
            qualityDefectId = $("#Quality_Defect_ID").val();
            if (qualityDefectId == "" || qualityDefectId == null)
            {
                alert("Please select the defect");
                return false;
            }
            


            qualityDefectItem = new Array();
            var cnt = 0;

            $("select#Quality_Defect_Item option:selected").each(function () {
                //alert(this.text + ' ' + this.value);
                if (this.value != "" || this.value != null) {
                    if (cnt == 0) {
                        qualityDefectItem[cnt] = this.value;
                    }
                    else {
                        qualityDefectItem[cnt] = this.value;
                        cnt = cnt + 1;
                    }
                    cnt = cnt + 1;
                }
            });



            if(cnt==0)
            {
                alert("Please select the defect item");
                return 0;
            }

        }



        // process to log deviation
        var stationId = $("#Capture_Station_ID").val();

        var plantId = $("#Plant_ID").val();
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var captureStationId = $("#Capture_Station_ID").val();
        var modelCode = $("#Model_Code").val();
        var serialNo = $("#Serial_No").val();
        var captureUserId = $("#Capture_User_ID").val();
        var insertedHost = $("#Inserted_Host").val();
        var correctiveAction = $("#quality_rework_corrective_action").val();

        var res = "plantId=" + plantId + "&shopId=" + shopId + "&lineId=" + lineId + "&stationId=" + captureStationId + "&captureUserId=" + captureUserId + "&insertedHost=" + insertedHost + "&serialNo=" + serialNo + "&modelCode=" + modelCode + "&checkListId=" + qualityCheckListId + "&checkpointId=" + qualityCheckpointId + "&qualityDefectId=" + qualityDefectId + "&qualityDefectItem=" + qualityDefectItem + "&isChecklist=" + isChecklist;
        //var res = "plantId=" + plantId+"&shopId=" + shopId ;
        var url = "/QualityLog/saveDefectDeviation";
        ajaxpack.postAjaxRequest(url, res, saveDefectToStation, "json");

    });

});


