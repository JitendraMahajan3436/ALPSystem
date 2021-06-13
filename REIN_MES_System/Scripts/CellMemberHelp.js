$(document).ready(function (e) {
    
    var url = "/HelpDesk/GetHelpType";
    ajaxpack.getAjaxRequest(url, "", showHelpType, "json");
    
    function showHelpType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                var res = "";
                for (var i = 0; i < jsonRes.length; i++) {
                    res += "<a id='help_type_id_" + jsonRes[i].Help_Type_ID + "' href='javascript:void(0);' class='list-group-item active'>" + jsonRes[i].Help_Name + "</a>"; //<option value='" +  + "'>" + jsonRes[i].Value + "</option>";                    
                }

                $("#CellMemberHelp .list-group").html(res);
            }
        }
    }

    $("#CellMemberHelp").on("click",".list-group a", function (e) {
        //alert($(this).attr("id"));
        var helpId = $(this).attr("id").split("help_type_id_");
        helpId = helpId[1];
        alert(helpId);
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