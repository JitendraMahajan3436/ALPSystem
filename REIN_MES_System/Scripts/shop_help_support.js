$(document).ready(function(e)
{
    // process to load the support category
    var plantId = 1;
    if (plantId) {
        var url = "/HelpCategory/getCategoryListByPlantId";
        ajaxpack.getAjaxRequest(url, "plantId=" + 1 + "", showCategoryDetails, "json");

    }

    function showCategoryDetails()
    {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
                var res = "";
                for(var i=0;i<jsonRes.length;i++)
                {
                    res = res + '<a href="#" id="' + jsonRes[i].Id + '" class="list-group-item ">' + jsonRes[i].Value + '</a>';
                }
                $("#shop_help_support .list-group").html(res);

                $('#shop_help_support').BootSideMenu({ side: "right" });
            }
        }
    }

    
    $("#shop_help_support").on("click","a",function (e) {
        var id = $(this).attr("id");
        //nil
        if(!confirm('Are  You sure?')) return false;
        var btn =this;
        setTimeout(function () { $(btn).attr('disabled'); }, 0);
        
        //nil
        var url = "/HelpDesk/addRequest";
        ajaxpack.getAjaxRequest(url, "categoryId=" + id + "", showAddRequestDetails, "json");
    });

    function showAddRequestDetails()
    {
        var myajax = ajaxpack.ajaxobj
        var myfiletype = ajaxpack.filetype
        if (myajax.readyState == 4) {

            if (myajax.status == 200 || window.location.href.indexOf("http") == -1) { //if request was successful or running script locally

                var jsonRes = $.parseJSON(myajax.responseText);
               
                if(jsonRes==true)
                {
                    $("#shop_help_support .toggler").trigger("click");

                    //$('#shop_help_support').BootSideMenu({ side: "right", autoClose: true });
                    $("#order-message-block").html("Help request has been sent !!!!!")
                    $("#srlmessage-block").html("Help request has been sent !!!!!");
                    $("#srlmessage-block").removeClass("bg-error"); $("#srlmessage-block").addClass("bg-success");
                    $("#order-message-block").addClass("bg-correct");
                }
            }
        }
    }

});