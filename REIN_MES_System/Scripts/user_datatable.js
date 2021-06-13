$(document).ready(function (e) {



    // datatable with search, filters and paginations
    $('.datatable_completes').dataTable({
        "bPaginate": true,
        "bLengthChange": false,
        "bFilter": true,
        "bSort": true,
        "bInfo": true,
        "bAutoWidth": false,
        "aoColumnDefs": [
        { 'bSortable': false, 'aTargets': [-1] }
        ]
    });

});