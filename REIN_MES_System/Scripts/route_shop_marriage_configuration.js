$(document).ready(function (e) {


    // plant change
    $(".route_shop_marriage_configuration #Plant_ID").change(function (e) {
        //var jsonData = JSON.stringify({ plantId: 7 });

        clearSelectBox("Sub_Shop_ID");
        clearSelectBox("Sub_Line_ID");
        clearSelectBox("Sub_Line_Station_ID");
        clearSelectBox("Marriage_Shop_ID");
        clearSelectBox("Marriage_Line_ID");
        clearSelectBox("Marriage_Station_ID");

        var plantId = $("#Plant_ID").val();
        if (plantId) {
            var url = "/Shop/GetShopByPlantID";
            ajaxpack.getAjaxRequest(url, "plantId=" + $("#Plant_ID").val() + "", showPlantShopID, "json");
        }
        else {
            // clear the line type and shop
            
        }
    });


    // show the shop on plant change
    function showPlantShopID() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Sub_Shop_ID");
            }
        }
    }

    // on sub shop change load the marriage shop, sub line
    $(".route_shop_marriage_configuration #Sub_Shop_ID").change(function (e) {
        //var jsonData = JSON.stringify({ plantId: 7 });

        
        clearSelectBox("Sub_Line_ID");
        clearSelectBox("Sub_Line_Station_ID");
        clearSelectBox("Marriage_Shop_ID");
        clearSelectBox("Marriage_Line_ID");
        clearSelectBox("Marriage_Station_ID");

        var shopId = $("#Sub_Shop_ID").val();
        if (shopId) {
            var url = "/Line/GetEndLineByShopID";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Sub_Shop_ID").val(), showSubLine, "json");

            // trigger to load the shop other than selected shop
            setTimeout(function () {
                url = "/Shop/GetShopByPlantID";
                ajaxpack.getAjaxRequest(url, "plantId=" + $("#Plant_ID").val() + "", showPlantMarriageShopID, "json");
            }, 2000);

        }
        else {
            // clear the sub line and sub line station
            
        }
    });

    // show the sub line
    function showSubLine() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Sub_Line_ID");
            }
        }
    }

    // show the marriage shop
    function showPlantMarriageShopID()
    {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Marriage_Shop_ID");
                $("#Marriage_Shop_ID option[value='" + $("#Sub_Shop_ID").val() + "']").remove();
            }
        }
    }

    // show the last station on sub line change
    $(".route_shop_marriage_configuration #Sub_Line_ID").change(function (e) {
        //var jsonData = JSON.stringify({ plantId: 7 });
                
        clearSelectBox("Sub_Line_Station_ID");
        clearSelectBox("Marriage_Line_ID");
        clearSelectBox("Marriage_Station_ID");

        var shopId = $("#Sub_Shop_ID").val();
        var lineId = $("#Sub_Line_ID").val();
        if (lineId && shopId) {
            
            var url = "/Line/GetLineStationForMarriageByLineID";
            ajaxpack.getAjaxRequest(url, "lineId=" + $("#Sub_Line_ID").val() + "&isStartStation=0", showSubLineEndStation, "json");
            

        }
        else {
            // clear the sub line and sub line station
            
            
        }
    });

    // show sub line end station
    function showSubLineEndStation()
    {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Sub_Line_Station_ID");
                $("#Sub_Line_Station_ID option[value=" + jsonRes[0].Id + "]").prop('selected', 'selected');
            }
        }
    }

    // show marriage line on marriage shop change
    $(".route_shop_marriage_configuration #Marriage_Shop_ID").change(function (e) {
        //var jsonData = JSON.stringify({ plantId: 7 });
               
        clearSelectBox("Marriage_Line_ID");
        clearSelectBox("Marriage_Station_ID");

        var shopId = $("#Marriage_Shop_ID").val();
        if (shopId) {
            var url = "/Line/GetLineByShopID";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Marriage_Shop_ID").val(), showMarriageSubLine, "json");
                      

        }
        else {
            // clear the sub line and sub line station

        }
    });



    // show marriage line
    function showMarriageSubLine() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Marriage_Line_ID");
            }
        }
    }

    function showEndStation() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Sub_Line_Station_ID");
                $("#Sub_Line_Station_ID option[value=" + jsonRes[0].Id + "]").prop('selected', 'selected');
            }
        }
    }

    // show station on marriage line change
    $(".route_shop_marriage_configuration #Marriage_Line_ID").change(function (e) {
        
        
        clearSelectBox("Marriage_Station_ID");

        var shopId = $("#Marriage_Shop_ID").val();
        var lineId = $("#Marriage_Line_ID").val();
        if (lineId && shopId) {
            
            var url = "/Station/GetStationByLineID";
            ajaxpack.getAjaxRequest(url, "lineId=" + $("#Marriage_Line_ID").val() + "&isStartStation=1", showMarriageStation, "json");
           

        }
        else {
            // clear the sub line and sub line station
                       
            clearSelectBox("Marriage_Station_ID");
        }
    });

    function showMarriageStation() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Marriage_Station_ID");
                //$("#Marriage_Station_ID option[value=" + jsonRes[0].Id + "]").prop('selected', 'selected');
            }
        }
    }


    //$(".route_shop_marriage_configuration #Line_ID").click(function (e) {

    //    var lineId = $("#Line_ID").val();
    //    if (lineId) {

    //        var url = "/Station/GetStationByLineID";
    //        ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showLineStationID, "json");
    //    }


    //});

    //function showLineStationID() {
    //    var myajax = ajaxpack.ajaxobj
    //    var myfiletype = ajaxpack.filetype
    //    if (myajax.readyState == 4) {
    //        if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

    //            var jsonRes = $.parseJSON(myajax.responseText);
    //            SelectOptionHTML(jsonRes, "Station_ID");
    //        }
    //    }
    //}

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