$(document).ready(function (e) {

    $(".Datename").hide("slow");
    $(".Monthname").hide("slow");
    $(".Yearname").hide("slow");
    $(".Quatername").hide("slow");

    $(".manager_Line #Shop_ID").change(function (e) {
        var shopId = $("#Shop_ID").val();
        if (shopId) {
            var url = "/OMOrderPlanning/GetLineID";
            ajaxpack.getAjaxRequest(url, "Shop_id=" + $("#Shop_ID").val() + "", showLineDetail, "json");
        }
        else {
            clearSelectBox("Line_ID");
        }
    });

    $(".manager_Line #Line_ID").change(function (e) {
        var Line_ID = $("#Line_ID").val();
        if (Line_ID) {
            var url = "/QualityReport/GetModelsByLine";
            ajaxpack.getAjaxRequest(url, "Line_ID=" + Line_ID + "", showModelsByShop, "json");
        }
        else {
            clearSelectBox("Line_ID");
        }
    });

    function showLineDetail() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTMLforLine(jsonRes, "Line_ID");
            }

        }
    }
    function SelectOptionHTMLforLine(jsonRes, targetId) {
        var res = "";
        for (var i = 0; i < jsonRes.length; i++) {
            res += "<option value='" + jsonRes[i].Line_ID + "'>" + jsonRes[i].Line_Name + "</option>";
        }
        res = "<option value='0'>" + $("#" + targetId + " option:first").html() + "</option>" + res;
        $("#" + targetId).html(res);

    }

    function SelectOptionHTML(jsonRes, targetId) {
        var res = "";
        for (var i = 0; i < jsonRes.length; i++) {
            res += "<option value='" + jsonRes[i].Id + "'>" + jsonRes[i].Value + "</option>";
        }
        res = "<option value='0'>" + $("#" + targetId + " option:first").html() + "</option>" + res;
        $("#" + targetId).html(res);

    }

    function showModelsByShop() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Model_Code");
            }

        }
    }
    $(".manager_Line #To_Type").change(function (e) {

        $("#Quater").val("0");
        $("#FromYear").val("2018");
        $("#ToYear").val("2018");
        if ($(this).val() == "01") {
            // alert("hello");
            $(".Datename").show("slow");
            $(".Monthname").hide("slow");
            $(".Yearname").hide("slow");
            $(".Quatername").hide("slow");
            // $(this).val(true);

            //$("#To_Type").val(01);
            // alert($("#To_Type").val());
        }
        else if ($(this).val() == "02") {
            $(".Datename").hide("slow");
            $(".Monthname").show("slow");
            $(".Yearname").show("slow");
            $(".Quatername").hide("slow");
            //$(this).val(true);
        }
        else if ($(this).val() == "03") {
            $(".Datename").hide("slow");
            $(".Monthname").hide("slow");
            $(".Yearname").show("slow");
            $(".Quatername").show("slow");
            //$(this).val(true);
        }
        else if ($(this).val() == "04") {
            $(".Datename").hide("slow");
            $(".Monthname").show("slow");
            $(".Yearname").hide("slow");
            $(".Quatername").hide("slow");
            //$(this).val(true);
        }
        else if ($(this).val() == "05") {
            $(".Datename").hide("slow");
            $(".Monthname").hide("slow");
            $(".Yearname").show("slow");
            $(".Quatername").hide("slow");
            //$(this).val(true);
        }
        else {
            // alert("hi");
            $(this).val("hide");
            $(".Datename").hide("slow");
            $(".Monthname").hide("slow");
            $(".Yearname").hide("slow");
            $(".Quatername").hide("slow");
        }



    });

    $(".manager_Line #Type").change(function (e) {


        if ($(this).val() == "01") {
            // alert("hello");
            $(".Datename").show("slow");
            $(".Monthname").hide("slow");
            $(".Yearname").hide("slow");
            // $(this).val(true);

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
            $(this).val("hide");
            $(".Datename").hide("slow");
            $(".Monthname").hide("slow");
            $(".Yearname").hide("slow");
        }
    });


    $(".SerialNumber").hide("slow");
    $(".ShopName").hide("slow");
    $(".LineName").hide("slow");
    $(".StationName").hide("slow");
    $(".ModelCode").hide("slow");
    $(".FromeDate").hide("slow");
    $(".ToDate").hide("slow");
    $(".manager_Line #To_Category").change(function (e) {


        if ($(this).val() == "01") {
            // alert("hello");
            $(".SerialNumber").show("slow");
            $(".ShopName").hide("slow");
            $(".LineName").hide("slow");
            $(".StationName").hide("slow");
            $(".ModelCode").hide("slow");
            $(".FromeDate").hide("slow");
            $(".ToDate").hide("slow");

        }
        else if ($(this).val() == "02") {
            $(".SerialNumber").hide("slow");
            $(".ShopName").show("slow");
            $(".LineName").show("slow");
            $(".StationName").show("slow");
            $(".ModelCode").show("slow");
            $(".FromeDate").show("slow");
            $(".ToDate").show("slow");

            //$(this).val(true);
        }
        else {
            // alert("hi");
            $(this).val("hide");
            $(".SerialNumber").hide("slow");
            $(".ShopName").hide("slow");
            $(".FromeDate").hide("slow");
            $(".ToDate").hide("slow");
        }
    });

    //$(".manager_Line #Shop_ID").change(function (e) {
    //    var shopId = $("#Shop_ID").val();
    //    if (shopId) {

    //        var url = "/Report/GetLineByShopID";
    //        ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopLineType, "json");
    //    }
    //});

    //$(".manager_Line #Line_ID").change(function (e) {

    //    var lineId = $("#Line_ID").val();
    //    //alert(lineId);
    //    if (lineId) {

    //        url = "/Report/GetStationByLineID";
    //        ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showLineStationType, "json");
    //    }
    //});




    $("#ReworkDetails").click(function (e) {
        var toCategory = $("#To_Category").val();
        var shopId = $("#Shop_ID").val();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        var SerialNo = $("#SeriesNo").val();
        //var Prod_Line_ID = $("#Prod_Line_ID").val();
        if (toCategory == 0) {
            alert("Please Select Category Type");
            return;
        }

        if (toCategory == 01) {
            if (SerialNo == "") {
                alert("Please Select Serial Number");
                return;
            }
        }
        if (toCategory == 02) {
            if (shopId == "") {
                alert("Please Select Shop Name");
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

        }

        if (Date.parse(frmDate) > Date.parse(toDate)) {
            alert("From date should not be later than To Date");
            return;
        }
        if (toCategory == 01) {
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fQty_Rework_Details_SerialNo&Serial=' + SerialNo);
        }
        else {
            debugger;
            var p = 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fQty_Rework_Details&Shop=' + shopId + '&from=' + frmDate + '&to=' + toDate + '&line=' + $("#Line_ID").val() + '&station=' + $("#Station_ID").val() + '&modelCode=' + $("#Model_Code").val();
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fQty_Rework_Details&Shop=' + shopId + '&from=' + frmDate + '&to=' + toDate + '&line=' + $("#Prod_Line_ID").val() + '&station=' + $("#Station_ID").val() + '&modelCode=' + $("#Model_Code").val());
            //alert("hi");
        }


    });


    $("#DefectCategory").click(function (e) {
        // alert("ProductionOrderSumarry");
        var shopId = $("#Shop_ID").val();
        // var lineId = $("#Line_ID").val();
        var stationId = $("#Station_ID").val();


        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        var toType = $("#To_Type").val();
        var Year = $("#Year option:selected").text();
        var LineID = $("#Line_ID").val();
        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }
        if (LineID == "0") {
            alert("Please Select Line Name");
            return;
        }

        else if (stationId == "") {
            alert("Please Select Station Name");
        }
        else if (toType == 0) {
            alert("Please Select Date Type");
            return;
        }
        //alert(toType);
        if (toType == 01) {
            if (frmDate == 0) {
                alert("Please Select Date");
                return;
            }
        }
        if (toType == 02) {
            if (toDate == 0) {
                alert("Please Select Month");
                return;
            }
            else if (Year == 0) {
                alert("Please Select Year");
                return;
            }
        }
        if (toType == 01) {
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fQty_Defect_Category_Daily&From=' + frmDate + '&Shop=' + shopId + '&Station=' + stationId + '&Line=' + LineID);
        }
        else {
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fQty_Defect_Category_Monthly&Mnth=' + toDate + '&Yr=' + Year + '&Shop=' + shopId + '&Station=' + stationId + '&Line=' + LineID);

        }



    });



    //$("#QPAudit_Category_DefectCount").click(function (e) {
    //    // alert("ProductionOrderSumarry");
    //    var shopId = $("#Shop_ID").val();
    //    var Audit = $("#Cat_ID").val();

    //    var frmDate = $("#From_Date").val();
    //    var toDate = $("#To_Date").val();
    //    var month = $("#month").val();
    //    var toType = $("#To_Type").val();
    //    var Year = $("#Year option:selected").val();
    //    var quater = $("#Quater option:selected").val();

    //    if (shopId == "") {
    //        alert("Please Select Shop Name");
    //        return;
    //    }


    //    if (Audit == "") {
    //        alert("Please Select Audit type");
    //        return;
    //    }



    //    else if (toType == 0) {
    //        alert("Please Select Date Type");
    //        return;
    //    }
    //    //alert(toType);
    //    if (toType == 01) {
    //        if (frmDate == 0) {
    //            alert("Please Select Date");
    //            return;
    //        }
    //    }

    //    if (toType == 02) {
    //        //if (toDate == 0) {
    //        //    alert("Please Select Month");
    //        //    return;
    //        //}
    //        if (Year == 0) {
    //            alert("Please Select Year");
    //            return;
    //        }
    //    }


    //    if (toType == 1) {
    //        Year = "";
    //        quater = "";
    //    }
    //    else if (toType == 2) {

    //        //frmDate = "";
    //        //toDate = "";
    //        quater = "";
    //    }
    //    else if (toType == 3) {
    //        //frmDate = "";
    //        //toDate = "";
    //        Year = "";
    //    }



    //    $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fAudit_Category_Wise_DefectCount&FromDate=' + frmDate + '&AuditType=' + Audit + '&ReportType=' + toType + '&ToDate=' + toDate + '&Shop=' + shopId + '&yr=' + Year + '&Quarter=' + quater);

    //});

    //------------------------------------------------------

    $("#QPStationwiseLPHRPH").click(function (e) {
        // alert("ProductionOrderSumarry");
        var shopId = $("#Shop_ID").val();
        // var lineId = $("#Line_ID").val();
        var stationId = $("#Station_ID").val();


        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        var month = $("#month").val();
        var toType = $("#To_Type").val();
        var Year = $("#Year").val();

        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }

        else if (stationId == "") {
            alert("Please Select Station Name");
        }
        else if (toType == 0) {
            alert("Please Select Date Type");
            return;
        }
        //alert(toType);
        if (toType == 01) {
            if (frmDate == 0) {
                alert("Please Select Date");
                return;
            }
        }
        if (toType == 02) {
            if (toDate == 0) {
                alert("Please Select Month");
                return;
            }
            else if (Year == 0) {
                alert("Please Select Year");
                return;
            }
        }
        if (toType == 01) {
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fQP_Station_Wise_RPH_Daily&From=' + frmDate + '&To=' + toDate + '&Shop=' + shopId + '&Station=' + stationId);
        }
        else {
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fQP_station_RPH_Monthly&Yr=' + Year + '&Shop=' + shopId + '&Station=' + stationId);

        }
    });


    $("#operatorWiseDefect").click(function (e) {
        var shopId = $("#Shop_ID").val();
        var toDate = $("#To_Date option:selected").val();
        var Year = $("#Year option:selected").text();
        var yearval = $("#Year option:selected").val();
        var LineId = $("#Line_ID").val();

        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }

        //if (toDate == 0) {
        //    alert("Please Select Month");
        //    return;
        //}
        //else if (yearval == 0) {
        //    alert("Please Select Year");
        //    return;
        //}

        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fOperator_Wise_Defect_Capture&mnth=' + toDate + '&yr=' + yearval + '&Shop=' + shopId + '&Line=' + LineId);

    });


    $("#QualityDashboard").click(function (e) {
        // alert("ProductionOrderSumarry");
        var shopId = $("#Shop_ID").val();
        // var lineId = $("#Line_ID").val();
        // var stationId = $("#Station_ID").val();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        //var toType = $("#To_Type").val();
        //var Year = $("#Year option:selected").text();
        if (frmDate == "" || toDate == "") {
            alert("please select Date");
            return;
        }
        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }
        if (Date.parse(frmDate) > Date.parse(toDate)) {
            alert("From date should not be later than To Date");
            return;
        }

        // if (toType == 01) {
        // $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fQuality_Dashboard_Quage_ShopWise&From=' + frmDate + '&To' + toDate + '&Shop' + shopId);
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fQty_Quality_Dashboard_Daily_New&From=' + frmDate + '&To=' + toDate);
        // }
        // else {
        //      $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fQty_Quality_Dashboard_Monthly_New&Mnth=' + toDate + '&Yr=' + Year);
        //  }

    });


    $("#LPHRPHTreand").click(function (e) {

        var shopId = parseInt($("#Shop_ID").val());
        var From = $("#From_Date").val();
        var To = $("#To_Date").val();
        var Type = $("#Type").val();//Monthly or Daily
        var isTesting = $("#Testing").val();//Testing or Assembly
        var chartType = $("#Chart_Type").val();//LPH or RPH
        var Line = $("#Line_ID").val();
        if (isNaN(shopId)) {
            alert("Please Select Shop Name");
            return;
        }
        if (Line == 0) {
            alert("Please select Line Name.");
            return;
        }
        if (Type == 01 && isTesting == 01)//Testing Daily
        {
            if (From == "" || From == " " || From == null || From == "undefined" || !isNaN(From)) {
                alert("Please Select From Date");
                return;
            }
            else if (To == "" || To == " " || To == null || To == "undefined" || !isNaN(To)) {
                alert("Please Select To Date");
                return;
            }
            if (Date.parse(From) > Date.parse(To)) {
                alert("From date should not be later than To Date");
                return;
            }
            if (chartType == 01) {
                //LPH DAY WISE
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTesting_LPH_DAYWISE_Trend&From=' + From + '&To=' + To + '&Shop=' + shopId + '&Line=' + Line);
            } else {
                //RPH DAY WISE
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTesting_RPH_DAYWISE_Trend&From=' + From + '&To=' + To + '&Shop=' + shopId + '&Line=' + Line);
            }
        }
        else if (Type == 01 && isTesting == 02)//Assembly Daily
        {

            if (From == "" || From == " " || From == null || From == "undefined" || !isNaN(From)) {
                alert("Please Select From Date");
                return;
            }
            else if (To == "" || To == " " || To == null || To == "undefined" || !isNaN(To)) {
                alert("Please Select To Date");
                return;
            }
            if (Date.parse(From) > Date.parse(To)) {
                alert("From date should not be later than To Date");
                return;
            }

            if (chartType == 01) {
                //LPH DAY WISE
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fLPH_DAYWISE_Trend&From=' + From + '&To=' + To + '&Shop=' + shopId + '&Line=' + Line);

            } else {
                //RPH DAY WISE
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fRPH_DAYWISE_Trend&From=' + From + '&To=' + To + '&Shop=' + shopId + '&Line=' + Line);
            }
        }
        if (Type == 02 && isTesting == 01)//Testing Monthly
        {
            var Year = $("#Year option:selected").text();
            if (isNaN(Year)) {
                alert("Please Select Year");
                return;
            }

            if (chartType == 01) {
                //LPH Month WISE
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTesting_LPH_Month&yr=' + Year + '&Shop=' + shopId + '&Line=' + Line);
            } else {
                //RPH Month WISE
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTesting_Month_Wise_RPH&Yr=' + Year + '&Shop=' + shopId + '&Line=' + Line);
            }

        }
        if (Type == 02 && isTesting == 02)//Assembly Monthly
        {
            var Year = $("#Year option:selected").text();
            if (isNaN(Year)) {
                alert("Please Select Year");
                return;
            }
            if (chartType == 01) {
                //LPH Month WISE
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fLPH_Month&yr=' + Year + '&Shop=' + shopId + '&Line=' + Line);
            } else {
                //RPH Month WISE
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fMonth_Wise_RPH&yr=' + Year + '&Shop=' + shopId + '&Line=' + Line);
            }
        }

        //if (isTesting = 0) {
        //    $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fQTY_LPH_RPH_TREND_TESTING_STATION_WISE&From=' + From + '&To=' + To + '&Shop=' + shopId);
        //}
        //else {
        //    $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fQTY_LPH_RPH_TREND&From=' + From + '&To=' + To + '&Shop=' + shopId);
        //}
    });


    $("#CLR").click(function (e) {

        var shopId = parseInt($("#Shop_ID").val());
        var From = $("#From_Date").val();
        var To = $("#To_Date").val();
        var Type = $("#Type").val();//Monthly or Daily

        if (isNaN(shopId)) {
            alert("Please Select Shop Name");
            return;
        }


        if (Type == 02)//Assembly Monthly
        {
            var Year = $("#Year option:selected").text();
            if (isNaN(Year)) {
                alert("Please Select Year");
                return;
            }
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fCLR_RPH_Month&Yr=' + Year + '&CLRType=' + shopId + "&Shop=" + $("#CLR_Shop_ID").val());
        }
        else if (Type == 01) {
            if (From == "" || From == " " || From == null || From == "undefined" || !isNaN(From)) {
                alert("Please Select From Date");
                return;
            }
            else if (To == "" || To == " " || To == null || To == "undefined" || !isNaN(To)) {
                alert("Please Select To Date");
                return;
            }
            if (Date.parse(From) > Date.parse(To)) {
                alert("From date should not be later than To Date");
                return;
            }

            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fCLR_By_Daywise&From=' + From + '&To=' + To + '&CLRType=' + shopId + "&Shop=" + $("#CLR_Shop_ID").val());
        }

    });

    $("#StationCLR").click(function (e) {

        var shopId = parseInt($("#Shop_ID").val());
        var From = $("#From_Date").val();
        var To = $("#To_Date").val();
        var Type = $("#Type").val();//Monthly or Daily
        var Station = parseInt($("#Station_ID").val());
        if (isNaN(shopId)) {
            alert("Please Select Shop Name");
            return;
        }
        if (isNaN(Station)) {
            alert("Please Select Station Name");
            return;
        }

        if (Type == 02)//Assembly Monthly
        {
            var Year = $("#Year option:selected").text();
            if (isNaN(Year)) {
                alert("Please Select Year");
                return;
            }
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fCLR_StationWise_by_Month&Yr=' + Year + '&Station=' + Station + '&CLRType=' + shopId + "&Shop=" + $("#CLR_Shop_ID").val());
        }
        else if (Type == 01) {
            if (From == "" || From == " " || From == null || From == "undefined" || !isNaN(From)) {
                alert("Please Select From Date");
                return;
            }
            else if (To == "" || To == " " || To == null || To == "undefined" || !isNaN(To)) {
                alert("Please Select To Date");
                return;
            }
            if (Date.parse(From) > Date.parse(To)) {
                alert("From date should not be later than To Date");
                return;
            }

            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fCLR_StationWise_by_Date&From=' + From + '&To=' + To + '&Station=' + Station + '&CLRType=' + shopId + "&Shop=" + $("#CLR_Shop_ID").val());
        }

    });

    $("#ModelCLR").click(function (e) {

        var shopId = parseInt($("#Shop_ID").val());
        var From = $("#From_Date").val();
        var To = $("#To_Date").val();
        var Type = $("#Type").val();//Monthly or Daily
        var model = $("#Model_ID").val();
        if (isNaN(shopId)) {
            alert("Please Select Shop Name");
            return;
        }


        if (Type == 02)//Assembly Monthly
        {
            var Year = $("#Year option:selected").text();
            if (isNaN(Year)) {
                alert("Please Select Year");
                return;
            }
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fCLR_ModelWise&Yr=' + Year + '&Atr=' + model + '&CLRType=' + shopId + "&Shop=" + $("#CLR_Shop_ID").val());
        }
        else if (Type == 01) {
            if (From == "" || From == " " || From == null || From == "undefined" || !isNaN(From)) {
                alert("Please Select From Date");
                return;
            }
            else if (To == "" || To == " " || To == null || To == "undefined" || !isNaN(To)) {
                alert("Please Select To Date");
                return;
            }
            if (Date.parse(From) > Date.parse(To)) {
                alert("From date should not be later than To Date");
                return;
            }
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fCLR_ModelWise_by_Date&From=' + From + '&To=' + To + '&Atr=' + model + '&CLRType=' + shopId + "&Shop=" + $("#CLR_Shop_ID").val());
        }

    });


    $("#ModelWiseLPHRPHTreand").click(function (e) {
        var shopId = parseInt($("#Shop_ID").val());
        //var ModelID = $("#Model_Code").val();
        var FamilyID = $("#Model_ID").val()
        var From = $("#From_Date").val();
        var To = $("#To_Date").val();
        var Type = $("#Type").val();
        var isTesting = $("#Testing").val();
        var chartType = $("#Chart_Type").val();
        if (isNaN(shopId)) {
            alert("Please Select Shop Name");
            return;
        }
        if (FamilyID == "") {
            alert("Please Select Family");
            return;
        }
        //if (ModelID == "") {
        //    alert("Please Select Model code");
        //    return;
        //}

        if (Type == 0) {
            alert("Please Select Type");
            return;
        }
        if (isTesting == 0) {
            alert("Please Select Area");
            return;
        }
        if (chartType == 0) {
            alert("Please Select Chart Type");
            return;
        }

        if (Type == 01 && isTesting == 01)//Testing Daily
        {
            if (From == "" || From == " " || From == null || From == "undefined" || !isNaN(From)) {
                alert("Please Select From Date");
                return;
            }
            else if (To == "" || To == " " || To == null || To == "undefined" || !isNaN(To)) {
                alert("Please Select To Date");
                return;
            }
            if (Date.parse(From) > Date.parse(To)) {
                alert("From date should not be later than To Date");
                return;
            }
            if (chartType == 01) {
                //LPH DAY WISE
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTesting_LPH_Modelwise_Daily&From=' + From + '&To=' + To + '&Atr=' + FamilyID + '&Shop=' + shopId );
            } else {
                //RPH DAY WISE
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTesting_RPH_Modelwise_Daily&From=' + From + '&To=' + To + '&Atr=' + FamilyID + '&Shop=' + shopId);
            }
        }
        else if (Type == 01 && isTesting == 02)//Assembly Daily
        {

            if (From == "" || From == " " || From == null || From == "undefined" || !isNaN(From)) {
                alert("Please Select From Date");
                return;
            }
            else if (To == "" || To == " " || To == null || To == "undefined" || !isNaN(To)) {
                alert("Please Select To Date");
                return;
            }
            if (Date.parse(From) > Date.parse(To)) {
                alert("From date should not be later than To Date");
                return;
            }

            if (chartType == 01) {
                //LPH DAY WISE
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fLPH_Modelwise_Daily&From=' + From + '&To=' + To + '&Atr=' + FamilyID + '&Shop=' + shopId );

            } else {
                //RPH DAY WISE
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fRPH_Modelwise_Daily&From=' + From + '&To=' + To + '&Atr=' + FamilyID + '&Shop=' + shopId );
            }
        }
        if (Type == 02 && isTesting == 01)//Testing Monthly
        {
            var Year = $("#Year").val();
            if (isNaN(Year)) {
                alert("Please Select Year");
                return;
            }

            if (chartType == 01) {
                //LPH Month WISE
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTesting_Month_Model_Wise_LPH&yr=' + Year + '&AtrLPH=' + FamilyID + '&ShopLPH=' + shopId );
            } else {
                //RPH Month WISE
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTesting_Month_Model_Wise_RPH&yr=' + Year + '&AtrRPH=' + FamilyID + '&ShopRPH=' + shopId );
            }

        }
        if (Type == 02 && isTesting == 02)//Assembly Monthly
        {
            var Year = $("#Year").val();
            if (isNaN(Year)) {
                alert("Please Select Year");
                return;
            }
            if (chartType == 01) {
                //LPH Month WISE
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fMonth_and_Model_wise_LPH&yr=' + Year + '&AtrLPH=' + FamilyID + '&ShopLPH=' + shopId );
            } else {
                //RPH Month WISE
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fMonth_and_Model_wise_RPH&yr=' + Year + '&AtrRPH=' + FamilyID + '&ShopRPH=' + shopId);
            }
        }
    });

    $("#TrackingStatus").click(function (e) {
        //alert('pvsa')
        var shopId = parseInt($("#Shop_ID").val());
        var LineId = parseInt($("#Line_ID").val());
        if (shopId == null) {
            alert("Please Select Shop");
            return;
        } else
            if (LineId == 0) {
                alert("Please Select Line");
                return;
            }
        if (shopId != 1) {
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fEngine_Tracking_Status&Shop=' + shopId + '&Line=' + LineId);
        }
        else {
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTractor_Tracking_Status');
        }

    });

    //Jitendra Mahajan and sandip takeintakeout report

    $("#TakeOutVehical").click(function (e) {


        var shopId = $("#Shop_ID").val();
        //  alert(shopId)
        if (shopId == 0) {
            alert("Please Select Shop ");
            return;
        }
        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }
        $('#ContentFrame').attr('src', 'http://MMZAHMESPDSDB1/ReportServer/Pages/ReportViewer.aspx?%2ftake_out_vehicle_shopwise&Shop_ID=' + shopId);

    });

    $("#TakeInTakeOut").click(function (e) {

        var criteria = $("#TakeInCriteria").val();
        var shopId = $("#Shop_ID").val();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();

        if (criteria == 0) {
            alert("Please Select Criteria Type");
            return;
        }
        if (shopId == 0) {
            alert("Please Select Shop");
            return;
        }

        if (criteria == 02) {
            if (shopId == "") {
                alert("Please Select Shop Name");
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
        }

        if (Date.parse(frmDate) > Date.parse(toDate)) {
            alert("From date should not be later than To Date");
            return;
        }

        if (criteria == 01) {
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fQuality_SummarReport_TI&ShopID=' + shopId + '&FromDate=' + frmDate + '&ToDate=' + toDate);
        }
        else {
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fQuality_SummarReport_TITO&fromdate=' + frmDate + '&todate=' + toDate + '&ShopID=' + shopId);
        }
    });



});


