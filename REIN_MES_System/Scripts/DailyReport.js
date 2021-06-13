


$(document).ready(function (e) {


    $("#btnTransferEmpSlide").click(function () {
        debugger
        var ShopID = $("#Shop_ID").val();
        var LineId = $("#Line_ID").val();
        var SetupId = $("#Setup_ID").val();
        var ShiftId = $("#Shift1").val();
        debugger
        $("#divListForEmployee").empty();
        $("#modalTransferEmployee").modal("show");

        setTimeout(function () {
            debugger
            $.ajax({
                type: 'POST',
                url: "/DailyReportForSupervisor/ExportAbsEmployee", // we are calling json method
                dataType: 'json',
                data: { Shopid: ShopID, LineId: LineId },
                success: function (data) {
                    debugger
                    $("#New_Line_ID").empty();
                    $("#New_Line_ID").append('<option value="-1">Select New Line</option>');

                    $.each(data, function (i, data) {
                        $("#New_Line_ID").append('<option value="' + data.Id + '">' + data.Value + '</option>');

                    });
                }
            })
        }, 1000);

        //else {
        //    $("#Modal_TransFer_Employee").modal("hide");
        //}
    })
    $("#New_Line_ID").change(function () {

        var ShopID = $("#Shop_ID").val();
        var LineId = $("#Line_ID").val();
        var setupId = $("#Setup_ID").val();
        var ShiftId = $("#Shift1").val();
        var nLineId = $("#New_Line_ID").val();
        //$("#New_Line_ID").empty();
        url = "/DailyReportForSupervisor/GetAbsOperatorByShiftID";
        ajaxpack.getAjaxRequest(url, "ShiftId=" + ShiftId + "" + "&ShopId=" + ShopID + "" + "&LineId=" + LineId + "&setupId=" + setupId, showSupervisorsListType, "json");

    })
    function showSupervisorsListType() {
        debugger
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                debugger
                var res = "";
                res += ' <table width="50%" class="emptable">';
                for (var i = 0; i < jsonRes.length; i++) {

                    res += '';
                    res += '  <tr class="supervisor_' + jsonRes[i].Id + '">';
                    res += '    <td width="10px">';
                    res += '         <input type="checkbox" id="@checkBoxId" class="chkclass" value="' + jsonRes[i].Id + '" />';
                    res += '     </td>';
                    res += '      <td id="@tdId" width="100px">';
                    res += '           ' + jsonRes[i].Value;
                    res += '       </td>';
                    res += '    </tr>';
                }
                res += ' </table>';







                $("#divListForEmployee").html(res);
            }
        }
    }
    $("#btnSave1").click(function () {
        debugger
        var ShopID = $("#Shop_ID").val();
        var LineId = $("#Line_ID").val();
        var SetupId = $("#Setup_ID").val();
        var ShiftId = $("#Shift1").val();
        var nLineId = $("#New_Line_ID").val();

        var Stations = [];
        var arrlist = [];
        //$("#divListForEmployee .chkclass").each(function (e) {
        $('.chkclass:checkbox:checked').each(function () {
            arrlist.push($(this).val());

        });

        var url = "/DailyReportForSupervisor/SaveTranserAbsOperators";
        ajaxpack.getAjaxRequest(url, "Operators=" + arrlist + "&shiftId=" + ShiftId + "&shopId=" + ShopID + "&LineId=" + LineId + "&nLineId=" + nLineId, SaveAssignedStation, "json");



    })




    function SaveAssignedStation() {
        debugger
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                //alert("Assigned Supervisor Saved Successfully...!");

                $('#testy').toastee({
                    type: 'success',
                    width: '300px',
                    height: '100px',
                    message: 'Transfer Operators Successfully...',
                });

                $("#divselectedSupervisors").html("");
                $("#divListofSupervisor").html("");
                //clear();               

                setTimeout(function () {
                    location.reload();
                }, 2000);

                //SelectOptionHTML(jsonRes, "Line_ID");
            }
            else {
                $('#testy').toastee({
                    type: 'error',
                    width: '200px',
                    height: '100px',
                    message: 'Some issue occured...',
                });
            }
        }
    }

    $("#btnclosetrans").click(function (e) {
        debugger
        //e.preventDefault();
        //$("#BtnTransferEmp").click();
        //$(".btnTransferEmpSlide1").css("checked: false");

        $(".btnTransferEmpSlide1").parent().find('input:checkbox').attr('checked', false);


    })
    //$(function () {
    //    debugger




    $("#btnExcelFormat").click(function () {
        var ShopId = $("#Shop_ID").val();
    })
    var btn;
    $("#Shift1 option:first").val("0");
    $('.station_operator').draggable('disable');
    //$(document).on("click", ".box-tools", function () {
    $("#operator_present_list").empty();
    var stationis = $("input[name='select_station']:checked").val();

    //$('input[select_station]:checked').val();
    //$(this).val();

    //if (stationis) {

    //var url = "/DailyReportForSupervisor/GetPresentSkilledEmployeeAgainstSelectedStation";
    //ajaxpack.getAjaxRequest(url, "stationis=" + stationis + "&ShiftID="+ $("#Shift1").val() , showPrescentEmployee, "json");

    //setTimeout(function () {
    //alert(stationis);
    //if (shopId > 0 && shiftId > 0 && lineId > 0) {
    //url = "/DailyReportForSupervisor/GetAbscentEmployeeUnderLogedinSupervisor";
    //ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "&ShiftID=" + $("#Shift1").val() + "&Line_ID=" + $("#Line_ID").val(), showAbscentEmployee, "json");
    //}, 5000);

    //setTimeout(function () {
    //alert(stationis);
    //url = "/DailyReportForSupervisor/GetAbsCoverageEmployeeUnderLogedinSupervisor";
    //  ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "&ShiftID=" + $("#Shift1").val() + "&Line_ID=" + $("#Line_ID").val(), showAbsCoverageEmployee, "json");
    //}, 5000);
    // }
    //}

    //});

    //alert("ok");

    //presentOperatorDraggable();
    //operatorDroppable();

    //$(".route_configuration #Shop_ID").change(function (e) {
    //    $("#operator_present_list").empty();
    //    $("#operator_abscent_list").empty();
    //    //clearSelectBox("Line_ID");
    //    $("#route_station_list").empty();
    //    var shopId = $("#Shop_ID").val();
    //    $('.oShopID').html(null);
    //    if (shopId) {

    //        $("#Shift1").val($("#Shift1 option:first").val());
    //        var url = "/DailyReportForSupervisor/GetLineByShopID";
    //        ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showLineShopID, "json");

    //        setTimeout(function () {
    //            url = "/DailyReportForSupervisor/GetCurrentShift";
    //            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShiftName, "json");
    //        }, 1000);


    //    }
    //    else {
    //        // clear the line type and shop
    //        clearSelectBox("Line_ID");
    //        //$("#route_station_list").empty();
    //        //$("#Shift1").val($("#Shift1 option:first").val());
    //    }
    //});

    $("#Shop_ID").change(function () {
        $("#operator_abscent_list").empty();
        $("#route_station_list").empty();
        $("#extra_operator_list").empty();
        $("#abscent_operator_station_list").empty();
        if ($("#Shop_ID").val() != "") {

            $('.oShopID').html(null);
            var options = {};
            options.url = "/dailyreportforsupervisor/getlinebyshopid";
            options.type = "POST";
            options.data = JSON.stringify({ shopId: $("#Shop_ID").val() });
            options.dataType = "json";
            options.contentType = "application/json";
            options.success = function (test) {
                $("#Line_ID").empty();

                $("#Line_ID").append($('<option>', { value: 0, text: 'Select' }));
                $.each(test, function (i, data) {
                    $("#Line_ID").append(
                        $('<option></option>').val(data.Line_ID).html(data.Line_Name));
                });
                $("#Line_ID").prop("disabled", false);
            };

            options.error = function () { };
            $.ajax(options);

            setTimeout(function () {
                var options = {};
                options.url = "/dailyreportforsupervisor/GetCurrentShift";
                options.type = "POST";
                options.data = JSON.stringify({ shopId: $("#Shop_ID").val() });
                options.dataType = "json";
                options.contentType = "application/json";
                options.success = function (test) {
                    $("#Shift1").empty();
                    $("#Shift1").append($('<option>', { value: 0, text: 'Select' }));
                    $.each(test, function (i, data) {
                        $("#Shift1").append(
                            $('<option></option>').val(data.Shift_ID).html(data.Shift_Name));
                    });
                    $("#Shift1").prop("disabled", false);
                };

                options.error = function () { };
                $.ajax(options);
            }, 2000)
        }
        else {
            $("#Line_ID").empty();
            $("#Shift1").empty();
            $("#Setup_ID").empty();
            $("#operator_abscent_list").empty();
            $("#route_station_list").empty();
            $("#extra_operator_list").empty();
        }

    });


    $("#Setup_ID").change(function (e) {
        if ($("#Setup_ID").val() > 0) {
            $('.oSetupID').html(null);
            var td = new Date().getDay();
            td = (td == 0) ? 7 : td;
            $('select[name=FromDay]').find('option').eq(td).prop('selected', true).end().change();
            $('select[name=Day]').find('option').eq(td).prop('selected', true).end().change();
        }
        else {
            $('.oSetupID').html('Please Select Setup');
        }
        $("#Shift1").val($("#Shift1 option:first").val());
        $("#operator_abscent_list").empty();
        $("#route_station_list").empty();
        $("#extra_operator_list").empty();
        $("#abscent_operator_station_list").empty();
    });

    $("#Line_ID").change(function (e) {
        $("#operator_present_list").empty();
        $("#operator_abscent_list").empty();
        $("#route_station_list").empty();
        $("#extra_operator_list").empty();
        $("#abscent_operator_station_list").empty();
        var lineId = $("#Line_ID").val();
        var shopId = $("#Shop_ID").val();
        // alert(shopId);


        if (lineId) {
            $('.olineID').html(null);
            var options = {};
            options.url = "/dailyreportforsupervisor/Getsetupbylineid";
            options.type = "POST";
            options.data = JSON.stringify({ lineId: $("#Line_ID").val() });
            options.dataType = "json";
            options.contentType = "application/json";
            options.success = function (test) {
                $("#Setup_ID").empty();

                $("#Setup_ID").append($('<option>', { value: 0, text: 'Select' }));
                $.each(test, function (i, data) {
                    $("#Setup_ID").append(
                        $('<option></option>').val(data.Id).html(data.Value));
                });
                $("#Setup_ID").prop("disabled", false);
            };

            options.error = function () { };
            $.ajax(options);

            //spinner();  
            //var url = "/DailyReportForSupervisor/Getsetupbylineid";
            //ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showSetupLineID, "json");
            // $("#Shift1").val("0");
            //$("#Shift1").val('1');   

            // $('.olineID').html(null);
            // $("#Shift1").val($("#Shift1 option:first").val());
        }
        else {
            $("#route_station_list").empty();
            $("#Shift1").val($("#Shift1 option:first").val());
        }

    });

    function showSetupLineID() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Setup_ID");
            }
        }
    }

    //Shift Wise Details
    $("#Shift1").change(function (e) {

        $("#operator_present_list").empty();
        $("#operator_abscent_list").empty();
        $("#extra_operator_list").empty();
        $("#abscent_operator_station_list").empty();
        var Shift1 = $("#Shift1").val();
        var lineId = $("#Line_ID").val();
        var shopId = $("#Shop_ID").val();
        var setupId = $("#Setup_ID").val();
        var Day = $('#Day').val();
        var FromDay = $('#FromDay').val();


        debugger

        if (Shift1 > 0 && shopId > 0 && setupId > 0 && Day > 0 && FromDay > 0 && lineId > 0) {

            $('#hideTransfernew').attr("style", "display:block");
        }
        else {
            $('#hideTransfernew').attr("style", "display:none");
        }
        //{
        //    $("#hideTransfer").atrr("style", "display:block");
        //}
        //else {
        //    $("#hideTransfer").atrr("style", "display:none");
        //}



        if (lineId > 0 && setupId > 0 && Day.length > 0 && FromDay.length > 0) {
            $('.olineID').html(null);
            $('.oDay').html(null);
            $('.ofDay').html(null);
            $('.oSetupID').html(null);

            if (Shift1.length > 0 && $("#Shift1").val() != "0") {
                $("#loading").show();
                $("#unloading").hide();
                $('.oShift').html(null);
                var url = "/DailyReportForSupervisor/GetStationListByLineID";
                // ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "&shift=" + Shift1 + "", showStationShopID, "json");
                ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "&shiftID=" + $("#Shift1").val() + "&setupID=" + $("#Setup_ID").val(), showStationShopID, "json");
            }
            else {
                $("#route_station_list").empty();
            }
        }
        else {
            if (Day.length == 0) {
                $('.oDay').html("Please Select To Day");
                $('#Day').focus();
            }
            if (FromDay.length == 0) {
                $('.ofDay').html("Please Select From Day");
                $('#FromDay').focus();
            }
            if (lineId == 0) {
                $('.olineID').html("Please Select Line");
                $('#Line_ID').focus();
            }
            $("#route_station_list").empty();
        }

    });

    function arrayBufferToBase64(buffer) {
        var binary = '';
        var bytes = new Uint8Array(buffer);
        var len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }


    function showPrescentEmployee() {
        //alert("Hi");
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                var res = "<ul class='users-list clearfix'>";
                for (var i = 0; i < jsonRes.length; i++) {
                    var buffer = jsonRes[i].Image_Content;
                    //var bytes = arrayBufferToBase64(buffer);
                    res += '<li id="' + jsonRes[i].Employee_No + '">';
                    //alert(jsonRes[i].Employee_No);
                    if (jsonRes[i].Image_Content == null)
                        res += '<img alt="user image" style="width:60px;height:70px" src="content/theme/dist/img/face-facial-hair-fine-looking-614810.jpg">';
                    else
                        res += '<img id="ItemPreview" alt="user image" style="width:60px;height:70px" src="data:image/png;base64,' + arrayBufferToBase64(buffer) + '">';
                    res += '<a href="#" class="users-list-name">' + jsonRes[i].Employee_Name.split(",")[0] + '</a>';
                    res += '<span class="users-list-date">' + jsonRes[i].Employee_No + '</span>';
                    res += '</li>';
                }
                $("#operator_present_list").html(res);
                res += '</ul>';
                presentOperatorDraggable();
            }
        }
    }


    function showAbscentEmployee() {

        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                var res = "<ul class='users-list clearfix'>";
                for (var i = 0; i < jsonRes.str5.length; i++) {
                    var buffer = jsonRes.str5[i].Image_Content;
                    //var bytes = arrayBufferToBase64(buffer);
                    var EmpName = jsonRes.str5[i].Employee_Name.split(",")[0];
                    res += '<li id="' + jsonRes.str5[i].Employee_No + '">';
                    //alert(jsonRes[i].Employee_No);
                    if (jsonRes.str5[i].Image_Content == null)
                        res += '<img alt="user image" style="width:60px;height:60px" src="content/theme/dist/img/face-facial-hair-fine-looking-614810.jpg">';
                    else
                        res += '<img id="ItemPreview" alt="user image" style="width:60px;height:60px" src="data:image/png;base64,' + arrayBufferToBase64(buffer) + '">';
                    res += '<a href="#" class="users-list-name">' + jsonRes.str5[i].Employee_Name.split(",")[0] + '</a>';
                    res += '<span class="users-list-date">' + jsonRes.str5[i].Employee_No + '</span>';
                    res += '</li>';
                }
                res += '</ul>';
                $("#operator_abscent_list").html(res);
                $("#lblAbsentEmp").html(jsonRes.count);
                var shopId = $("#Shop_ID").val();
                var shiftId = $("#Shift1").val();
                var lineId = $("#Line_ID").val();
                var setupId = $("#Setup_ID").val();
                if (shopId > 0 && shiftId > 0 && lineId > 0 && setupId > 0) {
                    url = "/DailyReportForSupervisor/GetAbsCoverageEmployeeUnderLogedinSupervisor";
                    ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "&ShiftID=" + $("#Shift1").val() + "&Line_ID=" + $("#Line_ID").val() + "&Setup_ID=" + $("#Setup_ID").val(), showAbsCoverageEmployee, "json");
                }

                // presentOperatorDraggable();
            }
        }
    }


    function showAbsCoverageEmployee() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                var res = "<ul class='users-list clearfix'>";
                for (var i = 0; i < jsonRes.str5.length; i++) {
                    var buffer = jsonRes.str5[i].Image_Content;
                    //var bytes = arrayBufferToBase64(buffer);
                    res += '<li id="' + jsonRes.str5[i].Employee_No + "_" + jsonRes.str5[i].StationId + '" onclick="showOJTHistory(' + jsonRes.str5[i].Employee_No + ')">';
                    //alert(jsonRes[i].Employee_No);
                    if (jsonRes.str5[i].Image_Content == null)
                        res += '<img alt="user image" style="width:60px;height:60px" src="content/theme/dist/img/face-facial-hair-fine-looking-614810.jpg">';
                    else
                        res += '<img id="ItemPreview" alt="user image" style="width:60px;height:60px" src="data:image/png;base64,' + arrayBufferToBase64(buffer) + '">';
                    res += '<a href="#" class="users-list-name">' + jsonRes.str5[i].Employee_Name.split(",")[0] + '</a>';
                    res += '<span class="users-list-date">' + jsonRes.str5[i].Employee_No + '</span>';
                    res += '</li>';
                }
                res += '</ul>';
                $("#extra_operator_list").html(res);
                $("#lblPACEmp").html(jsonRes.count);
                absCoverageOperatorDraggable();
                $("#unloading").show();
                $("#loading").hide();
                //presentOperatorDraggable();
            }
        }
    }



    function showLineShopID() {
        //alert("Hello");
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                clearStationLists();
                //loadHTMLTable();

                var jsonRes = $.parseJSON(myajax.responseText);
                for (var i = 0; i < jsonRes.length; i++) {
                    alert(jsonRes[i].Id);
                    //res += "<option value='" + jsonRes[i].Id + "'>" + jsonRes[i].Value + "</option>";
                }
                //SelectOptionHTML(jsonRes, "Line_ID");
            }
        }
    }

    function showShiftName() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                // clearStationLists();
                //loadHTMLTable();

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionShiftHTML(jsonRes, "currentShift_Id");
                SelectOptionHTML(jsonRes, "Shift1");
            }
        }
    }



    //Display After Swapping



    function showStationLineIDSwapping() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                //$(".route_station_list").html("");
                var jsonRes = $.parseJSON(myajax.responseText);


                for (var i = 0; i < jsonRes.length; i++) {
                    var res = "";

                    //$("#station_" + jsonRes[i].Station_ID).html("");
                    // $("#station_" + jsonRes[i].Station_ID).find("li").html("");
                    res = '<li id="assigned_operator_' + jsonRes[i].Station_ID + '_' + jsonRes[i].Employee_Token + '" >';
                    res += '<img alt="User Image" src="content/theme/dist/img/face-facial-hair-fine-looking-614810.jpg">';
                    if (jsonRes[i].Absent.length > 1) {
                        res += '<a style="color:red;" href="#" class="users-list-name" id="IsPresent">' + jsonRes[i].Employee_Name + '</a>';
                        res += '<span style="color:red;" class="users-list-date">' + jsonRes[i].Employee_Token + '</span>';
                    }
                    else {
                        res += '<a  href="#" class="users-list-name" id="IsPresent">' + jsonRes[i].Employee_Name + '</a>';
                        res += '<span class="users-list-date">' + jsonRes[i].Employee_Token + '</span>';
                    }

                    res += '</li>';
                    $("#station_" + jsonRes[i].Station_ID).html(res + $("#station_" + jsonRes[i].Station_ID).html());
                }
                //upperOperatorDraggable();
                operatorDroppable();
            }
        }
    }

    //End Display After Swapping

    //Display station wise Assigned user //29
    function showStationLineID() {

        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                //$(".route_station_list").html("");
                var jsonRes = $.parseJSON(myajax.responseText);

                //$("#lblStages").html(jsonRes.count)

                var PDECounter = 0;
                var PIDECounter = 0;
                var POECounter = 0;
                //var res1 = "";
                //res1 = '<ul>'
                for (var i = 0; i < jsonRes.str5.length; i++) {
                    var res = "";

                    var buffer = jsonRes.str5[i].Image_Content;

                    //var bytes = arrayBufferToBase64(buffer);
                    //$("#station_" + jsonRes[i].Station_ID).html("");
                    // $("#station_" + jsonRes[i].Station_ID).find("li").html("");
                    if (jsonRes.str5[i].Absent.length > 1) {
                        res = '<li id="assigned_operator_' + jsonRes.str5[i].Station_ID + '_' + jsonRes.str5[i].Employee_Token + '" >';

                    }

                    else
                        res = '<li class="PEmp" id="assigned_operator_' + jsonRes.str5[i].Station_ID + '_' + jsonRes.str5[i].Employee_Token + '" >';
                    if (jsonRes.str5[i].Image_Content == null)
                        res += '<img alt="user image" style="width:60px;height:60px" src="content/theme/dist/img/face-facial-hair-fine-looking-614810.jpg">';
                    else
                        res += '<img id="ItemPreview" alt="user image" style="width:60px;height:60px" src="data:image/png;base64,' + arrayBufferToBase64(buffer) + '">';
                    if (jsonRes.str5[i].Absent.length > 1) {
                        res += '<a style="color:red;" href="#" class="users-list-name" id="IsPresent">' + jsonRes.str5[i].Employee_Name.split(",")[0] + '</a>';
                        res += '<span style="color:red;" class="users-list-date">' + jsonRes.str5[i].Employee_Token + '</span>';
                        //res1 += '<li>' + jsonRes.str5[i].Station_Name + '</li><br/>'
                    }
                    else {

                        if (jsonRes.str5[i].IsOJT == true) {
                            POECounter++;
                            res += '<a style="color:blue;" href="#" class="users-list-name" id="IsPresent">' + jsonRes.str5[i].Employee_Name.split(",")[0] + '</a>';
                            res += '<span style="color:blue;" class="users-list-date">' + jsonRes.str5[i].Employee_Token + '</span>';
                        }
                            //else if (jsonRes.str5[i].IsOJT == false) {
                            //    res += '<a style="color:orange;" href="#" class="users-list-name" id="IsPresent">' + jsonRes.str5[i].Employee_Name.split(",")[0] + '</a>';
                            //    res += '<span style="color:orange;" class="users-list-date">' + jsonRes.str5[i].Employee_Token + '</span>';
                            //}
                        else {
                            if (jsonRes.str5[i].Station_Type_ID == 1)
                                PDECounter++;
                            if (jsonRes.str5[i].Station_Type_ID == 2)
                                PIDECounter++;
                            res += '<a style="color:green;" href="#" class="users-list-name" id="IsPresent">' + jsonRes.str5[i].Employee_Name.split(",")[0] + '</a>';
                            res += '<span style="color:green;" class="users-list-date">' + jsonRes.str5[i].Employee_Token + '</span>';
                        }

                    }

                    res += '</li>';
                    res += '&nbsp;&nbsp;';

                    $("#station_" + jsonRes.str5[i].Station_ID).html(res + $("#station_" + jsonRes.str5[i].Station_ID).html());
                }

                var res1 = "";
                res1 = '<ul>'
                for (var i = 0; i < jsonRes.list.length; i++) {
                    res1 += '<li>' + jsonRes.list[i] + " (" + jsonRes.OpCount[i] + " )" + '</li><br/>'
                }
                res1 += '</ul>'
                $("#abscent_operator_station_list").html(res1);
                $("#lblPDEmp").html(PDECounter + "/" + jsonRes.Direct);
                $("#lblPIDEmp").html(PIDECounter + "/" + jsonRes.InDirect);
                $("#lblPOTEmp").html(POECounter);
                upperOperatorDraggable();
                operatorDroppable();
                //setTimeout(function () {
                var shopId = $("#Shop_ID").val();
                var shiftId = $("#Shift1").val();
                var lineId = $("#Line_ID").val();
                var setupId = $("#Setup_ID").val();
                if (shopId > 0 && shiftId > 0 && lineId > 0 && setupId > 0) {
                    url = "/DailyReportForSupervisor/GetAbscentEmployeeUnderLogedinSupervisor";
                    ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "&ShiftID=" + $("#Shift1").val() + "&Line_ID=" + $("#Line_ID").val() + "&SetupID=" + $("#Setup_ID").val(), showAbscentEmployee, "json");
                }
                // }, 8000);
            }
        }
    }

    function showStationShopID() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally               
                $(".route_station_list").html("");
                var jsonRes = $.parseJSON(myajax.responseText);
                var res = "";
                res += '<div class="">';
                $("#lblStages").html(jsonRes.count);
                for (var i = 0; i < jsonRes.st.length; i++) {
                    //res += "<div id='station_" + jsonRes[i].Id + "' class='bg-aqua color-palette station_items'><span class='" + jsonRes[i].Id + "' >" + jsonRes[i].Value + "</span></div><br/>";
                    //res += "<option value='" + jsonRes[i].Id + "'>" + jsonRes[i].Value + "</option>";

                    //if (i == 0 || i % 4 == 0) {
                    //    //res +='<div class="row">';
                    //}
                    debugger;
                    res += '<div class="col-md-3">';

                    if (jsonRes.st[i].Type == "Direct") {
                        res += '<div class="box box-warning box-solid">';
                        res += '<div class="box-header with-border"  data-station-id="' + jsonRes.st[i].Id + '">';
                    }
                    if (jsonRes.st[i].Type == "External Training") {
                        res += '<div class="box box-warning box-solid" style="border: 1px solid #dc583a">';
                        res += '<div class="box-header ExternalTraining with-border" style="background-color:#dc583a" data-station-id="' + jsonRes.st[i].Id + '">';
                    }
                    if (jsonRes.st[i].Type == "InDirect") {
                        res += '<div class="box box-warning box-solid" style="border: 1px solid skyblue">';
                        res += '<div class="box-header with-border" style="background-color:skyblue"  data-station-id="' + jsonRes.st[i].Id + '">';
                    }
                    res += '<h3 class="box-title"><b>' + jsonRes.st[i].Value + '<b/></h3>';
                    res += '<div class="box-tools pull-right">';
                    //res += '<button class="btn btn-box-tool" data-widget="collapse"><i class="fa fa-minus"></i></button>';

                    if (jsonRes.st[i].Type == "External Training") {
                        res += '<input type="radio" name="select_station" id="station_id_' + jsonRes.st[i].Id + "_DM" + '" value=' + jsonRes.st[i].Id + ' />DM';
                    }
                    else {
                        res += '<input type="radio" name="select_station" id="station_id_' + jsonRes.st[i].Id + "_DM" + '" value=' + jsonRes.st[i].Id + ' />DM';
                        res += '<input type="radio" name="select_station" id="station_id_' + jsonRes.st[i].Id + "_OJT" + '" value=' + jsonRes.st[i].Id + ' />OJT';
                    }

                    res += '</div><!-- /.box-tools -->';
                    res += '</div><!-- /.box-header -->';
                    res += '<div class="box-body">';
                    res += '<div class="station_operator">';

                    if (jsonRes.st[i].Type == "External Training")
                        res += '<ul id="station_' + jsonRes.st[i].Id + '" class="users-list ExternalTraining clearfix">';
                    else
                        res += '<ul id="station_' + jsonRes.st[i].Id + '" class="users-list clearfix">';

                    res += '</ul>';
                    res += '</div>';
                    res += '</div><!-- /.box-body -->';
                    res += '</div><!-- /.box -->';
                    res += '</div>';

                    if (i % 3 == 0) {
                        //res += '</div>';
                    }
                }

                res += '</div>';
                $(".route_station_list").html(res);
                operatorDroppable();
                var shopId = $("#Shop_ID").val();
                var shiftId = $("#Shift1").val();
                var lineId = $("#Line_ID").val();
                var setupId = $("#Setup_ID").val();
                //setTimeout(function () {
                url = "/DailyReportForSupervisor/GetCurrentShiftOperatorAgainstStationListByLineID";
                //ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showStationLineID, "json");
                ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "&shiftID=" + $("#Shift1").val() + "&setupID=" + $("#Setup_ID").val() + "&Day=" + $("#Day option:selected").text(), showStationLineID, "json");
                //}, 4000);
                //setTimeout(function () {

                //    if (shopId > 0 && shiftId > 0 && lineId > 0) {
                //        url = "/DailyReportForSupervisor/GetAbscentEmployeeUnderLogedinSupervisor";
                //        ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "&ShiftID=" + $("#Shift1").val() + "&Line_ID=" + $("#Line_ID").val(), showAbscentEmployee, "json");
                //    }
                //}, 8000);

                //setTimeout(function () {
                //    if (shopId > 0 && shiftId > 0 && lineId > 0) {
                //        url = "/DailyReportForSupervisor/GetAbsCoverageEmployeeUnderLogedinSupervisor";
                //        ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "&ShiftID=" + $("#Shift1").val() + "&Line_ID=" + $("#Line_ID").val(), showAbsCoverageEmployee, "json");
                //    }
                //}, 12000);
            }
        }
    }



    $(".site_trash_box").droppable({

        accept: ".users-list li",
        activeClass: "ui-state-highlight",
        drop: function (event, ui) {
            var draggableId = ui.draggable.attr("id");
            if (draggableId) {
                //url = "DailyReportForSupervisor/DeleteOperator";
                $("#EmployeeNo").val(ui.draggable.attr("id"));
                $("#stationId").val(ui.draggable.attr("id"));
                $("#shift").val($('#Shift1').val());
                $("#line_ID").val($('#Line_ID').val());
                $("#fromDay").val($("#FromDay option:selected").text());
                $("#toDay").val($("#Day option:selected").text());
                //ajaxpack.getAjaxRequest(url, "EmployeeNo=" + ui.draggable.attr("id") + "&stationId=" + ui.draggable.attr("id") + "&shift=" + $('#Shift1').val() + "&line_ID=" + $('#Line_ID').val() + "&fromDay=" + $("#FromDay option:selected").text() + "&toDay=" + $("#Day option:selected").text(), deleteData, "json");
                $('#DeleteEmployee').modal('show');

            }
            // ui.draggable.remove();
        }
    });



    function stationAssignedOperatorDraggable() {
        $(".station_assigned_operator").draggable({
            cancel: "a.ui-icon", // clicking an icon won't initiate dragging
            revert: "invalid", // when not dropped, the item will revert back to its initial position
            containment: "document",
            helper: "clone",
            cursor: "move"
        });
    }

    function presentOperatorDraggable() {
        $("#operator_present_list .users-list li").draggable({
            cancel: "a.ui-icon", // clicking an icon won't initiate dragging
            revert: "invalid", // when not dropped, the item will revert back to its initial position
            containment: "document",
            helper: "clone",
            cursor: "move"
        });
    }

    function absCoverageOperatorDraggable() {
        $("#extra_operator_list .users-list li").draggable({
            cancel: "a.ui-icon", // clicking an icon won't initiate dragging
            revert: "invalid", // when not dropped, the item will revert back to its initial position
            containment: "document",
            helper: "clone",
            cursor: "move"
        });
    }

    function upperOperatorDraggable() {
        $(".users-list li").draggable({
            cancel: "a.ui-icon", // clicking an icon won't initiate dragging
            revert: "invalid", // when not dropped, the item will revert back to its initial position
            containment: "document",
            helper: "clone",
            cursor: "move"
        });
    }



    function operatorDroppable() {
        debugger;
        $("#extra_operator_list").droppable({
            accept: "#route_station_list .station_operator li.PEmp",
            activeClass: "ui-state-highlight",
            drop: function (event, ui) {

                var shiftID = $('#Shift1').val();
                var setupID = $('#Setup_ID').val();
                var draggableId = ui.draggable.attr("id");//123
                var droppableId = $(this).attr("id");//4
                var shopId = $("#Shop_ID").val();
                var lineId = $("#Line_ID").val();
                var stid = $("#route_station_list input[name='select_station']:checked").parent().parent().parent().parent().data('station-id');
                var stsid = droppableId.split('_');

                var OJTOperatorDroppable = $("#route_station_list input[name='select_station']:checked").find("Id");
                var isOJTOpearors = false;
                $("#hdnSelectedStation").val(droppableId);
                $("#hdnSelectedOperator").val(draggableId);

                //url = "/DailyReportForSupervisor/CheckIsValidSkillForCriticalStation";
                //ajaxpack.getAjaxRequest(url, "Employee_No=" + ui.draggable.attr("id") + "&Station_Id=" + $(this).attr("id"),
                //    vaildStationData(ui.draggable.attr("id"), $(this).attr("id"), $("#Shop_ID").val(), $("#Line_ID").val(), $("#Shift1").val(), $("#FromDay").val(), $("#Day").val(), draggableId, droppableId),
                //    "json");

                $.ajax(
{
    url: url = "/DailyReportForSupervisor/CheckIsValidSkillForCriticalStation?Employee_No=" + ui.draggable.attr("id") + "&Station_Id=" + $(this).attr("id") + "&Shop_Id=" + shopId + "&Line_Id=" + lineId,
    type: 'GET',
    data: "",
    contentType: 'application/json; charset=utf-8',
    success: function (data) {

        if (data.msg == true && data.success == "True") {

            if (draggableId) {
                if (data.bs == true) {
                    if (shiftID != "") {

                        // alert($("#Day").val());
                        url = "/DailyReportForSupervisor/saveAssociateAllocation";
                        // ajaxpack.getAjaxRequest(url, "EmployeeNo=" + ui.draggable.attr("id") + "", "stationId=" + $(this).attr("id") + "", showLineStationType, "json");
                        ajaxpack.getAjaxRequest(url, "EmployeeNo=" + ui.draggable.attr("id") + "&prevStationId=" + draggableId + "&stationId=" + droppableId + "&shopId=" + $("#Shop_ID").val() + "&lineId=" + $("#Line_ID").val() + "&shift=" + $("#Shift1").val() + "&FromDay=" + $("#FromDay").val() + "&ToDay=" + $("#Day").val() + "&IsOJT=" + isOJTOpearors + "&SetupID=" + setupID, SaveData, "json");


                    }
                    else {
                        //alert("Please select Shift");
                        $('#testy').toastee({
                            type: 'error',
                            width: '100px',
                            height: '100px',
                            message: 'Please select shift...',
                        });
                    }

                }
                else {
                    debugger
                    //alert("In valid station to save ");
                    $('#testy').toastee({
                        type: 'error',
                        width: '100px',
                        height: '100px',
                        message: 'Invalid station to save...',
                    });
                }
            }
        } else if (data.msg == false && data.success == "True") {
            $('#testy').toastee({
                type: 'error',
                width: '100px',
                height: '100px',
                message: 'You cannot assign  employee whose skillset less than 3...',
            });
        } else {
            $('#testy').toastee({
                type: 'error',
                width: '100px',
                height: '100px',
                message: 'Error occured...',
            });
        }
    }
});

            }
        });
        $("#route_station_list .station_operator ul").droppable({

            accept: "#operator_present_list .users-list li,#extra_operator_list .users-list li,#route_station_list .station_operator li",
            activeClass: "ui-state-highlight",
            drop: function (event, ui) {

                var shiftID = $('#Shift1').val();
                var setupID = $('#Setup_ID').val();
                var draggableId = ui.draggable.attr("id");//123
                var droppableId = $(this).attr("id");//4
                debugger;
                var shopId = $("#Shop_ID").val();
                var lineId = $("#Line_ID").val();

                //var stid = $("#route_station_list input[name='select_station']:checked").parent().parent().parent().parent().data('station-id');
                var stid = $("#route_station_list input[name='select_station']:checked")[0].value;
                var stsid = droppableId.split('_');

                var dragId = draggableId.split('_');
                var count = dragId.length;
                var empNo = dragId[0];
                var stationId = dragId[1];
                if (count > 2) {
                    empNo = ui.draggable.attr("id");
                    stationId = dragId[2];
                }

                debugger;
                var value = $("#route_station_list input[name='select_station']:checked").attr("Id");
                var isOJTOpearors = false;
                if (value != undefined) {
                    var Operator = value.split('_')[3];
                    if (Operator == "OJT")
                        isOJTOpearors = true;
                    if (Operator == "DM")
                        isOJTOpearors = false;




                    $("#hdnSelectedStation").val(droppableId);
                    $("#hdnSelectedOperator").val(draggableId);

                    if ($(this).hasClass("ExternalTraining")) {
                        $('#modalExternalTraining').modal('show');
                    }

                    var ExshopID = $('#hdnShopID').val(shopId);
                    $('#hdnLineID').val(lineId);
                    $('#hdnStationID').val(stid);
                    $('#hdnSetupID').val(setupID);
                    $('#hdnShiftID').val(shiftID);
                    $('#hdnEmpNo').val(empNo);

                    //url = "/DailyReportForSupervisor/CheckIsValidSkillForCriticalStation";
                    //ajaxpack.getAjaxRequest(url, "Employee_No=" + ui.draggable.attr("id") + "&Station_Id=" + $(this).attr("id"),
                    //    vaildStationData(ui.draggable.attr("id"), $(this).attr("id"), $("#Shop_ID").val(), $("#Line_ID").val(), $("#Shift1").val(), $("#FromDay").val(), $("#Day").val(), draggableId, droppableId),
                    //    "json");

                    $.ajax(
    {
        url: url = "/DailyReportForSupervisor/CheckIsValidSkillForCriticalStation?Employee_No=" + empNo + "&Station_Id=" + $(this).attr("id") + "&Shop_Id=" + shopId + "&Line_Id=" + lineId,
        type: 'GET',
        data: "",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {

            if (data.msg == true && data.success == "True") {

                if (draggableId) {
                    if (stsid[1] == stid) {
                        if (shiftID != "") {

                            // alert($("#Day").val());
                            url = "/DailyReportForSupervisor/saveAssociateAllocation";
                            // ajaxpack.getAjaxRequest(url, "EmployeeNo=" + ui.draggable.attr("id") + "", "stationId=" + $(this).attr("id") + "", showLineStationType, "json");
                            ajaxpack.getAjaxRequest(url, "EmployeeNo=" + empNo + "&prevStationId=" + stationId + "&stationId=" + droppableId + "&shopId=" + $("#Shop_ID").val() + "&lineId=" + $("#Line_ID").val() + "&shift=" + $("#Shift1").val() + "&FromDay=" + $("#FromDay").val() + "&ToDay=" + $("#Day").val() + "&IsOJT=" + isOJTOpearors + "&SetupID=" + setupID, SaveData, "json");


                        }
                        else {
                            //alert("Please select Shift");
                            $('#testy').toastee({
                                type: 'error',
                                width: '100px',
                                height: '100px',
                                message: 'Please select shift...',
                            });
                        }

                    }
                    else {
                        debugger
                        //alert("In valid station to save ");
                        $('#testy').toastee({
                            type: 'error',
                            width: '100px',
                            height: '100px',
                            message: 'Invalid station to save...',
                        });
                    }
                }
            } else if (data.msg == false && data.success == "True") {
                $('#testy').toastee({
                    type: 'error',
                    width: '100px',
                    height: '100px',
                    message: 'You cannot assign  employee whose skillset less than 3...',
                });
            } else {
                $('#testy').toastee({
                    type: 'error',
                    width: '100px',
                    height: '100px',
                    message: 'Error occured...',
                });
            }
        }
    });

                    //if (draggableId) {
                    //    if (stsid[1] == stid) {
                    //        if (shiftID != "") {
                    //            // alert($("#Day").val());
                    //            url = "/DailyReportForSupervisor/saveAssociateAllocation";
                    //            // ajaxpack.getAjaxRequest(url, "EmployeeNo=" + ui.draggable.attr("id") + "", "stationId=" + $(this).attr("id") + "", showLineStationType, "json");
                    //            ajaxpack.getAjaxRequest(url, "EmployeeNo=" + ui.draggable.attr("id") + "&stationId=" + $(this).attr("id") + "&shopId=" + $("#Shop_ID").val() + "&lineId=" + $("#Line_ID").val() + "&shift=" + $("#Shift1").val() + "&FromDay=" + $("#FromDay").val() + "&ToDay=" + $("#Day").val(), SaveData, "json");
                    //        }
                    //        else {
                    //            //alert("Please select Shift");
                    //            $('#testy').toastee({
                    //                type: 'error',
                    //                width: '100px',
                    //                height: '100px',
                    //                message: 'Please select shift...',
                    //            });
                    //        }

                    //    }
                    //    else {
                    //        //alert("In valid station to save ");
                    //        $('#testy').toastee({
                    //            type: 'error',
                    //            width: '100px',
                    //            height: '100px',
                    //            message: 'Invalid station to save...',
                    //        });
                    //    }
                    //}



                    // var operator_html = $("#" + draggableId).html();
                    // $("#" + $(this).attr("id")).html("<li class='station_assigned_operator' id='assigned_operator_" + draggableId + "_" + droppableId + "''>" + operator_html + "</li>" + $("#" + $(this).attr("id")).html());
                    // stationAssignedOperatorDraggable();




                    //ui.draggable.remove();

                    //if ($("#" + $(this).attr("id")).html() == null || $("#" + $(this).attr("id")).html() == "") {
                    //    if (ui.draggable.attr("class").indexOf("station_items") > 0) {
                    //        $("#" + $(this).attr("id")).html("<div id='" + draggableId + "' class='bg-aqua color-palette station_items'><span>" + station_name + "</span></div>");
                    //        ui.draggable.remove();
                    //    }
                    //    else {
                    //        if (ui.draggable.attr("class").indexOf("station_icon_items") > 0) {




                    //        }
                    //    }

                    //}
                }
                else {
                    $('#testy').toastee({
                        type: 'error',
                        width: '300px',
                        height: '100px',
                        message: 'Please select any station...',
                    });
                }
            }
        });



    }


    function deleteData() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                //alert("Data deleted Sucessfully");
                if (jsonRes.msg == "Success") {
                    $('#testy').toastee({
                        type: 'success',
                        width: '200px',
                        height: '100px',
                        message: 'Operator Deleted Successfully...',
                    });
                }
                else if (jsonRes.msg == "Previous") {
                    $('#testy').toastee({
                        type: 'error',
                        width: '300px',
                        height: '100px',
                        message: 'Can not  Deleted Previous Allocation...',
                    });
                }
                else {
                    $('#testy').toastee({
                        type: 'error',
                        width: '300px',
                        height: '100px',
                        message: 'Some problem occured while deleting operator...',
                    });
                }
                $('#Shift1').change();

            }
        }
    }

    function SaveData() {

        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype

        if (myajax.readyState == 4) {


            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                //alert(jsonRes);
                //check json res t or f

                if (jsonRes.result == "Success") {
                    //allocaed suss
                    var selectedStationId = $("#hdnSelectedStation").val();
                    var selectedOperator = $("#hdnSelectedOperator").val();
                    //alert("Data Saved Sucessfully");

                    $('#testy').toastee({
                        type: 'success',
                        width: '100px',
                        height: '100px',
                        message: 'Operator Assign Successfully...',
                    });
                    var stationId = selectedStationId.replace("station_", "");
                    if (selectedOperator.length > 8) {
                        var values = selectedOperator.split('_');
                        var prevStationId = values[2];
                        $("#" + selectedStationId).html("<li class='station_assigned_operator' id='assigned_operator_" + stationId + "_" + values[3] + "''>" + $("#" + selectedOperator).html() + "</li>" + $("#" + selectedStationId).html());
                        $("#" + selectedOperator).remove();
                    }

                    else
                        $("#" + selectedStationId).html("<li class='station_assigned_operator' id='assigned_operator_" + stationId + "_" + selectedOperator + "''>" + $("#" + selectedOperator).html() + "</li>" + $("#" + selectedStationId).html());
                    stationAssignedOperatorDraggable();
                    $('#Shift1').change();

                }
                else if (jsonRes.result == "Allready") {
                    //already allocated 
                    // alert("This Employee Already Allocated to This Station");
                    $('#testy').toastee({
                        type: 'error',
                        width: '300px',
                        height: '100px',
                        message: 'This Employee Already Allocated to This Station...',
                    });

                    stationAssignedOperatorDraggable();
                }
                else if (jsonRes.result == "dateerror") {
                    $('#testy').toastee({
                        type: 'error',
                        width: '320px',
                        height: '100px',
                        message: 'To Day should be greater than From Day or Current Day...',
                    });
                }

                else {

                    $('#testy').toastee({
                        type: 'error',
                        width: '300px',
                        height: '100px',
                        message: 'Previous allocation not allow...',
                    });
                }

            }

            //loademp();
        }
        //   loademp();
        // operatorDroppable();
    }

    //function presentOperatorDraggable() {
    //    $("#route_station_list .station_operator ul").draggable({
    //        cancel: "a.ui-icon", // clicking an icon won't initiate dragging
    //        revert: "invalid", // when not dropped, the item will revert back to its initial position
    //        containment: "document",
    //        helper: "clone",
    //        cursor: "move"
    //    });
    //}

    function loademp() {
        //debugger;
        //     var url = "/DailyReportForSupervisor/GetCurrentShiftOperatorAgainstStationListByLineID";
        //    // ajaxpack.getAjaxRequest(url, "EmployeeNo=" + ui.draggable.attr("id") + "", "stationId=" + $(this).attr("id") + "", showLineStationType, "json");
        //    ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "&shiftID=" + $("#Shift1").val() + "&Day=" + $("#Day option:selected").text(), showStationLineID, "json");

        url = "/DailyReportForSupervisor/GetCurrentShiftOperatorAgainstStationListByLineID";
        //ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showStationLineID, "json");
        ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "&shiftID=" + $("#Shift1").val() + "&Day=" + $("#Day option:selected").text(), showStationLineIDSwapping, "json");

        //  showStationLineID();
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

    function SelectOptionShiftHTML(jsonRes, targetId) {
        //var jsonRes = $.parseJSON(myajax.responseText);        
        var res = "";
        for (var i = 0; i < jsonRes.length; i++) {
            res += "<option>" + jsonRes[i].Value + "</option>";
        }

        res = "<option>" + $("#" + targetId).html() + "</option>" + res;
        $("#" + targetId).html(res);
    }


    function clearSelectBox(targetId) {
        var res = "";
        res = "<option value=''>" + $("#" + targetId + " option:first").html() + "</option>";
        $("#" + targetId).html(res);
    }

    function clearStationLists() {
        $(".station_lists").html("");
    }

    function clearRoute() {
        loadHTMLTable();
    }



    $('#btnShow').click(function () {
        if (validateWeekly()) {
            $('#WeeklyOperator').modal('show');
            var week = $('#weeks').val();
            var shift = $('#Shift2').val();
            var url = "/DailyReportForSupervisor/GetWeekandShiftWiseAssignedOperatorDetails";
            ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "&shopID=" + $("#Shop_ID").val() + "&week=" + $("#weeks").val() + "&Shift=" + $("#Shift2").val(), BindWeekandShiftWiseAssignedOperatorDetails, "json");
        }
    })

    function BindWeekandShiftWiseAssignedOperatorDetails() {
        var currentDay;
        var prevDay;
        var flag = 0;
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally         
                var jsonRes = $.parseJSON(myajax.responseText);

                if (jsonRes.length > 0) {
                    $('#btnAssign').prop('disabled', false);
                }
                else {
                    $('#btnAssign').prop('disabled', true);
                }

                var res = "<thead>";
                res += ' <tr>';
                res += ' <th>Day';
                res += ' </th>';
                res += ' <th>Station';
                res += ' </th>';
                res += ' <th>Employee Name';
                res += ' </th>';
                res += ' </tr>';
                res += '</thead>';

                for (var i = 0; i < jsonRes.length; i++) {
                    var cnt = i + 1;
                    var nextDay;
                    if (jsonRes.length > cnt) {
                        nextDay = jsonRes[cnt].Day;
                    }
                    else {
                        nextDay = "";
                    }

                    if (jsonRes[i].Day == "Monday") {
                        currentDay = "Monday";
                        if (flag == 0) {
                            res += "<tbody class='labels'>";
                            res += " <tr><td colspan='3'><label for='monday'>Monday</label>";
                            res += "<input type='checkbox' name='monday' id='monday' data-toggle='toggle'></td></tr></tbody>";
                            res += "<tbody class='hide_content' style='display:none;'>";
                            flag = 1;
                        }
                        res += '<tr>';
                        //  res += '<td>' + jsonRes[i].Day + '</td>';
                        res += '<td style="border: none !important;"></td>';
                        res += '<td>' + jsonRes[i].Station + '</td>';
                        res += '<td>' + jsonRes[i].Employee_Name + '</td>';
                        //res += '<td>' + jsonRes[i].Shift + '</td>';
                        res += '</tr>';

                        if (currentDay != nextDay) {
                            res += " </tbody>";
                            flag = 0;
                        }

                    }


                    if (jsonRes[i].Day == "Tuesday") {
                        currentDay = "Tuesday";
                        if (flag == 0) {
                            res += "<tbody class='labels'>";
                            res += " <tr><td colspan='5'><label for='tuesday'>Tuesday</label>";
                            res += "<input type='checkbox' name='tuesday' id='tuesday' data-toggle='toggle'></td></tr></tbody>";
                            res += "<tbody class='hide_content' style='display:none;'>";
                            flag = 1;
                        }
                        res += '<tr>';
                        // res += '<td>' + jsonRes[i].Day + '</td>';
                        res += '<td style="border: none !important;"></td>';
                        res += '<td>' + jsonRes[i].Station + '</td>';
                        res += '<td>' + jsonRes[i].Employee_Name + '</td>';
                        //res += '<td>' + jsonRes[i].Shift + '</td>';
                        res += '</tr>';

                        if (currentDay != nextDay) {
                            res += " </tbody>";
                            flag = 0;
                        }
                    }
                    if (jsonRes[i].Day == "Wednesday") {
                        currentDay = "Wednesday";
                        if (flag == 0) {
                            res += "<tbody class='labels'>";
                            res += " <tr><td colspan='5'><label for='wednesday'>Wednesday</label>";
                            res += "<input type='checkbox' name='wednesday' id='wednesday' data-toggle='toggle'></td></tr></tbody>";
                            res += "<tbody class='hide_content' style='display:none;'>";
                            flag = 1;
                        }
                        res += '<tr>';
                        // res += '<td>' + jsonRes[i].Day + '</td>';
                        res += '<td style="border: none !important;"></td>';
                        res += '<td>' + jsonRes[i].Station + '</td>';
                        res += '<td>' + jsonRes[i].Employee_Name + '</td>';
                        //res += '<td>' + jsonRes[i].Shift + '</td>';
                        res += '</tr>';
                        if (currentDay != nextDay) {
                            res += "</tbody>";
                            flag = 0;
                        }
                    }

                    if (jsonRes[i].Day == "Thursday") {
                        currentDay = "Thursday";
                        if (flag == 0) {
                            res += "<tbody class='labels'>";
                            res += " <tr><td colspan='3'><label for='thursday'>Thursday</label>";
                            res += "<input type='checkbox' name='thursday' id='thursday' data-toggle='toggle'></td></tr></tbody>";
                            res += "<tbody class='hide_content' style='display:none;'>";
                            flag = 1;
                        }
                        res += '<tr>';
                        // res += '<td>' + jsonRes[i].Day + '</td>';
                        res += '<td style="border: none !important;"></td>';
                        res += '<td>' + jsonRes[i].Station + '</td>';
                        res += '<td>' + jsonRes[i].Employee_Name + '</td>';
                        //res += '<td>' + jsonRes[i].Shift + '</td>';
                        res += '</tr>';
                        if (currentDay != nextDay) {
                            res += " </tbody>";
                            flag = 0;
                        }
                    }
                    if (jsonRes[i].Day == "Friday") {
                        currentDay = "Friday";
                        if (flag == 0) {
                            res += "<tbody class='labels'>";
                            res += " <tr><td colspan='3'><label for='friday'>Friday</label>";
                            res += "<input type='checkbox' name='friday' id='friday' data-toggle='toggle'></td></tr></tbody>";
                            res += "<tbody class='hide_content' style='display:none;'>";
                            flag = 1;
                        }
                        res += '<tr>';
                        // res += '<td>' + jsonRes[i].Day + '</td>';
                        res += '<td style="border: none !important;"></td>';
                        res += '<td>' + jsonRes[i].Station + '</td>';
                        res += '<td>' + jsonRes[i].Employee_Name + '</td>';
                        //res += '<td>' + jsonRes[i].Shift + '</td>';
                        res += '</tr>';

                        if (currentDay != nextDay) {
                            res += " </tbody>";
                            flag = 0;
                        }
                    }
                    if (jsonRes[i].Day == "Saturday") {
                        currentDay = "Saturday";
                        //var nextDay = jsonRes[i + 1].Day;
                        if (flag == 0) {
                            res += "<tbody class='labels'>";
                            res += " <tr><td colspan='5'><label for='saturday'>Saturday</label>";
                            res += "<input type='checkbox' name='saturday' id='saturday' data-toggle='toggle'></td></tr></tbody>";
                            res += "<tbody class='hide_content' style='display:none;'>";
                            flag = 1;
                        }
                        res += '<tr>';
                        // res += '<td>' + jsonRes[i].Day + '</td>';
                        res += '<td  style="border: none !important;"></td>';
                        res += '<td>' + jsonRes[i].Station + '</td>';
                        res += '<td>' + jsonRes[i].Employee_Name + '</td>';
                        //res += '<td>' + jsonRes[i].Shift + '</td>';
                        res += '</tr>';

                        if (currentDay != nextDay) {
                            res += " </tbody>";
                            flag = 0;
                        }
                    }
                    //sunday 

                    if (jsonRes[i].Day == "Sunday") {
                        currentDay = "Sunday";
                        //var nextDay = jsonRes[i + 1].Day;
                        if (flag == 0) {
                            res += "<tbody class='labels'>";
                            res += " <tr><td colspan='5'><label for='Sunday'>Sunday</label>";
                            res += "<input type='checkbox' name='Sunday' id='Sunday' data-toggle='toggle'></td></tr></tbody>";
                            res += "<tbody class='hide_content' style='display:none;'>";
                            flag = 1;
                        }
                        res += '<tr>';
                        // res += '<td>' + jsonRes[i].Day + '</td>';
                        res += '<td  style="border: none !important;"></td>';
                        res += '<td>' + jsonRes[i].Station + '</td>';
                        res += '<td>' + jsonRes[i].Employee_Name + '</td>';
                        //res += '<td>' + jsonRes[i].Shift + '</td>';
                        res += '</tr>';

                        if (currentDay != nextDay) {
                            res += " </tbody>";
                            flag = 0;
                        }
                    }

                }

                $('#Weekly_Operator_table').html(res);
            }
        }
    }

    $('#weeks').change(function () {
        if ($('#weeks').val().length > 0) {
            $('.eWeek').html(null);
        }
    })

    $('#Shift2').change(function () {
        if ($('#Shift2').val().length > 0) {
            $('.eShift2').html(null);
        }
    })

    $('#btnAssign').click(function () {
        if (ValidatPopup()) {
            btn = $(this);
            $(btn).buttonLoader('start');
            var url = "/DailyReportForSupervisor/AssignOperatorToNextWeek";
            ajaxpack.getAjaxRequest(url, "shopID=" + $("#Shop_ID").val() + "&lineID=" + $("#Line_ID").val() + "&copy_Week=" + $("#weeks").val() + "&fromDate=" + $("#txtFromDate").val() + "&toDate=" + $("#txtToDate").val() + "&Shift=" + $("#Shift3").val() + "&replace=NO", AssignOperatorToNextWeek, "json");
        }

    })

    function AssignOperatorToNextWeek() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally         
                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes.msg == "Success") {
                    // alert("Weekly Operator Assign Successfully...");
                    $('#WeeklyOperator').modal('hide');
                    $('#testy').toastee({
                        type: 'success',
                        width: '300px',
                        height: '100px',
                        message: 'Weekly Operator Assign Successfully...',
                    });
                    // $(btn).buttonLoader('stop');

                    setTimeout(function () {
                        location.reload();
                    }, 2000);

                }
                else if (jsonRes.msg == "Allready") {
                    if (confirm('Are you sure you want replace previous allocation ?')) {
                        var url = "/DailyReportForSupervisor/AssignOperatorToNextWeek";
                        ajaxpack.getAjaxRequest(url, "shopID=" + $("#Shop_ID").val() + "&lineID=" + $("#Line_ID").val() + "&copy_Week=" + $("#weeks").val() + "&fromDate=" + $("#txtFromDate").val() + "&toDate=" + $("#txtToDate").val() + "&Shift=" + $("#Shift3").val() + "&replace=YES", AssignOperatorToNextWeek, "json");
                    }
                    else {
                        $(btn).buttonLoader('stop');
                    }
                }
                else if (jsonRes.msg == "Previous") {
                    $('#testy').toastee({
                        type: 'error',
                        width: '350px',
                        height: '100px',
                        message: 'Previous allocation not available...',
                    });
                    $(btn).buttonLoader('stop');
                }
                else {
                    //alert("Some error occured while assigning operator");
                    $('#testy').toastee({
                        type: 'error',
                        width: '350px',
                        height: '100px',
                        message: 'Some error occured while assigning operator...',
                    });
                    $(btn).buttonLoader('stop');
                }
            }
        }

    }


    function ValidatPopup() {
        var flag = 0;
        if ($('#txtFromDate').val().length > 0) {
            flag = 1;
        }
        else {
            $('.fromDate').html('Please select from date');
            $('#txtFromDate').focus();
            flag = 0;
        }

        if ($('#txtToDate').val().length > 0) {
            flag = 1;
        }
        else {
            $('.toDate').html('Please select to date');
            $('#txtToDate').focus();
            flag = 0;
        }
        if (flag == 0) {
            return false;
        }
        else {
            return true;
        }
    }
    $('#weeks3').change(function () {
        if ($('#weeks3').val().length > 0) {
            $('.eWeek2').html(null);
        }
    })

    $('#Shift3').change(function () {
        if ($('#Shift3').val().length > 0) {
            $('.eShift3').html(null);
        }
    })


    function validateWeekly() {


        if ($('#Shop_ID').val().length > 0) {
            $('.oShopID').html(null);
        }
        else {
            $('.oShopID').html('Please select shop');
            $('#Shop_ID').focus();
        }

        if ($('#Line_ID').val().length > 0) {
            $('.olineID').html(null);
        }
        else {
            $('.olineID').html('Please select line');
            $('#Line_ID').focus();
        }

        if ($('#weeks').val().length > 0) {
            $('.eWeek').html(null);
        }
        else {
            $('.eWeek').html('Please select week');
            $('#weeks').focus();
        }
        if ($('#Shift2').val().length > 0) {
            $('.eShift2').html(null);
        }
        else {
            $('.eShift2').html('Please select shift');
            $('#Shift2').focus();
        }

        if ($('#Shop_ID').val().length > 0 && $('#Shift2').val().length > 0 && $('#Line_ID').val().length > 0 && $('#weeks').val().length > 0) {
            return true;
        }
        else {
            return false;
        }

    }

    $('#Day').change(function () {
        if ($('#Day').val().length > 0) {
            $('.oDay').html(null);
            if ($('#FromDay').val() > $('#Day').val()) {
                $('#testy').toastee({
                    type: 'error',
                    width: '350px',
                    height: '100px',
                    message: 'To Day should be greater than From Day...',
                });
            }
        }
        else {
            $('#Shift1').val(0);
            $('#Shift1').change();
        }
    });

    $('#FromDay').change(function () {
        if ($('#FromDay').val().length > 0) {
            $('.ofDay').html(null);
            var day = $('#Day').val();
            if ($('#FromDay').val() > $('#Day').val() && day.length > 0) {
                $('#testy').toastee({
                    type: 'error',
                    width: '350px',
                    height: '100px',
                    message: 'From Day should not be greater than To Day...',
                });
            }
        }
        else {
            $('#Shift1').val(0);
            $('#Shift1').change();
        }

    });

});
$('#btnPublish').click(function () {
    debugger;
    var ShopId = $("#Shop_ID").val();
    var LineId = $("#Line_ID").val();
    var SetupId = $("#Setup_ID").val();
    var ShiftId = $("#Shift1").val();
    if (ShopId > 0 && LineId > 0 && SetupId > 0 && ShiftId > 0) {
        $.ajax({
            url: "/DailyReportForSupervisor/publishstations",
            type: "POST",
            data: JSON.stringify({ 'Shopid': ShopId, 'Lineid': LineId, 'Shiftid': ShiftId, 'setupid': SetupId }),
            dataType: "json",
            traditional: true,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                debugger;
                if (data == true) {
                    $('#testy').toastee({
                        type: 'success',
                        width: '300px',
                        height: '100px',
                        message: 'Publish to dashboard Successfully...',
                    });
                } else {
                    $('#testy').toastee({
                        type: 'error',
                        width: '300px',
                        height: '100px',
                        message: 'Somthing wrong please check setup name...',
                    });
                }
            },
            error: function () {
                alert("Please Select Line, Shop, Setup, Shift!!!");
            }
        });
    }
    else {

        $('#testy').toastee({
            type: 'error',
            width: '300px',
            height: '100px',
            message: 'Please Select Line, Shop, Setup, Shift!!!...',
        });
    }


});
$('#btnSave').click(function () {
    debugger;
    var trainingName = $('#txtTrainingName').val();
    var trainingDesc = $('#txtTrainingDescription').val();

    var ExshopID = $('#hdnShopID').val();
    var ExlineId = $('#hdnLineID').val();
    var ExstationId = $('#hdnStationID').val();
    var ExsetupId = $('#hdnSetupID').val();
    var ExshiftId = $('#hdnShiftID').val();
    var ExemNo = $('#hdnEmpNo').val();

    url = "/DailyReportForSupervisor/SaveExternalTrainingData";
    ajaxpack.getAjaxRequest(url, "ShopId=" + ExshopID + "&LineId=" + ExlineId + "&StationId=" + ExstationId + "&SetupId=" + ExsetupId + "&ShiftId=" + ExshiftId + "&EmpNo=" + ExemNo + "&Name=" + trainingName + "&Desc=" + trainingDesc, showExternalTrainingData, "json");
});

