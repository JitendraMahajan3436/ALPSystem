$(document).ready(function (e) {

    //$(".tiny-toggle").tinyToggle();
    clearErrorMessages();
    clearAllInputFields();
    $(".srlno-input").keydown(function (event) {
        clearErrorMessages();
        clearAllInputFields();
        if (event.keyCode == 13) {
            loadSerialDetail();
        }
    });

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

                        var isRework = $("#isReworkStation").val();
                        if (isRework == "True") {

                        }
                        else {
                            $("#srlmessage-block").html("This vehicle is taken out.");
                            $("#srlmessage-block").addClass("bg-error");
                            return true;
                        }
                    }

                    $("#hdnSeries").val(jsonRes[0].Series);
                    $("#hdnFamily").val(jsonRes[0].Family);

                    // show quality checklist
                    var stationId = $("#Captured_Station_ID").val();
                    var family = $("#hdnFamily").val();
                    var shopId = $("#Shop_ID").val();
                    var isFinalQualityOK = $("#isFinalQualityStation").val();
                    var isRework = $("#isReworkStation").val();
                    //if (isFinalQualityOK == "True" || isRework == "True") {

                    //}
                    //else {
                    processSerialViewStatus();
                    showQualityDefectDetails();
                    // process to get the image group configured for this station
                    var url = "/QualityImageGroups/getImageGroupByStationAndFamily";
                    ajaxpack.getAjaxRequest(url, "attributeId=" + family + "&stationId=" + stationId, showQualityImageGroupDetails, "json");


                }
                else {
                    // no record found, process to clear things
                    clearQualityErrorMessage();
                    $("#srlmessage-block").html("Order Number is invalid.");
                    $("#srlmessage-block").addClass("bg-error");
                }

            }
        }
    }

    function showQualityImageGroupDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes.length == 0) {
                    $("#srlmessage-block").html("The image configuration is not added.");
                    $("#srlmessage-block").addClass("bg-error");
                    return true;
                }
                else {
                    SelectOptionHTML(jsonRes, "Image_Group_ID");
                    $("select#Image_Group_ID option").eq(1).attr("selected", "selected");
                    $("#Image_Group_ID").trigger("change");
                }
            }
        }
    }

    function processSerialViewStatus()
    {
        var serialNo = $(".srlno-input").val();
        if (serialNo) {
            
            var url = "/QualityCaptures/processSerialViewStatus";

            ajaxpack.getAjaxRequest(url, "serialNo=" + serialNo, showSerialViewStatusDetails, "json");
        }
    }




    function showSerialViewStatusDetails()
    {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
            }
        }
    }
    var viewIds = new Array();
    function processViewButtons()
    {
        // process to disable previous button
        $(".left_view").prop("disabled", true);
        $(".view_name").html("Select View");
        $(".right_view").removeAttr("disabled");
        //process to set right id

        // initiallize view id
        var i = 0;
        $("#View_ID option").each(function (e) {
            if($(this).val()=="" || $(this).val()==null)
            {
             
            }
            else
            {                
                viewIds[i] = $(this).val();
                i = i + 1;
            }
        });
        
        // process to set right value
        $(".right_view").attr("id", "right_view_" + viewIds[0]);

    }

    $(".right_view").click(function (e) {
        // process to remove disable from left
        $(".left_view").prop("disabled", true);

        var id = $(this).attr("id");
        id = id.split("_")[2];

        
        $(".left_view").removeAttr("disabled");
        var index = viewIds.indexOf(id);

        //index = index + 1;
        if (viewIds[index]==undefined)
        {
            // process to disable right 
            $(".right_view").prop("disabled", true);
        }
        else
        {

            $(".right_view").attr("id", "right_view_" + viewIds[index+1]);
            $(".left_view").attr("id", "left_view_" + viewIds[index-1]);

            // process to set view name
            $("#View_ID").val(viewIds[index]);

            $("#View_ID").trigger("change");

            $(".view_name").html($("#View_ID option:selected").text());



            if (viewIds[index+1] == undefined) {
                // process to disable right 
                $(".right_view").prop("disabled", true);
            }

        }


    });

    $(".left_view").click(function (e) {
        // process to remove disable from right
        $(".right_view").removeAttr("disabled");

        var id = $(this).attr("id");
        id = id.split("_")[2];

        
        $(".right_view").removeAttr("disabled");
        var index = viewIds.indexOf(id);

        //index = index - 1;
        if (viewIds[index] == undefined) {
            // process to disable right 
            $(".left_view").prop("disabled", true);
        }
        else {

            $(".left_view").attr("id", "left_view_" + viewIds[index-1]);

            $(".right_view").attr("id", "left_view_" + viewIds[index+1]);

            // process to set view name
            $("#View_ID").val(viewIds[index]);

            $("#View_ID").trigger("change");

            $(".view_name").html($("#View_ID option:selected").text());

            if (viewIds[index - 1] == undefined) {
                // process to disable right 
                $(".left_view").prop("disabled", true);
            }
        }


    });

    // process to change the group image
    $("#Image_Group_ID").change(function (e) {
        var groupId = $(this).val();
        if (groupId) {
            var url = "/QualityImageGroups/getQualityViewByImageGroupId";
            ajaxpack.getAjaxRequest(url, "groupId=" + groupId, showQualityViewDetails, "json");
        }
    });

    function showQualityViewDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "View_ID");

                processViewButtons();
            }
        }
    }

    // process to show the view image
    $("#View_ID").change(function (e) {

        var viewId = $(this).val();
        var groupId = $("#Image_Group_ID").val();
        clearSelectBox("Part_ID");
        clearSelectBox("Defect_ID");
        $("#image_grid").html("");
        $("#Defect_Category_ID").val("");
        $("#resolved_defect").prop("checked", false);
        $("#Corrective_Action_ID").val(""); //$(".corrective_actions").hide("slow");
        if (viewId) {

            $("#imageGroupId").val(groupId);
            $("#viewId").val(viewId);
            //$("#frm_show_image_grid").submit();
            $("#frm_show_image_grid").submit();
        }
    });

    $("#image_grid").on("click", ".grid_image", function (e) {
        //alert($(this).attr("id"));
        $("#image_grid .col-md-3").removeClass("active");
        $(this).parent().addClass("active");

        var shopId = $("#Shop_ID").val();
        var stationId = $("#Captured_Station_ID").val();
        var serialNo = $(".srlno-input").val();
        var attributeId = $("#hdnFamily").val();
        var imageGridId = $(this).attr("id");
        var groupId = $("#Image_Group_ID").val();
        var viewId = $("#View_ID").val();
        $("#hdnImageGridId").val("");
        clearSelectBox("Part_ID");
        clearSelectBox("Defect_ID");

        $("#Defect_Category_ID").val("");
        $("#resolved_defect").prop("checked", false);
        $("#Corrective_Action_ID").val(""); //$(".corrective_actions").hide("slow");
        // process to get the parts
        if (imageGridId) {
            $("#hdnImageGridId").val(imageGridId);
            var url = "/QualityCaptures/getStationPart";
            ajaxpack.getAjaxRequest(url, "shopId=" + shopId + "&stationId=" + stationId + "&serialNo=" + serialNo + "&attributeId=" + attributeId + "&groupId=" + groupId + "&viewId=" + viewId + "&gridId=" + imageGridId, showSelectedPartsDetails, "json");
        }

    });






    function showSelectedPartsDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Part_ID");
            }
        }
    }

    $("#Part_ID").change(function (e) {

        var partId = $(this).val();
        var shopId = $("#Shop_ID").val();
        var stationId = $("#Captured_Station_ID").val();
        var serialNo = $(".srlno-input").val();
        var attributeId = $("#hdnFamily").val();
        var imageGridId = $("#hdnImageGridId").val();
        var groupId = $("#Image_Group_ID").val();
        var viewId = $("#View_ID").val();
        clearSelectBox("Defect_ID");

        $("#Defect_Category_ID").val("");
        $("#resolved_defect").prop("checked", false);
        $("#Corrective_Action_ID").val(""); //$(".corrective_actions").hide("slow");

        if (partId) {
            //var url = "/QualityDefects/getAddedDefectToPart";
            //ajaxpack.getAjaxRequest(url, "shopId=" + shopId + "&partId=" + partId, showAddedDefectDetails, "json");

            var url = "/QualityCaptures/getStationDefect";
            ajaxpack.getAjaxRequest(url, "shopId=" + shopId + "&stationId=" + stationId + "&partId=" + partId + "&serialNo=" + serialNo + "&attributeId=" + attributeId + "&groupId=" + groupId + "&viewId=" + viewId + "&gridId=" + imageGridId, showAddedDefectDetails, "json");

            setTimeout(function () {
                var url = "/QualityParts/getStationUsersByPart";
                ajaxpack.getAjaxRequest(url, "partId=" + partId, showAddedUsersDetails, "json");
            }, 500);
        }
    });

    function showAddedDefectDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Defect_ID");
            }
        }
    }

    function showAddedUsersDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Checkpoint_Station_User_ID");
            }
        }
    }

    $(".add_defect").click(function (e) {
        if (isValid()) {
            var plantId = $("#Plant_ID").val();
            var shopId = $("#Shop_ID").val();
            var lineId = $("#Line_ID").val();
            var stationId = $("#Captured_Station_ID").val();
            var serialNo = $(".srlno-input").val();
            var groupId = $("#Image_Group_ID").val();
            var viewId = $("#View_ID").val();
            var gridId = $("#hdnImageGridId").val();
            var partId = $("#Part_ID").val();
            var defectId = $("#Defect_ID").val();
            if (defectId == "" || defectId == null)
            {
                defectId = 0;
            }
            var defectCategory = $("#Defect_Category_ID").val();
            var correctiveActionId = 0;
            if ($("#Corrective_Action_ID").val() == "" || $("#Corrective_Action_ID").val() == null) {
                correctiveActionId = 0;
            }
            else {
                correctiveActionId = $("#Corrective_Action_ID").val();
            }

            var checkpointStationUser = $("#Checkpoint_Station_User_ID").val();
            var clrShopId = $("#CLR_Shop_ID").val();
            var defectName = $("#new_defect").val();
            var url = "/QualityCaptures/saveImageDefect";
            ajaxpack.getAjaxRequest(url, "plantId=" + plantId + "&shopId=" + shopId + "&lineId=" + lineId + "&stationId=" + stationId + "&serialNo=" + serialNo + "&groupId=" + groupId + "&viewId=" + viewId + "&gridId=" + gridId + "&partId=" + partId + "&defectId=" + defectId + "&correctiveActionId=" + correctiveActionId + "&defectName=" + defectName, showImageDetails, "json");
        }
    });

    function showImageDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                var gridId = $("#hdnImageGridId").val();
                //$("#image_grid #" + gridId).trigger("click");

                setTimeout(function (e) {
                    $("#Part_ID").trigger("change");
                }, 500);
                

                $("#Defect_Category_ID").val("");
                $("#resolved_defect").prop("checked", false);
                $("#Corrective_Action_ID").val(""); //$(".corrective_actions").hide("slow");

                setTimeout(function (e) {
                    showQualityDefectDetails();
                }, 1000);

                $("#new_defect").val("");
                
            }
        }
    }

    function isValid() {
        clearErrorMessages();
        var res = true;
        if ($("#View_ID").val() == "" || $("#View_ID").val() == null) {
            $("#viewError").html("Please select view");
            res = false;
        }

        if (res == true) {
            if ($("#hdnImageGridId").val() == "" || $("#hdnImageGridId").val() == null) {
                $("#viewError").html("Please select image");
                res = false;
            }
        }

        if ($("#Part_ID").val() == "" || $("#Part_ID").val() == null) {
            $("#partError").html("Please select");
            res = false;
        }

        if ($("#new_defect").val() == "" || $("#new_defect").val() == null)
        {
            if ($("#Defect_ID").val() == "" || $("#Defect_ID").val() == null) {
                $("#selectedDefectError").html("Please select");
                res = false;
            }
        }
        //if ($("#Defect_Category_ID").val() == "" || $("#Defect_Category_ID").val() == null) {
        //    $("#defectCategoryError").html("Please select");
        //    res = false;
        //}

        //if ($("#Checkpoint_Station_User_ID").val() == "" || $("#Checkpoint_Station_User_ID").val() == null) {
        //    $("#checkpointStationUserError").html("Please select");
        //    res = false;
        //}

        //if ($("#CLR_Shop_ID").val() == "" || $("#CLR_Shop_ID").val() == null) {
        //    $("#clrShopId").html("Please select");
        //    res = false;
        //}

        //if ($("#resolved_defect").prop("checked") == true) {
        //    // check corrective actions
        //    if ($("#Corrective_Action_ID").val() == "" || $("#Corrective_Action_ID").val() == null) {
        //        $("#correctiveActionError").html("Please select");
        //        res = false;
        //    }
        //}

        return res;
    }

    //$(".show_defect_resolved_radio").click(function (e) {

    //    if ($(this).prop('checked') == true) {
    //        $(".corrective_actions").show("slow");
    //    }
    //    else
    //        if ($(this).prop('checked') == false) {
    //            $(".corrective_actions").hide("slow");
    //        }

    //});

    function showQualityDefectDetails() {
        $("#stationId").val($("#Captured_Station_ID").val());
        $("#serialNo").val($(".srlno-input").val());
        $("#frm_show_quality_image_defect_details").submit();
    }

    $("#image_grid_list").on("click", ".delete_defect", function (e) {

        var id = $(this).attr("id");
        id = id.split("_")[1];
        var res = confirm("Do you want to delete the record ?");
        if (res) {
            //alert(id);
            var url = "/QualityCaptures/deleteQualityImage";
            ajaxpack.getAjaxRequest(url, "qualityId=" + id, showRemoveDetails, "json");
        }
    });


    function showRemoveDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                $("#frm_show_quality_image_defect_details").submit();
                setTimeout(function (e) {
                    $("#Part_ID").trigger("change");
                },500);
                
            }
        }
    }

    $("#image_grid_list").on("click", ".edit_defect", function (e) {
        var id = $(this).attr("id");
        id = id.split("_")[1];
        //alert(id);
        if (id) {
            $("#hdnQualityId").val(id);
            var url = "/QualityCaptures/getEditImageDetails";
            ajaxpack.getAjaxRequest(url, "id=" + id, showEditDefectDetails, "json");
        }

    });


    function showEditDefectDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes.length > 0) {
                    clearEditInputFields();
                    $(".imageGroupId").val(jsonRes[0].Image_Group_ID);
                    $(".viewId").val(jsonRes[0].View_ID);
                    $("#hdnImageGridId").val(jsonRes[0].Image_Grid_ID);
                    $("#edit_part_name").val(jsonRes[0].Part_Name);
                    $("#edit_defect_name").val(jsonRes[0].Defect_Name);
                    $("#editDefectCategory").val(jsonRes[0].Defect_Category_ID);

                    $(".rowId").val($("#hdnQualityId").val());
                    $(".serialNo").val($(".srlno-input").val())
                    $("#frm_show_edit_image_grid").submit();
                    $("#quality_edit_image_popup").trigger("click");
                    $("#editCLRShopId").val(jsonRes[0].CLR_Shop_ID);
                    $("#edit_rework_man_power").val(jsonRes[0].Rework_Man_Power);

                    if (jsonRes[0].Rework_Man_Minutes != null) {
                        $("#edit_rework_man_hours").val(jsonRes[0].Rework_Man_Minutes["Hours"]);
                        $("#edit_rework_man_minute").val(jsonRes[0].Rework_Man_Minutes["Minutes"]);
                    }

                    $("#edit_quality_corrective_actions").val(jsonRes[0].Selected_Corrective_Actions_ID);
                    SelectOptionHTML(jsonRes[0].Checkpoint_Station_Users, "editDefectUser");

                    $("#editDefectUser").val(jsonRes[0].Checkpoint_Station_User_ID);

                    if (jsonRes[0].Selected_Corrective_Actions_ID) {
                        $(".rework_details").show("slow");
                    }
                    else {
                        $(".rework_details").hide("slow");
                    }
                }
            }
        }
    }


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

                $('html, body').animate({ scrollTop: 0 }, 900);
            }
        }
    }


    $(".edit_image_quality_data").click(function (e) {

        if (isEditValid()) {
            var id = $("#hdnQualityId").val();
            var manPower = $("#edit_rework_man_power").val();
            if (manPower == "" || manPower == null)
            {
                manPower = 0;
            }
            var manHours = $("#edit_rework_man_hours").val();
            if (manHours == "" || manHours == null)
            {
                manHours = 0;
            }
            var manMinutes = $("#edit_rework_man_minute").val();
            if (manMinutes == "" || manMinutes == null)
            {
                manMinutes = 0;
            }

            var correctiveAction = $("#edit_quality_corrective_actions").val();
            if (correctiveAction == "" || correctiveAction == null) {
                correctiveAction = 0;
            }
            var defectCategory = $("#editDefectCategory").val();
            var defectUser = $("#editDefectUser").val();

            var clrShopId = $("#editCLRShopId").val();
            var url = "/QualityCaptures/updateImageDefect";
            ajaxpack.getAjaxRequest(url, "id=" + id + "&manPower=" + manPower + "&manHours=" + manHours + "&manMinutes=" + manMinutes + "&correctiveActions=" + correctiveAction, showUpdateDefectDetails, "json");

        }
    });

    function showUpdateDefectDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                $(".close").trigger("click");
                $("#frm_show_quality_image_defect_details").submit();
                clearEditErrorFields();
            }
        }
    }

    function clearEditErrorFields() {
        $(".validation_error").html("");
    }

    function clearEditInputFields() {
        $("#edit_rework_man_power").val("");
        $("#edit_rework_man_hours").val("");
        $("#edit_rework_man_minute").val("");

        $("#edit_quality_corrective_actions").val("");
        $("#editDefectCategory").val("");
        $("#editDefectUser").val("");
    }

    function isEditValid() {
        clearEditErrorFields();
        var res = true;
        var correctiveAction = $("#edit_quality_corrective_actions").val();
        if (correctiveAction == "" || correctiveAction == null) {
        }
        else {
            if ($("#edit_rework_man_power").val() == "" || $("#edit_rework_man_power").val() == null) {
                $("#errorQualityEditReworkManPower").html("Please select");
                res = false;
            }
            else {
                if (!$.isNumeric($("#edit_rework_man_power").val())) {
                    $("#errorQualityEditReworkManPower").html("Invalid");
                    res = false;
                }
            }

            var manHours = $("#edit_rework_man_hours").val();
            if (manHours == "" || manHours == null) {
                $("#errorQualityEditReworkManHours").html("Please select");
                res = false;
            }
            else {
                if (!$.isNumeric(manHours)) {
                    $("#errorQualityEditReworkManHours").html("Invalid");
                    res = false;
                }
            }

            var manMinutes = $("#edit_rework_man_minute").val();
            if (manMinutes == "" || manMinutes == null) {
                $("#errorQualityEditReworkManMinute").html("Please select");
                res = false;
            }
            else {
                if (!$.isNumeric(manMinutes)) {
                    $("#errorQualityEditReworkManMinute").html("Invalid");
                    res = false;
                }
            }
        }

        //if ($("#editCLRShopId").val() == "" || $("#editCLRShopId").val() == null) {

        //    $("#errorEditclrShopId").html("Please select");
        //    res = false;
        //}
        //var correctiveAction = $("#edit_quality_corrective_actions").val();
        //if (correctiveAction == "" || correctiveAction == null) {
        //    $("#errorQualityEditCorrectiveActions").html("Please select");
        //    res = false;
        //}

        //var category = $("#editDefectCategory").val();
        //if (category == "" || category == null) {
        //    $("#errorDefectCategory").html("Please select");


        //    res = false;
        //}

        //var editDefectUser = $("#editDefectUser").val();
        //if (editDefectUser == "" || editDefectUser == null) {
        //    $("#errorDefectUser").html("Please select");

        //    res = false;
        //}

        return res;
    }

    $("#edit_quality_corrective_actions").change(function (e) {
        var correctiveAction = $(this).val();
        if (correctiveAction) {
            $(".rework_details").show("slow");
        }
        else {
            $(".rework_details").hide("slow");
        }
    });

    function clearAllInputFields() {
        clearSelectBox("Part_ID");
        clearSelectBox("Defect_ID");

        $("#image_grid").html("");
        clearSelectBox("View_ID");
        $("#Defect_Category_ID").val("");
        $("#resolved_defect").prop("checked", false);
        $("#Corrective_Action_ID").val(""); //$(".corrective_actions").hide("slow");
        $("#image_grid_list").val();

        clearSelectBox("Checkpoint_Station_User_ID");
        $("#checkpointStationUserError").html("");

        $("#srlmessage-block").removeClass("bg-correct");
        $("#srlmessage-block").removeClass("bg-error");
        $("#srlmessage-block").html("");
        $("#CLR_Shop_ID").val(4);
        $("#new_defect").val("");
    }

    function clearErrorMessages() {
        $("#viewError").html("");
        $("#partError").html("");
        $("#selectedDefectError").html("");
        $("#defectCategoryError").html("");
        $("#correctiveActionError").html("");
        $("#clrShopId").html("");
        $("#checkpointStationUserError").html("");
    }

    function clearQualityErrorMessage() {
        $("#srlmessage-block").removeClass("bg-correct");
        $("#srlmessage-block").removeClass("bg-error");

        $("#srlmessage-block").html("");
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