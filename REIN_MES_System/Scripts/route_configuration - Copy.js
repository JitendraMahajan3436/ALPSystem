$(document).ready(function (e) {


    //alert("ok");

    // initialize the items
    stationItemsDraggable();

    stationItemsSortable();
    stationIconDraggable();
    var rowTotal = 25, columnTotal = 25;
    loadHTMLTable();
    function loadHTMLTable() {
        var res = "";
        var count = 1;
        for (var i = 1; i <= rowTotal; i++) {
            res += "<tr>";
            for (var j = 1; j <= columnTotal; j++) {
                res = res + "<td id='" + count + "'></td>";
                count = count + 1;
            }
            res += "</tr>";
        }

        $("#line_configurations").html(res);

        //$("#line_configurations td").trigger("droppable");

    }

    function stationItemsDraggable() {
        $(".station_items").draggable({

            cancel: "a.ui-icon", // clicking an icon won't initiate dragging
            revert: "invalid", // when not dropped, the item will revert back to its initial position
            containment: "document",
            helper: "clone",
            cursor: "move"
        });
    }

    function stationItemsSortable() {
        $(".station_items").sortable();
    }

    function stationIconDraggable() {
        $(".station_icon_items").draggable({
            cancel: "a.ui-icon", // clicking an icon won't initiate dragging
            revert: "invalid", // when not dropped, the item will revert back to its initial position
            containment: "document",
            helper: "clone",
            cursor: "move"
        });
    }

    // line station or icon added in table should be draggable to remove from table
    function lineStationDraggable() {
        $("#line_configurations td .station_items").draggable({
            cancel: "a.ui-icon", // clicking an icon won't initiate dragging
            revert: "invalid", // when not dropped, the item will revert back to its initial position
            containment: "document",
            helper: "clone",
            cursor: "move"
        });
    }

    $("#line_configurations td").droppable({
        accept: ".station_items, .station_icon_items",
        activeClass: "ui-state-highlight",
        drop: function (event, ui) {
            //deleteImage( ui.draggable );
            console.log(ui.draggable.attr("id"));
            var draggableId = ui.draggable.attr("id");
            var droppableId = $(this).attr("id");

            var station_name = $("#" + draggableId).html();

            if ($("#" + $(this).attr("id")).html() == null || $("#" + $(this).attr("id")).html() == "") {
                if (ui.draggable.attr("class").indexOf("station_items") > 0) {
                    $("#" + $(this).attr("id")).html("<div id='" + draggableId + "' class='bg-aqua color-palette station_items'><span>" + station_name + "</span></div>");
                    ui.draggable.remove();
                }
                else {
                    if (ui.draggable.attr("class").indexOf("station_icon_items") > 0) {
                        // calculate class
                        var station_icon_class = "";
                        if (ui.draggable.attr("class").indexOf("line_arrow_left") > 0) { station_icon_class = "line_arrow_left"; }
                        else if (ui.draggable.attr("class").indexOf("line_arrow_left") > 0) { station_icon_class = "line_arrow_left"; }
                        else if (ui.draggable.attr("class").indexOf("line_arrow_right") > 0) { station_icon_class = "line_arrow_right"; }
                        else if (ui.draggable.attr("class").indexOf("line_arrow_down") > 0) { station_icon_class = "line_arrow_down"; }
                        else if (ui.draggable.attr("class").indexOf("line_arrow_up") > 0) { station_icon_class = "line_arrow_up"; }
                        else if (ui.draggable.attr("class").indexOf("line_start") > 0) { station_icon_class = "line_start"; }
                        else if (ui.draggable.attr("class").indexOf("line_end") > 0) { station_icon_class = "line_end"; }

                        var isOk = true;
                        if (station_icon_class == "line_arrow_left" || station_icon_class == "line_arrow_right") {
                            console.log($(this).attr("id"));
                            if (!isCorrectLeft($(this).attr("id"))) {
                                isOk = false;
                                $("#configuration_errors").html("Invalid place to add the control");
                            }
                        }
                        if (station_icon_class == "line_arrow_up" || station_icon_class == "line_arrow_down") {
                            console.log($(this).attr("id"));
                            if (!isCorrectTop($(this).attr("id"))) {
                                isOk = false;
                                $("#configuration_errors").html("Invalid place to add the control");
                            }
                        }

                        if (station_icon_class == "line_arrow_right" || station_icon_class == "line_arrow_left") {
                            console.log($(this).attr("id"));
                            if (!isCorrectRight($(this).attr("id"))) {
                                isOk = false;
                                $("#configuration_errors").html("Invalid place to add the control");
                            }
                        }

                        if (station_icon_class == "line_arrow_up" || station_icon_class == "line_arrow_down") {
                            console.log($(this).attr("id"));
                            if (!isCorrectBottom($(this).attr("id"))) {
                                isOk = false;
                                $("#configuration_errors").html("Invalid place to add the control");
                            }
                        }

                        if (isOk) {

                            $("#configuration_errors").html("");
                            var res = $("#" + draggableId).html();
                            $("#" + $(this).attr("id")).html("<div id='" + draggableId + "' class='bg-aqua color-palette station_icon_items " + station_icon_class + "'>" + res + "</div>");
                        }

                    }
                }

            }

            lineStationDraggable();
            stationIconDraggable();
        }
    });



    function isCorrectLeft(tblClassId) {
        var i = 1;
        for (var j = 1; j <= columnTotal; j++) {
            if (tblClassId == i) {
                return false;
            }
            i = i + columnTotal;
        }

        return true;
    }

    function isCorrectTop(tblClassId) {
        var i = 1;
        for (var j = 1; j <= columnTotal; j++) {
            if (tblClassId == j) {
                return false;
            }
            //i=i+j;
        }

        return true;
    }

    function isCorrectRight(tblClassId) {
        var i = columnTotal;
        for (var j = 1; j <= rowTotal; j++) {
            if (tblClassId == i) {
                return false;
            }
            i = columnTotal * j;
        }

        return true;
    }

    function isCorrectBottom(tblClassId) {

        var i = columnTotal;
        var item = columnTotal * (rowTotal - 1);
        item = item + 1;
        var itemTotal = rowTotal * columnTotal;
        for (var j = item; j <= itemTotal; j++) {
            if (tblClassId == j) {
                return false;
            }
            //i=columnTotal * j;
        }

        return true;
    }



    $(".validate_route_configuration").click(function (e) {
        var routeArray = new Array();
        var rowItem = 1;
        $("#line_configurations td").removeClass("table_error");
        for (var i = 1; i <= rowTotal; i++) {
            for (var j = 1; j <= columnTotal; j++) {

                if ($("#line_configurations td#" + rowItem).html() == "" || $("#line_configurations td#" + rowItem).html() == null) {
                    // no record

                    //rowItem=rowItem+1;
                }
                else {

                    // process to check items
                    var station_icon_class = getDroppedItem(rowItem);

                    /*$("#line_configurations td#"+i+" div");
                     if(station_icon_class.attr("class").indexOf("line_arrow_left") > 0) { station_icon_class="line_arrow_left"; }
                    else if(station_icon_class.attr("class").indexOf("line_arrow_left") > 0) { station_icon_class="line_arrow_left"; }
                    else if(station_icon_class.attr("class").indexOf("line_arrow_right") > 0) { station_icon_class="line_arrow_right"; }
                    else if(station_icon_class.attr("class").indexOf("line_arrow_down") > 0) { station_icon_class="line_arrow_down"; }
                    else if(station_icon_class.attr("class").indexOf("line_arrow_up") > 0) { station_icon_class="line_arrow_up"; }
                    else if(station_icon_class.attr("class").indexOf("line_start") > 0) { station_icon_class="line_start"; }
                    else if(station_icon_class.attr("class").indexOf("line_end") > 0) { station_icon_class="line_end"; }
                    else if(station_icon_class.attr("class").indexOf("station_items") > 0) { station_icon_class="station_items"; } */
                    //alert(station_icon_class);

                    if (station_icon_class == "station_items") {
                        // check top, left, right and down arrow only from the station
                        var currentTop = rowItem - columnTotal;
                        var currentLeft = rowItem - 1;
                        if (currentLeft % columnTotal == 0) {
                            currentLeft = 0;
                        }
                        var currentRight = rowItem + 1;
                        if (currentRight % (columnTotal + 1) == 0) {
                            currentRight = 0;
                        }
                        var currentDown = rowItem + columnTotal;

                        //alert("inside i="+i+" j= "+j+" currentTop="+currentTop+ " rowItem :"+rowItem);
                        // process top
                        if (currentTop > 0) {
                            // check if up arrow
                            var topArrow = getDroppedItem(currentTop);
                            if (topArrow == "" || topArrow == null) {
                                //$("#line_configurations td#"+currentTop).css("border-color","red");
                            }
                            else
                                if (topArrow == "line_arrow_up") {
                                    // process to check station attached to top arrow										
                                    var itemIndex = currentTop - columnTotal;
                                    if (itemIndex > 0) {
                                        // check is station
                                        var stationItem = getDroppedItem(itemIndex);
                                        if (stationItem == "station_items") {
                                            // ok found link
                                            saveRouteStation(rowItem, itemIndex, 0, 0);
                                        }
                                        else {
                                            $("#line_configurations td#" + currentTop).addClass("table_error");
                                        }
                                    }
                                }

                        }


                        // process bottom
                        //alert("currentDown"+currentDown);
                        if (currentDown < (columnTotal * rowTotal)) {
                            // check if up arrow
                            var bottomArrow = getDroppedItem(currentDown);
                            if (bottomArrow == "" || bottomArrow == null) {
                                //$("#line_configurations td#"+currentTop).css("border-color","red");
                            }
                            else
                                if (bottomArrow == "line_arrow_down") {
                                    // process to check station attached to top arrow										
                                    var itemIndex = currentDown + columnTotal;
                                    //alert("itemIndex"+itemIndex);
                                    if (itemIndex > 0) {
                                        // check is station
                                        var stationItem = getDroppedItem(itemIndex);
                                        if (stationItem == "station_items") {
                                            // ok found link
                                            saveRouteStation(rowItem, itemIndex, 0, 0);
                                        }
                                        else {
                                            $("#line_configurations td#" + currentDown).addClass("table_error");
                                        }
                                    }
                                }

                        }




                        // process left
                        //alert("currentDown"+currentDown);
                        if (currentLeft > 0) {
                            // check if up arrow
                            var currentLeftArrow = getDroppedItem(currentLeft);
                            if (currentLeftArrow == "" || currentLeftArrow == null) {
                                //$("#line_configurations td#"+currentTop).css("border-color","red");
                            }
                            else
                                if (currentLeftArrow == "line_arrow_left") {
                                    // process to check station attached to top arrow										
                                    var itemIndex = currentLeft - 1;
                                    //alert("itemIndex"+itemIndex);
                                    if (itemIndex > 0) {
                                        // check is station
                                        var stationItem = getDroppedItem(itemIndex);
                                        if (stationItem == "station_items") {
                                            // ok found link
                                            saveRouteStation(rowItem, itemIndex, 0, 0);
                                        }
                                        else {
                                            $("#line_configurations td#" + currentLeft).addClass("table_error");
                                        }
                                    }
                                }

                        }



                        // process right
                        //alert("currentRight"+currentRight);
                        if (currentRight > 0) {
                            // check if up arrow
                            var rightArrow = getDroppedItem(currentRight);
                            if (rightArrow == "" || rightArrow == null) {
                                //$("#line_configurations td#"+currentTop).css("border-color","red");
                            }
                            else
                                if (rightArrow == "line_arrow_right") {
                                    // process to check station attached to top arrow										
                                    var itemIndex = currentRight + 1;
                                    //alert("itemIndex"+itemIndex);
                                    if (itemIndex > 0) {
                                        // check is station
                                        var stationItem = getDroppedItem(itemIndex);
                                        if (stationItem == "station_items") {
                                            // ok found link
                                            saveRouteStation(rowItem, itemIndex, 0, 0);
                                        }
                                        else {
                                            $("#line_configurations td#" + currentRight).addClass("table_error");
                                        }
                                    }
                                }

                        }



                    }


                    // check if it is arrow and not connected to any station

                    if (station_icon_class == "line_arrow_left" || station_icon_class == "line_arrow_right" || station_icon_class == "line_arrow_down" || station_icon_class == "line_arrow_up") {

                        var indexCheck = 0;
                        if (station_icon_class == "line_arrow_left") {
                            indexCheck = rowItem + 1;
                        }
                        else
                            if (station_icon_class == "line_arrow_right") {
                                indexCheck = rowItem - 1;
                            }
                            else
                                if (station_icon_class == "line_arrow_down") {
                                    indexCheck = rowItem - columnTotal;
                                }
                                else
                                    if (station_icon_class == "line_arrow_up") {
                                        indexCheck = rowItem + columnTotal;
                                    }

                        var stationItem = getDroppedItem(indexCheck);
                        if (stationItem == "station_items") {
                            // ok found link
                        }
                        else {
                            $("#line_configurations td#" + rowItem).addClass("table_error");
                        }
                    }


                    // check line start and line end


                }


                rowItem = rowItem + 1;

            }
        }
        alert("ok");
    });


    function getDroppedItem(i) {
        if ($("#line_configurations td#" + i).html() == "" || $("#line_configurations td#" + i).html() == null) {
            return "";
        }
        else {

            var station_icon_class = $("#line_configurations td#" + i + " div");
            if (station_icon_class.attr("class").indexOf("line_arrow_left") > 0) { station_icon_class = "line_arrow_left"; }
            else if (station_icon_class.attr("class").indexOf("line_arrow_left") > 0) { station_icon_class = "line_arrow_left"; }
            else if (station_icon_class.attr("class").indexOf("line_arrow_right") > 0) { station_icon_class = "line_arrow_right"; }
            else if (station_icon_class.attr("class").indexOf("line_arrow_down") > 0) { station_icon_class = "line_arrow_down"; }
            else if (station_icon_class.attr("class").indexOf("line_arrow_up") > 0) { station_icon_class = "line_arrow_up"; }
            else if (station_icon_class.attr("class").indexOf("line_start") > 0) { station_icon_class = "line_start"; }
            else if (station_icon_class.attr("class").indexOf("line_end") > 0) { station_icon_class = "line_end"; }
            else if (station_icon_class.attr("class").indexOf("station_items") > 0) { station_icon_class = "station_items"; }

            return station_icon_class;
        }
    }

    function saveRouteStation(currentStation, nextStation, isStartStation, isEndStation)
    {
        
        currentStation = $("#line_configurations td#" + currentStation + " div").attr("id");
        currentStation = currentStation.replace("station_", "");

        nextStation = $("#line_configurations td#" + nextStation + " div").attr("id");
        nextStation = nextStation.replace("station_", "");

        var link = { currentStation: currentStation, nextStation: nextStation, isStartStation: "0", isEndStation: "0" };

        //var currentStation = $("#line_configurations td#" + rowItem + " div").attr("id");

        var route_station_insert = new Array();
        var route_station = "";
        if ($("#hdnRouteConfiguration").val() == "" || $("#hdnRouteConfiguration").val() == null)
        {
            route_station_insert[0] = link;
        }
        else
        {
            route_station_insert = $.parseJSON($("#hdnRouteConfiguration").val());
            route_station_insert[route_station_insert.length + 1] = link;
        }
        

        $("#hdnRouteConfiguration").val(JSON.stringify(route_station_insert));
    }

    function isStation() {
    }

    $(".station_lists").droppable({
        accept: "#line_configurations td .station_items",
        activeClass: "ui-state-highlight",
        drop: function (event, ui) {
            //deleteImage( ui.draggable );
            console.log($(this).attr("id"));


            var draggableId = ui.draggable.attr("id");

            var res = "<div id='" + draggableId + "' class='bg-aqua color-palette station_items'><span>" + $("#" + draggableId).html() + "</span></div>";
            $(".station_lists").html($(".station_lists").html() + res);

            ui.draggable.remove();


            stationItemsDraggable();

            stationItemsSortable();

        }
    });

    $(".site_trash_box").droppable({
        accept: "#line_configurations td .station_icon_items",
        activeClass: "ui-state-highlight",
        drop: function (event, ui) {
            //deleteImage( ui.draggable );
            console.log($(this).attr("id"));

            ui.draggable.remove();

            stationItemsDraggable();

            stationItemsSortable();

        }
    });

    $(".route_configuration #Plant_ID").change(function (e) {
        //var jsonData = JSON.stringify({ plantId: 7 });
        
        var plantId = $("#Plant_ID").val();
        if (plantId) {
            var url = "/Shop/GetShopByPlantID";
            ajaxpack.getAjaxRequest(url, "plantId=" + $("#Plant_ID").val() + "", showPlantShopID, "json");
        }
        else {
            // clear the line type and shop
            clearSelectBox("Line_ID");
            clearSelectBox("Shop_ID");
        }
    });

    function showPlantShopID() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                clearStationLists();
                //loadHTMLTable();

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Shop_ID");
            }
        }
    }

    $(".route_configuration #Shop_ID").change(function (e) {
        //var jsonData = JSON.stringify({ plantId: 7 });

        //clearStationLists();
        //loadHTMLTable();

        var shopId = $("#Shop_ID").val();
        if (shopId) {
            var url = "/Line/GetLineByShopID";
            ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showLineShopID, "json");

            setTimeout(
              function () {
                  var url = "/Station/GetStationByShopIDForRoute";
                  ajaxpack.getAjaxRequest(url, "shopId=" + $("#Shop_ID").val() + "", showStationShopID, "json");
              }, 2000);

            
        }
        else {
            // clear the line type and shop
            clearSelectBox("Line_ID");            
        }
    });

    function showLineShopID()
    {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                clearStationLists();
                //loadHTMLTable();

                var jsonRes = $.parseJSON(myajax.responseText);
                SelectOptionHTML(jsonRes, "Line_ID");
            }
        }
    }

    function showStationShopID()
    {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                //loadHTMLTable();

                var jsonRes = $.parseJSON(myajax.responseText);
                var res = "";
                for (var i = 0; i < jsonRes.length; i++) {
                    res += "<div id='station_" + jsonRes[i].Id + "' class='bg-aqua color-palette station_items'><span class='" + jsonRes[i].Id + "' >" + jsonRes[i].Value + "</span></div><br/>";
                    //res += "<option value='" + jsonRes[i].Id + "'>" + jsonRes[i].Value + "</option>";
                }
                //var res = '<div id="station_1" class="bg-aqua color-palette station_items"><span class="1" >Station 1</span></div>';
                $(".station_lists").html(res);
                $("#line_configurations td").trigger("droppable");
                stationItemsDraggable();
                //SelectOptionHTML(jsonRes, "Line_ID");
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

    function clearSelectBox(targetId) {
        var res = "";
        res = "<option value=''>" + $("#" + targetId + " option:first").html() + "</option>";
        $("#" + targetId).html(res);
    }

    function clearStationLists()
    {
        $(".station_lists").html("");
    }

    function clearRoute() {
        loadHTMLTable();
    }

});