function showExternalTrainingData() {
    var myajax = ajaxpack.ajaxobj
    var myfiletype = ajaxpack.filetype
    if (myajax.readyState == 4) {
        if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
            debugger;
            var jsonRes = $.parseJSON(myajax.responseText);
            if (jsonRes == true) {
                $('#modalExternalTraining').modal('hide');
                $('#testy').toastee({
                    type: 'success',
                    width: '300px',
                    height: '100px',
                    message: 'External Training Details Added Successfully...',
                });
            }
        }
    }
}
function showOJTHistory(EmpNo) {
    
    url = "/DailyReportForSupervisor/GetOJTHistory";
    ajaxpack.getAjaxRequest(url, "empNumber=" + EmpNo, showOJTEmpHistory, "json");
}

function showOJTEmpHistory() {
   
    var myajax = ajaxpack.ajaxobj
    var myfiletype = ajaxpack.filetype
    if (myajax.readyState == 4) {
        if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
            
            var jsonRes = $.parseJSON(myajax.responseText);
            var res = "";
            res += "<thead>";
            res += "<tr><th style='text-align:center'>Sr. No.</th><th style='text-align:center'>Token Number</th><th style='text-align:center'>Station Name</th><th style='text-align:center'>Duration(in days)</th></tr>";
            res += "<tbody>";
            if (jsonRes.str.length > 0) {
                for (var i = 0; i < jsonRes.str.length; i++) {
                    var j = i;
                    j++;
                    res += "<tr>";
                    res += "<td style='text-align:center'>" + j + "</td>";
                    res += "<td style='text-align:center'>" + jsonRes.str[i].EmpToken + "</td>";
                    res += "<td style='text-align:center'>" + jsonRes.str[i].stationName + "</td>";
                    res += "<td style='text-align:center'>" + jsonRes.str[i].Duration + "</td>";
                    res += "</tr>";
                }
            }
            else {
                res += "<tr>";
                res += "<td colspan='4'>No on job training data available</td>";
            }
            res += "</tbody>";
            $('#lblOJTHistory').html(jsonRes.Name);
            $('#tblOJTHistory').html(res);
            $('#OJTHistory').modal('show');
        }
    }
}
