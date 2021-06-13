
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

    $(".Shopname").hide("slow");
    $(".Monthname").hide("slow");
    $(".Yearname").hide("slow");
    $(".Partnumber").hide("slow");
    $(".MonthnamePS").hide("slow");
    $(".YearnamePS").hide("slow");
    $(".manager_Line #Category").change(function (e) {


        if ($(this).val() == "01") {
            $(".Shopname").show("slow");
            $(".Monthname").show("slow");
            $(".Yearname").show("slow");
            $(".Partnumber").hide("slow");
            $(".MonthnamePS").hide("slow");
            $(".YearnamePS").hide("slow");

        }
        else if ($(this).val() == "02") {
            $(".Shopname").hide("slow");
            $(".Monthname").hide("slow");
            $(".Yearname").hide("slow");

            $(".Partnumber").show("slow");
            $(".MonthnamePS").show("slow");
            $(".YearnamePS").show("slow");
            //$(this).val(true);
        }
        else {
            // alert("hi");
            //$(this).val("hide");
            $(".Shopname").hide("slow");
            $(".Monthname").hide("slow");
            $(".Yearname").hide("slow");
            $(".Partnumber").hide("slow");
            $(".MonthnamePS").hide("slow");
            $(".YearnamePS").hide("slow");
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
        var lineId = $("#Line_ID").val();
        var stationId = $("#Station_ID").val();
        var mnth = $("#From_Date").val();
        var yr = $("#To_Date").val();
        //alert(shopId);
        //alert(lineId);
        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }
        else if (lineId == "0") {
            alert("Please Select Line Name");
            return;
        }
        else if (stationId == "") {
            alert("Please Select Station Name");
            return;
        }
        else if (mnth == 0) {
            alert("Please Select Month");
            return;
        }
        else if (yr == 0) {
            alert("Please Select Year");
            return;
        }

        // alert(orderStateId);
        // alert(ModelId);
        // alert(seriesId);
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fPRODUCTION_MONTHLY_PLAN&Mnth=' + mnth + '&Yr=' + yr + '&Shop=' + shopId + '&Line=' + $("#Line_ID").val());

        // var url = "/Report/GenerateReportProductionOrderSummary";
        // ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "&lineId=" + $("#Line_ID").val() + "&stationId=" + $("#Station_ID").val() + "&orderStateId=" + $("#Order_State").val() + "&frmDate=" + $("#From_Date").val() + "&toDate=" + $("#To_Date").val() + "", showShopLineType, "json"); //+ "&ModelId=" + $("#Model").val() + "&seriesId=" + $("#Series").val()

    });

    $("#ProductionOrderSumarry").click(function (e) {


        var shopId = parseInt($("#Shop_ID").val());
        var stationId = $("#Station_ID").val();
        var OrderState = $("#Order_State option:selected").text();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        var Shift = $("#Shift_ID").val();
        var Line = $("#Line_ID").val();

        if (Line == "") {
            Line = 0;
        }

        if (Shift == "") {
            Shift = 0;
        }


        if (isNaN(shopId)) {
            alert("Please Select Shop Name");
            return;
        }


        if (frmDate == "" || frmDate == " " || frmDate == null || frmDate == "undefined" || !isNaN(frmDate)) {
            alert("Please Select From Date");
            return;
        }
        else if (toDate == "" || toDate == " " || toDate == null || toDate == "undefined" || !isNaN(toDate)) {
            alert("Please Select To Date");
            return;
        }
        if (Date.parse(frmDate) > Date.parse(toDate)) {
            alert("From date should not be later than To Date");
            return;
        }


        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fPRODUCTION_ORDER_SUMMARY_ALL_SHOP&Shop=' + shopId + '&From=' + frmDate + '&To=' + toDate + "&Shift=" + Shift + "&Line=" + Line);

    });

    $("#PlanVSActualDashboard").click(function (e) {

        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fPlanVsAtual_Dashboard');


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
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fProd_Planned_Actual_Daily&From=' + frmDate1 + '&To=' + frmDate + '&Shop=' + shopId);
        }
        else {
            // alert('r2');
            //alert(toDate +' '+ Year + ' ' + shopId)
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fProd_Planned_Actual_Monthly&Mnth=' + toDate + '&Yr=' + Year + '&Shop=' + shopId);
        }




        // var search = window.location.search;
        //$("#myIframe").attr("src", $("#myIframe").attr("src") + search);

        //var url = "/Report/GenerateReportMonthlyProductionPlan";
        //ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "&lineId=" + $("#Line_ID").val() + "&stationId=" + $("#Station_ID").val() + "&frmDate=" + $("#From_Date").val() + "&toDate=" + $("#To_Date").val() + "", showShopLineType, "json");


    });


    $("#DailyOrderStartCount").click(function (e) {
        // alert("ProductionOrderSumarry");
        var shopId = $("#Shop_ID").val();
        var frmDate = $("#From_Date").val();


        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }
        else if (frmDate == 0) {
            alert("Please Select Date");
            return;
        }
        if (frmDate) {
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fProd_DailyOrderStartCount&Shop=' + shopId + '&Dt=' + frmDate);
        }
    });

    $("#SOPStatus").click(function (e) {
        var ModelCode = $("#ModelCode").val();        
        var ShopID = $("#Shop_ID ").val();
        if (ShopID == "") {
            alert("Please Select Shop");
            return;
        }

        if (ModelCode == "") {
            alert("Please Select Model Code");
            return;
        }
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fSOP_Report&Model_Code=' + ModelCode + "&Shop=" + $("#Shop_ID").val() + "&Line_ID=" + $("#Line_ID").val() + "&Station_ID=" + $("#Station_ID").val());
    });
    $("#EWT").click(function (e) {

        var shopId = $("#Shop_ID").val();
        var toDate = $("#To_Date").val();
        //var Year = $("#Year option:selected").text();

        if (shopId == "") {
            alert("Please Select Shop");
            return;
        }
        if (toDate == "") {
            alert("Please Select Date");
            return;
        }
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fEWT&From=' + toDate + '&ShopID=' + shopId + '&LineID=' + $("#Line_ID").val() + '&ShiftID=' + $("#Shift_ID").val());

    });

    $("#DailyUses").click(function (e) {

        var shopId = $("#Shop_ID").val();
        var frmDate = $("#From_Date :text").val();
        var toDate = $("#To_Date :text").val();
        var toType = $("#To_Type").val();
        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }
        if (frmDate == "") {
            alert("Please Select From Date");
            return;
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
                alert("Please Select Date");
                return;
            }

        }
        if (toType == 01) {
            if (toDate == "") {
                alert("Please Select To Date");
                return;
            }
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fDaily_Usage_report_DayWise&dt=' + toDate + '&Shop=' + shopId);
        }
        else {
            if (frmDate == "") {
                alert("Please Select From Date");
                return;
            }
            else if (toDate == "") {
                alert("Please Select To Date");
                return;
            }
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fDailyUses_Report&dt1=' + frmDate + '&dt2=' + toDate + '&Shop=' + shopId);

        }



    });

    $("#HelpDesk").click(function (e) {

        //var shopId = parseInt($("#Shop_ID").val());
        var toDate = $("#To_Date :text").val();
        var fromdate = $("#From_Date :text").val();

        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fHelpDeskReport&dt1=' + toDate + '&dt=' + fromdate);

    });

    $("#HelpDeskTime").click(function (e) {

        var fromDate = $("#From_Date :text").val();
        var toDate = $("#To_Date :text").val();

        if (fromDate == "") {
            alert("Please Select From Date");
            return;
        }
        else if (toDate == "") {
            alert("Please Select To Date");
            return;
        }

        if (Date.parse(fromDate) > Date.parse(toDate)) {
            alert("From date should not be later than To Date");
            return;
        }

        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fHelpDesk_By_Time&dt1=' + fromDate + '&dt2=' + toDate);

    });

    $("#HelpDeskOccurance").click(function (e) {

        var fromDate = $("#From_Date :text").val();
        var toDate = $("#To_Date :text").val();

        if (fromDate == "") {
            alert("Please Select From Date");
            return;
        }
        else if (toDate == "") {
            alert("Please Select To Date");
            return;
        }

        if (Date.parse(fromDate) > Date.parse(toDate)) {
            alert("From date should not be later than To Date");
            return;
        }

        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fHelp_Issue_Details_Occurence&dt1=' + fromDate + '&dt2=' + toDate);

    });

    $("#WIPReport").click(function (e) {

        var shopId = $("#Shop_ID").val();
        var frmDate = $("#From_Date").val();
        //alert(shopId);
        //alert(frmDate);
        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }
        //else if (frmDate == 0) {
        //    alert("Please Select Date");
        //}
        
    });

    $("#ShopProductionReport").click(function (e) {

        var shopId = $("#Shop_ID").val();
        //var frmDate = $("#From_Date").val();
        //alert(shopId);
        //alert(frmDate);
        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }
        //else if (frmDate == 0) {
        //    alert("Please Select Date");
        //}
 //$('#ContentFrame').attr('src', 'http://MMZAHMESPDSDB1/ReportServer/Pages/ReportViewer.aspx?%2fFDReports%2fWIP_Tractor&Shop=' + shopId);
        // $('#ContentFrame').attr('src', 'http://MMZAHMESPDSDB1/ReportServer/Pages/ReportViewer.aspx?%2fFDReports%2fEngine_WIP_Report');
    });

  $("#TBMReport").click(function (e) {
        // alert('Hi');
        var shopId = $("#Shop_ID").val();
        //var lineId = $("#Line_ID").val();
        var frmDate = $("#From_Date :text").val();
        var toDate = $("#To_Date :text").val();
        var eqpID = $("#EQP_ID").val();
        var machineID = $("#Machine_ID").val();
        // alert(machineID);
        //alert(lineId);
        //  alert(frmDate);
        //  alert(toDate);

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
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTBMReport&dt1=' + frmDate + '&dt2=' + toDate + '&MachineID=' + machineID + '&EQPID=' + eqpID + '&Shop=' + shopId);

    });

    $("#ProductionStatus").click(function (e) {
        // alert("ProductionOrderSumarry");
        var category = $("#Category").val();

        var shopId = $("#Shop_ID").val();
        var mnt = $("#From_Date").val();
        var yr = $("#To_Date option:selected").text();

        var partnumber = $("#PartNumber").val();
        var mntps = $("#From_DatePS").val();
        var yrps = $("#To_DatePS option:selected").text();

        //var Year = $("#Year option:selected").text();

        if (category == 0) {
            alert("Please Select Category");
            return;
        }

        //alert(mnt);
        //alert(yr);
        if (category == 01) {
            if (shopId == 0) {
                alert("Please Select Shop Name");
                return;
            }
            else if (mnt == 0) {
                alert("Please Select Month");
                return;
            }
            else if ($("#To_Date option:selected").val() == 0) {
                alert("Please Select Year");
                return;
            }
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fweekly_production_Status&Shop=' + shopId + '&Mnth=' + mnt + '&Yr=' + yr);

        }
        if (category == 02) {
            if (partnumber == "") {
                alert("Please Select Part Number");
                return;
            }
            else if (mntps == 0) {
                alert("Please Select Month");
                return;
            }
            else if ($("#To_DatePS option:selected").val() == 0) {
                alert("Please Select Year");
                return;
            }
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fWeekly_Production_Status_Part_No_Wise&Mnth=' + mntps + '&Yr=' + yrps + '&PartNo=' + partnumber);

        }

        //alert();


    });

    $("#PlanProductionDashboard").click(function (e) {


        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fProduction_Dashboard_Daily');


    });

    $("#ActualTrend").click(function (e) {
        //alert('pvsa')
        var shopId = $("#Shop_ID").val();
        var Chart = $("#Chart_Type").val();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        var toType = $("#To_Type").val();
        var Year = $("#Year").val();
        var LineID = $("#Line_ID").val();
        if (shopId == "") {
            alert("Please Select Shop");
            return;
        }
        if (shopId == "") {
            alert("Please Select Line");
            return;
        }
        else if (Chart == "") {
            alert("Please Select Chart Type");
            return;
        }
        else if (toType == "") {
            alert("Please Select To Type");
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

        if (Date.parse(frmDate) > Date.parse(toDate)) {
            alert("From date should not be later than To Date");
            return;
        }

        if (toType == 02) {
            if (Year == "") {
                alert("Please Select Year");
                return;
            }
            if (Chart == 01) {
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fActual_Trend_Column_Chart&ShopID=' + shopId + '&Year=' + Year + '&LineID=' + $("#Line_ID").val());
            }
            else {
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fActual_Trend_Monthly_Shop_Wise&ShopID=' + shopId + '&Year=' + Year + '&LineID=' + $("#Line_ID").val());
            }
        }
        else if (toType == 01) {
            if (Chart == 01) {
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fActaul_Trend_By_Shop_Column&From=' + frmDate + '&To=' + toDate + '&ShopID=' + shopId + '&LineID=' + $("#Line_ID").val());
            }
            else {
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fActaul_Trend_By_Shop&From=' + frmDate + '&To=' + toDate + '&ShopID=' + shopId + '&LineID=' + $("#Line_ID").val());
            }
        }

    });
    $("#HistoryCard").click(function (e) {
        //alert('pvsa')
        var SerialValue = "";
        var SerialNo = $("#Serial_No").val();
        var txt_SerialNo = $("#txt_Serial_No").val();
        //var toDate = $("#To_Date").val();
        if (SerialNo == "" && txt_SerialNo == "") {
            alert("Please Select Serial No");
            return;
        }
        else {
            if (SerialNo != "") {
                SerialValue = SerialNo;
            }
            else if (txt_SerialNo != "") {
                SerialValue = txt_SerialNo;
            }
        }

        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTractor_History_Card&SerialNo=' + SerialValue);


    });
    //Add Engine History Card
    //Author: Ashok
    $("#Engine_HistoryCard").click(function (e) {
        //alert('pvsa')
        var SerialValue = "";
        var SerialNo = $("#Serial_No").val();
        var txt_SerialNo = $("#txt_Serial_No").val();
        //var toDate = $("#To_Date").val();
        if (SerialNo == "" && txt_SerialNo == "") {
            alert("Please Select Serial No");
            return;
        }
        else {
            if (SerialNo != "") {
                SerialValue = SerialNo;
            }
            else if (txt_SerialNo != "") {
                SerialValue = txt_SerialNo;
            }
        }
        var url = "/Report/IsValidSerialNumber";
        $.getJSON(url, { serial_number: SerialValue }, function (data) {
            if (data == false) {
                alert("Invalid Serial Number");
            } else {
                $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fHistoryCardForEngine&SerialNo=' + SerialValue);
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            location.reload();
        });
    });

    $("#Jbar_HistoryCard").click(function (e) {
        //alert('pvsa')
        var SerialValue = "";
        var SerialNo = $("#Serial_No").val();
        var txt_SerialNo = $("#txt_Serial_No").val();
        //var toDate = $("#To_Date").val();
        if (SerialNo == "" && txt_SerialNo == "") {
            alert("Please Select Serial No");
            return;
        }
        else {
            if (SerialNo != "") {
                SerialValue = SerialNo;
            }
            else if (txt_SerialNo != "") {
                SerialValue = txt_SerialNo;
            }
        }
        var url = "/Report/IsValidSerialNumber";
        $.getJSON(url, { serial_number: SerialValue }, function (data) {
            if (data == false) {
                alert("Invalid Serial Number");
            } else {
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fHistoryCardForJbar&SerialNo=' + SerialValue);
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            location.reload();
        });



    });

    $("#Machining_HistoryCard").click(function (e) {        
        var SerialValue = "";
        var SerialNo = $("#Serial_No").val();
        var lineId = $("#Line_ID").val();       
        var txt_SerialNo = $("#txt_Serial_No").val();
        
        if (lineId == "" || lineId == null) {
            alert("Please Select line");
            return;
        } else
            if (SerialNo == "" && txt_SerialNo == "") {
                alert("Please Select Serial No");
                return;
            } else {
                if (SerialNo != "") {
                    SerialValue = SerialNo;
                }
                else if (txt_SerialNo != "") {
                    SerialValue = txt_SerialNo;
                }
            }
        var url = "/Report/IsValidSerialNumber";
        $.getJSON(url, { serial_number: SerialValue }, function (data) {
            if (data == false) {
                alert("Invalid Serial Number");
            } else {
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fHistoryCardForMachining&SerialNo=' + SerialValue + '&Line=' + lineId);
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            location.reload();
        });



    });

    //sahitya
    $("#CutOffReport").click(function (e) {
        // alert('Hi');
        var shopId = $("#Shop_ID").val();
        //  var Type = $("#To_Type").val();
        //  var Chart = $("#Chart_Type").val();
        var frmDate = $("#Inserted_Date").val();
        // var toDate = $("#To_Date").val();
        //  var Year = $("#Year option:selected").text()
        // var Family = $("#Family option:selected").val()
        var ShiftID = $("#Shift_ID").val();
        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }
        if (ShiftID == "") {
            alert("Please Select Shift ");
            return;
        }
        if (frmDate == "") {
            alert("Please Select Date ");
            return;
        }

        else {
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fSerial_No_CutOff_Report&ShopName=' + shopId + '&dt=' + frmDate + '&Shift=' + ShiftID);
        }



    });

    $("#DPRReport").click(function (e) {
        // alert('Hi');
        var shopId = $("#Shop_ID").val();
        var Type = $("#To_Type").val();
        var Chart = $("#Chart_Type").val();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        var Year = $("#Year option:selected").text()
        var Family = $("#Family option:selected").val()
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
        else if (Family == "") {
            alert("Please Select Family");
            return;
        }
        if (Type == "01") {
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTractor_DPR_Report&dt1=' + frmDate + '&dt2=' + toDate + '&ChartType=' + Chart + '&Family=' + Family + '&Shop=' + shopId);

        }
        else {
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTractor_DPR_MonthWise&yr=' + Year + '&ChartType=' + Chart + '&Family=' + Family + '&Shop=' + shopId);
        }



    });

     $("#FTRReport").click(function (e) {
        // alert('Hi');
        var shopId = $("#Shop_ID").val();
        var Type = $("#To_Type").val();
        var Chart = $("#Chart_Type").val();
        var frmDate = $("#From_Date").val();
        var Shift = $("#Shift_ID").val();
        var toDate = $("#To_Date").val();
        var Year = $("#Year option:selected").text()
        if (shopId == "") {
            alert("Please Select Shop Name");
            return;
        }
        else if (Type == 0) {

            alert("Please Select Type");
            return;

        }
        else if (Chart == 0) {

            alert("Please Select Chart");
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

        if (Date.parse(frmDate) > Date.parse(toDate)) {
            alert("From date should not be later than To Date");
            return;
        }
        if (Type == "01") {
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTractor_Clean_Traveller_Card_Report&Shop=' + shopId + '&dt1=' + frmDate + '&dt2=' + toDate + '&ChartType=' + Chart  + '&Shift=' + Shift);

        }
        else {
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTractor_Clean_TravellerCard_DayWise&Shop=' + shopId + '&yr=' + Year + '&ChartType=' + Chart  + '&Shift=' + Shift);
        }



    });


    //Add Transmission History Card
    //Author: Ashok
    $("#Transmission_HistoryCard").click(function (e) {
        // alert('pvsa')
        var SerialValue = "";
        var SerialNo = $("#Serial_No").val();
        var txt_SerialNo = $("#txt_Serial_No").val();
        //var toDate = $("#To_Date").val();
        if (SerialNo == "" && txt_SerialNo == "") {
            alert("Please Select Serial No");
            return;
        }
        else {
            if (SerialNo != "") {
                SerialValue = SerialNo;
            }
            else if (txt_SerialNo != "") {
                SerialValue = txt_SerialNo;
            }
        }
        var url = "/Report/IsValidSerialNumber";

        $.getJSON(url, { serial_number: SerialValue }, function (data) {
            if (data == false) {
                alert("Invalid Serial Number");
            } else {
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fHistoryCardForTransmission&SerialNo=' + SerialValue);
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            location.reload();
        });



    });
    //Add Transmission History Card
    //Author: Ashok
    $("#Hydraulic_HistoryCard").click(function (e) {
        // alert('pvsa')
        var SerialValue = "";
        var SerialNo = $("#Serial_No").val();
        var txt_SerialNo = $("#txt_Serial_No").val();
        //var toDate = $("#To_Date").val();
        if (SerialNo == "" && txt_SerialNo == "") {
            alert("Please Select Serial No");
            return;
        }
        else {
            if (SerialNo != "") {
                SerialValue = SerialNo;
            }
            else if (txt_SerialNo != "") {
                SerialValue = txt_SerialNo;
            }
        }

        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fHistoryForHydraulic&SerialNo=' + SerialValue+"&Line=0");
    });

    $("#btnHide").click(function (e) {
        // alert("hiii");
        // $("div1").hide();
        document.getElementById("div1").style.visibility = "hidden";

    });

    $("#TrackingStatus").click(function (e) {
        //alert('pvsa')
        var shopId = parseInt($("#Shop_ID").val());
        if (shopId == null) {
            alert("Please Select Shop");
        }
        if (shopId == 1 || shopId == 2 || shopId == 3) {
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fEngine_Tracking_Status&Shop=' + shopId);
        }
        else if (shopId == 4) {
            $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fTractor_Tracking_Status');
        }

    });

    //$("#CentralPPC").click(function (e) {
    //    // var shopId = parseInt($("#Shop_ID").val());

    //    $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fCentral_PPC_Report');

    //});


    $("#PPCWIPREPORT").click(function (e) {
        //alert('pvsa')
        var shopId = parseInt($("#Shop_ID").val());
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fWIP_Tractor&Shop=' + shopId);

    });

    $("#DeletedOrders").click(function (e) {
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        var shop = $("#Shop_ID").val();
        if (shop == "") {
            alert("Please Select Shop name");
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
            alert(" From date should not be later than To Date");
            return;
        }
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fDeleted_Orders_Summary&From=' + frmDate + '&To=' + toDate + '&Shop=' + shop + '&Line=' + $("#Line_ID").val());

    });


    $("#linestopreasons").click(function (e) {

        var shopId = $("#Shop_ID").val();
        var frmDate = $("#From_Date").val();
        var toDate = $("#To_Date").val();
        $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fLineStopReasons&from=' + frmDate + '&to=' + toDate + '&shop=' + shopId);
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


//Add Equipment Creation Status
//Author: Jitendra Mahajan
$("#SAP_Equipment_Creation").click(function (e) {

    var shopId = $("#Shop_ID").val();
    var shift = $("#Shift").val();
    var stationId = $("#Station_ID").val();
    var OrderState = $("#Order_State option:selected").text();
    var frmDate = $("#From_Date").val();
    var toDate = $("#To_Date").val();
    var status = $("#Status").val();
    var LineID = $("#Line_ID").val();



    if (shopId == "") {
        alert("Please select shop");
        return;
    }

    if (LineID == "0") {
        alert("Please select Line");
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

    if (Date.parse(frmDate) > Date.parse(toDate)) {
        alert("From date should not be later than To Date");
        return;
    }

    $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fSAP_EquipmentCreation_Status&shop=' + shopId + '&Data_Status=' + status + '&fromdate=' + frmDate + '&todate=' + toDate+'&Line='+LineID);

    // $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fSAP_EquipmentCreation_Status&fromdate=' + frmDate + '&todate=' + toDate);

});


//Add Paint Shop Summary
//Author: Jitendra Mahajan
$("#PaintShop_Summary").click(function (e) {

    var shopId = parseInt($("#Shop_ID").val());
    var stationId = $("#Station_ID").val();
    var OrderState = $("#Order_State option:selected").text();
    var frmDate = $("#From_Date").val();
    var toDate = $("#To_Date").val();


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


    $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fPNT_Summary_Details&from=' + frmDate + '&to=' + toDate);

});


//Add Colorwise Summary
//Author: Jitendra Mahajan
$("#Colorwise_Summary").click(function (e) {

    var shopId = parseInt($("#Shop_ID").val());
    var stationId = $("#Station_ID").val();
    var OrderState = $("#Order_State option:selected").text();
    var frmDate = $("#From_Date").val();
    var toDate = $("#To_Date").val();


    if (frmDate == "") {
        alert("Please Select From Date");
        return;
    }
    else if (toDate == "") {
        alert("Please Select To Date");
        return;
    }

    if (Date.parse(frmDate) >Date.parse(toDate)) {
        alert("From date should not be later than To Date");
        return;
    }

    $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fPNT_Colourwise_Summary&from=' + frmDate + '&to=' + toDate);

});


//Add Paint Shop Details Summary
//Author: Jitendra Mahajan
$("#PaintShop_Details_Summary").click(function (e) {

    var shopId = parseInt($("#Shop_ID").val());
    var stationId = $("#Station_ID").val();
    var OrderState = $("#Order_State option:selected").text();
    var frmDate = $("#From_Date").val();
    var toDate = $("#To_Date").val();


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

    $('#ContentFrame').attr('src', 'http://mmngpdrnrep1/ReportServer/Pages/ReportViewer.aspx?%2fNGP_Report%2fPNT_Shop_Details&from=' + frmDate + '&to=' + toDate);

});
