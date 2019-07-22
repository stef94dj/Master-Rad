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
            drawTable(data.tables[0]);
        }
    });
}

function drawTable(data) {
    debugger;
    var tableHeader = $('#table-header');
    var tableBody = $('#table-body');


    tableHeader.html('');
    tableBody.html('');

    $.each(data.columns, function (index, value) {
        tableHeader.append('<th scope="col">' + value + '</th>');
    });

    $.each(data.rows, function (rowIndex, row) {
        var newRow = '<tr>';
        tableBody.append('<tr>');
        $.each(row, function (cellIndex, cell) {
            newRow += '<td>' + cell + '</td>';
        });
        newRow += '</tr>';
        tableBody.append(newRow);

    });
}
