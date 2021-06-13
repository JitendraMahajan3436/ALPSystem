$(document).ready(function (e) {

    $(".LineName").hide("slow");
    $("#checkBox").change(function (e) {
        // alert($("#checkBox").val());
        if ($(this).val() == "hide") {
            // alert($(this).val());                
            $(".LineName").show("slow");
            $(this).val(true);
            // $("#TACT_Time").val("00:00:00");
        }
        else {
            //alert("hi");
            $(this).val("hide");
            $(".LineName").hide("slow");
            // $("#TACT_Time").val("00:00:00");
        }
    });



    

    // $(".manager_Line #Line_ID").change(function (e) {
    //alert("Hi nil");
    //var jsonData = JSON.stringify({ plantId: 7 });
    //var url = "/Station/GetCriticalStationByLineId";
    //ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showLineCriticalStationType, "json");


    // });

    $(".manager_Line #Line_ID").change(function (e) {
        //alert("Hi nil");
        var jsonData = JSON.stringify({ plantId: 7 });
        var lineId = $("#Line_ID").val();
        //var shopId = $("#Shop_ID").val();
        if (lineId) {
            //var url = "/Station/GetNoCriticalStationByShopID";
            // ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopStationType, "json");

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


            //setTimeout(function () {
            //    url = "/ShopManagerConfiguration/GetSupervisorByLineId";
            //    ajaxpack.getAjaxRequest(url, "lineId=" + $("#Line_ID").val() + "", showLineSupervisorType, "json");
            //}, 2000);
        }
        else
        {
            clearSelectBox("Station_ID");
            clearSelectBox("myListBox1");
        }



    });


    $(".manager_Line #Supervisor_ID").change(function (e) {
        var shopId = $("#Supervisor_ID").val();
        //clearSelectBox("Line_ID");
        if (shopId) {

            var jsonData = JSON.stringify({ plantId: 7 });
            var url = "/ShopManagerConfiguration/GetoperatorsBySupervisorID";
            ajaxpack.getAjaxRequest(url, "supervisorId=" + $("#Supervisor_ID").val() + "", showOperatorsListType, "json");
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

        setTimeout(function () {
            url = "/ShopManagerConfiguration/GetOperatorByStationID";
            ajaxpack.getAjaxRequest(url, "stationId=" + $("#Station_ID").val() + "", showStationOperatorType, "json");
        }, 1000);


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

    //$(".manager_Line #AssignedEmployee").change(function (e) {
    //    // alert("hi");
    //    var assignedEmpId = $("#AssignedEmployee").val();
    //    //clearSelectBox("Line_ID");
    //    if (assignedEmpId) {

    //        var jsonData = JSON.stringify({ plantId: 7 });
    //        var url = "/ShopManagerConfiguration/GetTrainingByAssignedEmployeeId";
    //        ajaxpack.getAjaxRequest(url, "assignedEmpId=" + $("#AssignedEmployee").val() + "", showSkillsListType, "json");
    //    }


    //});



    $(".manager_Line #Manager_ID").change(function (e) {
        var managerId = $("#Manager_ID").val();
        //clearSelectBox("Line_ID");
        if (managerId) {
            //this method is used for showing the list officer list against the shop selected
            var jsonData = JSON.stringify({ plantId: 7 });
            var url = "/ShopManagerConfiguration/GetSupervisorsByManagerID";
            ajaxpack.getAjaxRequest(url, "managerId=" + $("#Manager_ID").val() + "", showSupervisorsListType, "json");

            // //this method is used for showing the list assigned officer list against the shop selected
            setTimeout(function () {
                var jsonData = JSON.stringify({ plantId: 7 });
                var url = "/ShopManagerConfiguration/GetAssignedSupervisorsByManagerID";
                ajaxpack.getAjaxRequest(url, "managerId=" + $("#Manager_ID").val() + "", showAssignedSupervisorsListType, "json");
            }, 1000);
        }
        else
        {
            clearSelectBox("ListofSupervisor");
            clearSelectBox("selectedSupervisors");
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



    $(".manager_Line #Shop_ID").change(function (e) {
        //alert("hi");
        var shopId = $("#Shop_ID").val();
        //alert(shopId);
       // clearSelectBox("Line_ID");
        if (shopId) {

            var jsonData = JSON.stringify({ plantId: 7 });
            var url = "/Line/GetLineByShopID";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopLineType, "json");
        }

        setTimeout(function () {
            url = "/ShopManagerConfiguration/GetSupervisorByShopId";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopSupervisorType, "json");
        }, 1000);

        //this check that selected list containns the name which is not assigned as supervisor,operator or alredy assigned manager
        setTimeout(function () {
            url = "/ShopManagerConfiguration/GetManagerByShopId";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopManagerType, "json");
        }, 2000);

        //this method is used for AssignSupervisorToManager form to get manager list against that shop
        setTimeout(function () {
            url = "/ShopManagerConfiguration/GetManagerListByShopId";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopManagerListType, "json");
        }, 3000);


    });

    $(".manager_Line #Plant_ID").change(function (e) {
       // alert("gft");
        var plantId = $("#Plant_ID").val();
       // alert(plantId);
        //clearSelectBox("Shop_ID");
        //clearSelectBox("Line_ID");
        //clearSelectBox("Station_ID");
        if (plantId) {
            var url = "/Shop/GetShop";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showShopID, "json");

            setTimeout(function () {
                url = "/User/GetUserByPlantID";
                ajaxpack.getAjaxRequest(url, "plantId=" + $("#Plant_ID").val() + "", showEmployeeList, "json");
            }, 1000);
        }



    });


    function showShopID() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Shop_ID");
            }
        }
    }

    function showPlantShopID() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Shop_ID");
            }
        }
    }


    function showShopStationType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Station_ID");
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


    //function showLineStationType() {
    //    var myajax = ajaxpack.ajaxobj
    //    var myfiletype = ajaxpack.filetype
    //    if (myajax.readyState == 4) {
    //        if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

    //            var jsonRes = $.parseJSON(myajax.responseText);
    //            SelectOptionHTML(jsonRes, "Station_ID");
    //        }
    //    }
    //}


    $(".select_defect").click(function (e) {

        $('#myListBox1 option').prop('selected', true);


        return true;
    });


    $(".select_manager").click(function (e) {

        $('#selectedSupervisors option').prop('selected', true);


        return true;
    });

    $(".select_Employee").click(function (e) {

        $('#AssignedEmployee option').prop('selected', true);


        return true;
    });


    function showShopCriticalStationType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "myListBox1");
            }
        }
    }

    function showLineCriticalStationType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "myListBox1");
            }
        }
    }

    function showLineSupervisorType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Supervisor_ID");
            }
        }
    }

   


    function showStationOperatorType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Operator_ID");
            }
        }
    }

    function showShopSupervisorType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Employee_ID");
            }
        }
    }

    function showShopManagerType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Manager_ID");
            }
        }
    }

    function showShopManagerListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Employee_ID");
            }
        }
    }

    function showOperatorsListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "ListofOperator");
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

    function showSessionsListType()
    {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Training_Session_ID");
            }
        }

    }

    function showSkillsListType()
    {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Skill_ID");
            }
        }
        
    }

    function showSupervisorsListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "ListofSupervisor");
            }
        }
    }


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

    function showAssignedSupervisorsListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "selectedSupervisors");
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

    function showEmployeeList() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Employee_ID");
            }
        }
    }

    function showPlantLineType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Line_Type_Id");

                var url = "/Shop/GetShopByPlantID";
                ajaxpack.getAjaxRequest(url, "plantId=" + $("#Plant_ID").val() + "", showPlantShop, "json");
            }
        }
    }

    function showPlantShop() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Shop_Id");
            }
        }
    }

    function SelectOptionHTML(jsonRes, targetId) {
        //var jsonRes = $.parseJSON(myajax.responseText);        
        var res = "";
        for (var i = 0; i < jsonRes.length; i++) {
            res += "<option value='" + jsonRes[i].Id + "'>" + jsonRes[i].Value + "</option>";
        }

        res = "<option value=''>" + $("#" + targetId + " option:first").html() + "</option>" + res;
        $("#" + targetId).html(res);
    }

});



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