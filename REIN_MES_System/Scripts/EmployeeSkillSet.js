


$(document).ready(function (e) {



    $(".manager_Line #Shop_ID").change(function (e) {
        var shopId = $("#Shop_ID").val();
        if (shopId) {
            var jsonData = JSON.stringify({ plantId: 7 });
            var url = "/Line/GetLineByShopID";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopLineType, "json");
        }
    });

    $(".manager_Line #Line_ID").change(function (e) {
        //alert("Hi nil");
        var jsonData = JSON.stringify({ plantId: 7 });
        var lineId = $("#Line_ID").val();
        //var shopId = $("#Shop_ID").val();
        if (lineId) {

            url = "/EmployeeSkillSet/GetStationListByLineID";
            ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showLineStationType, "json");

        }
    });

    $(".manager_Line #Station_ID").change(function (e) {
        var stationId = $("#Station_ID").val();
        if (stationId) {

            var jsonData = JSON.stringify({ plantId: 7 });
            url = "/ShopManagerConfiguration/GetOperatorByStationID";
            ajaxpack.getAjaxRequest(url, "stationId=" + $("#Station_ID").val() + "", showStationOperatorType, "json");
        }

    });



    function showStationOperatorType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Employee_ID");
            }
        }
    }


    function showLineStationType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Station_ID");
            }
        }
    }

    function showShopLineType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Line_ID");
            }
        }
    }


});



function SelectOptionHTML(jsonRes, targetId) {
    //var jsonRes = $.parseJSON(myajax.responseText);        
    var res = "";
    for (var i = 0; i < jsonRes.length; i++) {
        res += "<option value='" + jsonRes[i].Id + "'>" + jsonRes[i].Value + "</option>";
    }

    res = "<option value=''>" + $("#" + targetId + " option:first").html() + "</option>" + res;
    $("#" + targetId).html(res);
}



function clear() {
    clearSelectBox("Line_ID");
    clearSelectBox("Shop_ID");
    clearSelectBox("Plant_ID");

}

function clearSelectBox(targetId) {
    var res = "";
    res = "<option value=''>" + $("#" + targetId + " option:first").html() + "</option>";
    $("#" + targetId).html(res);
}


this.searchSelectBox = function (textBoxId, targetId) {
    //alert(targetId);
    if ($("#" + textBoxId).val() == "" || $("#" + textBoxId).val() == null) {
        $("#defectCheckpoint option").show();
    }
    else {
        var searchString = $("#" + textBoxId).val().toUpperCase().trim();
        $("#Employee_ID option").each(function () {
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