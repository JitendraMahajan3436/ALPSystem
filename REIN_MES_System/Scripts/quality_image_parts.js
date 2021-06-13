$(document).ready(function (e) {
    $("#View_ID").change(function (e) {

        var viewId = $(this).val();
        var groupId = $("#Image_Group_ID").val();
        clearSelectBox("Part_ID");
        clearSelectBox("selectedPart");
        if (viewId) {

            $("#imageGroupId").val(groupId);
            $("#viewId").val(viewId);
            //$("#frm_show_image_grid").submit();
            $("#frm_show_image_grid").submit();
        }
    });

    // process to get the image group by station and family
    $("#Attribute_ID").change(function (e) {
        var attributeId = $(this).val();
        var stationId = $("#Station_ID").val();
        if (attributeId) {
            setTimeout(function (e) {
                var url = "/QualityImageGroups/getImageGroupByStationAndFamily";
                ajaxpack.getAjaxRequest(url, "attributeId=" + attributeId + "&stationId=" + stationId, showQualityImageGroupDetails, "json");
            }, 500);
        }
    });

    function showQualityImageGroupDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Image_Group_ID");
            }
        }
    }


    // process to show the quality parts for the selected image

    $("#image_grid").on("click", ".grid_image", function (e) {
        //alert($(this).attr("id"));
        $("#image_grid .col-md-3").removeClass("active");
        $(this).parent().addClass("active");

        var shopId = $("#Shop_ID").val();
        var stationId = $("#Station_ID").val();
        var attributeId = $("#Attribute_ID").val();
        var imageGridId = $(this).attr("id");
        $("#hdnImageGridId").val(imageGridId);
        // process to get the parts

        clearSelectBox("Part_ID");
        clearSelectBox("selectedPart");

        var url = "/QualityImagePart/getSelectedParts";
        ajaxpack.getAjaxRequest(url, "shopId=" + shopId + "&stationId=" + stationId + "&attributeId=" + attributeId + "&imageGridId=" + imageGridId, showSelectedPartsDetails, "json");

        setTimeout(function (e) {
            var url = "/QualityImagePart/getNotSelectedParts";
            ajaxpack.getAjaxRequest(url, "shopId=" + shopId + "&stationId=" + stationId + "&attributeId=" + attributeId + "&imageGridId=" + imageGridId, showNotSelectedPartsDetails, "json");
        }, 2000);

    });

    function showSelectedPartsDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "selectedPart");
            }
        }
    }

    function showNotSelectedPartsDetails() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Part_ID");
            }
        }
    }

    $(".save_quality_image_parts").click(function (e) {

        $('#selectedPart option').prop('selected', true);
        
        var plantId = $("#Plant_ID").val();
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var stationId = $("#Station_ID").val();
        var attributeId = $("#Attribute_ID").val();
        var imageGroupId = $("#Image_Group_ID").val();
        var viewId = $("#View_ID").val();
        var imageGridId = $("#hdnImageGridId").val();
        var partId = "";
        $("#selectedPart").each(function (e) {
            if (partId == "") {
                partId = $(this).val();
            }
            else {
                partId = partId + "," + $(this).val();
            }
        });

        if(isValid())
        {
            
            if (partId == "")
            {
                $("#partError").html("Please select");
                return;
            }


            $("#partError").html("");

            var url = "/QualityImagePart/addPartsToGrid";
            ajaxpack.getAjaxRequest(url, "plantId=" + plantId + "&shopId=" + shopId + "&lineId=" + lineId + "&stationId=" + stationId + "&attributeId=" + attributeId + "&imageGroupId=" + imageGroupId + "&viewId=" + viewId + "&imageGridId=" + imageGridId + "&selectedParts=" + partId, showSavedPartDetails, "json");
        }

    });


    function showSavedPartDetails()
    {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                $('.content-wrapper div.myalert').remove();
                if (jsonRes == true) {

                    className = "alert-success";
                    message = "Part is successfully added for the grid image";
                }
                else {
                    className = "alert-warning";
                    message = "Error occured";
                }

                var alertHtml = '<div class="box-body myalert">' +
                                          '<div class="alert ' + className + ' alert-dismissable">' +
                                          '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                          '<h4><i class="icon fa fa-ban"></i> Quality Part To Image Grids </h4>' +
                                          '' + message
                '</div></div>';
                $('.content-wrapper .content').before(alertHtml);
                $('html, body').animate({ scrollTop: 0 }, 900);
            }
        }
    }

    function isValid() {
        var res = true;
        clearErrorFields();
        if ($("#Shop_ID").val() == "" || $("#Shop_ID").val() == null) {
            $("#shopError").html("Please select");
            res = false;
        }

        if ($("#Line_ID").val() == "" || $("#Line_ID").val() == null) {
            $("#lineError").html("Please select");
            res = false;
        }

        if ($("#Station_ID").val() == "" || $("#Station_ID").val() == null) {
            $("#stationError").html("Please select");
            res = false;
        }

        if ($("#Attribute_ID").val() == "" || $("#Attribute_ID").val() == null) {
            $("#attributeError").html("Please select");
            res = false;
        }

        if ($("#Image_Group_ID").val() == "" || $("#Image_Group_ID").val() == null) {
            $("#groupError").html("Please select");
            res = false;
        }

        if ($("#View_ID").val() == "" || $("#View_ID").val() == null) {
            $("#viewError").html("Please select");
            res = false;
        }

        return res;
    }

    $("#Shop_ID").change(function(e){
        clearSelectBox("Station_ID");
        clearSelectBox("Line_ID");
        clearSelectBox("Image_Group_ID");
        $("#image_grid").html("");

        clearSelectBox("Part_ID");
        clearSelectBox("selectedPart");
        clearSelectBox("View_ID");
    });

    $("#Line_ID").change(function (e) {
        clearSelectBox("Station_ID");       
        clearSelectBox("Image_Group_ID");
        clearSelectBox("View_ID");
        $("#image_grid").html("");

        clearSelectBox("Part_ID");
        clearSelectBox("selectedPart");
    });

    $("#Station_ID").change(function (e) {        
        clearSelectBox("Image_Group_ID");
        $("#image_grid").html("");
        clearSelectBox("Part_ID");
        clearSelectBox("selectedPart");
        clearSelectBox("View_ID");
        $("#Attribute_ID").trigger("change");
    });

    $("#Image_Group_ID").change(function (e) {
        //clearSelectBox("Image_Group_ID");
        $("#image_grid").html("");
        clearSelectBox("Part_ID");
        clearSelectBox("selectedPart");

        // process to load the view
        clearSelectBox("View_ID");
        var groupId = $(this).val();
        if(groupId)
        {
            var url = "/QualityImageGroups/getQualityViewByImageGroupId";
            ajaxpack.getAjaxRequest(url, "groupId=" + groupId, showQualityViewDetails, "json");
        }
    });

    function showQualityViewDetails()
    {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "View_ID");
            }
        }
    }


    function clearErrorFields()
    {
        $("#shopEror").html("");

        $("#lineError").html("");
        $("#stationError").html("");
        $("#attributeError").html("");
        $("#groupError").html("");
        $("#viewError").html("");
        //$("#image_grid").html("");
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


});
