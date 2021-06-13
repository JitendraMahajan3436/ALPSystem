$(document).ready(function (e) {

    $(".plant_line_configuration #Plant_ID").change(function (e) {
        //var jsonData = JSON.stringify({ plantId: 7 });

        clearSelectBox("Checklist_ID");
        clearSelectBox("selectedChecklist");
        var plantId = $("#Plant_ID").val();
        if (plantId) {

            var url = "/Shop/GetShopByPlantID";
            ajaxpack.getAjaxRequest(url, "plantId=" + $("#Plant_ID").val() + "", showPlantShopID, "json");



            setTimeout(function (e) {
                var url = "/QualityChecklist/getModelFamilyByPlant";
                ajaxpack.getAjaxRequest(url, "plantId=" + $("#Plant_ID").val() + "", showPlantFamily, "json")
            }, 2000);

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
        if (shopId.length > 0) {
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
            clearSelectBox("Attribute_ID");
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


    $(".quality_defect_station #Line_ID").click(function (e) {

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

            if (flag == 0 && (sourceId == "Checkpoint_ID" || sourceId == "selectedCheckpoint")) {
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
        clearSelectBox("Attribute_ID");
        clearSelectBox("Model_ID");
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

    $(".show_family_quality #Attribute_ID").change(function (e) {
        clearSelectBox("Model_ID");
        var Attribute_ID = $(this).val();
        var Shop_ID = $("#Shop_ID").val();
        if (Attribute_ID) {
            var url = "/QualityChecklist/GetModelCodeByFamily";
            ajaxpack.getAjaxRequest(url, "Shop_ID=" + Shop_ID + "&Attribute_ID="+Attribute_ID, ShowFamilyModels, "json");
        }
    });

    function ShowFamilyModels() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Model_ID");
            }
        }
    }

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

    $(".checklist_checkpoint_configuration #Plant_ID").change(function (e) {
        var plantId = $(this).val();

        clearSelectBox("Attribute_ID");
        clearSelectBox("Checklist_ID");
        clearSelectBox("Checkpoint_ID");
        clearSelectBox("selectShopId");
        clearSelectBox("selectedCheckpoint");
        if (plantId) {
            // process to get the plant family
            var url = "/QualityChecklist/getModelFamilyByPlant";
            ajaxpack.getAjaxRequest(url, "plantId=" + plantId, showModelFamily, "json");
        }
    });

    $(".checklist_checkpoint_configuration #Shop_ID").change(function (e) {
        var plantId = $("#Plant_ID").val();
        var shopId = $(this).val();
        clearSelectBox("Attribute_ID");
        clearSelectBox("Checklist_ID");
        clearSelectBox("Checkpoint_ID");
        clearSelectBox("selectShopId");
        clearSelectBox("selectedCheckpoint");
        if (plantId && shopId) {
            // process to get the plant family
            var url = "/QualityChecklist/getModelFamilyByPlantShop";
            ajaxpack.getAjaxRequest(url, "plantId=" + plantId + "&shopId=" + shopId, showModelFamily, "json");
        }
        else {
            S('#Checkpoint_ID').empty();
            S('#selectedCheckpoint').empty();
        }
    });

    $(".checklist_checkpoint_configuration #Attribute_ID").change(function (e) {
        var plantId = $("#Plant_ID").val();
        var shopId = $("#Shop_ID").val();
        var attributeId = $(this).val();
        clearSelectBox("Checklist_ID");
        clearSelectBox("Checkpoint_ID");
        clearSelectBox("selectedCheckpoint");
        clearSelectBox("selectShopId");
        if (attributeId) {
            // process to get the checklist
            var url = "/QualityChecklist/getChecklistByFamilyPlantAndShop";
            ajaxpack.getAjaxRequest(url, "plantId=" + plantId + "&shopId=" + shopId + "&attributeId=" + attributeId, showChecklistDetail, "json");
        }
    });

    function showChecklistDetail() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Checklist_ID");
            }
        }
    }

    $(".checklist_checkpoint_configuration #Checklist_ID").change(function (e) {
        var checklist = $(this).val();
        clearSelectBox("Checkpoint_ID");
        clearSelectBox("selectedCheckpoint");
        clearSelectBox("selectShopId");
        if (checklist) {
            // process to get the list of checkpoint added under the checklist

            var url = "/QualityChecklist/getSelectedCheckpointByChecklist";
            ajaxpack.getAjaxRequest(url, "checklist=" + checklist, showAddedCheckpoint, "json");
            var shopId = $("#Shop_ID").val();
            setTimeout(function () {
                var url = "/QualityChecklist/getNotSelectedCheckpointByChecklistShop";
                ajaxpack.getAjaxRequest(url, "checklist=" + checklist + "&shopId=" + shopId, showNotAddedCheckpoint, "json");
            }, 2000);

            // process to load the shop id if user select other that tractor
            if (shopId) {
                setTimeout(function () {
                    var plantId = $("#Plant_ID").val();
                    var url = "/Shop/GetShopByPlantID";
                    ajaxpack.getAjaxRequest(url, "plantId=" + plantId+"&shop_ID="+shopId, showShopDetails, "json");
                }, 1000);
            }
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
        clearSelectBox("Checkpoint_ID");
       // clearSelectBox("selectedCheckpoint");
        // process to get all the checkpoints of that shop
        if (shopId) {
            var checklist = $("#Checklist_ID").val();
            var url = "/QualityChecklist/getNotSelectedCheckpointByChecklistShop";
            ajaxpack.getAjaxRequest(url, "checklist=" + checklist + "&shopId=" + shopId, showNotAddedCheckpoint, "json");
        }
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
                SelectOptionHTML(jsonRes, "Checkpoint_ID");
            }
        }
    }

    $(".select_defect").click(function (e) {

        $('.select_all_items option').prop('selected', true);


        return true;
    });

    // process for checkpoint to defects
    $(".quality_checkpoint_defects #Checkpoint_ID").change(function (e) {
        var checkpointId = $(this).val();
        var shopId = $("#Shop_ID").val();
        clearSelectBox("Defect_ID");
        clearSelectBox("selectedDefects");
        if (checkpointId) {
            var url = "/QualityDefects/getAddedDefectToCheckpoint";
            ajaxpack.getAjaxRequest(url, "checkpointId=" + checkpointId, showAddedDefects, "json");

            setTimeout(function () {
                var url = "/QualityDefects/getNotAddedDefectToCheckpointByShopId";
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
    $(".quality_checkpoint_defects #Shop_ID").change(function (e) {
        var shopId = $(this).val();

        clearSelectBox("Checkpoint_ID");
        clearSelectBox("Defect_ID");
        clearSelectBox("selectedDefects");
        if (shopId) {

            setTimeout(function (e) {
                var url = "/QualityCheckpoint/getCheckpointByShopId";
                ajaxpack.getAjaxRequest(url, "shopId=" + shopId, showCheckpointDetails, "json");
            }, 200);

        }
    });

    function showCheckpointDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Checkpoint_ID");
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

    $(".quality_defect_corrective_actions #Defect_ID").change(function (e) {

        var defectId = $(this).val();

        var shopId = $("#Shop_ID").val();
        clearSelectBox("selectedCorrectiveActions");
        clearSelectBox("Corrective_Action_ID");
        if (defectId) {
            var url = "/QualityCaptures/getCorrectiveActionByDefectId";
            ajaxpack.getAjaxRequest(url, "defectId=" + defectId, showQualityCorrectiveActions, "json");

            setTimeout(function (e) {
                var url = "/QualityCaptures/getNotCorrectiveActionByShopIdDefectId";

                ajaxpack.getAjaxRequest(url, "shopId=" + shopId + "&defectId=" + defectId, showNotSelectedQualityCorrectiveActions, "json");
            }, 2000);
        }
    });

    $(".quality_defect_corrective_actions #Shop_ID").change(function (e) {

        var shopId = $(this).val();
        clearSelectBox("Defect_ID");
        clearSelectBox("selectedCorrectiveActions");
        clearSelectBox("Corrective_Action_ID");
        if (shopId) {
            var url = "/QualityDefects/getQualityDefectByShop";
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

});