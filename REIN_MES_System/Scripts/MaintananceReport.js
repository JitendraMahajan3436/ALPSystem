$(document).ready(function (e) {
    ///alert('hiiii');
    $(".manager_Line #Shop_ID").change(function (e) {
        var shopId = $("#Shop_ID").val();
        if (shopId) {

            var url = "/MaintananceReport/GetLineByShopID";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopLineType, "json");
        }
    });

    //$(".manager_Line #Line_ID").change(function (e) {

    //    var lineId = $("#Line_ID").val();
    //    //alert(lineId);
    //    if (lineId) {

    //        url = "/Report/GetStationByLineID";
    //        ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showLineStationType, "json");
    //    }
    //});



    $("#LeakTestSummary").click(function (e) {

        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        //alert(shopId);
        if (lineId == "" || lineId == null) {
            lineId = 0;
        }
        // alert(frmDate);
        //alert(toDate);

        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }
        else 
        //if (lineId == "") {
        //    alert("Please Select Line Name");
        //    return;
        //}
        //else
            if (frmDate == "") {

            alert("Please Select From Date");
            return;

        }
        else if (toDate == "") {
            alert("Please Select To Date");
            return;
        }

        if (Date.parse(frmDate) > Date.parse(toDate)) {
            alert("From date should not be later than To Date");
            return;
        }

        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fMain_Leak_Test&From=' + frmDate + '&To=' + toDate + '&Shop=' + shopId + '&Line=' + lineId);



    });


    $("#DesouterData").click(function (e) {
        var serialno = $("#Serial_No").val();
        if (serialno == null || serialno == "" || serialno == " ") {
            alert("Please Select Enter Serial No");
            return;
        }
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTorque_By_Serial_No_Tabular&SerialNo=' + serialno);
    });

    $("#OilDispensingSummary").click(function (e) {

        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        if (lineId == "" || lineId == null) {
            lineId = 0;
        }
        //alert(shopId);
        //alert(lineId);
        // alert(frmDate);
        //alert(toDate);

        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }
        else
        //    if (lineId == "") {
        //    alert("Please Select Line Name");
        //    return;
        //}
        //    else
                if (frmDate == "") {

            alert("Please Select From Date");
            return;

        }
        else if (toDate == "") {
            alert("Please Select To Date");
            return;
        }

        if (Date.parse(frmDate) > Date.parse(toDate)) {
            alert("From date should not be later than To Date");
            return;
        }

        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fOil_Dispensing_Trend&Shop=' + shopId + '&Line=' + lineId + '&From=' + frmDate + '&To=' + toDate);

    });

    $("#OilDispensingSummarytabular").click(function (e) {

        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        //alert(shopId);
        //alert(lineId);
        // alert(frmDate);
        //alert(toDate);

        if (lineId == "" || lineId == null) {
            lineId = 0;
        }

        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }
        else
        //    if (lineId == "") {
        //    alert("Please Select Line Name");
        //    return;
        //}
        //    else
                if (frmDate == "") {

            alert("Please Select From Date");
            return;

        }
        else if (toDate == "") {
            alert("Please Select To Date");
            return;
        }

        if (Date.parse(frmDate) > Date.parse(toDate)) {
            alert("From date should not be later than To Date");
            return;
        }

        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fMain_Oil_Dispense&Shop=' + shopId + '&Line=' + lineId + '&From=' + frmDate + '&To=' + toDate);

    });
    $("#StationswiseDowntimeSummary").click(function (e) {
        //alert('Hi');
        $("#ContentFrame").css('display', 'block');
        var shopId = $("#Shop_ID").val();
        //var lineId = $("#Line_ID").val();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        var ShiftID = $("#Shift_ID").val();
        var LineID = $("#Line_ID").val();
        //alert(shopId);
        //alert(lineId);
        // alert(frmDate);
        //alert(toDate);

        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }
            //else if (lineId == "") {
            //    alert("Please Select Line Name");
            //}
        else if (frmDate == "") {

            alert("Please Select From Date");
            return;

        }
        else if (toDate == "") {
            alert("Please Select To Date");
            return;
        }

        if (Date.parse(frmDate) > Date.parse(toDate)) {
            alert("From date should not be later than To Date");
            return;
        }

        //alert('Hi');
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fCall_Details_By_Stations&From=' + frmDate + '&To=' + toDate + '&Shop=' + shopId + '&Line=' + LineID + '&Shift=' + ShiftID);

    });
    $("#LinewiseDowntimeSummary").click(function (e) {
        //alert('Hi');
        var shopId = $("#Shop_ID").val();
        //var lineId = $("#Line_ID").val();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        var ShiftID = $("#Shift_ID").val();
        var LineID = $("#Line_ID").val();
        //alert(shopId);
        //alert(lineId);
        // alert(frmDate);
        //alert(toDate);

        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }
            //else if (lineId == "") {
            //    alert("Please Select Line Name");
            //}
        else if (frmDate == "") {

            alert("Please Select From Date");
            return;

        }
        else if (toDate == "") {
            alert("Please Select To Date");
            return;
        }

        if (Date.parse(frmDate) > Date.parse(toDate)) {
            alert("From date should not be later than To Date");
            return;
        }

        //alert('Hi');
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fLineWise_Downtime_summary_with_ConsumeTime&From=' + frmDate + '&To=' + toDate + '&Shop=' + shopId + '&Line=' + LineID + '&Shift=' + ShiftID);

    });

    $("#Downtimeconsumed").click(function (e) {
        
        var shopId = $("#Shop_ID").val();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        var ShiftID = $("#Shift_ID").val();
        var LineID = $("#Line_ID").val();
        if (frmDate == "") {
            alert("Please Select From Date");
            return;
        }
        else if (toDate == "") {
            alert("Please Select To Date");
            return;
        }

        if (Date.parse(frmDate) > Date.parse(toDate)) {
            alert("From date should not be later than To Date");
            return;
        }
        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }

        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fSignal_Calls_Summary_By_Shop&Shop=' + shopId+'&From='+frmDate+'&To='+toDate+'&Line='+LineID+'&Shift='+ShiftID);

    });

    $("#DowntimeSummary").click(function (e) {

        var shopId = $("#Shop_ID").val();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        var Line = $("#Line_ID").val();
        var ShiftID = $("#Shift_ID").val();
        if (frmDate == "") {
            alert("Please Select From Date");
            return;
        }
        else if (toDate == "") {
            alert("Please Select To Date");
            return;
        }

        if (Date.parse(frmDate) > Date.parse(toDate)) {
            alert("From date should not be later than To Date");
            return;
        }
        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }

        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fCall_Type_By_Stations&Shop=' + shopId + '&From=' + frmDate + '&To=' + toDate+ '&Line=' + Line + '&Shift=' + ShiftID);

    });

    $("#TorqueandangleTrend").click(function (e) {

        var boltId = $("#Bolt_ID").val();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        if (boltId == "") {
            alert("Please Select Bolt Name");
            return;
        }
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTorque_And_Angle_Trend_Line_Chart&Bolt=' + boltId + '&From=' + frmDate + '&To=' + toDate);

    });

    $("#cbmdata").click(function (e) {

        var boltId = $("#Machine_ID").val();
        var ShopID = $("#Shop_ID").val();
        var cbmid = $("#CBM_ID").val();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        var Ctype = $("#Type").val();
        if (ShopID == "") {
            alert("Please Select Shop Name");
            return;
        }
        if (boltId == "") {
            alert("Please Select Machine Name");
            return;
        }
        if (cbmid == "") {
            alert("Please Select Parameter Name");
            return;
        }
        if (Date.parse(frmDate)>Date.parse(toDate)) {
            alert("from date should be less than to date");
            return;
        }
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fCBM_Trend&Machine=' + boltId + '&fromdate=' + frmDate + '&todate=' + toDate + '&cbm=' + cbmid + '&Shop=' + ShopID + '&CType=' + Ctype);

    });

    $("#TestBedOutput").click(function (e) {
        //alert('Hi');
        var shopId = $("#Shop_ID").val();
        //var lineId = $("#Line_ID").val();
        var frmDate = $("#From_Date :text").val();
        var toDate = $("#To_Date :text").val();
        //alert(shopId);
        //alert(lineId);
        // alert(frmDate);
        //alert(toDate);

        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }
            //else if (lineId == "") {
            //    alert("Please Select Line Name");
            //}
        else if (frmDate == "") {

            alert("Please Select From Date");
            return;

        }
        else if (toDate == "") {
            alert("Please Select To Date");
            return;
        }

        if (Date.parse(frmDate) > Date.parse(toDate)) {
            alert("From date should not be later than To Date");
            return;
        }

        //alert('Hi');
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTestBed_Output&dt1=' + frmDate + '&dt2=' + toDate );

    });


    //function showLineStationType() {
    //    var myajax = ajaxpack.ajaxobj
    //    var myfiletype = ajaxpack.filetype
    //    if (myajax.readyState == 4) {
    //        if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

    //            var jsonRes = $.parseJSON(myajax.responseText);
    //            SelectOptionHTML(jsonRes, "Station_ID");
    //        }
    //    }
    //}

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

//function SelectOptionHTML1(jsonRes, targetId) {
//    //var jsonRes = $.parseJSON(myajax.responseText);        
//    var res = "<option value='" + 0 + "'>" + "All Lines" + "</option>";
//    for (var i = 0; i < jsonRes.length; i++) {
//        res += "<option value='" + jsonRes[i].Id + "'>" + jsonRes[i].Value + "</option>";
//    }

//    res = "<option value=''>" + $("#" + targetId + " option:first").html() + "</option>" + res;
//    $("#" + targetId).html(res);
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




