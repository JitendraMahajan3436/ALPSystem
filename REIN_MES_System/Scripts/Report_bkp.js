$(document).ready(function (e) {

    $(".Datename").hide("slow");
    $(".Monthname").hide("slow");
    $(".Yearname").hide("slow");
    $(".manager_Line #To_Type").change(function (e) {


        if ($(this).val() == "01") {
            // alert("hello");
            $(".Datename").show("slow");
            $(".Monthname").hide("slow");
            $(".Yearname").hide("slow");
            //$(this).val(true);
            //$("#To_Type").val(01);
           // alert($("#To_Type").val());
        }
        else if ($(this).val() == "02") {
            $(".Datename").hide("slow");
            $(".Monthname").show("slow");
            $(".Yearname").show("slow");
            //$(this).val(true);
        }
        else {
            // alert("hi");
            //$(this).val("hide");
            $(".Datename").hide("slow");
            $(".Monthname").hide("slow");
            $(".Yearname").hide("slow");
        }
    });


    $(".manager_Line #Shop_ID").change(function (e) {
        var shopId = $("#Shop_ID").val();
        if (shopId) {

            var url = "/Report/GetLineByShopID";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopLineType, "json");
        }
    });

    $(".manager_Line #Line_ID").change(function (e) {

        var lineId = $("#Line_ID").val();
        if (lineId) {

            url = "/Report/GetStationByLineID";
            ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showLineStationType, "json");
        }
    });

    $("#MonthlyProductionPlan").click(function (e) {
        // alert("MonthlyProductionPlan");
        var shopId = $("#Shop_ID").val();
        //var lineId = $("#Line_ID").val();
        //var stationId = $("#Station_ID").val();
        var mnth = $("#From_Date").val();
        var yr = $("#To_Date option:selected").text();
        //alert(shopId);
        //alert(lineId);
        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }
        //else if (lineId == "") {
        //    alert("Please Select Line Name");
        //}
        //else if (stationId == "") {
        //    alert("Please Select Station Name");
        //}
        else if (mnth == 0) {
            alert("Please Select Month");
            return;
        }
        else if (yr == "-Select Year-") {
            alert("Please Select Year");
            return;
        }

        // alert(orderStateId);
        // alert(ModelId);
        // alert(seriesId);

        $('#ContentFrame').attr('src', 'http://MMZAHMESPDSDB1/ReportServer/Pages/ReportViewer.aspx?%2fFDReports%2fPrd_Monthly_Production_Plan&Mnth=' + mnth + '&Yr=' + yr + '&Shop=' + shopId + '&Line=' + lineId);

        // var url = "/Report/GenerateReportProductionOrderSummary";
        // ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "&lineId=" + $("#Line_ID").val() + "&stationId=" + $("#Station_ID").val() + "&orderStateId=" + $("#Order_State").val() + "&frmDate=" + $("#From_Date").val() + "&toDate=" + $("#To_Date").val() + "", showShopLineType, "json"); //+ "&ModelId=" + $("#Model").val() + "&seriesId=" + $("#Series").val()

    });

    $("#ProductionOrderSumarry").click(function (e) {
        //alert("ProductionOrderSumarry");
        var shopId = parseInt($("#Shop_ID").val());
        //var lineId = parseInt($("#Line_ID").val());
        var stationId = $("#Station_ID").val();
        var OrderState = $("#Order_State option:selected").text();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        // alert(frmDate + " "+ toDate);
        // alert(shopId);
        //alert(lineId);
        // alert(stationId);
        //alert(OrderState);
        //alert(frmDate);
        //alert(toDate);
        if (isNaN(shopId)) {
            alert("Please Select Shop Name");
            return;
        }
        //else if (isNaN(lineId)) {
        //    alert("Please Select Line Name");
        //}
        //else if (stationId == "") {
        //    alert("Please Select Station Name");
        //}
        else if (OrderState == "-Select Order State-") {
            alert("Please Select OrderState");
            return;
        }
        else if (frmDate == "") {
            alert("Please Select From Date");
            return;
        }
        else if (toDate == "") {
            alert("Please Select To Date");
            return;
        }

        if (frmDate > toDate) {
            alert("Invalid Date");
            return;
        }

        //$('#ContentFrame').attr('src', 'http://MMZAHMESPDSDB1/ReportServer/Pages/ReportViewer.aspx?%2fFDReports%2fProd_Production_Orders_Summary&Shop=' + shopId + '&Line=' + lineId + '&OrderStatus=' + OrderState + '&From=' + frmDate + '&To=' + toDate);

        if (OrderState == "QOK") {
            $('#ContentFrame').attr('src', 'http://MMZAHMESPDSDB1/ReportServer/Pages/ReportViewer.aspx?%2fFDReports%2fProd_Production_Orders_Summary_QOK&Shop=' + shopId + '&From=' + frmDate + '&To=' + toDate);
        }
        else if (OrderState == "PRODB") {
            $('#ContentFrame').attr('src', 'http://MMZAHMESPDSDB1/ReportServer/Pages/ReportViewer.aspx?%2fFDReports%2fProd_Production_Orders_Summary_Book&Shop=' + shopId + '&From=' + frmDate + '&To=' + toDate);
        }
        else if (OrderState == "Started") {
            $('#ContentFrame').attr('src', 'http://MMZAHMESPDSDB1/ReportServer/Pages/ReportViewer.aspx?%2fFDReports%2fProd_Production_Orders_Summary&Shop=' + shopId + '&OrderStatus=' + OrderState + '&From=' + frmDate + '&To=' + toDate);
        }
        else if (OrderState == "TakeOut") {
            $('#ContentFrame').attr('src', 'http://MMZAHMESPDSDB1/ReportServer/Pages/ReportViewer.aspx?%2fFDReports%2fProd_Production_Orders_Summary_TakeOut&Shop=' + shopId + '&From=' + frmDate + '&To=' + toDate);
        }
        else if (OrderState == "Closed") {
            $('#ContentFrame').attr('src', 'http://MMZAHMESPDSDB1/ReportServer/Pages/ReportViewer.aspx?%2fFDReports%2fProd_Production_Orders_Summary_Closed&Shop=' + shopId + '&From=' + frmDate + '&To=' + toDate);
        }
        else if (OrderState == "Hold" || OrderState == "Release") {
            $('#ContentFrame').attr('src', 'http://MMZAHMESPDSDB1/ReportServer/Pages/ReportViewer.aspx?%2fFDReports%2fProd_Production_Orders_Summary_RH&Shop=' + shopId + '&OrderStatus=' + OrderState + '&From=' + frmDate + '&To=' + toDate);
        }

        //var url = "/Report/GenerateReportMonthlyProductionPlan";
        //ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "&lineId=" + $("#Line_ID").val() + "&stationId=" + $("#Station_ID").val() + "&frmDate=" + $("#From_Date").val() + "&toDate=" + $("#To_Date").val() + "", showShopLineType, "json");


    });

    $("#PlanVSActualDashboard").click(function (e) {
        //alert('pvsa')

        $('#ContentFrame').attr('src', 'http://MMZAHMESPDSDB1/ReportServer/Pages/ReportViewer.aspx?%2fFDReports%2fPlanVsAtual_Dashboard');


    });

    $("#PlanVSActual").click(function (e) {
        // alert("ProductionOrderSumarry");
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var stationId = $("#Station_ID").val();
        var frmDate = $("#From_Date").val();

        var d = new Date(frmDate);
        d.setDate(d.getDate() - 1);

        var date = new Date(d);

        // GET YYYY, MM AND DD FROM THE DATE OBJECT
        var yyyy = date.getFullYear().toString();
        var mm = (date.getMonth() + 1).toString();
        var dd = date.getDate().toString();

        // CONVERT mm AND dd INTO chars
        var mmChars = mm.split('');
        var ddChars = dd.split('');

        // CONCAT THE STRINGS IN YYYY-MM-DD FORMAT
        var datestring = yyyy + '-' + (mmChars[1] ? mm : "0" + mmChars[0]) + '-' + (ddChars[1] ? dd : "0" + ddChars[0]);

        var frmDate1 = datestring;
        
        //alert(frmDate1 + ' ' + frmDate)
        var toDate = $("#To_Date").val();
        var toType = $("#To_Type").val();
        //alert(toType);
        var Year = $("#Year option:selected").text();

        if (shopId == "") {
            alert("Please Select Shop Name");
        }
        //else if (lineId == "") {
        //    alert("Please Select Line Name");
        //}
        else if (stationId == "") {
            alert("Please Select Station Name");
        }
        else if (toType == 0) {
            alert("Please Select Date Type");
        }
        //alert(toType);
        if (toType == 01) {
            if (frmDate == 0) {
                alert("Please Select Date");
            }
        }
        if (toType == 02) {
            if (toDate == 0) {
                alert("Please Select Month");
            }
            else if (Year == 0) {
                alert("Please Select Year");
            }
        }
        //alert('Hi : ' + toType)
        if (toType == 01) {
            //alert(frmDate +' '+ frmDate1 + ' ' + shopId)
            $('#ContentFrame').attr('src', 'http://MMZAHMESPDSDB1/ReportServer/Pages/ReportViewer.aspx?%2fFDReports%2fProd_Planned_Actual_Daily&From=' + frmDate1 + '&To=' + frmDate + '&Shop=' + shopId);
        }
        else {
            // alert('r2');
            //alert(toDate +' '+ Year + ' ' + shopId)
            $('#ContentFrame').attr('src', 'http://MMZAHMESPDSDB1/ReportServer/Pages/ReportViewer.aspx?%2fFDReports%2fProd_Planned_Actual_Monthly&Mnth=' + toDate + '&Yr=' + Year + '&Shop=' + shopId);
        }



       
        // var search = window.location.search;
        //$("#myIframe").attr("src", $("#myIframe").attr("src") + search);

        //var url = "/Report/GenerateReportMonthlyProductionPlan";
        //ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "&lineId=" + $("#Line_ID").val() + "&stationId=" + $("#Station_ID").val() + "&frmDate=" + $("#From_Date").val() + "&toDate=" + $("#To_Date").val() + "", showShopLineType, "json");


    });

    $("#PlanProductionDashboard").click(function (e) {
        // alert("ProductionOrderSumarry");
        //var shopId = $("#Shop_ID").val();
        //var lineId = $("#Line_ID").val();
        //var stationId = $("#Station_ID").val();
        //var frmDate = $("#From_Date").val();
        //var toDate = $("#To_Date").val();

        $('#ContentFrame').attr('src', 'http://MMZAHMESPDSDB1/ReportServer/Pages/ReportViewer.aspx?%2fFDReports%2fProduction_Dashboard_Daily');
        // var search = window.location.search;
        //$("#myIframe").attr("src", $("#myIframe").attr("src") + search);

        //var url = "/Report/GenerateReportMonthlyProductionPlan";
        //ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "&lineId=" + $("#Line_ID").val() + "&stationId=" + $("#Station_ID").val() + "&frmDate=" + $("#From_Date").val() + "&toDate=" + $("#To_Date").val() + "", showShopLineType, "json");


    });



    $("#btnHide").click(function (e) {
        // alert("hiii");
        // $("div1").hide();
        document.getElementById("div1").style.visibility = "hidden";

    });




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