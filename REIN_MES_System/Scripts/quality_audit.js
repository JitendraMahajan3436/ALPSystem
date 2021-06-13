$(document).ready(function (e) {

    $("#Audit_Category_ID").on("change", function () {
        $("#Sub_Category_ID").html("");
        $('#Sub_Category_ID').append('<option value="">- Select Sub Category -</option>');
        var Audit_Category_ID = $("#Audit_Category_ID").val();
        if (Audit_Category_ID.length > 0) {
            $.getJSON('/AuditType/getSubCategoryByCategory', { audit_Category_ID: Audit_Category_ID }, function (data) {
                if (data.length > 0) {
                    $('#Sub_Category_ID option').remove();
                    $('#Sub_Category_ID').append('<option value="">- Select Sub Category -</option>');
                    for (var i = 0; i < data.length; i++) {
                        $('#Sub_Category_ID').append('<option value="' + data[i].Id + '">' + data[i].Value + '</option>');
                    }
                }
            });
        }
    });

    $("#Shop_ID").on("change", function () {
        $("#Model_ID").html("");
        $('#Model_ID').append('<option value="">- Select Model Code -</option>');
        var Shop_ID = $("#Shop_ID").val();
        if (Shop_ID.length > 0) {
            $.getJSON('/AuditCheckList/getModelCodeByShopID', { Shop_ID: Shop_ID }, function (data) {
                if (data.length > 0) {
                    $('#Model_ID option').remove();
                    $('#Model_ID').append('<option value="">- Select Model Code -</option>');
                    for (var i = 0; i < data.length; i++) {
                        $('#Model_ID').append('<option value="' + data[i].Id + '">' + data[i].Value + '</option>');
                    }
                }
            });
        }
    });
});