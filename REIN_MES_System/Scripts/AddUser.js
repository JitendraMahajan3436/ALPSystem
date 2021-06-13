

$(document).ready(function (e) {
    //$("#Lines_ID").select2({
    //    multiple: 'multiple',
    //    placeholder: 'Select Line',
    //    maximumSelectionLength: 50
    //});
    //$("#Shops_ID").select2({
    //    multiple: 'multiple',
    //    placeholder: 'Select Shop',
    //    maximumSelectionLength: 1
    //});


    //$(document).on('click', '.select2-selection__choice__remove', function () {
    //    $('#Lines_ID').empty();
    //    $('#Lines_ID').val(null).trigger("change");
    //});

    debugger;
    $(".PlantName").css("visibility", "hidden");
    $(".ShopName").css("visibility", "hidden");
    $(".LineName").css("visibility", "hidden");

    var plantId = $("#Plants_ID").val();
    var shopId = $(".manager_Line #Shops_ID").val();
    var shopId_text = $(".manager_Line #Shops_ID option:selected").text();
    var lineId = $("#Lines_ID").val();

    var categoryId = $(".manager_Line #Category_Id").val();

    if (categoryId == 6 || categoryId == 7) {
        if (plantId != null || plantId != "Select Plant" || plantId != 0) {
            // alert(plantId);
            $(".PlantName").css("visibility", "visible");
        }

    }
    else if (categoryId == 3) {
        debugger;
        if (shopId != null || shopId != "Select Shop" || shopId != 0) {
            //  alert(shopId);
            $(".ShopName").css("visibility", "visible");
            $("#Shops_ID").select2();
        }
        else {
            $("#Shops_ID").select2({
                multiple: 'multiple',
                placeholder: 'Select Shop',
                maximumSelectionLength: 5
            });
            $("#Plants_ID").select2({
                multiple: 'multiple',
                placeholder: 'Select Plant',
                maximumSelectionLength: 1
            });
        }
        
    }
    else if (categoryId == 2) {

        $(".ShopName").css("visibility", "visible");
        var url = "/AddUsers/GetShop";
        ajaxpack.getAjaxRequest(url, "", showShopID, "json");


        $('#Shops_ID').val(shopId[0]);
        $("#Shops_ID option[value=" + shopId[0] + "]").prop('selected', 'selected');

        if (lineId != null || lineId != "Select Plant" || lineId != 0) {
            // alert(lineId);
            $(".LineName").css("visibility", "visible");
        }
        $("#Lines_ID").select2({
            multiple: 'multiple',
            placeholder: 'Select Line',
            maximumSelectionLength: 50
        });
        $("#Shops_ID").select2({
            multiple: 'multiple',
            placeholder: 'Select Shop',
            maximumSelectionLength: 1
        });
        $("#Plants_ID").select2({
            multiple: 'multiple',
            placeholder: 'Select Plant',
            maximumSelectionLength: 1
        });
    }
    // alert("hi");

    $(".manager_Line #Category_Id").change(function (e) {
        //alert("hi");
        $(".PlantName").css("visibility", "hidden");
        $(".ShopName").css("visibility", "hidden");
        $(".LineName").css("visibility", "hidden");

        //$("#Shops_ID").select2('val', "");
        //$("#Lines_ID").select2('val', "");

        //$("#Shops_ID").html('').select2();
        //$("#Lines_ID").html('').select2();
        //$(".select2-selection select2-selection--multiple").html(null);
        //$('#Lines_ID').select2('data', []);

        //$('#Shops_ID').select2('destroy');
        //$('#Shops_ID').html('<option></option>');
        //$('#Shops_ID').select2();
        $('#Shops_ID').removeClass('select2-offscreen');
        $('#Lines_ID').removeClass('select2-offscreen');

        //$('#Lines_ID').select2('destroy');
        //$('#Lines_ID').html('<option></option>');
        //$('#Lines_ID').select2();

        //$('#Shops_ID').val(null).trigger('change');
        //$('#Lines_ID').val(null).trigger('change');

        //$("#Shops_ID").select2('data', null)
        //$("#Lines_ID").select2('data', null);
        //$('#Shops_ID').val('').trigger('change');
        //$('#Lines_ID').val('').trigger('change');
        //$('#Shops_ID').val([]);
        //$('#Lines_ID').val([]);
        //$("#Shops_ID").select2('data', { id: null, text: null });
        //$("#Lines_ID").select2('data', { id: null, text: null });

        // $('#Shops_ID').trigger('change');

        // $('#Shops_ID').val(null).trigger("change");

        var categoryId = $("#Category_Id").val();
        //alert(categoryId);

        //alert(categoryId);
        if (categoryId == 3) {
            var url = "/AddUsers/GetShop";
            ajaxpack.getAjaxRequest(url, "", showShopID, "json");
            $(".PlantName").css("visibility", "hidden");
            $(".ShopName").css("visibility", "visible");
            $(".LineName").css("visibility", "hidden");
            $("#Shops_ID").select2({
                multiple: 'multiple',
                placeholder: 'Select Shop',
                maximumSelectionLength: 5
            });
            //$("#Plants_ID").select2({
            //    multiple: 'multiple',
            //    placeholder: 'Select Plant',
            //    maximumSelectionLength: 1
            //});


        }
        else if (categoryId == 6 || categoryId == 7) {
            $(".PlantName").css("visibility", "visible");
            $(".ShopName").css("visibility", "hidden");
            $(".LineName").css("visibility", "hidden");
            //$("#Plants_ID").select2({
            //    multiple: 'multiple',
            //    // placeholder: 'Select Plant',
            //    maximumSelectionLength: 10
            //});
        }
        else if (categoryId == 2) {
            //alert("Sandip");
            $(".PlantName").css("visibility", "hidden");
            $(".ShopName").css("visibility", "visible");
            $(".LineName").css("visibility", "visible");
            $("#Lines_ID").html(null);
            $("#Shops_ID").html(null);
            var url = "/AddUsers/GetShop";
            ajaxpack.getAjaxRequest(url, "", showShopID, "json");


            $("#Plants_ID").select2({
                multiple: 'multiple',
                placeholder: 'Select Plant',
                maximumSelectionLength: 1
            });

            $("#Lines_ID").select2({
                multiple: 'multiple',
                placeholder: 'Select Line',
                maximumSelectionLength: 50
            });
            $("#Shops_ID").select2({
                placeholder: 'Select Shop',
                maximumSelectionLength: 2
            });

        }


        //var url = "/AddUsers/GetPlant";
        //ajaxpack.getAjaxRequest(url, "Category_Id=" + $("#Category_Id").val() + "", showPlantID, "json");



    });

    //$(".manager_Line #Category_Id").trigger("change");



    //$(".manager_Line #Plants_ID").change(function (e) {
    //    var plantId = $("#Plants_ID").val();
    //    if (plantId) {
    //        var url = "/AddUsers/GetShopByPlantID";
    //        ajaxpack.getAjaxRequest(url, "plantId=" + $("#Plants_ID").val() + "", showShopID, "json");
    //    }

    //});

    $(".manager_Line #Shops_ID").change(function (e) {
        var shopId = $("#Shops_ID").val();
        //$('#Lines_ID option:selected').remove();
        //$('#Lines_ID').val(null);
        //$('#Lines_ID').val(null).trigger("change");        

        $('#Lines_ID').val(null).trigger("change");
        $('#Lines_ID').empty();
        if (shopId) {
            var url = "/AddUsers/GetLineByShopID";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shops_ID").val() + "", showShopLineType, "json");
        }

    });


    $(".manager_Line #Lines_ID").change(function (e) {
        //alert($('#Lines_ID').val());
    });


    function showShopID() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Shops_ID");
            }
        }
        if (categoryId == 2) {
            $('#Shops_ID').val(shopId[0]);
            $("#Shops_ID option[value=" + shopId[0] + "]").prop('selected', 'selected');
        }
    }

    function showPlantID() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Plants_ID");
            }
        }
    }

    function showShopLineType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes.length > 0) {
                    SelectOptionHTML(jsonRes, "Lines_ID");
                }
                else {
                    $("#Lines_ID").val(null).trigger("change");
                    $("#Lines_ID").empty();
                }


            }
        }
    }

    function SelectOptionHTML(jsonRes, targetId) {
        //var jsonRes = $.parseJSON(myajax.responseText);     

        var res = "";
        for (var i = 0; i < jsonRes.length; i++) {
            //if (categoryId == 2) {
            //    if (jsonRes[i].Id == shopId[0]) {
            //        res += "<option value='" + jsonRes[i].Id + "' selected=selected'>" + jsonRes[i].Value + "</option>";
            //    }
            //    else {
            //        res += "<option value='" + jsonRes[i].Id + "'>" + jsonRes[i].Value + "</option>";
            //    }

            //}
            //else {
            res += "<option value='" + jsonRes[i].Id + "'>" + jsonRes[i].Value + "</option>";
            $("#" + targetId).html(res);
            //  }

        }

        //res = "<option value=''>" + $("#" + targetId + " option:first").html() + "</option>" + res;
        //$("#" + targetId).html(res);

    }



});







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