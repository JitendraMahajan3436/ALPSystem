$(document).ready(function (e) {

    //$(".plant-line #Plant_ID").change(function (e) {
    //    //var jsonData = JSON.stringify({ plantId: 7 });

    //    var plantId = $("#Plant_ID").val();
    //    if (plantId) {
    //        var url = "/LineType/GetLineTypeByPlantID";
    //        ajaxpack.getAjaxRequest(url, "plantId=" + $("#Plant_ID").val() + "", showPlantLineType, "json");
    //    }
    //    else
    //    {
    //        // clear the line type and shop
    //        clearSelectBox("Line_Type_Id");
    //        clearSelectBox("Shop_ID");
    //    }
        
       
        

        
    //    //var jsonRes = $.parseJSON(myajax.responseText);
    //    //SelectOptionHTML(jsonRes, "Shop_ID");

    //    //var jsonData = JSON.stringify({ plantId: $("#Plant_ID").val() });
    //    //$.ajax({
    //    //    type: "POST",
    //    //    contentType: "application/json; charset=utf-8",
    //    //    url: '/LineType/GetLineTypeByPlantID',
    //    //    data: jsonData,
    //    //    async: false,
    //    //    success: function (data) {
    //    //        if (data != undefined && data != "")
    //    //            months = data;
    //    //    },
    //    //    //error: Common.AjaxErrorHandler
    //    //});
    //});

    $(".plant-stations #Plant_ID").change(function (e) {
        var plantId = $("#Plant_ID").val();
        if (plantId) {
            var url = "/Shop/GetShopByPlantID";
            ajaxpack.getAjaxRequest(url, "plantId=" + $("#Plant_ID").val() + "", showPlantShopID, "json");

            //setTimeout(function () {

            //    //var JSONObject = { "id": "Channel1.Device1.tag1", "v": "42"};

            //    var JSONObject = "'id': 'Channel1.Device1.tag1'";
                
            //    var jsonData = JSON.stringify(JSONObject);
            //    $.ajax({
            //        url: "http://10.4.1.162:39320/iotgateway/read",
            //        type: "POST",
            //        dataType: "json",
            //        data: jsonData,
            //        contentType: "application/json; charset=utf-8",
            //        async: false,
            //        cache: false,
            //        processData: false,
            //        success: function (response) {
            //            alert("scucess");
            //            alert(JSON.stringify(response));
            //        },
            //        error: function (err) {
            //            alert("Fail");
            //            alert(JSON.stringify(err));
            //        }
            //    });

            //    var res = "[{ 'id': 'Channel1.Device1.tag1', 'v': 42}]";
            //    //var res = "plantId=" + plantId+"&shopId=" + shopId ;
            //    var url = "http://10.4.1.162:39320/iotgateway/write";
            //    ajaxpack.postAjaxRequest(url, res, ok, "json");
            //},2000);
        }
        else
        {            
            clearSelectBox("Shop_ID");
        }
    });

    function ok()
    {

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

    //function showPlantLineType()
    //{
    //    var myajax=ajaxpack.ajaxobj
    //    var myfiletype=ajaxpack.filetype
    //    if (myajax.readyState == 4)
    //    {
    //        if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                
    //            var jsonRes = $.parseJSON(myajax.responseText);
    //            SelectOptionHTML(jsonRes, "Line_Type_Id");

    //            var url = "/Shop/GetShopByPlantID";
    //            ajaxpack.getAjaxRequest(url, "plantId=" + $("#Plant_ID").val() + "", showPlantShop, "json");
    //        }
    //    }
    //}

    $(".plant-stations #Shop_ID").change(function (e) {
        var shopId = $("#Shop_ID").val();
        if (shopId) {
            var url = "/Line/GetLineByShopID";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showLineDetail, "json");
        }
        else {
            clearSelectBox("Line_ID");
        }
    });

    function showLineDetail()
    {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Line_ID");
            }
        }
    }

    //function showPlantShop() {
    //    var myajax = ajaxpack.ajaxobj
    //    var myfiletype = ajaxpack.filetype
    //    if (myajax.readyState == 4) {
    //        if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                
    //            var jsonRes = $.parseJSON(myajax.responseText);
    //            SelectOptionHTML(jsonRes, "Shop_ID");
    //        }
    //    }
    //}

    function SelectOptionHTML(jsonRes,targetId)
    {
        //var jsonRes = $.parseJSON(myajax.responseText);        
        var res = "";
        for (var i = 0; i < jsonRes.length; i++) {
            res += "<option value='" + jsonRes[i].Id + "'>" + jsonRes[i].Value + "</option>";
        }

        res = "<option value=''>" + $("#" + targetId + " option:first").html() + "</option>" + res;
        $("#" + targetId).html(res);
    }

    function clearSelectBox(targetId)
    {
        var res = "";        
        res = "<option value=''>" + $("#" + targetId + " option:first").html() + "</option>";
        $("#" + targetId).html(res);
    }

});