//------------------------------Quality Audit Report------------------------------//

$("#LPHRPHTreand_Audit").click(function (e) {

    var shopId = parseInt($("#Shop_ID").val());

    var From = $("#From_Date").val();
    var To = $("#To_Date").val();
    var From_Month = $("#From_Month").val();
    var To_Month = $("#To_Month").val();
    var Year = $("#Year").val();

    var Type = $("#To_Type").val();//Monthly or Daily
    var Audit = $("#Cat_ID").val();//Audit
    var Audit_name = $("#Audit_name").val();//Audit name
    var Audit_type = $("#Audit_type").val();//Audit type
    var is_model = $("#Is_Model").is(":checked");//IS model model or family
    var chartType = $("#Chart_Type").val();//LPH or RPH
    var Model_family = $("#Model_family").val();

    if (isNaN(shopId)) {
        alert("Please Select Shop Name.....!");
        return;
    }

    if (isNaN(Type) || Type == "0") {
        alert("Please Select Date Type.....!");
        return;
    }

    //From Date and To Date validation
    if (Type == 01) {
        if (From == "" || From == " " || From == null || From == "undefined" || !isNaN(From)) {
            alert("Please Select From Date");
            return;
        }
        else if (To == "" || To == " " || To == null || To == "undefined" || !isNaN(To)) {
            alert("Please Select To Date");
            return;
        }
        if (Date.parse(From) > Date.parse(To)) {
            alert("From date should not be later than To Date");
            return;
        }
    }
    ////From Date and To Date validation
    if (Type == 04) {
        if (From_Month == "" || From_Month == " " || From_Month == null || From_Month == "undefined" || !isNaN(From_Month)) {
            alert("Please Select From Month");
            return;
        }
        else if (To_Month == "" || To_Month == " " || To_Month == null || To_Month == "undefined" || !isNaN(To_Month)) {
            alert("Please Select To Month");
            return;
        }
        if (From_Month > To_Month) {
            alert("From Month should not be later than To Month");
            return;
        }
    }


    if (isNaN(chartType) || chartType == "0") {
        alert("Please Select Report Data Type.....!");
        return;
    }

    debugger;

    if (Type == 01) {
        if (chartType == 01) {
            //LPH DAY WISE           
            if (is_model == true) {
                //Model wise
                $('#ContentFrame').attr('src', ' http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fLPH_DAYWISE_AUDIT_TREND&From=' + From + '&To=' + To + '&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&Audit_type=' + Audit_type + '&Is_Model=' + is_model + '&Model=' + Model_family + '&Family=' + null);
            }
            else {
                //Family wise
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fLPH_DAYWISE_AUDIT_TREND&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&From=' + From + '&To=' + To + '&Family=' + Model_family + '&Is_Model=' + is_model + '&Model=' + null + '&Audit_type=' + Audit_type);

            }
        }
        else if (chartType == 02) {
            //RPH DAY WISE
            if (is_model == true) {
                //Model wise
                $('#ContentFrame').attr('src', ' http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fRPH_DAYWISE_AUDIT_TREND&From=' + From + '&To=' + To + '&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&Audit_type=' + Audit_type + '&Is_Model=' + is_model + '&Model=' + Model_family + '&Family=' + null);
            }
            else {
                //Family wise
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fRPH_DAYWISE_AUDIT_TREND&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&From=' + From + '&To=' + To + '&Family=' + Model_family + '&Is_Model=' + is_model + '&Model=' + null + '&Audit_type=' + Audit_type);

            }
        }
        else if (chartType == 03) {
            //PPM DAY WISE
            if (is_model == true) {
                //Model wise
                $('#ContentFrame').attr('src', ' http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fPPM_DAYWISE_AUDIT_TREND&From=' + From + '&To=' + To + '&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&Audit_type=' + Audit_type + '&Is_Model=' + is_model + '&Model=' + Model_family + '&Family=' + null);
            }
            else {
                //Family wise
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fPPM_DAYWISE_AUDIT_TREND&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&From=' + From + '&To=' + To + '&Family=' + Model_family + '&Is_Model=' + is_model + '&Model=' + null + '&Audit_type=' + Audit_type);

            }
        }
        else if (chartType == 04) {
            //IQS Score
            if (is_model == true) {
                //Model wise
                $('#ContentFrame').attr('src', ' http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fIQS_DAYWISE_AUDIT_TREND&From=' + From + '&To=' + To + '&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&Audit_type=' + Audit_type + '&Is_Model=' + is_model + '&Model=' + Model_family + '&Family=' + null);
            }
            else {
                //Family wise
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fIQS_DAYWISE_AUDIT_TREND&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&From=' + From + '&To=' + To + '&Family=' + Model_family + '&Is_Model=' + is_model + '&Model=' + null + '&Audit_type=' + Audit_type);

            }
        }

    }

    //Monthwise
    if (Type == 04) {
        if (chartType == 01) {
            //LPH Month WISE
            if (is_model == true) {
                //Model wise
                $('#ContentFrame').attr('src', ' http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fLPH_AUDIT_MONTWISE&From=' + From_Month + '&To=' + To_Month + '&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&Audit_type=' + Audit_type + '&Is_Model=' + is_model + '&Model=' + Model_family + '&Family=' + null);
            }
            else {
                //Family wise
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fLPH_AUDIT_MONTWISE&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&From=' + From_Month + '&To=' + To_Month + '&Family=' + Model_family + '&Is_Model=' + is_model + '&Model=' + null + '&Audit_type=' + Audit_type);

            }
        }
        else if (chartType == 02) {
            //RPH Month WISE
            if (is_model == true) {
                //Model wise
                $('#ContentFrame').attr('src', ' http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fRPH_AUDIT_MONTWISE&From=' + From_Month + '&To=' + To_Month + '&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&Audit_type=' + Audit_type + '&Is_Model=' + is_model + '&Model=' + Model_family + '&Family=' + null);
            }
            else {
                //Family wise
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fRPH_AUDIT_MONTWISE&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&From=' + From_Month + '&To=' + To_Month + '&Family=' + Model_family + '&Is_Model=' + is_model + '&Model=' + null + '&Audit_type=' + Audit_type);

            }
        }
        else if (chartType == 03) {
            //PPM Month WISE
            if (is_model == true) {
                //Model wise
                $('#ContentFrame').attr('src', ' http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fPPM_AUDIT_MONTWISE&From=' + From_Month + '&To=' + To_Month + '&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&Audit_type=' + Audit_type + '&Is_Model=' + is_model + '&Model=' + Model_family + '&Family=' + null);
            }
            else {
                //Family wise
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fPPM_AUDIT_MONTWISE&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&From=' + From_Month + '&To=' + To_Month + '&Family=' + Model_family + '&Is_Model=' + is_model + '&Model=' + null + '&Audit_type=' + Audit_type);

            }
        }
        else if (chartType == 04) {
            //IQS Score WISE
            if (is_model == true) {
                //Model wise
                $('#ContentFrame').attr('src', ' http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fIQS_AUDIT_MONTWISE&From=' + From_Month + '&To=' + To_Month + '&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&Audit_type=' + Audit_type + '&Is_Model=' + is_model + '&Model=' + Model_family + '&Family=' + null);
            }
            else {
                //Family wise
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fIQS_AUDIT_MONTWISE&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&From=' + From_Month + '&To=' + To_Month + '&Family=' + Model_family + '&Is_Model=' + is_model + '&Model=' + null + '&Audit_type=' + Audit_type);

            }
        }
    }

    //Yearwise
    if (Type == 05) {
        if (chartType == 01) {
            //LPH DAY WISE
            if (is_model == true) {
                //Model wise
                $('#ContentFrame').attr('src', ' http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fLPH_AUDIT_YEARLY&yr=' + Year + '&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&Audit_type=' + Audit_type + '&Is_Model=' + is_model + '&Model=' + Model_family + '&Family=' + null);
            }
            else {
                //Family wise
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fLPH_AUDIT_YEARLY&yr=' + Year + '&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&Family=' + Model_family + '&Is_Model=' + is_model + '&Model=' + null + '&Audit_type=' + Audit_type);

            }
        }
        else if (chartType == 02) {
            //RPH DAY WISE
            if (is_model == true) {
                //Model wise
                $('#ContentFrame').attr('src', ' http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fRPH_AUDIT_YEARLY&yr=' + Year + '&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&Audit_type=' + Audit_type + '&Is_Model=' + is_model + '&Model=' + Model_family + '&Family=' + null);
            }
            else {
                //Family wise
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fRPH_AUDIT_YEARLY&yr=' + Year + '&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&Family=' + Model_family + '&Is_Model=' + is_model + '&Model=' + null + '&Audit_type=' + Audit_type);

            }

        }
        else if (chartType == 03) {
            //PPM DAY WISE
            if (is_model == true) {
                //Model wise
                $('#ContentFrame').attr('src', ' http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fPPM_AUDIT_YEARLY&yr=' + Year + '&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&Audit_type=' + Audit_type + '&Is_Model=' + is_model + '&Model=' + Model_family + '&Family=' + null);
            }
            else {
                //Family wise
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fPPM_AUDIT_YEARLY&yr=' + Year + '&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&Family=' + Model_family + '&Is_Model=' + is_model + '&Model=' + null + '&Audit_type=' + Audit_type);

            }

        }
        else if (chartType == 04) {
            //IQS Score
            if (is_model == true) {
                //Model wise
                $('#ContentFrame').attr('src', ' http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fIQS_AUDIT_YEARLY&yr=' + Year + '&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&Audit_type=' + Audit_type + '&Is_Model=' + is_model + '&Model=' + Model_family + '&Family=' + null);
            }
            else {
                //Family wise
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fIQS_AUDIT_YEARLY&yr=' + Year + '&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&Family=' + Model_family + '&Is_Model=' + is_model + '&Model=' + null + '&Audit_type=' + Audit_type);

            }
        }

    }
});


