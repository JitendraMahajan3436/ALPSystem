$(document).ready(function (e) {
    var sFlag = 0;
    var lFlag = 0;
    var supFlag = 0;
    $('#Employee_ID').keyup(function () {
        searchTable($(this).val());
    });

    function clearSelectBox(targetId) {
        var res = "";
        res = "<option value=''>" + $("#" + targetId + " option:first").html() + "</option>";
        $("#" + targetId).html(res);
    }
    function searchTable(inputVal) {
        var table = $('#divListofOperator');
        // var table = $('#divselectedOperators');
        table.find('tr').each(function (index, row) {
            var allCells = $(row).find('td');
            if (allCells.length)//&gt; 0
            {
                var found = false;
                allCells.each(function (index, td) {
                    var regExp = new RegExp(inputVal, 'i');
                    if (regExp.test($(td).text())) {
                        found = true;
                        return false;
                    }
                });
                if (found == true) $(row).show(); else $(row).hide();
            }
        });
    }

    $('#selectedOperators').keyup(function () {
        searchTable1($(this).val());
    });


    function searchTable1(inputVal) {
        var table = $('#divselectedOperators');
        // var table = $('#divselectedOperators');
        table.find('tr').each(function (index, row) {
            var allCells = $(row).find('td');
            if (allCells.length)//&gt; 0
            {
                var found = false;
                allCells.each(function (index, td) {
                    var regExp = new RegExp(inputVal, 'i');
                    if (regExp.test($(td).text())) {
                        found = true;
                        return false;
                    }
                });
                if (found == true) $(row).show(); else $(row).hide();
            }
        });
    }

    $(".manager_Line #Shop_ID").change(function (e) {
        //alert("hi");
        var shopId = $("#Shop_ID").val();
        sFlag = 0;
        clearSelectBox("Line_ID");
        clearSelectBox("Supervisor_ID");
        $('#divListofOperator').empty();
        $('#divselectedOperators').empty();
        if (shopId) {
            sFlag = 1;
            url = "/Line/GetLineByShopID";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopLineType, "json");
        }
        else {

            $('#divListofOperator').empty();
            $('#divselectedOperators').empty();

        }
    });


    $(".manager_Line #Line_ID").change(function (e) {
        var lineId = $("#Line_ID").val();
        $('#divListofOperator').empty();
        $('#divselectedOperators').empty();
        lFlag = 0;
        if (lineId) {
            lFlag = 1;
        }
        if ($('#Leave_From').val().length > 9) {
            $('.Leave_F').html(null);
        }
        else {
            $('.Leave_F').html('Please select from date');
            $('#Leave_From').focus();
            return false;
        }
        if ($('#Leave_TO').val().length > 9) {
            $('.Leave_T').html(null);
        }
        else {
            $('.Leave_T').html('Please select to date');
            $('#Leave_TO').focus();
            return false;
        }
        if (lineId) {
            lFlag = 1;
            url = "/LeaveManagement/GetSupervisorByLineId";
            ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showLineSupervisorType, "json");

        }
        else {
            $('#divListofOperator').empty();
            $('#divselectedOperators').empty();
            clearSelectBox("Supervisor_ID");
        }

    });


    $(".manager_Line #Supervisor_ID").change(function (e) {

        var fromDate = $("#Leave_From").val();
        var toDate = $("#Leave_TO").val();
        var supervisorId = $("#Supervisor_ID").val();
        supFlag = 0;
        if (supervisorId) {
            supFlag = 1;
            url = "/LeaveManagement/GetoperatorsBySupervisorID";
            ajaxpack.getAjaxRequest(url, "supervisorId=" + $("#Supervisor_ID").val() + "&fromDate=" + $("#Leave_From").val() + "&toDate=" + $("#Leave_TO").val() + "", showOperatorsListType, "json");

            setTimeout(function () {
                url = "/LeaveManagement/GetAssignedOperatorsBySupervisorID";
                ajaxpack.getAjaxRequest(url, "supervisorId=" + $("#Supervisor_ID").val() + "", showAssignedOperatorsListType, "json");
            }, 1000);
        }

        else {
            $('#divListofOperator').empty();
            $('#divselectedOperators').empty();
        }

    });


    function showOperatorsListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                // SelectOptionHTML(jsonRes, "ListofOperator");
                var res = "";
                res += ' <table width="100%">';
                for (var i = 0; i < jsonRes.length; i++) {

                    res += '';
                    res += '  <tr class="operator_' + jsonRes[i].Id + '">';
                    res += '    <td width="10px">';
                    res += '         <input type="checkbox" id="@checkBoxId" class="chkclass" value="' + jsonRes[i].Id + '" />';
                    res += '     </td>';
                    res += '      <td id="@tdId" width="100px">';
                    res += '           ' + jsonRes[i].Value;
                    res += '       </td>';
                    res += '    </tr>';
                }
                res += ' </table>';

                $("#divListofOperator").html(res);
            }
        }
    }

    function showAssignedOperatorsListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                var res = "";
                res += ' <table width="100%">';
                for (var i = 0; i < jsonRes.length; i++) {
                    res += '';
                    res += '  <tr class="operator_' + jsonRes[i].Id + '">';
                    res += '    <td width="10px">';
                    res += '         <input type="checkbox" id="@checkBoxId" class="chkclass" value="' + jsonRes[i].Id + '" />';
                    res += '     </td>';
                    res += '      <td id="@tdId" width="100px">';
                    res += '           ' + jsonRes[i].Value;
                    res += '       </td>';
                    res += '    </tr>';
                }
                res += ' </table>';
                $("#divselectedOperators").html(res);
            }
        }
    }


    function showLineSupervisorType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Supervisor_ID");
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


    this.save = function () {
        var supervisorId = $("#Supervisor_ID").val();
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var fromDate = $("#Leave_From").val();
        var toDate = $("#Leave_TO").val();
        var isPlanned = $("#Is_Planned").val();
        var Stations = [];
        var arrlist = [];
        $("#divselectedOperators .chkclass").each(function (e) {
            arrlist.push($(this).val());
        });
        if (sFlag == 1 && lFlag == 1 && supFlag == 1 && $('#Leave_From').val().length > 9 && $('#Leave_TO').val().length > 9) {
            var url = "/LeaveManagement/SaveAssignedOperators";
            ajaxpack.getAjaxRequest(url, "Stations=" + arrlist + "&fromDate=" + $("#Leave_From").val() + "&toDate=" + $("#Leave_TO").val() + "&supervisorId=" + $("#Supervisor_ID").val() + "&shopId=" + $("#Shop_ID").val() + "&lineId=" + $("#Line_ID").val() + "&isPlanned=" + $("#Is_Planned").val(), SaveAssignedOperator, "json");
        }
        else {
            if (sFlag == 0) {
                $('.shop').html("Please select shop");
                $('#Shop_ID').focus();
            }
            else {
                $('.shop').html("");
            }
            if (lFlag == 0) {
                $('.line').html("Please select line");
                $('#Line_ID').focus();
            }
            else {
                $('.line').html("");
            }
            if (supFlag == 0) {
                $('.supervisor').html("Please select supervisor");
                $('#Supervisor_ID').focus();
            }
            else {
                $('.supervisor').html("");
            }
            if ($('#Leave_From').val().length > 9) {
                $('.Leave_F').html(null);
            }
            else {
                $('.Leave_F').html('Please select from date');
                $('#Leave_From').focus();
            }
            if ($('#Leave_TO').val().length > 9) {
                $('.Leave_T').html(null);
            }
            else {
                $('.Leave_T').html('Please select to date');
                $('#Leave_TO').focus();
            }
        }


    }


    function SaveAssignedOperator() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                alert("Selected Cell Member Leave Saved Sucessfully");
                //clearrSelectBox("divNoCriticalStationlist");
                //clearrSelectBox("divCriticalStationlist");
                $('#divListofOperator').empty();
                $('#divselectedOperators').empty();
                //clear();
                //return("Index");
                location.reload();
                //SelectOptionHTML(jsonRes, "Line_ID");
            }
        }
    }


    this.swapItemLeft = function () {
        $("#divListofOperator .chkclass").each(function (e) {
            if ($(this).is(":checked")) {
                //alert($(this).val());
                //alert($(".noCritical_" + $(this).val()).html());
                var res = '<tr class="operator_' + $(this).val() + '">' + $(".operator_" + $(this).val()).html() + '</tr>';
                $("#divselectedOperators table").html($("#divselectedOperators table").html() + res);
                $("#divListofOperator .operator_" + $(this).val()).html("");
                $("#divListofOperator table tr").removeClass("operator_" + $(this).val());
            }
        });
    }


    this.swapItemLeftAll = function () {
        $("#divListofOperator .chkclass").each(function (e) {
            // if ($(this).is(":checked")) {
            // alert($(this).val());
            //alert($(".noCritical_" + $(this).val()).html());
            var res = '<tr class="operator_' + $(this).val() + '">' + $(".operator_" + $(this).val()).html() + '</tr>';
            $("#divselectedOperators table").html($("#divselectedOperators table").html() + res);
            $("#divListofOperator .operator_" + $(this).val()).html("");
            $("#divListofOperator table tr").removeClass("operator_" + $(this).val());
            // }
        });
    }



    this.swapItemRight = function () {
        $("#divselectedOperators .chkclass").each(function (e) {
            if ($(this).is(":checked")) {
                //alert($(this).val());
                // criticalStation=$(this).val();                
                // alert($(".Critical_" + $(this).val()).html());
                var res = '<tr class="operator_' + $(this).val() + '">' + $(".operator_" + $(this).val()).html() + '</tr>';
                $("#divListofOperator table").html($("#divListofOperator table").html() + res);
                $("#divselectedOperators .operator_" + $(this).val()).html("");
                $("#divselectedOperators table tr").removeClass("operator_" + $(this).val());
            }
        });
        //  alert(criticalStation);
    }


    this.swapItemRightAll = function () {
        $("#divselectedOperators .chkclass").each(function (e) {
            // if ($(this).is(":checked")) {
            //alert($(this).val());
            // criticalStation=$(this).val();                
            // alert($(".Critical_" + $(this).val()).html());
            var res = '<tr class="operator_' + $(this).val() + '">' + $(".operator_" + $(this).val()).html() + '</tr>';
            $("#divListofOperator table").html($("#divListofOperator table").html() + res);
            $("#divselectedOperators .operator_" + $(this).val()).html("");
            $("#divselectedOperators table tr").removeClass("operator_" + $(this).val());
            // }
        });
        //  alert(criticalStation);
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