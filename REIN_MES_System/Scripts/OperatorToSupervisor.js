
$(document).ready(function (e) {
    var sFlag = 0;
    var lFlag = 0;
    var oFlag = 0;
    $('#AssignedOperator_ID').keyup(function () {
        searchTable($(this).val());
    });


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

    //$("#Shop_ID").select2({
    //    allowClear: true
    //});

    //$("#Line_ID").select2({
    //    allowClear: true
    //});

    //$("#Supervisor_ID").select2({
    //    allowClear: true
    //});

    $(".manager_Line #Shop_ID").change(function (e) {

        var shopId = $("#Shop_ID").val();
        clearSelectBox("Line_ID");
        clearSelectBox("Supervisor_ID");//LINE OFFICER
        $('#divListofOperator').empty();
        $('#divselectedOperators').empty();
        if (shopId) {
            //var jsonData = JSON.stringify({ plantId: 7 });
            $('.shop').html(null);
            sFlag = 1;
            url = "/Line/GetLineByShopID";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopLineType, "json");
        }
        else {
            clearSelectBox("ListofOperator");
            clearSelectBox("selectedOperators");
            $('.shop').html("Please select shop");
            sFlag = 0;
        }
    });

    $(".manager_Line #Line_ID").change(function (e) {
        // var jsonData = JSON.stringify({ plantId: 7 });
        var lineId = $("#Line_ID").val();

        clearSelectBox("Supervisor_ID");//LINE OFFICER
        $('#divListofOperator').empty();
        $('#divselectedOperators').empty();
        if (lineId) {
            $('.line').html(null);
            lFlag = 1;
            url = "/AssignOperatorToSupervisor/GetSupervisorByLineId";
            ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showLineSupervisorType, "json");

        }
        else {
            clearSelectBox("ListofOperator");
            clearSelectBox("selectedOperators");
            $('.line').html("Please select line");
            lFlag = 0;
        }

    });


    $(".manager_Line #Type").change(function (e) {
        var typeId = $("#Type").val();
        // alert(typeId);                                                                                                                    
        if (typeId == "0") {
            //alert("hi");
            window.location = "/AssignOperatorToSupervisor/Create";
            // $("#Type").val("False");
            //$("#Type").val("False");
            // $("#Type").attr('selectedIndex', typeId);
        }
        else {
            window.location = "/AssignSupervisorToManager/Create";
            // $("#Type").val("True");
            // $("#Type").attr('selectedIndex', typeId);
        }

    });


    $(".manager_Line #Supervisor_ID").change(function (e) {
        var supervisorId = $("#Supervisor_ID").val();
        var lineId = $("#Line_ID").val();
        $('#divListofOperator').empty();
        $('#divselectedOperators').empty();
        if (supervisorId) {
            $('.officer').html(null);
            oFlag = 1;
            //var jsonData = JSON.stringify({ plantId: 7 });
            url = "/AssignOperatorToSupervisor/GetoperatorsBySupervisorID";
            ajaxpack.getAjaxRequest(url, "supervisorId=" + $("#Supervisor_ID").val() + "", showOperatorsListType, "json");


            setTimeout(function () {
                // var jsonData = JSON.stringify({ plantId: 7 });

                url = "/AssignOperatorToSupervisor/GetAssignedOperatorsByLineIDSupervisorID";
                ajaxpack.getAjaxRequest(url, "lineId=" + lineId + "&supervisorId=" + $("#Supervisor_ID").val() + "", showAssignedOperatorsListType, "json");
            }, 1000);
        }

        else {
            clearSelectBox("ListofOperator");
            clearSelectBox("selectedOperators");
            $('.officer').html("Please select officer");
            oFlag = 0;
        }

    });

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


    function showAssignedOperatorsListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                //SelectOptionHTML(jsonRes, "selectedOperators");
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


    this.save = function () {
        var supervisorId = $("#Supervisor_ID").val();
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var Stations = [];
        var arrlist = [];
        $("#divselectedOperators .chkclass").each(function (e) {
            //if ($(this).is(":checked")) {
            // alert($(this).val());
            // alert(lineId);
            // arr = $(this).val();
            arrlist.push($(this).val());
            // alert($(".Critical_" + $(this).val()).html());                
            // }
        });
        if (sFlag == 1 && lFlag == 1 && oFlag == 1) {
            var url = "/AssignOperatorToSupervisor/SaveAssignedOperators";
            ajaxpack.getAjaxRequest(url, "Stations=" + arrlist + "&supervisorId=" + $("#Supervisor_ID").val() + "&shopId=" + $("#Shop_ID").val() + "&lineId=" + $("#Line_ID").val(), SaveAssignedOperator, "json");
        }
        else {
            if (lFlag == 0) {
                $('.line').html("Please select line");
                $('#Line_ID').focus();
            }
            if (sFlag == 0) {
                $('.shop').html("Please select shop");
                $('#Shop_ID').focus();
            }
            if (oFlag == 0) {
                $('.officer').html("Please select officer");
                $('#Supervisor_ID').focus();
            }
        }
    }


    function SaveAssignedOperator() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                /// alert("Assigned Operators Saved Sucessfully");

                $('#testy').toastee({
                    type: 'success',
                    width: '300px',
                    height: '100px',
                    message: 'Assigned Operators Saved Successfully...',
                });


                $("#divselectedOperators").html("");
                $("#divListofOperator").html("");
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


    this.swapItemLeft = function () {
        var count = 0;
        $("#divListofOperator .chkclass").each(function (e) {
            if ($(this).is(":checked")) {
                count = 1;
                var res = '<tr class="operator_' + $(this).val() + '">' + $(".operator_" + $(this).val()).html() + '</tr>';
                $("#divselectedOperators table").html($("#divselectedOperators table").html() + res);
                $("#divListofOperator .operator_" + $(this).val()).html("");
                $("#divListofOperator table tr").removeClass("operator_" + $(this).val());
            }
        });

        if (count == 0) {
            $('#testy').toastee({
                type: 'error',
                width: '200px',
                height: '100px',
                message: 'Please select cell member...',
            });
        }

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
        var count = 0;
        $("#divselectedOperators .chkclass").each(function (e) {
            if ($(this).is(":checked")) {
                count = 1;
                var res = '<tr class="operator_' + $(this).val() + '">' + $(".operator_" + $(this).val()).html() + '</tr>';
                $("#divListofOperator table").html($("#divListofOperator table").html() + res);
                $("#divselectedOperators .operator_" + $(this).val()).html("");
                $("#divselectedOperators table tr").removeClass("operator_" + $(this).val());
            }
        });
        if (count == 0) {
            $('#testy').toastee({
                type: 'error',
                width: '200px',
                height: '100px',
                message: 'Please select cell member...',
            });
        }

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


    window.swapValue = function (sourceId, targetId, direction) {
        if (direction == ">") {
            // source to target
            $("#" + sourceId + " :selected").each(function (i, selected) {
                if ($(selected).val() == "" || $(selected).val() == null) {

                }
                else {
                    $("#" + targetId).append($('<option>', {
                        value: $(selected).val(),
                        text: $(selected).text()
                    }));
                    //foo[i] = $(selected).text();

                    // remove item from source
                    $("#" + sourceId + " option[value='" + $(selected).val() + "']").remove();
                }

            });
        }
        else {
            // target to source
        }
    }

    $(".select_defect").click(function (e) {
        //$('#select_all').click(function () {
        $('#selectedOperators option').prop('selected', true);
        //});

        return true;
    });

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