//Audit top issue PI chart
$("#Audit_top_issue_PI").click(function (e) {
    var shopId = parseInt($("#Shop_ID").val());
    var From = $("#From_Date").val();
    var To = $("#To_Date").val();
    var Audit = $("#Cat_ID").val();//Audit
    var Audit_name = $("#Audit_name").val();//Audit name
    var Audit_type = $("#Audit_type").val();//Audit name
    var Model_family = $("#Model_family").val();
    var is_model = $("#Is_Model").is(":checked");


    if (isNaN(shopId)) {
        alert("Please Select Shop Name.....!");
        return;
    }

    //if (isNaN(Audit) || Audit == "") {
    //    alert("Please Select Audit.....!");
    //    return;
    //}

    //if (isNaN(Audit_name) || Audit_name == "") {
    //    alert("Please Select Audit Name.....!");
    //    return;
    //}

    if (From == "" || From == " " || From == null || From == "undefined" || !isNaN(From)) {
        alert("Please Select From Date");
        return;
    }
    else if (To == "" || To == " " || To == null || To == "undefined" || !isNaN(To)) {
        alert("Please Select To Date");
        return;
    }
    if (Date.parse(From) > Date.parse(To)) {
        alert("From date should not be later than To Date");
        return;
    }
    //if (isNaN(Model_family) || Model_family == "") {
    //    alert("Please Select Model/Family.....!");
    //    return;
    //}



    if (is_model == true) {
        //Model wise
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fAudit_Top_Issuss_PI_Chart&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&From=' + From + '&To=' + To + '&Model=' + Model_family + '&Is_Model=' + is_model + '&Family=' + null + '&Audit_type=' + Audit_type);
    }
    else {
        //Family wise
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fAudit_Top_Issuss_PI_Chart&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&From=' + From + '&To=' + To + '&Family=' + Model_family + '&Is_Model=' + is_model + '&Model=' + null + '&Audit_type=' + Audit_type);

    }

});

