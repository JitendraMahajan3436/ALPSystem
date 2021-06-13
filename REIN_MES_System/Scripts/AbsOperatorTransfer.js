
$(document).ready(function (e) {
    var sFlag = 0;
    var lFlag = 0;
    var mFlag = 0;
    var pFlag = 0;
    var nsFlag = 0;
    $('#Employee_ID').keyup(function () {
        searchTable($(this).val());
    });


    function searchTable(inputVal) {
        var table = $('#divListofSupervisor');
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
        var table = $('#divselectedSupervisors');
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
    $(".manager_Line #Old_Line_ID").change(function (e) {
        var lineid = $("#Old_Line_ID").val();
        if (lineid)
        {
            $('.line').html(null);
        }
    })
    $(".manager_Line #New_Line_ID").change(function (e) {
        var lineid = $("#New_Line_ID").val();
        if (lineid) {
            $('.nline').html(null);
        }
    })
    $(".manager_Line #Plant_ID").change(function (e) {
        var plantId = $("#Plant_ID").val();
        clearSelectBox("Old_Shop_ID");
        clearSelectBox("Old_Line_ID");
        clearSelectBox("Shift_ID");
        $('#divListofSupervisor').empty();
        $('#divselectedSupervisors').empty();
        if (plantId) {
            $('.plant').html(null);
            pFlag = 1;
            url = "/AbsOperatorTransfer/GetShopListByPlantId";
            ajaxpack.getAjaxRequest(url, "plantId=" + $("#Plant_ID").val() + "", showShopPlantListType, "json");
        }
        else {
            clearSelectBox("ListofSupervisor");
            clearSelectBox("selectedSupervisors");
            $('.plant').html("Please select plant");
            pFlag = 0;
        }

    });

    function showShopPlantListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Old_Shop_ID");
                SelectOptionHTML(jsonRes, "New_Shop_ID");
            }
        }
    }



    $(".manager_Line #Old_Shop_ID").change(function (e) {
        debugger;
        var shopId = $("#Old_Shop_ID").val();

        clearSelectBox("Old_Line_ID");
        clearSelectBox("Shift_ID");
        $('#divListofSupervisor').empty();
        $('#divselectedSupervisors').empty();
        if (shopId) {
            $('.shop').html(null);
            sFlag = 1;
            url = "/AbsOperatorTransfer/GetLineListByShopId";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Old_Shop_ID").val() + "", showShopLineListType, "json");

            setTimeout(function () {
                url = "/AbsOperatorTransfer/GetShiftListByShopID";
                ajaxpack.getAjaxRequest(url, "shopId=" + $("#Old_Shop_ID").val() + "", showShopShiftListType, "json");
            }, 1000);
        }
        else {
            clearSelectBox("ListofSupervisor");
            clearSelectBox("selectedSupervisors");
            $('.shop').html("Please select shop");
            sFlag = 0;
        }

    });

    function showShopLineListType() {
        debugger;
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                debugger;
                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Old_Line_ID");
            }
        }
    }

    function showShopShiftListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Shift_ID");
            }
        }
    }

    $(".manager_Line #New_Shop_ID").change(function (e) {
        var shopId = $("#New_Shop_ID").val();

        clearSelectBox("New_Line_ID");
        $('#divListofSupervisor').empty();
        $('#divselectedSupervisors').empty();
        if (shopId) {
            $('.nshop').html(null);
            nsFlag = 1;
            url = "/AbsOperatorTransfer/GetNewLineListByNewShopId";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#New_Shop_ID").val() + "", showNewShopLineListType, "json");
        }
        else {
            clearSelectBox("ListofSupervisor");
            clearSelectBox("selectedSupervisors");
            $('.nshop').html("Please select shop");
            nsFlag = 0;
        }

    });

    function showNewShopLineListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "New_Line_ID");
            }
        }
    }

    $(".manager_Line #Shift_ID").change(function (e) {
        var shiftId = $("#Shift_ID").val();
        var shopId = $("#Old_Shop_ID").val();
        var lineId = $("#Old_Line_ID").val();
        var plantId = $("#Plant_ID").val();
        var nshopId = $("#New_Shop_ID").val();
        var nlineId = $("#New_Line_ID").val();
        clearSelectBox("ListofSupervisor");
        clearSelectBox("selectedSupervisors");
        $('#divListofSupervisor').empty();
        $('#divselectedSupervisors').empty();
        if (shiftId) {
            mFlag = 1;
            $('.shift').html("");
           
            url = "/AbsOperatorTransfer/GetAbsOperatorByShiftID";
            ajaxpack.getAjaxRequest(url, "shiftId=" + $("#Shift_ID").val() + "" + "&shopId=" + shopId + "" + "&lineId=" + lineId + "", showSupervisorsListType, "json");  //+ "&shopId=" + $("#Shop_ID").val()

           
            setTimeout(function () {
               
                url = "/AbsOperatorTransfer/GetTransferOperatorByShiftID";
                ajaxpack.getAjaxRequest(url, "shiftId=" + $("#Shift_ID").val() + "" + "&shopId=" + shopId + "" + "&lineId=" + lineId + "", showAssignedSupervisorsListType, "json");
            }, 1000);
        }
        else {
            mFlag = 0;
            clearSelectBox("ListofSupervisor");
            clearSelectBox("selectedSupervisors");
            $('.shift').html("Please select Shift");
        }




    });



    function showSupervisorsListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                
                var jsonRes = $.parseJSON(myajax.responseText);
               
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

                $("#divListofSupervisor").html(res);
            }
        }
    }

    function showAssignedSupervisorsListType() {

        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                
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

                $("#divselectedSupervisors").html(res);

            }
        }
    }

    this.save = function () {
        debugger;
        var shiftId = $("#Shift_ID").val();
        var plantId = $("#Plant_ID").val();
        var shopId = $("#Old_Shop_ID").val();
        var lineId = $("#Old_Line_ID").val();
        var nshopId = $("#New_Shop_ID").val();
        var nlineId = $("#New_Line_ID").val();
        //var checkBoxId=$("#@checkBoxId").val();
        var Stations = [];
        var arrlist = [];
        $("#divselectedSupervisors .chkclass").each(function (e) {
            
            arrlist.push($(this).val());
         
        });
        if (sFlag == 1 && mFlag == 1 && lineId > 0 && nshopId > 0 && nlineId > 0) {
            var url = "/AbsOperatorTransfer/SaveTranserAbsOperators";
            ajaxpack.getAjaxRequest(url, "Operators=" + arrlist + "&shiftId=" + shiftId + "&shopId=" + shopId + "&lineId=" + lineId  + "&nshopId=" + nshopId + "&nlineId=" + nlineId, SaveAssignedStation, "json");
        }
        else {
            if (mFlag == 0) {
                $('.shift').html("Please select shift");
                $('#Shift_ID').focus();
            }
            if (plantId == 0) {
                $('.plant').html("Please select Plant");
                $('#Plant_ID').focus();
            }
            if (nshopId == 0) {
                $('.nshop').html("Please select New Shop");
                $('#New_Shop_ID').focus();
            }
            if (nlineId == 0) {
                $('.nline').html("Please select New Line");
                $('#New_Line_ID').focus();
            }
            if (sFlag == 0) {
                $('.shop').html("Please select shop");
                $('#Old_Shop_ID').focus();
            }
            if (lFlag == 0) {
                $('.line').html("Please select line");
                $('#Old_Line_ID').focus();
            }
            //if (checkBoxId == 0) {
            //    $('.line').html("Please select line");
            //    $('#Old_Line_ID').focus();
            //}
        }


    }


    function SaveAssignedStation() {
        debugger;
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                debugger;
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




    this.swapItemLeft = function () {
        var count = 0;
        $("#divListofSupervisor .chkclass").each(function (e) {
            if ($(this).is(":checked")) {
                count = 1;
                var res = '<tr class="supervisor_' + $(this).val() + '">' + $(".supervisor_" + $(this).val()).html() + '</tr>';
                $("#divselectedSupervisors table").html($("#divselectedSupervisors table").html() + res);
                $("#divListofSupervisor .supervisor_" + $(this).val()).html("");
                $("#divListofSupervisor table tr").removeClass("supervisor_" + $(this).val());
            }
        });
        if (count == 0) {
            $('#testy').toastee({
                type: 'error',
                width: '200px',
                height: '100px',
                message: 'Please select operator...',
            });
        }
    }


    this.swapItemLeftAll = function () {
        $("#divListofSupervisor .chkclass").each(function (e) {
            // if ($(this).is(":checked")) {
            // alert($(this).val());
            //alert($(".noCritical_" + $(this).val()).html());
            var res = '<tr class="supervisor_' + $(this).val() + '">' + $(".supervisor_" + $(this).val()).html() + '</tr>';
            $("#divselectedSupervisors table").html($("#divselectedSupervisors table").html() + res);
            $("#divListofSupervisor .supervisor_" + $(this).val()).html("");
            $("#divListofSupervisor table tr").removeClass("supervisor_" + $(this).val());
            // }
        });
    }



    this.swapItemRight = function () {
        var count = 0;
        $("#divselectedSupervisors .chkclass").each(function (e) {
            if ($(this).is(":checked")) {
                count = 1;
                var res = '<tr class="supervisor_' + $(this).val() + '">' + $(".supervisor_" + $(this).val()).html() + '</tr>';
                $("#divListofSupervisor table").html($("#divListofSupervisor table").html() + res);
                $("#divselectedSupervisors .supervisor_" + $(this).val()).html("");
                $("#divselectedSupervisors table tr").removeClass("supervisor_" + $(this).val());
            }
        });

        if (count == 0) {
            $('#testy').toastee({
                type: 'error',
                width: '200px',
                height: '100px',
                message: 'Please select operator...',
            });
        }
    }


    this.swapItemRightAll = function () {
        $("#divselectedSupervisors .chkclass").each(function (e) {
            // if ($(this).is(":checked")) {
            //alert($(this).val());
            // criticalStation=$(this).val();                
            // alert($(".Critical_" + $(this).val()).html());
            var res = '<tr class="supervisor_' + $(this).val() + '">' + $(".supervisor_" + $(this).val()).html() + '</tr>';
            $("#divListofSupervisor table").html($("#divListofSupervisor table").html() + res);
            $("#divselectedSupervisors .supervisor_" + $(this).val()).html("");
            $("#divselectedSupervisors table tr").removeClass("supervisor_" + $(this).val());
            // }
        });
        //  alert(criticalStation);
    }



    $(".select_manager").click(function (e) {

        $('#selectedSupervisors option').prop('selected', true);


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
            $('#selectedSupervisors option').prop('selected', true);
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





//function clear() {
//    clearSelectBox("Line_ID");
//    clearSelectBox("Shop_ID");
//    clearSelectBox("Plant_ID");

//}

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