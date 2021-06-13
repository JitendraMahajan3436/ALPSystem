
$(document).ready(function (e) {
    var sFlag = 0;
    var lFlag = 0;
    $('#Station_ID').keyup(function () {
        searchTable($(this).val());
    });


    function searchTable(inputVal) {
        var table = $('#divNoCriticalStationlist');
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

    $('#myListBox1').keyup(function () {
        searchTable1($(this).val());
    });


    function searchTable1(inputVal) {
        var table = $('#divCriticalStationlist');
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
        clearSelectBox("Line_ID");
        if (shopId) {
            $('.shop').html('');
            sFlag = 1;
            var jsonData = JSON.stringify({ plantId: 7 });
            var url = "/Line/GetLineByShopID";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopLineType, "json");
        }
        else {
            $("#divCriticalStationlist").html("");
            $("#divNoCriticalStationlist").html("");
            $('.shop').html('Please select shop');
            sFlag = 0;
        }

    });


    $(".manager_Line #Line_ID").change(function (e) {

        var jsonData = JSON.stringify({ plantId: 7 });
        var lineId = $("#Line_ID").val();
        //alert(lineId);
        if (lineId) {
            //var url = "/Station/GetNoCriticalStationByShopID";
            // ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopStationType, "json");
            $('.line').html('');
            lFlag = 1;
            var url = "/Station/GetNoCriticalStationByLineID";
            ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showLineStationType, "json");

            // setTimeout(function () {
            //    url = "/Station/GetCriticalStationByShopId";
            //    ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopCriticalStationType, "json");
            // }, 1000);

            setTimeout(function () {
                url = "/Station/GetCriticalStationByLineId";
                ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showLineCriticalStationType, "json");
            }, 1000);


            //setTimeout(function () {
            //    url = "/Station/GetStationListByLineID";
            //    ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showLineStationType, "json");
            //}, 2000);



        }
        else {
            $("#divCriticalStationlist").html("");
            $("#divNoCriticalStationlist").html("");
            $('.line').html('Please select line');
            lFlag = 0;
        }



    });



    function showLineStationType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                // SelectOptionHTML(jsonRes, "Station_ID");
                var res = "";
                res += ' <table width="100%">';
                for (var i = 0; i < jsonRes.length; i++) {
                    res += '';
                    res += '  <tr class="noCritical_' + jsonRes[i].Id + '">';
                    res += '    <td width="10px">';
                    res += '         <input type="checkbox" id="@checkBoxId" class="chkclass" value="' + jsonRes[i].Id + '" />';
                    res += '     </td>';
                    res += '      <td id="@tdId" width="100px">';
                    res += '           ' + jsonRes[i].Value;
                    res += '       </td>';
                    res += '    </tr>';
                }
                res += ' </table>';

                $("#divNoCriticalStationlist").html(res);

            }
        }
    }
    function showLineCriticalStationType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                //SelectOptionHTML(jsonRes, "myListBox1");
                var res = "";
                res += ' <table width="100%">';
                for (var i = 0; i < jsonRes.length; i++) {
                    res += '';
                    res += '  <tr class="noCritical_' + jsonRes[i].Id + '">';
                    res += '    <td width="10px">';
                    res += '         <input type="checkbox" id="@checkBoxId" class="chkclass" value="' + jsonRes[i].Id + '" />';
                    res += '     </td>';
                    res += '      <td id="@tdId" width="100px">';
                    res += '           ' + jsonRes[i].Value;
                    res += '       </td>';
                    res += '    </tr>';
                }
                res += ' </table>';

                $("#divCriticalStationlist").html(res);
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
        $('#divCriticalStationlist').prop('selected', true);
        //});

        return true;
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

    function clearrSelectBox(targetId) {
        var res = "";
        res = "<option value=''>" + $("." + targetId + " option:first").html() + "</option>";
        $("#" + targetId).html(res);
    }

    this.swapItemLeft = function () {
        var flag = 0;
        $("#divNoCriticalStationlist .chkclass").each(function (e) {
            if ($(this).is(":checked")) {
                flag = 1;
                var res = '<tr class="noCritical_' + $(this).val() + '">' + $(".noCritical_" + $(this).val()).html() + '</tr>';
                $("#divCriticalStationlist table").html($("#divCriticalStationlist table").html() + res);
                $("#divNoCriticalStationlist .noCritical_" + $(this).val()).html("");
                $("#divNoCriticalStationlist table tr").removeClass("noCritical_" + $(this).val());
            }
        });
        if (flag == 0) {

            $('#testy').toastee({
                type: 'error',
                width: '200px',
                height: '100px',
                message: 'Please select station...',
            });
        }

    }


    this.swapItemLeftAll = function () {
        $("#divNoCriticalStationlist .chkclass").each(function (e) {
            // if ($(this).is(":checked")) {
            // alert($(this).val());
            //alert($(".noCritical_" + $(this).val()).html());
            var res = '<tr class="noCritical_' + $(this).val() + '">' + $(".noCritical_" + $(this).val()).html() + '</tr>';
            $("#divCriticalStationlist table").html($("#divCriticalStationlist table").html() + res);
            $("#divNoCriticalStationlist .noCritical_" + $(this).val()).html("");
            $("#divNoCriticalStationlist table tr").removeClass("noCritical_" + $(this).val());
            // }
        });
    }



    this.swapItemRight = function () {
        var flag = 0;
        $("#divCriticalStationlist .chkclass").each(function (e) {
            if ($(this).is(":checked")) {
                flag = 1;
                var res = '<tr class="noCritical_' + $(this).val() + '">' + $(".noCritical_" + $(this).val()).html() + '</tr>';
                $("#divNoCriticalStationlist table").html($("#divNoCriticalStationlist table").html() + res);
                $("#divCriticalStationlist .noCritical_" + $(this).val()).html("");
                $("#divCriticalStationlist table tr").removeClass("noCritical_" + $(this).val());
            }
        });
        if (flag == 0) {
            $('#testy').toastee({
                type: 'error',
                width: '200px',
                height: '100px',
                message: 'Please select station...',
            });
        }
    }


    this.swapItemRightAll = function () {
        $("#divCriticalStationlist .chkclass").each(function (e) {
            // if ($(this).is(":checked")) {
            //alert($(this).val());
            // criticalStation=$(this).val();                
            // alert($(".Critical_" + $(this).val()).html());
            var res = '<tr class="noCritical_' + $(this).val() + '">' + $(".noCritical_" + $(this).val()).html() + '</tr>';
            $("#divNoCriticalStationlist table").html($("#divNoCriticalStationlist table").html() + res);
            $("#divCriticalStationlist .noCritical_" + $(this).val()).html("");
            $("#divCriticalStationlist table tr").removeClass("noCritical_" + $(this).val());
            // }
        });
        //  alert(criticalStation);
    }

    this.save = function () {

        var lineId = $("#Line_ID").val();
        var Stations = [];
        var arrlist = [];
        $("#divCriticalStationlist .chkclass").each(function (e) {
            arrlist.push($(this).val());
        });
        if (sFlag == 1 && lFlag == 1) {
            $('#myModal').modal('show');
            var url = "/AddCriticalStation/SaveCriticalStations";
            ajaxpack.getAjaxRequest(url, "Stations=" + arrlist + "&lineId=" + $("#Line_ID").val(), SaveStations, "json");
        }
        else {
            if (sFlag == 0) {
                $('.shop').html('Please select shop');
                $('#Shop_ID').focus();
            }
            if (lFlag == 0) {
                $('.line').html('Please select line');
                $('#Line_ID').focus();
            }
        }
    }


    function SaveStations() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes) {
                   // alert("Critical Stations Saved Sucessfully");
                    $("#divCriticalStationlist").html("");
                    $("#divNoCriticalStationlist").html("");
                    $('#testy').toastee({
                        type: 'success',
                        width: '300px',
                        height: '100px',
                        message: 'Critical station Saved Successfully...',
                    });
                    setTimeout(function () {
                        location.reload();
                    }, 2000);
                }
                else {
                    $('#myModal').modal('hide');
                    alert("Please select station");

                }

            }
        }
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
});