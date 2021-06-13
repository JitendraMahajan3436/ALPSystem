
$(document).ready(function (e) {
    var sFlag = 0;
    var lFlag = 0;
    var stFlag = 0;
    var mFlag = 0;
    $('#Maintenance_Part_ID').keyup(function () {
        searchTable($(this).val());
    });


    function searchTable(inputVal) {
        var table = $('#divListofPart');
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

    $('#selectedParts').keyup(function () {
        searchTable1($(this).val());
    });


    function searchTable1(inputVal) {
        var table = $('#divselectedParts');
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

        var shopId = $("#Shop_ID").val();
        clearSelectBox("Line_ID");
        clearSelectBox("Station_ID");
        clearSelectBox("Machine_ID");
        $('#divListofPart').empty();
        $('#divselectedParts').empty();
        if (shopId) {
            //var jsonData = JSON.stringify({ plantId: 7 });
            $('.shop').html(null);
            sFlag = 1;
            url = "/Line/GetLineByShopID";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopLineType, "json");

            setTimeout(function () {
                // var jsonData = JSON.stringify({ plantId: 7 });
                url = "/MaintenanceMachinePart/GetMachineByShopId";
                ajaxpack.getAjaxRequest(url, "Shop_ID=" + $("#Shop_ID").val() + "", showShopMachineType, "json");
            }, 2000);
        }
        else {
            clearSelectBox("divListofPart");
            clearSelectBox("divselectedParts");
            $('.shop').html("Please select shop");
            sFlag = 0;
        }
    });

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

    function showShopMachineType() {

        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes.length > 0) {
                    $('#Machine_ID option').remove();
                    $('#Machine_ID').append('<option value="">Select Machine Name</option>');
                    for (var i = 0; i < jsonRes.length; i++) {
                        $('#Machine_ID').append('<option value="' +
                                   jsonRes[i].Id + '">' + jsonRes[i].Value + '</option>');
                    }
                }
                //SelectOptionHTML(jsonRes, "Machine_ID");
            }
        }
    }

    $(".manager_Line #Line_ID").change(function (e) {
        debugger;
        // var jsonData = JSON.stringify({ plantId: 7 });
        var lineId = $("#Line_ID").val();

        clearSelectBox("Station_ID");
        //clearSelectBox("Machine_ID");
        $('#divListofPart').empty();
        $('#divselectedParts').empty();
        if (lineId) {
            $('.line').html(null);
            lFlag = 1;
            url = "/Station/GetStationListByLineID";
            ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showLineStation, "json");

        }
        else {
            clearSelectBox("divListofPart");
            clearSelectBox("divselectedParts");
            $('.line').html("Please select line");
            lFlag = 0;
        }

    });

    function showLineStation() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Station_ID");
            }
        }
    }

    $(".manager_Line #Type").change(function (e) {
        var typeId = $("#Type").val();
        // alert(typeId);
        if (typeId != "1") {
            window.location = "/MaintenanceMachinePart/Create";
            // $("#Type").val(true);
        }
    });


    $(".manager_Line #Machine_ID").change(function (e) {
        //var managerId = $("#Manager_ID").val();
        var machineId = $("#Machine_ID").val();
        clearSelectBox("divListofPart");
        clearSelectBox("divselectedParts");
        //clearSelectBox("Line_ID");
        if (machineId) {
            $('.machine').html(null);
            mFlag = 1;
            //this method is used for showing the list officer list against the shop selected
            // var jsonData = JSON.stringify({ plantId: 7 });
            url = "/MaintenanceMachinePart/GetPartsByMachineID";
            ajaxpack.getAjaxRequest(url, "machineId=" + $("#Machine_ID").val() + "", showPartsListType, "json");  //+ "&shopId=" + $("#Shop_ID").val()

            // //this method is used for showing the list assigned officer list against the shop selected
            setTimeout(function () {
                // var jsonData = JSON.stringify({ plantId: 7 });
                url = "/MaintenanceMachinePart/GetAssignedPartsByMachineID";
                ajaxpack.getAjaxRequest(url, "machineId=" + $("#Machine_ID").val() + "", showAssignedPartsListType, "json");
            }, 1000);
        }
        else {
            clearSelectBox("divListofPart");
            clearSelectBox("divselectedParts");
            $('.machine').html("Select Machine Name");
            mFlag = 0;
        }




    });

    function showAssignedPartsListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                // SelectOptionHTML(jsonRes, "selectedSupervisors");
                var res = "";
                res += ' <table width="100%">';
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

                $("#divselectedParts").html(res);

            }
        }
    }

    function showPartsListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                //alert("hi");
                var jsonRes = $.parseJSON(myajax.responseText);
                //SelectOptionHTML(jsonRes, "ListofSupervisor");
                var res = "";
                res += ' <table width="100%">';
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

                $("#divListofPart").html(res);
            }
        }
    }

    this.save = function () {
        debugger;
        //var managerId = $("#Manager_ID").val();
        var machineId = $("#Machine_ID").val();
        var plantId = $("#Plant_ID").val();
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var stationId = $("#Station_ID").val();
        var Parts = [];
        var arrlist = [];


        $("#divselectedParts .chkclass").each(function (e) {

            arrlist.push($(this).val());
        });
        if (sFlag == 1 && lFlag == 1 && mFlag == 1) {
            var url = "/MaintenanceMachinePart/SaveAssignedPart";
            ajaxpack.getAjaxRequest(url, "Parts=" + arrlist + "&machineId=" + $("#Machine_ID").val() + "&plantId=" + $("#Plant_ID").val() + "&shopId=" + $("#Shop_ID").val() + "&lineId=" + $("#Line_ID").val() + "&stationId=" + $("#Station_ID").val(), SaveAssignedPart, "json");
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
            if (mFlag == 0) {
                $('.machine').html("Please select machine");
                $('#Machine_ID').focus();
            }
        }
    }


    function SaveAssignedPart() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);

                if (jsonRes.Status == true) {
                    if (jsonRes.Message == "") {
                        $('#testy').toastee({
                            type: 'success',
                            width: '200px',
                            height: '100px',
                            message: 'Assigned Part Saved Sucessfully',
                        });
                        //alert("Assigned Part Saved Sucessfully");
                    }
                    else {
                        $('#testy').toastee({
                            type: 'success',
                            width: '200px',
                            height: '100px',
                            message: "Assigned Part Saved Sucessfully ! following parts can not delete :" + "\n\n" + jsonRes.Message,
                        });
                        //alert("Assigned Part Saved Sucessfully ! following parts can not delete :"+"\n\n" + jsonRes.Message);
                    }
                    $("#divListofPart").html("");
                    $("#divselectedParts").html("");
                    //clear();
                    setTimeout(function () {
                        location.reload();
                    }, 2000);
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

    }

    this.swapItemLeft = function () {
        var flag = 0;
        $("#divListofPart .chkclass").each(function (e) {
            if ($(this).is(":checked")) {
                flag = 1;
                var res = '<tr class="supervisor_' + $(this).val() + '">' + $(".supervisor_" + $(this).val()).html() + '</tr>';
                $("#divselectedParts table").html($("#divselectedParts table").html() + res);
                $("#divListofPart .supervisor_" + $(this).val()).html("");
                $("#divListofPart table tr").removeClass("supervisor_" + $(this).val());
            }
        });
        if (flag == 0) {
            $('#testy').toastee({
                type: 'error',
                width: '200px',
                height: '100px',
                message: 'please select part...',
            });

        }
    }


    this.swapItemLeftAll = function () {
        $("#divListofPart .chkclass").each(function (e) {
            // if ($(this).is(":checked")) {
            // alert($(this).val());
            //alert($(".noCritical_" + $(this).val()).html());
            var res = '<tr class="supervisor_' + $(this).val() + '">' + $(".supervisor_" + $(this).val()).html() + '</tr>';
            $("#divselectedParts table").html($("#divselectedParts table").html() + res);
            $("#divListofPart .supervisor_" + $(this).val()).html("");
            $("#divListofPart table tr").removeClass("supervisor_" + $(this).val());
            // }
        });
    }



    this.swapItemRight = function () {
        var flag = 0;
        $("#divselectedParts .chkclass").each(function (e) {
            if ($(this).is(":checked")) {
                flag = 1;
                var res = '<tr class="supervisor_' + $(this).val() + '">' + $(".supervisor_" + $(this).val()).html() + '</tr>';
                $("#divListofPart table").html($("#divListofPart table").html() + res);
                $("#divselectedParts .supervisor_" + $(this).val()).html("");
                $("#divselectedParts table tr").removeClass("supervisor_" + $(this).val());
            }
        });
        if (flag == 0) {
            $('#testy').toastee({
                type: 'error',
                width: '200px',
                height: '100px',
                message: 'please select part.',
            });
        }

    }


    this.swapItemRightAll = function () {
        $("#divselectedParts .chkclass").each(function (e) {
            // if ($(this).is(":checked")) {
            //alert($(this).val());
            // criticalStation=$(this).val();                
            // alert($(".Critical_" + $(this).val()).html());
            var res = '<tr class="supervisor_' + $(this).val() + '">' + $(".supervisor_" + $(this).val()).html() + '</tr>';
            $("#divListofPart table").html($("#divListofPart table").html() + res);
            $("#divselectedParts .supervisor_" + $(this).val()).html("");
            $("#divselectedParts table tr").removeClass("supervisor_" + $(this).val());
            // }
        });
        //  alert(criticalStation);
    }



    $(".select_manager").click(function (e) {

        $('#selectedParts option').prop('selected', true);


        return true;
    });

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

    $(".select_manager").click(function (e) {
        $('#select_all').click(function () {
            $('#selectedParts option').prop('selected', true);
        });

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





function clear() {
    clearSelectBox("Line_ID");
    clearSelectBox("Shop_ID");
    clearSelectBox("Plant_ID");

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