$(document).ready(function (e) {


    $("#Take_In_Line_ID").click(function (e) {

        var lineId = $("#Take_In_Line_ID").val();
        if (lineId) {
           
            var url = "/Station/GetStationByLineID";
            ajaxpack.getAjaxRequest(url, "lineId=" + $("#Take_In_Line_ID").val() + "", showStationShopID, "json");

        }
        else {
            clearSelectBox("Take_In_Station_ID");
        }

    });



    function showStationShopID() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Take_In_Station_ID");
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

    $("#Serial_No").change(function (e) {
        //alert("ok");
        var serialNo = $("#Serial_No").val();
        if (serialNo) {
            var url = "/QualityCaptures/GetOrderDetails";
            ajaxpack.getAjaxRequest(url, "serialNo=" + $("#Serial_No").val() + "", showSerialDetail, "json");
        }
    });

    function showSerialDetail()
    {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                clearFields();
                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes.length > 0) {

                    if (jsonRes[0].Is_Take_Out > 0) {
                        alert("This vehicle is taken out.");
                        return true;
                    }                                        
                    $("#Model_Code").val(jsonRes[0].Model_Code);
                    $("#Order_No").val(jsonRes[0].Order_No);
                    
                }
                else {
                    // no record found, process to clear things

                    $("#Model_Code").val("");                    
                    alert("Order Number is invalid.");
                }
                //SelectOptionHTML(jsonRes, "Shop_ID");
            }
        }
    }

    $(window).keydown(function (event) {
        if (event.keyCode == 13) {

            $("#Serial_No").trigger("change");
            event.preventDefault();
            return false;
        }
    });

    function clearFields()
    {
        $("#Model_Code").val("");
        $("#Order_No").val("");
        $("#Take_Out_Station_ID option[value='']").prop('selected', 'selected');
        $("#Quality_TO_Reason_ID option[value='']").prop('selected', 'selected');
        $("#Remark").val("");
    }

    function clearAll()
    {
        clearFields();
        $("#Serial_No").val("");
    }
});