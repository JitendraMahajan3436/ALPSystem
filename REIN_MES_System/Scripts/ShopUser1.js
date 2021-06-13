

$(document).ready(function (e) {

    $('#Partgroup_ID').focus();

    $('form').on('keyup keypress', function (e) {
        var code = e.keyCode || e.which;
        if (code == 13) {
            e.preventDefault();
            if (($('#Partgroup_ID').val() != "" && $('#Partgroup_ID').val() != null)) {
                $('#Part_No').focus();
            }
            else if (($('#Part_No').val() != "" && $('#Part_No').val() != null)) {

                var tokenId = $('#Partgroup_ID').val();
                if (tokenId) {

                    var jsonData = JSON.stringify({ plantId: 7 });
                    var url = "/CriticalStationLogin/IsExistToken";
                    ajaxpack.getAjaxRequest(url, "tokenId=" + $("#Partgroup_ID").val() + "", showTokenListType, "json");
                }                
            }
        }

        // return false;
    });
    function showTokenListType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes == true) {
                    var jsonData = JSON.stringify({ plantId: 7 });
                    var url = "/CriticalStationLogin/IsOperatorToStationAllocated";
                    var res = "operatorTokenId=" + $("#Partgroup_ID").val() + "&stationId=" + stationId + "&shiftId=" + shiftId;

                    ajaxpack.getAjaxRequest(url, res, showOperatorToStationAllocatedType, "json");
                }
                else {

                }
                // SelectOptionHTML(jsonRes, "Partgroup_ID");
            }
        }
    }

    function showOperatorToStationAllocatedType() {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {
            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                if (jsonRes == true) {
                    //redirect to home page of shop user
                }
                else {
                    //alert u r nt valid
                }
                //SelectOptionHTML(jsonRes, "Partgroup_ID");
            }
        }
    }        
});