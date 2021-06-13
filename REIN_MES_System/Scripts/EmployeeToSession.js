$(document).ready(function (e) {



    $(".manager_Line #Shop_ID").change(function (e) {
        var shopId = $("#Shop_ID").val();
        if (shopId) {
            var jsonData = JSON.stringify({ plantId: 7 });
            var url = "/Line/GetLineByShopID";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopLineType, "json");
        }
    });


    $(".manager_Line #Line_ID").change(function (e) {
        // alert("Hi nil");
        // var jsonData = JSON.stringify({ plantId: 7 });
        var lineId = $("#Line_ID").val();
        //var shopId = $("#Shop_ID").val();
        if (lineId) {

            url = "/Station/GetStationByLineID";
            ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showLineStationType, "json");

        }
    });


    $(".manager_Line #Station_ID").change(function (e) {
        var stationId = $("#Station_ID").val();
        //clearSelectBox("Line_ID");
        if (stationId) {

            var jsonData = JSON.stringify({ plantId: 7 });
            var url = "/ShopManagerConfiguration/GetTrainingsByStationID";
            ajaxpack.getAjaxRequest(url, "stationId=" + $("#Station_ID").val() + "", showTrainingsListType, "json");
        }
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
            // alert("hi");
            var jsonData = JSON.stringify({ plantId: 7 });
            var url = "/ShopManagerConfiguration/GetEmployeesBySessionID";
            ajaxpack.getAjaxRequest(url, "sessionId=" + $("#Training_Session_ID").val() + "", showEmployeesListType, "json");

            setTimeout(function () {
                var jsonData = JSON.stringify({ plantId: 7 });
                var url = "/ShopManagerConfiguration/GetAssignedEmployeesBySessionID";
                ajaxpack.getAjaxRequest(url, "sessionId=" + $("#Training_Session_ID").val() + "", showAssignedEmployeesListType, "json");
            }, 1000);
        }
        else {
            clearSelectBox("Employee_ID");
            clearSelectBox("AssignedEmployee");
        }

    });


    function showEmployeesListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Employee_List");
            }
        }
    }


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

    function showTrainingsListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Training_ID");
            }
        }
    }

    function showLineStationType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Station_ID");
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

$(".select_Employee").click(function (e) {
    //$('#select_all').click(function () {
    $('#AssignedEmployee option').prop('selected', true);
    //});

    return true;
});


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