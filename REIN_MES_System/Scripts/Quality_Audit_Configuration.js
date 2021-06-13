$(document).ready(function (e) {

      $(".audit_checklist_checkpoint_configuration #Sub_Category_ID").on("change", function () {
                $("#Audit_Type_ID").html("");
                $('#Audit_Type_ID').append('<option value="">Select Audit Type</option>');
                var Sub_Category_ID = $("#Sub_Category_ID").val();
                if (Sub_Category_ID.length > 0) {
                    $.getJSON('/AuditChecklist/getAuditTypeBySubCategory', { sub_Category_ID: Sub_Category_ID }, function (data) {
                        if (data.length > 0) {
                            $('#Audit_Type_ID option').remove();
                            $('#Audit_Type_ID').append('<option value="">Select Audit Type</option>');
                            for (var i = 0; i < data.length; i++) {
                                $('#Audit_Type_ID').append('<option value="' + data[i].Id + '">' + data[i].Value + '</option>');
                            }
                        }
                    });
                }
            });

            $(".audit_checklist_checkpoint_configuration #Audit_Category_ID").on("change", function () {
                $("#Sub_Category_ID").html("");
                $('#Sub_Category_ID').append('<option value="">Select Sub Category</option>');
                $("#Audit_Type_ID").html("");
                $('#Audit_Type_ID').append('<option value="">Select Audit Type</option>');
                var Audit_Category_ID = $("#Audit_Category_ID").val();
                if (Audit_Category_ID.length > 0) {
                    $.getJSON('/AuditType/getSubCategoryByCategory', { audit_Category_ID: Audit_Category_ID }, function (data) {
                        if (data.length > 0) {
                            $('#Sub_Category_ID option').remove();
                            $('#Sub_Category_ID').append('<option value="">Select Sub Category</option>');
                            for (var i = 0; i < data.length; i++) {
                                $('#Sub_Category_ID').append('<option value="' + data[i].Id + '">' + data[i].Value + '</option>');
                            }
                        }
                    });
                }

            });

    $(".plant_line_configuration #Plant_ID").change(function (e) {
        //var jsonData = JSON.stringify({ plantId: 7 });

        clearSelectBox("Checklist_ID");
        clearSelectBox("selectedChecklist");
        var plantId = $("#Plant_ID").val();
        if (plantId) {
            var url = "/Shop/GetShopByPlantID";
            ajaxpack.getAjaxRequest(url, "plantId=" + $("#Plant_ID").val() + "", showPlantShopID, "json");

            var url = "/QualityChecklist/getModelFamilyByPlant";
            ajaxpack.getAjaxRequest(url, "plantId=" + $("#Plant_ID").val() + "", showPlantFamily, "json");
        }
        else {
            // clear the line type and shop
            clearSelectBox("Line_ID");
            clearSelectBox("Shop_ID");
            clearSelectBox("Station_ID");
            clearSelectBox("Attribute_ID")
        }
    });

    function showPlantFamily() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Attribute_ID");
            }
        }
    }

    function showPlantShopID() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Shop_ID");
            }
        }
    }

    $(".plant_line_configuration #Shop_ID").change(function (e) {
        //var jsonData = JSON.stringify({ plantId: 7 });

        //clearStationLists();
        //loadHTMLTable();

        clearSelectBox("Checklist_ID");
        clearSelectBox("selectedChecklist");

        var shopId = $("#Shop_ID").val();
        if (shopId) {
            var url = "/Line/GetLineByShopID";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showLineShopID, "json");
            var plantId = $("#Plant_ID").val();
            setTimeout(function (e) {

                var url = "/QualityChecklist/getModelFamilyByPlantShop";
                ajaxpack.getAjaxRequest(url, "plantId=" + plantId + "&shopId=" + shopId, showModelFamily, "json");
            }, 2000);


        }
        else {
            // clear the line type and shop
            clearSelectBox("Line_ID");
            clearSelectBox("Station_ID");
        }
    });


    function showLineShopID() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Line_ID");
            }
        }
    }

    $(".plant_line_configuration #Line_ID").click(function (e) {

        clearSelectBox("Checklist_ID");
        clearSelectBox("selectedChecklist");

        var lineId = $("#Line_ID").val();
        if (lineId) {

            var url = "/QualityStationList/GetQualityStationByLineID";
            ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showLineStationID, "json");
        }
    });

    function showLineStationID() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Station_ID");
            }
        }
    }

    $(".quality_defect_station #Attribute_ID").change(function (e) {
        var stationId = $("#Station_ID").val();
        var shopId = $("#Shop_ID").val();
        var family = $("#Attribute_ID").val();
        if (stationId && family) {

            clearSelectBox("Checklist_ID");
            clearSelectBox("selectedChecklist");

            var plantId = $("#Plant_ID").val();
            var url = "/QualityStationChecklist/getNoAddedChecklistToStationId";
            ajaxpack.getAjaxRequest(url, "shopId=" + shopId + "&stationId=" + stationId + "&family=" + family + "", showNoAddedChecklistToStation, "json");

            setTimeout(function () {
                url = "/QualityStationChecklist/getAddedChecklistToStationId";
                ajaxpack.getAjaxRequest(url, "shopId=" + shopId + "&stationId=" + stationId + "&family=" + family + "", showAddedChecklistToStation, "json");
            }, 1000);
        }
        else {
            clearSelectBox("Checklist_ID");
            clearSelectBox("selectedChecklist");
        }
    });


    $(".plant_line_configuration #Station_ID").change(function (e) {
        clearSelectBox("Checklist_ID");
        clearSelectBox("selectedChecklist");

        var stationId = $(this).val();
        if (stationId) {
            $(".quality_defect_station #Attribute_ID").trigger("change");
        }

    });

    function showNoAddedChecklistToStation() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Checklist_ID");
            }
        }
    }

    function showAddedChecklistToStation() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "selectedChecklist");
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

    window.swapValue = function (sourceId, targetId, direction) {
        if (direction == ">") {
            // source to target
            // alert(sourceId);
            var flag = 0;
            $("#" + sourceId + " :selected").each(function (i, selected) {
                if ($(selected).val() == "" || $(selected).val() == null) {

                }
                else {
                    $("#" + targetId).append($('<option>', {
                        value: $(selected).val(),
                        text: $(selected).text()
                    }));
                    //foo[i] = $(selected).text();
                    flag = 1;
                    // remove item from source
                    $("#" + sourceId + " option[value='" + $(selected).val() + "']").remove();
                }
            });
           // alert(flag);
            if (flag == 0 && (sourceId == "Checklist_ID" || sourceId == "selectedChecklist")) {
                $('#testy').toastee({
                    type: 'error',
                    width: '100px',
                    height: '100px',
                    message: 'Please select checklist...',
                });
            }

            if (flag == 0 && (sourceId == "Audit_Checkpoint_ID" || sourceId == "selectedCheckpoint")) {
                $('#testy').toastee({
                    type: 'error',
                    width: '100px',
                    height: '100px',
                    message: 'Please select checkpoint...',
                });
            }

            if (flag == 0 && (sourceId == "Defect_ID" || sourceId == "selectedDefects")) {
                $('#testy').toastee({
                    type: 'error',
                    width: '100px',
                    height: '100px',
                    message: 'Please select defect...',
                });
            }
            if (flag == 0 && (sourceId == "Corrective_Action_ID" || sourceId == "selectedCorrectiveActions")) {
                $('#testy').toastee({
                    type: 'error',
                    width: '100px',
                    height: '100px',
                    message: 'Please select corrective action...',
                });
            }
        }
        else {
            // target to source          
        }
    }

    $(".select_defect").click(function (e) {
        $('#selectedChecklist option').prop('selected', true);
        return true;
    });

    // quality checkpoint js

    $(".quality_checkpoint #Plant_ID").change(function (e) {
        var plantId = $(this).val();
        clearSelectBox("Checklist_ID");

        if (plantId) {
            var url = "/QualityDefects/getDefectByPlantID";
            ajaxpack.getAjaxRequest(url, "plantId=" + plantId + "", showQualityCheckpointDetail, "json");
        }
    });

    function showQualityCheckpointDetail() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Checklist_ID");
                //SelectOptionHTML(jsonRes, "Shop_ID");
            }
        }
    }

    //quality defect item js

    $(".quality_defect_item #Plant_ID").change(function (e) {
        var plantId = $(this).val();
        clearSelectBox("Quality_Checklist_ID");
        if (plantId) {
            var url = "/QualityDefectMaster/getDefectMasterByPlantID";
            ajaxpack.getAjaxRequest(url, "plantId=" + plantId + "", showQualityDefectMasterDetails, "json");
        }
    });

    function showQualityDefectMasterDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Quality_Checklist_ID");
            }
        }
    }

    // quality defect master to station

    $(".quality_station_defects #Station_ID").change(function (e) {
        var stationId = $("#Station_ID").val();
        if (stationId) {

            clearSelectBox("Quality_Checklist_ID");
            clearSelectBox("selectedChecklist");

            var plantId = $("#Plant_ID").val();

            var url = "/QualityDefectMaster/getNoAddedDefectToStation";
            ajaxpack.getAjaxRequest(url, "stationId=" + stationId + "&plantId=" + plantId + "", showNoDefectAddedToStationMaster, "json");

            setTimeout(function () {
                url = "/QualityDefectMaster/getAddedDefectToStation";
                ajaxpack.getAjaxRequest(url, "stationId=" + stationId + "&plantId=" + plantId + "", showDefectAddedToStationMaster, "json");
            }, 1000);
        }
        else {
            clearSelectBox("Quality_Checklist_ID");
            clearSelectBox("selectedChecklist");
        }
    });

    function showNoDefectAddedToStationMaster() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Quality_Checklist_ID");
            }
        }
    }

    function showDefectAddedToStationMaster() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "selectedChecklist");
            }
        }
    }

    // show quality family
    $(".show_family_quality #Plant_ID").change(function (e) {
        var plantId = $(this).val();
        if (plantId) {
            var url = "/QualityChecklist/getModelFamilyByPlant";
            ajaxpack.getAjaxRequest(url, "plantId=" + plantId, showModelFamily, "json");
        }
        else {
            clearSelectBox("Attribute_ID");
        }
    });

    $(".show_family_quality #Shop_ID").change(function (e) {
        var plantId = $("#Plant_ID").val();
        var shopId = $(this).val();
        if (plantId) {
            var url = "/QualityChecklist/getModelFamilyByPlantShop";
            ajaxpack.getAjaxRequest(url, "plantId=" + plantId + "&shopId=" + shopId, showModelFamily, "json");
        }
        else {
            clearSelectBox("Attribute_ID");
        }
    });

    function showModelFamily() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Attribute_ID");
            }
        }
    }

    $(".audit_checklist_checkpoint_configuration #Plant_ID").change(function (e) {
        var plantId = $(this).val();
        clearSelectBox("Attribute_ID");
        clearSelectBox("Audit_Checklist_ID");
        clearSelectBox("Audit_Checkpoint_ID");
        clearSelectBox("selectShopId");
        clearSelectBox("selectedCheckpoint");
        if (plantId) {
            // process to get the plant family
            var url = "/QualityChecklist/getModelFamilyByPlant";
            ajaxpack.getAjaxRequest(url, "plantId=" + plantId, showModelFamily, "json");
        }
    });

    $(".audit_checklist_checkpoint_configuration #Audit_Category_ID").change(function (e) {
        clearSelectBox("Audit_Checklist_ID");
        var categoryID = $(this).val();
        if (categoryID == 1)
            $(".attributeDiv").hide();
        else
            $(".attributeDiv").show();
    });

    $(".audit_checklist_checkpoint_configuration #Audit_Type_ID").change(function (e) {

        var Audit_Category_ID = $("#Audit_Category_ID").val();
        var Audit_Type_ID = $("#Audit_Type_ID").val();
        var Sub_Category_ID = $("#Sub_Category_ID").val();
        if (Audit_Category_ID == 1) {
            $("#attributeDiv").hide();
            var plantId = $("#Plant_ID").val();
            var shopId = $("#Shop_ID").val();
            var attributeId = $("#Attribute_ID").val();
            clearSelectBox("Audit_Checklist_ID");
            clearSelectBox("Audit_Checkpoint_ID");
            clearSelectBox("selectedCheckpoint");
            clearSelectBox("selectShopId");
            //if (attributeId) {
            // process to get the checklist
            var url = "/AuditChecklist/getChecklistByFamilyPlantAndShop";
            ajaxpack.getAjaxRequest(url, "plantId=" + plantId + "&shopId=" + shopId + "&attributeId=" + attributeId + "&Audit_Category_ID=" + Audit_Category_ID + "&Sub_Category_ID=" + Sub_Category_ID + "&Audit_Type_ID=" + Audit_Type_ID, showChecklistDetail, "json");
            //  }
        }
        else {
            $(".attributeDiv").show();
        }
    });

    $(".audit_checklist_checkpoint_configuration #Shop_ID").change(function (e) {
        var plantId = $("#Plant_ID").val();
        var shopId = $(this).val();
        clearSelectBox("Attribute_ID");
        clearSelectBox("Audit_Checklist_ID");
        clearSelectBox("Audit_Checkpoint_ID");
        clearSelectBox("selectShopId");
        clearSelectBox("selectedCheckpoint");
        if (shopId.length>0) {
            // process to get the plant family
            var url = "/AuditChecklist/getModelFamilyByPlantShops";
            ajaxpack.getAjaxRequest(url, "plantId=" + plantId + "&shopId=" + shopId, showModelFamily, "json");
        }
    });

    $(".audit_checklist_checkpoint_configuration #Attribute_ID").change(function (e) {
        var Audit_Category_ID = $("#Audit_Category_ID").val();
        var Audit_Type_ID = $("#Audit_Type_ID").val();
        var Sub_Category_ID = $("#Sub_Category_ID").val();

        if (Audit_Category_ID == 2) {
            var plantId = $("#Plant_ID").val();
            var shopId = $("#Shop_ID").val();
            var attributeId = $(this).val();
            clearSelectBox("Audit_Checklist_ID");
            clearSelectBox("Audit_Checkpoint_ID");
            clearSelectBox("selectedCheckpoint");
            clearSelectBox("selectShopId");
            
            //if (attributeId) {
            // process to get the checklist
            var url = "/AuditChecklist/getChecklistByFamilyPlantAndShop";
            ajaxpack.getAjaxRequest(url, "plantId=" + plantId + "&shopId=" + shopId + "&attributeId=" + attributeId + "&Audit_Category_ID=" + Audit_Category_ID + "&Sub_Category_ID=" + Sub_Category_ID + "&Audit_Type_ID=" + Audit_Type_ID, showChecklistDetail, "json");
            //  }
        }
    });

    function showChecklistDetail() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Audit_Checklist_ID");
            }
        }
    }

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
            //alert("checked");
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

                //$("#" + checkpointId + " .quality_checkpoint_station_users").show("slow");

                // process to load the defect for the selected checkpoint

                $("#hdnSelectedCheckpoint").val(checkpointId);
                var url = "/QualityDefects/getAddedDefectToCheckpoint";
                ajaxpack.getAjaxRequest(url, "checkpointId=" + checkpointId, showDefectDetail, "json");

                loadStationUsers();
                $("#" + checkpointId + " .quality_defect_category").show("slow");
            }
        }
        else {
            //alert("not checked");
            // process to hide all the inputs
            $("#" + checkpointId + " .quality_defect").hide("slow");

            $("#" + checkpointId + " .defect_value").hide("slow");
            $("#" + checkpointId + " .quality_remarks").hide("slow");

            $("#" + checkpointId + " .defect_resolved").hide("slow");
            $("#" + checkpointId + " .quality_corrective_actions").hide("slow");
            uncheckQualityResolved(checkpointId);


            clearSelectBox("checkpoint_defect_" + checkpointId);

            //$("#" + checkpointId + " .quality_checkpoint_station_users").hide("slow");
        }
    });

    $(".audit_checklist_checkpoint_configuration #Audit_Checklist_ID").change(function (e) {
        var checklist = $("#Audit_Checklist_ID").val();
        var Audit_Category_ID = $("#Audit_Category_ID").val();
        clearSelectBox("Audit_Checkpoint_ID");
        clearSelectBox("selectedCheckpoint");
        clearSelectBox("selectShopId");
          //  $(".attributeDiv").hide();
            if (checklist) {
                // process to get the list of checkpoint added under the checklist

                var url = "/AuditChecklist/getSelectedCheckpointByChecklist";
                ajaxpack.getAjaxRequest(url, "checklist=" + checklist + "&Audit_Category_ID=" + Audit_Category_ID, showAddedCheckpoint, "json");
                var shopId = $("#Shop_ID").val();
                setTimeout(function () {
                    var url = "/AuditChecklist/getNotSelectedCheckpointByChecklistShop";
                    ajaxpack.getAjaxRequest(url, "checklist=" + checklist + "&shopId=" + shopId + "&Audit_Category_ID=" + Audit_Category_ID, showNotAddedCheckpoint, "json");
                }, 2000);

                // process to load the shop id if user select other that tractor
                //if (shopId == "4") {
                //    setTimeout(function () {
                //        var plantId = $("#Plant_ID").val();
                //        var url = "/Shop/GetShopByPlantID";
                //        ajaxpack.getAjaxRequest(url, "plantId=" + plantId, showShopDetails, "json");
                //    }, 1000);
                //}
            }
    });


    function showShopDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "selectShopId");
            }
        }
    }

    $("#selectShopId").change(function (e) {

        var shopId = $(this).val();

        // process to get all the checkpoints of that shop
        var checklist = $("#Audit_Checklist_ID").val();
        var url = "/AuditChecklist/getNotSelectedCheckpointByChecklistShop";
        ajaxpack.getAjaxRequest(url, "checklist=" + checklist + "&shopId=" + shopId, showNotAddedCheckpoint, "json");
    });

    function showAddedCheckpoint() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "selectedCheckpoint");
            }
        }
    }

    function showNotAddedCheckpoint() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Audit_Checkpoint_ID");
            }
        }
    }

    $(".select_defect").click(function (e) {
        $('.select_all_items option').prop('selected', true);
        return true;
    });

    // process for checkpoint to defects
    $(".quality_checkpoint_audit_defects #Audit_Checkpoint_ID").change(function (e) {
        var checkpointId = $(this).val();
        var shopId = $("#Shop_ID").val();
        clearSelectBox("Defect_ID");
        clearSelectBox("selectedDefects");
        if (checkpointId) {
            var url = "/AuditDefects/getAddedDefectToCheckpoint";
            ajaxpack.getAjaxRequest(url, "checkpointId=" + checkpointId, showAddedDefects, "json");

            setTimeout(function () {
                var url = "/AuditDefects/getNotAddedDefectToCheckpointByShopId";
                ajaxpack.getAjaxRequest(url, "shopId=" + shopId + "&checkpointId=" + checkpointId, showNotAddedDefects, "json");
            }, 2000);
        }
    });

    function showAddedDefects() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "selectedDefects");
            }
        }
    }

    // process for checkpoint to defects
    $(".quality_checkpoint_audit_defects #Shop_ID").change(function (e) {
        var shopId = $(this).val();

        clearSelectBox("Audit_Checkpoint_ID");
        clearSelectBox("Defect_ID");
        clearSelectBox("selectedDefects");
        if (shopId) {

            //setTimeout(function (e) {
                var url = "/AuditCheckpoint/getCheckpointByShopId";
                ajaxpack.getAjaxRequest(url, "shopId=" + shopId, showCheckpointDetails, "json");
         //   }, 200);

        }
    });

    function showCheckpointDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Audit_Checkpoint_ID");
            }
        }
    }

    function showNotAddedDefects() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Defect_ID");
            }
        }
    }

    $(".audit_defect_corrective_actions #Defect_ID").change(function (e) {

        var defectId = $(this).val();

        var shopId = $("#Shop_ID").val();
        clearSelectBox("selectedCorrectiveActions");
        clearSelectBox("Corrective_Action_ID");
        if (defectId) {
            var url = "/QualityAuditCaptures/getCorrectiveActionByDefectId";
            ajaxpack.getAjaxRequest(url, "defectId=" + defectId, showQualityCorrectiveActions, "json");

            setTimeout(function (e) {
                var url = "/QualityAuditCaptures/getNotCorrectiveActionByShopIdDefectId";
                ajaxpack.getAjaxRequest(url, "shopId=" + shopId + "&defectId=" + defectId, showNotSelectedQualityCorrectiveActions, "json");
            }, 2000);
        }
    });

    $(".audit_defect_corrective_actions #Shop_ID").change(function (e) {

        var shopId = $(this).val();
        clearSelectBox("Defect_ID");
        clearSelectBox("selectedCorrectiveActions");
        clearSelectBox("Corrective_Action_ID");
        if (shopId) {
            var url = "/AuditDefects/getQualityDefectByShop";
            ajaxpack.getAjaxRequest(url, "shopId=" + shopId, showQualityDefectDetails, "json");
        }
    });

    function showQualityDefectDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Defect_ID");
            }
        }
    }

    function showQualityCorrectiveActions() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);

                SelectOptionHTML(jsonRes, "selectedCorrectiveActions");
            }
        }
    }

    function showNotSelectedQualityCorrectiveActions() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);

                SelectOptionHTML(jsonRes, "Corrective_Action_ID");
            }
        }
    }

    this.searchSelectBox = function (textBoxId, targetId) {
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

});