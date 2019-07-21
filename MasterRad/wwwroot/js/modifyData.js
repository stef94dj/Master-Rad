$(document).ready(function () {

    dropdownSelector = '#tableSelector';
    populateTableDropdown(dropdownSelector, '/api/Schema/tables/AdventureWorks2017');
    $(dropdownSelector).change(tableSelected);
});

function populateTableDropdown(selector, apiUrl) {
    $.ajax({
        url: apiUrl,
        type: 'GET',
        success: function (data) {
            $('#tableSelector').html('');
            $.each(data, function (index, value) {
                $(selector).append('<option value="' + value + '">' + value + '</option>')
            });
        }
    });
}

function tableSelected() {
    var tableName = $(this).val();

    $.ajax({
        url: '/api/Data/read/AdventureWorks2017/' + tableName,
        type: 'GET',
        success: function (data) {
            debugger;
            drawTable(data);
        }
    });
}

function drawTable(data) {
    debugger;
}
