$(document).ready(function (e) {

    $("#Employee_ID").select2({

        allowClear: true
    });

    $(".manager_Line #Training_ID").change(function (e) {
        // alert("hi");
        var trainingId = $("#Training_ID").val();
        //clearSelectBox("Line_ID");
        if (trainingId) {

            var jsonData = JSON.stringify({ plantId: 7 });
            var url = "/ShopManagerConfiguration/GetSessionsByTrainingId";
            ajaxpack.getAjaxRequest(url, "trainingId=" + $("#Training_ID").val() + "", showSessionsListType, "json");
        }


    });

    $(".manager_Line #Training_Session_ID").change(function (e) {
        //alert("hi");
        var sessionId = $("#Training_Session_ID").val();
        //clearSelectBox("Line_ID");
        if (sessionId) {
           
                var jsonData = JSON.stringify({ plantId: 7 });
                var url = "/ShopManagerConfiguration/GetAssignedEmployeesBySessionID";
                ajaxpack.getAjaxRequest(url, "sessionId=" + $("#Training_Session_ID").val() + "", showAssignedEmployeesListType, "json");
           
        }      

    });

    function showAssignedEmployeesListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "AssignedEmployee");
            }
        }
    }

    function showSessionsListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Training_Session_ID");
            }
        }

    }
   


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