//Audit top issue 
$("#Audit_top_issue").click(function (e) {
    var shopId = parseInt($("#Shop_ID").val());
    var From = $("#From_Date").val();
    var To = $("#To_Date").val();
    var Audit = $("#Cat_ID").val();//Audit
    var Audit_name = $("#Audit_name").val();//Audit name
    var Model_family = $("#Model_family").val();
    var is_model = $("#Is_Model").is(":checked");
    var Audit_type = $("#Audit_type").val();//Audit type

    var Is_model_val;

    if (isNaN(shopId)) {
        alert("Please Select Shop Name.....!");
        return;
    }

    if (isNaN(Audit) || Audit == "") {
        alert("Please Select Audit.....!");
        return;
    }

    if (isNaN(Audit_name) || Audit_name == "") {
        alert("Please Select Audit Name.....!");
        return;
    }

    if (From == "" || From == " " || From == null || From == "undefined" || !isNaN(From)) {
        alert("Please Select From Date");
        return;
    }
    else if (To == "" || To == " " || To == null || To == "undefined" || !isNaN(To)) {
        alert("Please Select To Date");
        return;
    }
    if (Date.parse(From) > Date.parse(To)) {
        alert("From date should not be later than To Date");
        return;
    }
    if (Audit != 1 && Model_family == "") {
        alert("Please Select Model/Family.....!");
        return;
    }


    //if (is_model == true)
    //{
    //    Is_model_val = 1;
    //}
    //else
    //{
    //    Is_model_val = 2;
    //}

    if (is_model == true) {
        //Model wise
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fAudit_top_issue&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&From=' + From + '&To=' + To + '&Model=' + Model_family + '&Is_Model=' + is_model + '&Family=' + '' + '&Audit_type=' + Audit_type);
    }
    else {
        //Family wise
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fAudit_top_issue&Shop=' + shopId + '&Audit=' + Audit + '&Audit_name=' + Audit_name + '&From=' + From + '&To=' + To + '&Family=' + Model_family + '&Is_Model=' + is_model + '&Model=' + '' + '&Audit_type=' + Audit_type);

    }

});


