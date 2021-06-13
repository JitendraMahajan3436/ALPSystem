
$(document).ready(function (e) {
    var sFlag = 0;
    var mFlag = 0;
    $('#AssignedSupervisor_ID').keyup(function () {
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

    $('#selectedSupervisors').keyup(function () {
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

    //$("#Shop_ID").select2({
    //    allowClear: true
    //});

    //$("#Manager_ID").select2({
    //    allowClear: true
    //});

    $(".manager_Line #Shop_ID").change(function (e) {
        var shopId = $("#Shop_ID").val();

        // clearSelectBox("ListofSupervisor");
        clearSelectBox("Manager_ID");
        $('#divListofSupervisor').empty();
        $('#divselectedSupervisors').empty();
        // clearSelectBox("Line_ID");
        if (shopId) {
            $('.shop').html(null);
            sFlag = 1;
            //this method is used for AssignSupervisorToManager form to get manager list against that shop
            url = "/AssignSupervisorToManager/GetManagerListByShopId";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopManagerListType, "json");
        }
        else {
            clearSelectBox("ListofSupervisor");
            clearSelectBox("selectedSupervisors");
            $('.shop').html("Please select shop");
            sFlag = 0;
        }

    });

    $(".manager_Line #Type").change(function (e) {
        var typeId = $("#Type").val();
        // alert(typeId);
        if (typeId == "1") {
            //alert("hi");
            window.location = "/AssignOperatorToSupervisor/Create";
            // $("#Type").val(false);
        }
        else {
            window.location = "/AssignSupervisorToManager/Create";
            // $("#Type").val(true);
        }

    });


    $(".manager_Line #Manager_ID").change(function (e) {
        var managerId = $("#Manager_ID").val();
        var shopId = $("#Shop_ID").val();
        clearSelectBox("ListofSupervisor");
        clearSelectBox("selectedSupervisors");
        $('#divListofSupervisor').empty();
        $('#divselectedSupervisors').empty();
        if (managerId) {
            mFlag = 1;
            $('.manager').html("");
            //this method is used for showing the list officer list against the shop selected
            // var jsonData = JSON.stringify({ plantId: 7 });
            url = "/AssignSupervisorToManager/GetSupervisorsByManagerID";
            ajaxpack.getAjaxRequest(url, "managerId=" + $("#Manager_ID").val() + "" + "&shopId=" + shopId, showSupervisorsListType, "json");  //+ "&shopId=" + $("#Shop_ID").val()

            // //this method is used for showing the list assigned officer list against the shop selected
            setTimeout(function () {
                // var jsonData = JSON.stringify({ plantId: 7 });
                url = "/AssignSupervisorToManager/GetAssignedSupervisorsByManagerID";
                ajaxpack.getAjaxRequest(url, "managerId=" + $("#Manager_ID").val() + "" + "&shopId=" + shopId, showAssignedSupervisorsListType, "json");
            }, 1000);
        }
        else {
            mFlag = 0;
            clearSelectBox("ListofSupervisor");
            clearSelectBox("selectedSupervisors");
            $('.manager').html("Please select manager");
        }




    });



    function showSupervisorsListType() {

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

                $("#divselectedSupervisors").html(res);

            }
        }
    }

    this.save = function () {
        var managerId = $("#Manager_ID").val();
        var shopId = $("#Shop_ID").val();
        var Stations = [];
        var arrlist = [];
        $("#divselectedSupervisors .chkclass").each(function (e) {
            //if ($(this).is(":checked")) {
            // alert($(this).val());
            // alert(lineId);
            // arr = $(this).val();
            arrlist.push($(this).val());
            // alert($(".Critical_" + $(this).val()).html());                
            // }
        });
        if (sFlag == 1 && mFlag == 1) {
            var url = "/AssignSupervisorToManager/SaveAssignedSupervisor";
            ajaxpack.getAjaxRequest(url, "Stations=" + arrlist + "&managerId=" + $("#Manager_ID").val() + "&shopId=" + $("#Shop_ID").val(), SaveAssignedSupervisor, "json");
        }
        else {
            if (mFlag == 0) {
                $('.manager').html("Please select manager");
                $('#Manager_ID').focus();
            }
            if (sFlag == 0) {
                $('.shop').html("Please select shop");
                $('#Shop_ID').focus();
            }
        }


    }


    function SaveAssignedSupervisor() {
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
                    message: 'Assigned Supervisor Saved Successfully...',
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

    function showShopManagerListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Manager_ID");
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
                message: 'Please select line officer...',
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
                message: 'Please select line officer...',
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