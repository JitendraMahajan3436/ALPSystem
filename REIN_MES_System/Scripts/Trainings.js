$(document).ready(function (e) {

    //$("#Employee_ID").select2({

    //    allowClear: true
    //});


    //$("#Shop_ID").select2({

    //    allowClear: true
    //});

    //$("#Line_ID").select2({

    //    allowClear: true
    //});

    //$("#Station_ID").select2({

    //    allowClear: true
    //});


    $(".manager_Line #To_Date").change(function (e) {
        var stationId = $("#Station_ID").val();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();

        if (frmDate > toDate) {
                alert("From date should not be later than To Date");
                return;
            }

        if (stationId) {

            var url = "/Training/GetCellMemberBYStationID";
            ajaxpack.getAjaxRequest(url, "stationId=" + $("#Station_ID").val() + "&frmDate=" + $("#From_Date").val() + "&toDate=" + $("#To_Date").val() + "", showCellMemberType, "json");
            //ajaxpack.getAjaxRequest(url, "EmployeeNo=" + ui.draggable.attr("id") + "&stationId=" + ui.draggable.attr("id"), deleteData, "json");
        }
    });


    $(".manager_Line #From_Date").change(function (e) {
        var stationId = $("#Station_ID").val();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();

        if (frmDate > toDate) {
            alert("From date should not be later than To Date");
            return;
        }

        if (stationId) {

            var url = "/Training/GetCellMemberBYStationID";
            ajaxpack.getAjaxRequest(url, "stationId=" + $("#Station_ID").val() + "&frmDate=" + $("#From_Date").val() + "&toDate=" + $("#To_Date").val() + "", showCellMemberType, "json");
            //ajaxpack.getAjaxRequest(url, "EmployeeNo=" + ui.draggable.attr("id") + "&stationId=" + ui.draggable.attr("id"), deleteData, "json");
        }
    });


    $(".manager_Line #Station_ID").change(function (e) {
        var stationId = $("#Station_ID").val();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();

        if (frmDate == "" || toDate == "")
        {
            alert("please select Date");
            return;
        }
       
        if (stationId) {

            var url = "/Training/GetCellMemberBYStationID";
            ajaxpack.getAjaxRequest(url, "stationId=" + $("#Station_ID").val() + "&frmDate=" + $("#From_Date").val() + "&toDate=" + $("#To_Date").val() + "", showCellMemberType, "json");
            //ajaxpack.getAjaxRequest(url, "EmployeeNo=" + ui.draggable.attr("id") + "&stationId=" + ui.draggable.attr("id"), deleteData, "json");
        }
    });

    $(".manager_Line #Shop_ID").change(function (e) {
        var shopId = $("#Shop_ID").val();
        if (shopId) {

            var url = "/Line/GetLineByShopID";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopLineType, "json");
        }
    });

    $(".manager_Line #Line_ID").change(function (e) {

        var lineId = $("#Line_ID").val();
        if (lineId) {

            url = "/Station/GetStationByLineID";
            ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showLineStationType, "json");
        }
    });

    function showCellMemberType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Emp_no");
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
        $("#Emp_no option").each(function () {
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