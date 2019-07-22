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
    var tableHeader = $('#table-header');
    var tableBody = $('#table-body');


    tableHeader.html('');
    tableBody.html('');

    //columns
    tableHeader.append('<th scope="col"></th>');
    $.each(data.columns, function (index, value) {
        tableHeader.append('<th scope="col">' + value + '</th>');
    });

    //rows
    $.each(data.rows, function (rowIndex, row) {
        var newRow = '<tr><td><button type="button" class="btn btn-danger btn-sm" onclick="deleteRecord(this)">-</button></td>';
        tableBody.append('<tr>');
        $.each(row, function (cellIndex, cell) {
            cell = cell.replace(/"/g, '&quot;').replace(/</g, '&lt;').replace(/'/g, "&#39;");
            newRow += '<td><input class="form-control form-control-sm" type="text" data-value-original="' + cell + '"value="' + cell + '"></td>';
        });
        newRow += '</tr>';
        tableBody.append(newRow);
    });

    //final row
    var finalRow = '<td><button type="button" class="btn btn-info btn-sm">+</button></td>';
    $.each(data.columns, function (index, value) {
        finalRow += '<td><input class="form-control form-control-sm" type="text" value=""></td>';
    });
    tableBody.append(finalRow);
}

function deleteRecord(btnElem) {
    var inputs = $(btnElem).parent().parent().find('td input');

    var requestValues = jQuery.map(inputs, function (item, index) {
        return {
            "ColumnName": $('#table-header th').eq(index + 1).text(),
            "Value": $(item).attr('data-value-original')
        };
    });

    var rqBody = {
        "DatabaseName": "AdventureWorks2017",
        "TableName": $('#tableSelector').val(),
        "Values": requestValues
    }

    $.ajax({
        url: '/api/Data/delete',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            debugger;
            alert("success");
        }
    });
}