$("#MannualDataReport").click(function (e) {
    debugger;
    var Shop_ID = $("#Shop_ID").val();
    var Line_ID = $("#Line_ID").val();
    var From_Date = $("#From_Date").val();
    var To_Date = $("#To_Date").val();
    var Serial_Number = $("#Serial_Number").val();


    if (Shop_ID == 0) {
        alert("Please Select Shop");
        return;
    }
    else if (Line_ID == 0) {
        alert("Please Select Line");
        return;
    }
    else if (From_Date == "" || From_Date == " " || From_Date == null || From_Date == "undefined" || !isNaN(From_Date)) {
        alert("Please Select From Date");
        return;
    }
    else if (To_Date == "" || To_Date == " " || To_Date == null || To_Date == "undefined" || !isNaN(To_Date)) {
        alert("Please Select To Date");
        return;
    }
    if (Date.parse(From_Date) > Date.parse(To_Date)) {
        alert("From date should not be later than To Date");
        return;
    }

    if (Serial_Number != "") {
        var url = "/Report/IsValidSerialNumber";
        $.getJSON(url, { serial_number: Serial_Number }, function (data) {
            if (data == false) {
                alert("Invalid Serial Number");
            } else {
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fMannual_Data_Collection&Shop=' + Shop_ID + '&Line=' + Line_ID + '&From=' + From_Date + '&To=' + To_Date + '&SerialNumber=' + Serial_Number);
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            location.reload();
        });
    } else {
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fMannual_Data_Collection&Shop=' + Shop_ID + '&Line=' + Line_ID + '&From=' + From_Date + '&To=' + To_Date + '&SerialNumber=' + Serial_Number);
    }

});
$(".mannualData #Shop_ID").change(function () {
    var shopId = $(this).val();
    var url = "/Line/GetLineByShopID";
    $("#Line_ID").html("");
    $("#Line_ID").html("<option>Select Line</option>")
    $.getJSON(url, { shopId: shopId }, function (data) {
        if (data) {
            var res = "<option value=0>Select Line</option>";
            for (var i = 0; i < data.length; i++) {
                res += "<option value='" + data[i].Id + "'>" + data[i].Value + "</option>";
            }
            $("#Line_ID").html(res);
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        location.reload();
    });

});

$("#QualityCaptureLog").click(function (e) {

    var shopId = $("#Shop_ID").val();
    var Line_ID = $("#Line_ID").val();
    var Station_ID = $("#Station_ID").val();
    var Shift_ID = $("#Shift_ID").val();
    var SerialNumber = $("#SerialNumber").val();
    var ModelCode = $("#Model_Code").val();

    var frmDate = $("#From_Date").val();
    var toDate = $("#To_Date").val();


    if (shopId == 0) {
        alert("Please Select Shop");
        return;
    }

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

    if (SerialNumber != "") {
        var url = "/Report/IsValidSerialNumber";
        $.getJSON(url, { serial_number: SerialNumber }, function (data) {
            if (data == false) {
                alert("Invalid Serial Number");
            } else {
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fQuality_Capture_Log&FromDate=' + frmDate + '&ToDate=' + toDate + '&ShopID=' + shopId + '&LineID=' + Line_ID + '&StationID=' + Station_ID + '&ShiftID=' + Shift_ID + '&SerialNumber=' + SerialNumber + '&ModelCode=' + ModelCode);
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            location.reload();
        });
    }
    else {
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fQuality_Capture_Log&FromDate=' + frmDate + '&ToDate=' + toDate + '&ShopID=' + shopId + '&LineID=' + Line_ID + '&StationID=' + Station_ID + '&ShiftID=' + Shift_ID + '&SerialNumber=' + SerialNumber + '&ModelCode=' + ModelCode);

    }

});
