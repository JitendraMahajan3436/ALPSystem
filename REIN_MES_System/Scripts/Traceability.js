$(document).ready(function () {
    $("#ScanValue").focus();
    $('.modelist').on('change', function () {

        $("#ReworkMode").prop("checked", false);
        $("#Linemode").prop("checked", false);
        $('.modelist').not(this).prop('checked', false);
        if ($("#Linemode").prop("checked") == true) {
            $("#ReworkMode").prop("checked", false);
            $("#Linemode").prop("checked", true);
        }
        else {
            $("#ReworkMode").prop("checked", true);
            $("#Linemode").prop("checked", false);
        }
    });
    //$('.modelist').click(function () {
    //    $(this).siblings('input:checkbox').prop('checked', false);
    //});
    window.onload = function () {


        $('#Validated').attr('disabled', 'disabled');
        $('#ScanValue').attr('disabled', 'disabled');
        $('#Savedata').attr('disabled', 'disabled');

        if ($("#Linemode").prop("checked") == true) {
            $("#ReworkMode").prop("checked", false);
        }
        else {
            $("#ReworkMode").prop("checked", true);
            $("#Linemode").prop("checked", false);
        }

        url = "/Traceability/OnLoad";
        $.ajax({
            url: '/Traceability/OnLoad',
            type: 'GET',
            data: "",
            contentType: 'application/json;charset-utf-8',
            success: function (data) {
                //alert(data.userhost);

                if (data.stationcnt == true) {
                    //alert(data.userhost);
                    $('#Validated').prop('disabled', false);
                    $('#ScanValue').prop('disabled', false);
                    $("#ScanValue").focus();
                }
                else {
                    // alert(data.userhost+ " " + data.userhost);
                    $('#testy').toastee({
                        type: 'error',
                        width: '500px',
                        height: '100px',
                        message: 'is Not Configured for Traceability Scanning... ' + data.userhost,
                    });
                }
            }
        });
    }

    this.Refresh = function () {
        $("#ScanValue").val("");
        $("#EngineId").html("");
        $("#EngineDetail").hide();
        $("#Tracebilitydata").html("");
        $("#ScanValue").focus();
    }
    this.Exit = function () {
        var path = '/home/Index';

        window.open(path)
    }
    this.Save = function () {
        debugger;
        var statuslist = new Array();
        $('#Tracebilitydata tr:not(:first)').each(function () {
            var statusdata = {};

            var tds1 = $(this).find('td');
            statusdata.Part_Id = $(tds1[0]).html();
            statusdata.Part_Desc = $(tds1[1]).html();
            statusdata.Part_NO_BOM = $(tds1[2]).html();
            statusdata.Scan_Value = $(tds1[3]).html();
            statusdata: $(tds1[3]).html(),
            statusdata.Status = $(tds1[4]).html();
            statusdata.VCode = $(tds1[5]).html();
            statusdata.MFGDATE = $(tds1[6]).html();
            statusdata.MFGSHIFT = $(tds1[7]).html();
            statusdata.MFGSRNO = $(tds1[8]).html();
            statusdata.PartNo = $(tds1[9]).html();
            statusdata.Zpart = $(tds1[10]).html();
            statusdata.Eroorproof = $(tds1[11]).html();

            statuslist.push(statusdata);
        });

        //$.ajax({
        //    type: "POST",
        //    url: '/Traceability/Savescandata',
        //    data: JSON.stringify({ data: statuslist, LineMode: $("#Linemode").val(), ReworkMode: $("#ReworkMode").val(), Engine: $("#Engine").text(), Model: $("#ModelID").text() }),
        //    contentType: "application/json; charset=utf-8",
        //    dataType: "json",
        //    success: function (data) {

        //        $("#ScanValue").val("");
        //        $("#EngineId").html("");
        //        $("#EngineDetail").hide();
        //        $("#Tracebilitydata").html("");
        //        $('#testy').toastee({
        //            type: 'sucess',
        //            width: '500px',
        //            height: '100px',
        //            message: data.Message,
        //        });
        //    }
        //});

        var statuslist1 = new Array();
        var statusdata1 = {};
        statusdata1.obj = statuslist;
        statusdata1.Linemode = $("#Linemode").val();
        statusdata1.Reworkmode = $("#ReworkMode").val();
        statusdata1.EngineNo = $("#Engine").text();
        statusdata1.ModelCode = $("#ModelID").text();
        statuslist1.push(statusdata1);

        // var url = '@Url.Action("Savescandata","Traceability")';
        var data1 = statuslist1;
        $.ajax({
            url: "Traceability/Savescandata",
            // url:url,JSON.stringify({ data: scanlist, LineMode: Line, ReworkMode: Rework })
            type: "POST",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(data1),
            //async: true,
            //processData: false,
            //cache: false,
            success: function (data2) {
                debugger;
                $("#ScanValue").val("");
                $("#EngineId").html("");
                $("#EngineDetail").hide();
                $("#Tracebilitydata").html("");
                //if (data2[0].Status == true)
                //{
                //    $('#testy').toastee({
                //        type: 'success',
                //        width: '500px',
                //        height: '100px',
                //        message: data2[0].Message,
                //    });
                //}
                //else
                //{
                //    $('#testy').toastee({
                //        type: 'error',
                //        width: '500px',
                //        height: '100px',
                //        message: data2[0].Message,
                //    });
                //}

            },
            error: function (xhr, ajaxOptions, thrownError) {
                debugger;
                //alert(newURL);
                // alert(xhr.status);
                // alert(thrownError);
            }
        });
    }


    var input = document.getElementById("ScanValue");

    // Execute a function when the user releases a key on the keyboard
    input.addEventListener("keyup", function (event) {
        // Number 13 is the "Enter" key on the keyboard
        if (event.keyCode === 13) {
            event.preventDefault();
            var Scan = $("#ScanValue").val();
            var url = "/Traceability/ScanData";
            ajaxpack.getAjaxRequest(url, "Scanvalue=" + Scan, ScanValueDetail, "json");
            $("#ScanValue").focus();
        }
    });

    ////$('#ScanValue').onc(function () {
    ////    debugger;
    ////    var Scan = $("#ScanValue").val();
    ////    var url = "/Traceability/ScanData";
    ////    ajaxpack.getAjaxRequest(url, "Scanvalue=" + Scan, ScanValueDetail, "json");
    ////    $("#ScanValue").focus();
    ////})

    this.Validate = function () {
        var Scan = $("#ScanValue").val();
        var url = "/Traceability/ScanData";
        ajaxpack.getAjaxRequest(url, "Scanvalue=" + Scan, ScanValueDetail, "json");
        $("#ScanValue").focus();
    }

    


    function ScanValueDetail() {

        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally
                var jsonRes = $.parseJSON(myajax.responseText);
                var marry = [];
                if (jsonRes.Message != null) {
                    $('#testy').toastee({
                        type: 'error',
                        width: '500px',
                        height: '100px',
                        message: jsonRes.Message,
                    });
                    $("#ScanValue").val("");
                    ////$("#EngineId").empty();
                    ////$("#EngineId").html("");
                }

                else if (jsonRes.BOMData != null) {
                    var temp = new Array();
                    var Bom = new Array();
                    for (var i = 0; i < jsonRes.strArr.length; i++) {

                        temp.push(jsonRes.strArr[i]);
                    }
                    var res = "";
                    //res += '<br>'
                    //res += '</br>'
                    res += '<label>'; res += ' Engine Sr.No:  '; res += '</label>';
                    res += '<label id ="Engine", Style="margin-left: 10px;margin-right: 10px;">'; res += '' + temp[0]; res += '</label>'
                    res += '<label>'; res += ' Model No: '; res += '</label>'
                    res += '<label id="ModelID" Style="margin-left: 10px">'; res += '' + temp[1]; res += '</label>'
                    $("#EngineId").html(res);
                    $("#EngineDetail").show();
                    var res = "";
                    res += ' <table class="table table-striped table-bordered" id ="scantable">';
                    res += ' <tr>';
                    res += ' <th>'; res += 'Part_Id'; res += '</th>'
                    res += ' <th>'; res += 'Part ID Desc.'; res += '</th>';
                    res += ' <th>'; res += 'BOM Part No.'; res += '</th>';
                    res += ' <th>'; res += 'Scan Value'; res += '</th>';
                    res += ' <th>'; res += 'Status'; res += '</th>';
                    res += ' <th>'; res += 'VCode'; res += '</th>';
                    res += ' <th>'; res += 'MFGDATE'; res += '</th>';
                    res += ' <th>'; res += 'MFGSHIFT'; res += '</th>';
                    res += ' <th>'; res += 'MFGSRNO'; res += '</th>';
                    res += ' <th>'; res += 'PartNo'; res += '</th>';
                    res += ' <th>'; res += 'Zpart'; res += '</th>';
                    res += ' <th>'; res += 'Eroor Prof.'; res += '</th>';
                    res += ' </tr>';
                    for (var i = 0; i < jsonRes.BOMData.length; i++) {
                        res += '';
                        res += ' <tr>';
                        res += ' <td >'; res += '' + jsonRes.BOMData[i].Part_Id; res += '</td>';
                        res += ' <td >'; res += '' + jsonRes.BOMData[i].Part_Desc; res += '</td>';
                        res += ' <td >'; res += '' + jsonRes.BOMData[i].Part_NO_BOM; res += '</td>';
                        res += ' <td >'; res += '' + jsonRes.BOMData[i].Scan_Value; res += '</td>';
                        res += ' <td >'; res += '' + jsonRes.BOMData[i].Status; res += '</td>';
                        res += ' <td >'; res += '' + jsonRes.BOMData[i].VCode; res += '</td>';
                        res += ' <td >'; res += '' + jsonRes.BOMData[i].MFGDATE; res += '</td>';
                        res += ' <td >'; res += '' + jsonRes.BOMData[i].MFGSHIFT; res += '</td>';
                        res += ' <td >'; res += '' + jsonRes.BOMData[i].MFGSRNO; res += '</td>';
                        res += ' <td >'; res += '' + jsonRes.BOMData[i].PartNo; res += '</td>';
                        res += ' <td >'; res += '' + jsonRes.BOMData[i].Zpart; res += '</td>';
                        res += ' <td >'; res += '' + jsonRes.BOMData[i].Error; res += '</td>';
                        res += ' </tr>';
                    }
                    res += ' <table width="100%">';
                    $("#Tracebilitydata").html(res);
                    $("#ScanValue").val("");
                }
                else {
                    var customers = new Array();
                    var scanlist = new Array();
                    var Line, Rework;
                    if ($("#Linemode").prop("checked") == true) {

                        Line = true;
                        Rework = false;
                    }
                    else {
                        Line = false;
                        Rework = true;

                    }

                    //= $("#ReworkMode:checked").val();
                    //  = $("#Linemode:checked").val();
                    $('#Tracebilitydata tr:not(:first)').each(function () {
                        var Tabledata = {};
                        var customer = {};
                        var tds = $(this).find('td');
                        Tabledata.Part_Id = $(tds[0]).html();
                        Tabledata.Part_Desc = $(tds[1]).html();
                        Tabledata.Part_NO_BOM = $(tds[2]).html();
                        Tabledata.Scan_Value = $(tds[3]).html();
                        Tabledata: $(tds[3]).html(),
                        Tabledata.Status = $(tds[4]).html();
                        Tabledata.VCode = $(tds[5]).html();
                        Tabledata.MFGDATE = $(tds[6]).html();
                        Tabledata.MFGSHIFT = $(tds[7]).html();
                        Tabledata.MFGSRNO = $(tds[8]).html();
                        Tabledata.PartNo = $(tds[9]).html();
                        Tabledata.Zpart = $(tds[10]).html();
                        Tabledata.Eroorproof = $(tds[11]).html();
                        scanlist.push(Tabledata);
                    });

                    var options = {};
                    options.url = '/Traceability/scan',
                    options.type = "POST",
                    options.dataType = "json",
                    options.data = JSON.stringify({ data: scanlist, LineMode: Line, ReworkMode: Rework }),
                    //options.data = JSON.stringify({ scanlist,Line,Rework });
                    options.contentType = "application/json; charset=utf-8",

                    options.success = function (data) {

                        var res = "";
                        res += ' <table class="table table-striped table-bordered" id ="scantable">';
                        res += ' <tr>';
                        res += ' <th>'; res += 'Part_Id'; res += '</th>'
                        res += ' <th>'; res += 'Part ID Desc.'; res += '</th>';
                        res += ' <th>'; res += 'BOM Part No.'; res += '</th>';
                        res += ' <th>'; res += 'Scan Value'; res += '</th>';
                        res += ' <th>'; res += 'Status'; res += '</th>';
                        res += ' <th>'; res += 'VCode'; res += '</th>';
                        res += ' <th>'; res += 'MFGDATE'; res += '</th>';
                        res += ' <th>'; res += 'MFGSHIFT'; res += '</th>';
                        res += ' <th>'; res += 'MFGSRNO'; res += '</th>';
                        res += ' <th>'; res += 'PartNo'; res += '</th>';
                        res += ' <th>'; res += 'Zpart'; res += '</th>';
                        res += ' <th>'; res += 'Eroor Prof.'; res += '</th>';
                        res += ' </tr>';
                        for (var i = 0; i < data.length; i++) {

                            res += '';
                            res += ' <tr>';
                            res += ' <td >'; res += '' + data[i].Part_Id; res += '</td>';
                            res += ' <td >'; res += '' + data[i].Part_Desc; res += '</td>';
                            res += ' <td >'; res += '' + data[i].Part_NO_BOM; res += '</td>';
                            res += ' <td >'; res += '' + data[i].Scan_Value; res += '</td>';
                            if (data[i].Status == "YES") {
                                res += '<td style="color:Green">'; res += '' + data[i].Status; res += '</td>';
                            }
                            else {
                                res += '<td style="color:Red">'; res += '' + data[i].Status; res += '</td>';
                            }
                            res += ' <td >'; res += '' + data[i].VCode; res += '</td>';
                            res += ' <td >'; res += '' + data[i].MFGDATE; res += '</td>';
                            res += ' <td >'; res += '' + data[i].MFGSHIFT; res += '</td>';
                            res += ' <td >'; res += '' + data[i].MFGSRNO; res += '</td>';
                            res += ' <td >'; res += '' + data[i].PartNo; res += '</td>';
                            res += ' <td >'; res += '' + data[i].Zpart; res += '</td>';
                            res += ' <td >'; res += '' + data[i].Error; res += '</td>';
                            res += ' </tr>';
                        };
                        $("#Tracebilitydata").html(res);
                        $("#ScanValue").val("");

                        var statuslist = new Array();
                        $('#Tracebilitydata tr:not(:first)').each(function () {
                            var statusdata = {};

                            var tds1 = $(this).find('td');
                            statusdata.Part_Id = $(tds1[0]).html();
                            statusdata.Part_Desc = $(tds1[1]).html();
                            statusdata.Part_NO_BOM = $(tds1[2]).html();
                            statusdata.Scan_Value = $(tds1[3]).html();
                            statusdata: $(tds1[3]).html(),
                            statusdata.Status = $(tds1[4]).html();
                            statusdata.VCode = $(tds1[5]).html();
                            statusdata.MFGDATE = $(tds1[6]).html();
                            statusdata.MFGSHIFT = $(tds1[7]).html();
                            statusdata.MFGSRNO = $(tds1[8]).html();
                            statusdata.PartNo = $(tds1[9]).html();
                            statusdata.Zpart = $(tds1[10]).html();
                            statusdata.Eroorproof = $(tds1[11]).html();

                            statuslist.push(statusdata);
                        });
                        var j = 0, k = 0;
                        for (var i = 0; i < statuslist.length; i++) {
                            if (statuslist[i].Status == "YES") {
                                j++;
                                k++;
                            }
                            else if (statuslist[i].Status == "NO") {
                                k++;
                            }
                        }

                        if (statuslist.length == j) {

                            var ReworkMode = $("#ReworkMode").val();
                            var Engine = $("#Engine").text();
                            var Model = $("#ModelID").text();
                            var LineMode = $("#Linemode").val()

                            tracbilitySave(statuslist, Line, Rework, Engine, Model)


                        }
                        else if (statuslist.length == k) {
                            $('#Savedata').prop('disabled', false);
                        }
                    }
                    options.error = function () {
                        debugger;
                        //alert("Invalid Feeder ");
                        //$("#ShopID").show()
                    },
                    $.ajax(options);
                    //$.ajax({



                }
            }
        }
    }

    function tracbilitySave(statuslist, LineMode1, ReworkMode1, Engine1, Model1) {
        debugger;
        var params = { data: statuslist, LineMode: LineMode1, ReworkMode: ReworkMode1, Engine: Engine1, Model: Model1 };
        var statuslist1 = new Array();
        var statusdata1 = {};
        statusdata1.obj = statuslist;
        statusdata1.Linemode = LineMode1;
        statusdata1.Reworkmode = ReworkMode1;
        statusdata1.EngineNo = Engine1;
        statusdata1.ModelCode = Model1;
        statuslist1.push(statusdata1);
        var data = statuslist1;

        var options = {};
        options.url = '/Traceability/savescandata',
        options.type = "POST",
        options.dataType = "json",
        options.data = JSON.stringify({ data1: statuslist1 }),
        //options.data = JSON.stringify({ scanlist,Line,Rework });
        options.contentType = "application/json; charset=utf-8",

        options.success = function (data) {
            debugger;
            $("#ScanValue").val("");
            $("#EngineId").html("");
            $("#EngineDetail").hide();
            //$("#Engine").html("");
            //$("#ModelID").html("");
            $("#Tracebilitydata").html("");
            $("#ScanValue").focus();
            if (data[0].Status == true) {
                $('#testy').toastee({
                    type: 'success',
                    width: '500px',
                    height: '100px',
                    message: data[0].Message,
                });
            }
            else {
                $('#testy').toastee({
                    type: 'error',
                    width: '500px',
                    height: '100px',
                    message: data[0].Message,
                });
            }
        }

        options.error = function () {
            debugger;
        }
        $.ajax(options)

    }
});

