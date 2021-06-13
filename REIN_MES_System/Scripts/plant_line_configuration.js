$(document).ready(function (e) {


    $(".plant_line_configuration #Plant_ID").change(function (e) {
        //var jsonData = JSON.stringify({ plantId: 7 });

        var plantId = $("#Plant_ID").val();
        if (plantId) {
            var url = "/Shop/GetShopByPlantID";
            ajaxpack.getAjaxRequest(url, "plantId=" + $("#Plant_ID").val() + "", showPlantShopID, "json");
        }
        else {
            // clear the line type and shop
            clearSelectBox("Line_ID");
            clearSelectBox("Shop_ID");
            clearSelectBox("Station_ID");
        }
    });

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

        var shopId = $("#Shop_ID").val();
        if (shopId) {
            var url = "/Line/GetLineByShopID";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showLineShopID, "json");
            
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


    $(".plant_line_configuration #Line_ID").change(function (e) {

        var lineId = $("#Line_ID").val();
        if (lineId) {
           
            var url = "/Station/GetStationByLineID";
            ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showLineStationID, "json");
        }
        else
        {
            clearSelectBox("Station_ID");
        }

    });

    function showLineStationID()
    {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Station_ID");
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

});