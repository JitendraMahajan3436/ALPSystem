
$(document).ready(function () {
    
    $("#ScanValue").on('keypress', function (event) {
      
        var key = event.which;
        if (key == 13) {

            var Scanvalue = $("#ScanValue").val();

            $.ajax({
                type: "POST",
                url: '/EngineDropping/ScanData',
                data: JSON.stringify({ Scanvalue: Scanvalue }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    debugger;
                    var res = "";

                    res += '<table class="table-responsive" style="text-align:center">';
                    res += '<tr style="font-size:18px;font-weight:bold;background-color:#80808066">';
                    res += '<td style="width:10%">Sr No</td>';
                    res += '<td style="width:10%">Model Code</td>';
                    res += '<td style="width:20%">Model Desc</td>';
                    res += '<td style="width:20%">Engine Sr No</td>';
                    res += '<td style="width:10%">Status</td>';
                    res += '    </tr>';

                    for (var i = 0; i < data.Result.length; i++) {
                        var j = i + 1;
                        res += '';


                        res += '  <tr  style="color:black" style="font-size:16px">';

                        res += '    <td>';
                        res += '         ' + j;
                        res += '     </td>';

                        res += '      <td  style  = "font-size:15px; font-weight: 700 ">';
                        res += '           ' + data.Result[i].model_code;
                        res += '       </td>';

                        res += '      <td  style  = "font-size:15px; font-weight: 700 ">';
                        res += '           ' + data.Result[i].modedesc;
                        res += '       </td>';

                        if (j == 1) {

                            //res += '      <td style="background-color:lawngreen; font-size:15px; font-weight: 700">';
                            res += '      <td style="font-size:15px; font-weight: 700">';
                            res += '           ' + data.Result[i].Eng_Sr;
                            res += '       </td>';
                        }
                        else {
                            res += '      <td  style  = "font-size:15px; font-weight: 700 ">';
                            res += '           ' + data.Result[i].Eng_Sr;
                            res += '       </td>';
                        }

                        if (data.Result[i].ENGStatus == true) {

                            res += '      <td style="background-color:lawngreen; font-size:15px; font-weight: 700">';
                            res += 'OK';

                        }
                        else {
                            res += '      <td  style  ="background-color:red; font-size:15px; font-weight: 700 ">';
                            res += 'NOK';
                            res += '       </td>';
                        }
                        res += '    </tr>';
                    }


                    res += ' </table>';
                    // res += '    </div>';
                    $("#tableScanData").html(res);
                    // $("#lblSerial").html("Last Serial Number Scanned : ");
                    $("#lblLastSerial").html(data.LastSerialNo.toUpperCase());
                    //$("#lbltotalcount").html("Total Count : ");
                    // $("#lbllasttotalcount").html(data.Tatal_Count);
                    $("#lblLastSerial").addClass('SerialNoblinker');

                    if (data.Status == true) {
                        UpdateDroppingCount();
                        $("#ScanValue").focus();
                        $("#ScanValue").val("");




                        if (data.EngineOK == true) {
                            $('#testy').toastee({

                                type: 'success',
                                width: '500px',
                                height: '100px',
                                message: data.Message,
                            });
                        }
                        else {
                            $('#testy').toastee({

                                type: 'error',
                                width: '500px',
                                height: '100px',
                                message: data.Message,
                            });
                        }

                    }

                    else {
                       
                        $("#ScanValue").val("");
                        $('#testy').toastee({
                            type: 'error',
                            width: '500px',
                            height: '100px',
                            message: data.Message,
                        });
                    }
                }


            });

        }

        else {

        }
    });


    //setInterval(UpdateDroppingCount, 9000);



    function UpdateDroppingCount() {
      
        var options = {};
        options.url = "/EngineDropping/GetDroppingCount";
        options.type = "POST";
        //options.data = JSON.stringify({ Scanvalue: Scanvalue });
        options.dataType = "json";
        options.contentType = "application/json";
        options.success = function (test) {

            $("#lbltotalcount").html("Total Count : ");
            $("#lbllasttotalcount").html(test.Tatal_Count);

        };

        options.error = function () { };
        $.ajax(options);
    }



    window.onload = function () {
     
        $.ajax({
            url: '/EngineDropping/OnLoad',
            type: 'GET',
            data: "",
            contentType: 'application/json;charset-utf-8',
            success: function (data) {
                //alert(data.userhost);

                if (data.stationcnt == true) {

                    $("#ScanValue").focus();
                    UpdateDroppingCount();


                    $.ajax({
                        type: "GET",
                        url: '/EngineDropping/GetPresentdetailsOnLoad',
                        data: JSON,
                        //data: JSON.stringify({ Scanvalue: Scanvalue }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {

                            if (data.Status == true) {

                                $("#ScanValue").focus();
                                //  $("#ScanValue").val("");

                                var res = "";
                                //res += '<div class="col-md-6">';
                                res += '<table class="table-responsive" style="text-align:center">';
                                res += '<tr style="font-size:18px;font-weight:bold;background-color:#80808066">';
                                res += '<td style="width:10%">Sr No</td>';
                                res += '<td style="width:10%">Model Code</td>';
                                res += '<td style="width:20%">Model Desc</td>';
                                res += '<td style="width:20%">Engine Sr No</td>';
                                res += '<td style="width:10%">Status</td>';
                                res += '    </tr>';

                                for (var i = 0; i < data.Result.length; i++) {
                                    var j = i + 1;
                                    $("#lblLastSerial").html(data.Result[0].Eng_Sr);
                                    res += '';


                                    res += '  <tr  style="color:black" style="font-size:16px">';

                                    res += '    <td>';
                                    res += '         ' + j;
                                    res += '     </td>';

                                    res += '      <td  style  = "font-size:15px; font-weight: 700 ">';
                                    res += '           ' + data.Result[i].model_code;
                                    res += '       </td>';

                                    res += '      <td  style  = "font-size:15px; font-weight: 700 ">';
                                    res += '           ' + data.Result[i].modedesc;
                                    res += '       </td>';

                                    if (j == 1) {

                                        //res += '      <td style="background-color:lawngreen; font-size:15px; font-weight: 700">';
                                        res += '      <td style="font-size:15px; font-weight: 700">';
                                        res += '           ' + data.Result[i].Eng_Sr;
                                        res += '       </td>';
                                    }
                                    else {
                                        res += '      <td  style  = "font-size:15px; font-weight: 700 ">';
                                        res += '           ' + data.Result[i].Eng_Sr;
                                        res += '       </td>';
                                    }

                                    if (data.Result[i].ENGStatus == true) {

                                        res += '      <td style="background-color:lawngreen; font-size:15px; font-weight: 700">';
                                        res += 'OK';

                                    }
                                    else {
                                        res += '      <td  style  ="background-color:red; font-size:15px; font-weight: 700 ">';
                                        res += 'NOK';
                                        res += '       </td>';
                                    }
                                    res += '    </tr>';
                                }


                                res += ' </table>';
                                // res += '    </div>';
                                $("#tableScanData").html(res);
                                // $("#lblSerial").html("Last Serial Number Scanned : ");
                                // $("#lblLastSerial").html(data.LastSerialNo.toUpperCase());
                                $("#lbltotalcount").html("Total Count : ");
                                $("#lbllasttotalcount").html(data.Tatal_Count);
                                $("#lblLastSerial").addClass('SerialNoblinker');


                            }
                        }
                    });
                    
                }
                else {

                    $("#ScanValue").attr('disabled', 'disabled');
                    $('#testy').toastee({
                        type: 'error',
                        width: '500px',
                        height: '100px',
                        message: data.userhost + ' is Not Configured for Engine Dropping... ',
                    });
                }
            }
        });


        


    }
});