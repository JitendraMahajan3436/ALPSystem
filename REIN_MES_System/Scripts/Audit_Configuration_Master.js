$(document).ready(function (e) {

    //$("#Shop_ID").on("change", function () {
    //    $("#Audit_Category_ID").html("");
    //    $('#Audit_Category_ID').append('<option value="">Select Category</option>');
    //    $("#Sub_Category_ID").html("");
    //    $('#Sub_Category_ID').append('<option value="">Select Sub Category</option>');
    //    $("#Audit_Type_ID").html("");
    //    $('#Audit_Type_ID').append('<option value="">Select Audit Type</option>');
    //    var Shop_ID = $("#Shop_ID").val();
    //    if (Shop_ID.length > 0) {
    //        $.getJSON('/AuditConfigurationMaster/getCategoryByShopId', { Shop_ID: Shop_ID }, function (data) {
    //            if (data.length > 0) {
    //                $('#Audit_Category_ID option').remove();
    //                $('#Audit_Category_ID').append('<option value="">Select Category</option>');
    //                for (var i = 0; i < data.length; i++) {
    //                    $('#Audit_Category_ID').append('<option value="' + data[i].Id + '">' + data[i].Value + '</option>');
    //                }
    //            }
    //        });
    //    }
    //});

    $("#Audit_Category_ID").on("change", function () {
        $("#Sub_Category_ID").html("");
        $('#Sub_Category_ID').append('<option value="">Select Sub Category</option>');
        $("#Audit_Type_ID").html("");
        $('#Audit_Type_ID').append('<option value="">Select Audit Type</option>');
        var Audit_Category_ID = $("#Audit_Category_ID").val();
        if (Audit_Category_ID.length > 0) {
            $.getJSON('/AuditConfigurationMaster/getSubCategoryByAuditCategoryId', { Audit_Category_ID: Audit_Category_ID }, function (data) {
                if (data.length > 0) {
                    $('#Sub_Category_ID option').remove();
                    $('#Sub_Category_ID').append('<option value="">Select Sub Category</option>');
                    for (var i = 0; i < data.length; i++) {
                        $('#Sub_Category_ID').append('<option value="' + data[i].Id + '">' + data[i].Value + '</option>');
                    }
                }
            });
        }
    });

    $("#Sub_Category_ID").on("change", function () {
        $("#Audit_Type_ID").html("");
        $('#Audit_Type_ID').append('<option value="">Select Audit Type</option>');
        var Sub_Category_ID = $("#Sub_Category_ID").val();
        if (Sub_Category_ID.length > 0) {
            $.getJSON('/AuditConfigurationMaster/getAuditTypeBySubCategoryId', { Sub_Category_ID: Sub_Category_ID }, function (data) {
                if (data.length > 0) {
                    $('#Audit_Type_ID option').remove();
                    $('#Audit_Type_ID').append('<option value="">Select Audit Type</option>');
                    for (var i = 0; i < data.length; i++) {
                        $('#Audit_Type_ID').append('<option value="' + data[i].Id + '">' + data[i].Value + '</option>');
                    }
                }
            });
        }
    });

});