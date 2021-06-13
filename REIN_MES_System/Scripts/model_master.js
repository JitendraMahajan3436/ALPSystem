


$(function () {    
    $("#Model_Code").change(function () {
        var model_code = $('#Model_Code').val();
        var Shop_ID = $("#Shop_ID").val();
        if ($("#Shop_ID").val().length > 0) {

            url = "/ModelMaster/CheckModelCodeExist";
            $.getJSON(url, { Shop_ID: Shop_ID, model_code: model_code }, function (data) {
                if (data) {
                    $('#errorModelMsg').html('');
                    $('#btnSave').attr("disabled", false);
                }
                else {
                    $('#errorModelMsg').html('Model Code Allready Exist');
                    $('#btnSave').attr("disabled", true);
                    $('#Model_Code').focus();
                }
            })
        }
        else {
            alert("Please select shop");
            $('#Model_Code').val('');
            $('#Model_Code').focus();
        }
    })

    $("#Series_Description").autocomplete({
        source: function (request, response) {
            if ($("#Shop_ID").val().length > 0) {
                $.ajax({
                    url: "/ModelMaster/GetSeriesByShop",
                    data: "{ 'prefix': '" + request.term + "','Shop_ID': '" + $('#Shop_ID').val() + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        response($.map(data, function (item) {
                            return item;
                        }))
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            }
            else {
                alert("Please select shop");
                $('#Series_Description').val('');
                $('#Series_Description').focus();
            }
        },
        select: function (e, i) {
            $("#hfSeries_Description").val(i.item.val);

        },
        minLength: 1
    });
    $("#Series_Description").change(function () {
        var family = $('#Series_Description').val();
        url = "/ModelMaster/CheckSeriesExists";// 
        $.getJSON(url, { family: family }, function (data) {
            if (data) {
                $('#errorMsg').html('');
                $('#btnSave').attr("disabled", false);
            }
            else {
                $('#errorMsg').html('Series Allready Exist');
                $('#btnSave').attr("disabled", true);
                $('#Series_Description').focus();
            }
        })
    })

    

    $("#Model_Type1").autocomplete({
        source: function (request, response) {
            if ($("#Shop_ID").val().length > 0) {
                $.ajax({
                    //url: '/Home/AutoComplete/',
                    url: "/ModelMaster/GetModelType",
                    data: "{ 'prefix': '" + request.term + "','Shop_ID': '" + $('#Shop_ID').val() + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        response($.map(data, function (item) {
                            return item;
                        }))
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            }
            else {
                alert("Please select shop");
                $('#Model_Type1').val('');
                $('#Model_Type1').focus();
            }
        },
        select: function (e, i) {
            $("#hfModel_Type").val(i.item.val);
        },
        minLength: 1
    });
    //Family
    $("#Varient1").autocomplete({
        source: function (request, response) {
            if ($("#Shop_ID").val().length > 0) {
                $.ajax({
                    //url: '/Home/AutoComplete/',
                    url: "/ModelMaster/GetVarientByShop",
                    data: "{ 'prefix': '" + request.term + "','Shop_ID': '" + $('#Shop_ID').val() + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        response($.map(data, function (item) {
                            return item;
                        }))
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            }
            else {
                alert("Please select shop");
                $('#Varient1').focus();
                $('#Varient1').val('');
            }
        },
        select: function (e, i) {
            $("#hfVarient").val(i.item.val);
        },
        minLength: 1
    });

    $("#Family1").autocomplete({
        source: function (request, response) {
            if ($("#Shop_ID").val().length > 0) {
                $.ajax({
                    //url: '/Home/AutoComplete/',
                    url: "/ModelMaster/GetFamilyCodesByShop",
                    data: "{ 'prefix': '" + request.term + "','Shop_ID': '" + $('#Shop_ID').val() + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        response($.map(data, function (item) {
                            return item;
                        }))
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            }
            else {
                alert("Please select shop");
                $('#Family1').val('');
                $('#Family1').focus();
            }
        },
        select: function (e, i) {
            $("#hfFamily").val(i.item.val);
        },
        minLength: 1
    });
});



$(document).ready(function () {
    if ($("#Shop_ID").val() == "4" || $("#Shop_ID").val() == "10") {
        $('#tyreMake').css('display', 'block');
        $('#divColor').css('display', 'block');
    }
    else {
        $('#tyreMake').css('display', 'none');
        $('#divColor').css('display', 'none');
    }

    var series = $("#Series_Description").val();
    var Length = series.length;
    if ($("#Series_Description").val() != "" || $("#Series_Description").val() != null) {
        if (!(Length >= 3 && Length <= 6)) {
            $("#spanSeriesValidation").html("Series length must be between 3 to 6");
            return false;
        }
        else {
            $("#spanSeriesValidation").html("");
        }
    }
    //$("#OMconfig_ID").select2({
    //    allowClear: true
    //});

    //$("#Shop_ID").select2({
    //    allowClear: true
    //});

    //$("#Series_Code").select2({
    //    allowClear: true
    //});

    //$("#Model_Type").select2({
    //    allowClear: true
    //});

    //$("#Varient").select2({
    //    allowClear: true
    //});

    //$("#Family").select2({
    //    allowClear: true
    //});





    $("#Shop_ID").on("change", function () {
        alert();
        var Plant = $("#Plant_ID").val();
        var Shop = $("#Shop_ID").val();
        alert(Plant,Shop);

        //$('#OMconfig_ID').select2("val", "");
       
        if (Plant.length > 0 && Shop.length > 0) {

            // Populate categories when the page is loaded.
            $.getJSON('/ModelMaster/GetConfigueData', { Plant_Id: Plant, Shop_id: Shop }, function (data) {

                if (data.length > 0) {
                    // Ajax success callback function.
                    // Populate dropdown from Json data returned from server.
                    $('#OMconfig_ID option').remove();
                    $('#OMconfig_ID').append('<option value="">Select Configuration</option>');

                    for (i = 0; i < data.length; i++) {
                        $('#OMconfig_ID').append('<option value="' +
                                     data[i].OMconfig_ID + '">' + data[i].OMconfig_Desc + '</option>');
                    }
                } else {
                    $('#OMconfig_ID option').remove();
                    $('#OMconfig_ID').append('<option value="">Select Configuration</option>');
                    alert("No Configuration Available for this Shop!");
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                // Ajax fail callback function.
                alert('Error getting PartGroups !');
            });

            $.getJSON('/ModelMaster/getSerialNoConfigByShopID', { ShopID: Shop }, function (result) {
                if (result.length > 0) {
                    $('#Config_ID option').remove();
                    $('#Config_ID').append('<option value="">Select serial Config</option>');

                    for (i = 0; i < result.length; i++) {
                        $('#Config_ID').append('<option value="' +
                                     result[i].Config_ID + '">' + result[i].Display_Name + '</option>');
                    }
                }
                else {
                    $('#Config_ID option').remove();
                    $('#Config_ID').append('<option value="">Select serial Config</option>');
                    alert("No Serial No. Configuration Available for this Shop!");
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                // Ajax fail callback function.
                alert('Error getting configuration !');
            });

            // process to get shop details
            $.getJSON('/Shop/GetShopDetails', { ShopID: Shop }, function (result) {
                if (result.length > 0) {
                    if (result[0].Is_Main == true) {
                        // process to show color and tyre
                        $("#hdnIsManin").val("1");
                        $('#tyreMake').css('display', 'block');
                        $('#divColor').css('display', 'block');
                    } else {
                        // process to hide color and tyre
                        $("#hdnIsManin").val("0");
                        $('#tyreMake').css('display', 'none');
                        $('#divColor').css('display', 'none');
                    }

                    if (result[0].Is_Number_Punching == true) {
                        // process to show color and tyre
                        $("#hdnIsNumberPunching").val("1");
                        $('.numberpunching').css('display', 'block');                        
                    } else {
                        // process to hide color and tyre
                        $("#hdnIsNumberPunching").val("0");
                        $('.numberpunching').css('display', 'none');
                    }
                    
                    if (result[0].Is_Normed_Based_PPC == true) {
                        // process to show color and tyre
                        $("#hdnIs_Normed_Based_PPC").val("1");
                        $('.Norms_Quantity').css('display', 'block');
                    } else {
                        // process to hide color and tyre
                        $("#hdnIs_Normed_Based_PPC").val("0");
                        $('.Norms_Quantity').css('display', 'none');
                    }
                }
                else {
                    
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                // Ajax fail callback function.                
            });
           
        }
        else {
             $('#OMconfig_ID option').remove();
             $('#OMconfig_ID').append('<option value="">Select Configuration</option>');
             $('#Config_ID option').remove();
             $('#Config_ID').append('<option value="">Select serial Config</option>');
        }


    }); // Shop Select Change Event End

    

});