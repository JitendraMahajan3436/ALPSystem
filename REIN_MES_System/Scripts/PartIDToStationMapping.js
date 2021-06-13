
$(document).ready(function (e) {
    var sFlag = 0;
    var lFlag = 0;
    var mFlag = 0;
    var pFlag = 0;
    $('#PartID').keyup(function () {
        searchTable($(this).val());
    });


    function searchTable(inputVal) {
        var table = $('#divListofPartID');
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

    $('#selectedPartID').keyup(function () {
        searchTable1($(this).val());
    });


    function searchTable1(inputVal) {
        var table = $('#divselectedPartID');
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
        $('#divListofPartID').empty();
        $('#divselectedPartID').empty();
        if (shopId) {
            $('.shop').html(null);
            sFlag = 1;
            url = "/PartIDStationConfig/GetLineListByShopId";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopLineListType, "json");
        }
        else {
            clearSelectBox("ListofPartID");
            clearSelectBox("selectedPartID");
            $('.shop').html("Please select shop");
            sFlag = 0;
        }

    });

    function showShopLineListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Line_ID");
            }
        }
    }

    $(".manager_Line #Line_ID").change(function (e) {
        var lineId = $("#Line_ID").val();


        clearSelectBox("Station_ID");
        $('#divListofPartID').empty();
        $('#divselectedPartID').empty();
        if (lineId) {
            $('.line').html(null);
            lFlag = 1;
            url = "/PartIDStationConfig/GetSetupListByLineId";
            ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showLineSetupListType, "json");
        }
        else {
            clearSelectBox("ListofPartID");
            clearSelectBox("selectedPartID");
            $('.line').html("Please select line");
            lFlag = 0;
        }

    });

    function showLineSetupListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Station_ID");
            }
        }
    }

    $(".manager_Line #Station_ID").change(function (e) {
        var StationId = $("#Station_ID").val();
        var shopId = $("#Shop_ID").val();
        var lineId = $("#Line_ID").val();
        var plantId = $('#Plant_ID').val();
        clearSelectBox("ListofPartID");
        clearSelectBox("selectedPartID");
        $('#divListofPartID').empty();
        $('#divselectedPartID').empty();
        if (StationId) {
            mFlag = 1;
            $('.Station').html("");
            //this method is used for showing the station list against the setup selected
            // var jsonData = JSON.stringify({ plantId: 7 });
            url = "/PartIDStationConfig/GetPartIDBySatationID";
            ajaxpack.getAjaxRequest(url, "StationId=" + $("#Station_ID").val() + "" + "&shopId=" + shopId + "" + "&lineId=" + lineId + "" + "&plantId=" + plantId, showSupervisorsListType, "json");  

            // //this method is used for showing the list assigned station list against the setup selected
            setTimeout(function () {
                // var jsonData = JSON.stringify({ plantId: 7 });
                url = "/PartIDStationConfig/GetAssignedPartIDByStationID";
                ajaxpack.getAjaxRequest(url, "StationId=" + $("#Station_ID").val() + "" + "&shopId=" + shopId + "" + "&lineId=" + lineId + "" + "&plantId=" + plantId, showAssignedSupervisorsListType, "json");
            }, 1000);
        }
        else {
            mFlag = 0;
            clearSelectBox("ListofSupervisor")
            clearSelectBox("selectedSupervisors");
            $('.Station').html("Please select station");
        }




    });



    function showSupervisorsListType() {
        debugger;
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
                    res += '           ' + jsonRes[i].Value + '(' + jsonRes[i].Desc + ')';
                    res += '       </td>';
                    res += '    </tr>';
                }
                res += ' </table>';

                $("#divListofPartID").html(res);
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
                res += '<table width="100%">';
                for (var i = 0; i < jsonRes.length; i++) {
                    res += '';
                    res += '<tr class="supervisor_' + jsonRes[i].Id + '">';
                    res += '<td width="10px">';
                    res += '<input type="checkbox" id="@checkBoxId" class="chkclass" value="' + jsonRes[i].Id + '" />';
                    res += '</td>';
                    res += '<td id="@tdId" width="100px">';
                    res += '' + jsonRes[i].Value + '(' + jsonRes[i].Desc + ')';
                    res += '</td>';
                    res += '</tr>';
                }
                res += '</table>';

                $("#divselectedPartID").html(res);

            }
        }
    }
    this.Yes = function () {
        var StationId = $("#Station_ID").val();
        var shopId = $("#Shop_ID").val();
        var plantId = $("#Plant_ID").val();
        var lineId = $("#Line_ID").val();
        var PartIDList = [];
        var arrlist = [];
        $("#divselectedPartID .chkclass").each(function (e) {

            arrlist.push($(this).val());
          
        });
        var url = "/PartIDStationConfig/SaveAssignedPartIds";
        ajaxpack.getAjaxRequest(url, "PartIDList=" + arrlist + "&StationId=" + $("#Station_ID").val() + "&shopId=" + $("#Shop_ID").val() + "&lineId=" + $("#Line_ID").val() + "&plantId=" + $("#Plant_ID").val(), SaveAssignedStation, "json");
    }
    this.No = function () {
        var StationId = $("#Station_ID").val();
        var shopId = $("#Shop_ID").val();
        var plantId = $("#Plant_ID").val();
        var lineId = $("#Line_ID").val();
        var PartIDList = [];
        var arrlist = [];
        $("#divselectedPartID .chkclass").each(function (e) {

            arrlist.push($(this).val());

        });
        var url = "/PartIDStationConfig/NoallredyAssignedPartIds";
        ajaxpack.getAjaxRequest(url, "PartIDList=" + arrlist + "&StationId=" + $("#Station_ID").val() + "&shopId=" + $("#Shop_ID").val() + "&lineId=" + $("#Line_ID").val() + "&plantId=" + $("#Plant_ID").val(), SaveAssignedStation, "json");
    }

    this.save = function () {
     
        var StationId = $("#Station_ID").val();
        var shopId = $("#Shop_ID").val();
        var plantId = $("#Plant_ID").val();
        var lineId = $("#Line_ID").val();
        var PartIDList = [];
        var arrlist = [];
        $("#divselectedPartID .chkclass").each(function (e) {
       
            arrlist.push($(this).val());
            // alert($(".Critical_" + $(this).val()).html());                
            // }
        });
        //if (sFlag == 1 && mFlag == 1 && lFlag == 1 > lineId > 0 && plantId > 0 && StationId > 0) {
        //url = "/PartIDStationConfig/GetAssignedPartIDByStationID";
        var url = "/PartIDStationConfig/CheckAssignedPartIds";
        ajaxpack.getAjaxRequest(url, "PartIDList=" + arrlist + "&StationId=" + $("#Station_ID").val() + "&shopId=" + $("#Shop_ID").val() + "&lineId=" + $("#Line_ID").val() + "&plantId=" + $("#Plant_ID").val(), CheckAssignedPartID, "json");
            //var url = "/PartIDStationConfig/SaveAssignedPartIds";
            //ajaxpack.getAjaxRequest(url, "PartIDList=" + arrlist + "&StationId=" + $("#Station_ID").val() + "&shopId=" + $("#Shop_ID").val() + "&lineId=" + $("#Line_ID").val() + "&plantId=" + $("#Plant_ID").val(), SaveAssignedStation, "json");
        //}
        //else {           
        //    if (sFlag == 0) {
        //        $('.shop').html("Please select shop");
        //        $('#Shop_ID').focus();
        //    }
        //    if (lineId == 0) {
        //        $('.line').html("Please select Line");
        //        $('#Line_ID').focus();
        //    }
        //    if (StationId == 0) {
        //        $('.Station').html("Please select Station_ID");
        //        $('#Station_ID').focus();
        //    }
        //}


    }

    function CheckAssignedPartID()
    {
        var id = "PartId";
        var station = "Station";
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                //alert("hi");
             
                debugger;
                var jsonRes = $.parseJSON(myajax.responseText);
                //SelectOptionHTML(jsonRes, "ListofSupervisor");
              
                var res = "";
                res += ' <table class="table table-striped table-bordered">';
                res += '  <tr>';
                res += '    <th width="10px">' + id + '</th>';
                res += '    <th width="10px">' + station + '</th>';
                res += '    </tr>';              
                for (var i = 0; i < jsonRes.length; i++) {
                 
                    //res += '';
                    res += '  <tr>';
                    res += '    <td width="10px">';
                    res += '       ' + jsonRes[i].PartID;
                    res += '     </td>';
                    res += '      <td width="100px">';
                    res += '           ' + jsonRes[i].Station;
                    res += '       </td>';
                    res += '    </tr>';
                }
              
                res += ' </table>';

                $("#assginPartIDView").html(res);
                var infoModel = $('#PartIDView');
                infoModel.modal('show');

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
    function SaveAssignedStation() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        debugger;
        var infoModel = $('#PartIDView');
        infoModel.modal('hide');
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                //alert("Assigned Supervisor Saved Successfully...!");

                $('#testy').toastee({
                    type: 'success',
                    width: '300px',
                    height: '100px',
                    message: 'Assigned Station Saved Successfully...',
                });


                $("#divselectedPartID").html("");
                $("#divListofPartID").html("");
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
        $("#divListofPartID .chkclass").each(function (e) {
            if ($(this).is(":checked")) {
                count = 1;
                var res = '<tr class="supervisor_' + $(this).val() + '">' + $(".supervisor_" + $(this).val()).html() + '</tr>';
                $("#divselectedPartID table").html($("#divselectedPartID table").html() + res);
                $("#divListofPartID .supervisor_" + $(this).val()).html("");
                $("#divListofPartID table tr").removeClass("supervisor_" + $(this).val());
            }
        });
        if (count == 0) {
            $('#testy').toastee({
                type: 'error',
                width: '200px',
                height: '100px',
                message: 'Please select station...',
            });
        }
    }


    this.swapItemLeftAll = function () {
        $("#divListofPartID .chkclass").each(function (e) {
            // if ($(this).is(":checked")) {
            // alert($(this).val());
            //alert($(".noCritical_" + $(this).val()).html());
            var res = '<tr class="supervisor_' + $(this).val() + '">' + $(".supervisor_" + $(this).val()).html() + '</tr>';
            $("#divselectedPartID table").html($("#divselectedPartID table").html() + res);
            $("#divListofPartID .supervisor_" + $(this).val()).html("");
            $("#divListofPartID table tr").removeClass("supervisor_" + $(this).val());
            // }
        });
    }



    this.swapItemRight = function () {
        var count = 0;
        $("#divselectedPartID .chkclass").each(function (e) {
            if ($(this).is(":checked")) {
                count = 1;
                var res = '<tr class="supervisor_' + $(this).val() + '">' + $(".supervisor_" + $(this).val()).html() + '</tr>';
                $("#divListofPartID table").html($("#divListofPartID table").html() + res);
                $("#divselectedPartID .supervisor_" + $(this).val()).html("");
                $("#divselectedPartID table tr").removeClass("supervisor_" + $(this).val());
            }
        });

        if (count == 0) {
            $('#testy').toastee({
                type: 'error',
                width: '200px',
                height: '100px',
                message: 'Please select station...',
            });
        }
    }


    this.swapItemRightAll = function () {
        $("#divselectedPartID .chkclass").each(function (e) {
            // if ($(this).is(":checked")) {
            //alert($(this).val());
            // criticalStation=$(this).val();                
            // alert($(".Critical_" + $(this).val()).html());
            var res = '<tr class="supervisor_' + $(this).val() + '">' + $(".supervisor_" + $(this).val()).html() + '</tr>';
            $("#divListofPartID table").html($("#divListofPartID table").html() + res);
            $("#divselectedPartID .supervisor_" + $(this).val()).html("");
            $("#divselectedPartID table tr").removeClass("supervisor_" + $(this).val());
            // }
        });
        //  alert(criticalStation);
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