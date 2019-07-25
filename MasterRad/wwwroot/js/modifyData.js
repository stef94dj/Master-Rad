$(document).ready(function () {
    dropdownSelector = '#tableSelector';
    populateTableDropdown(dropdownSelector, '/api/Metadata/tables/' + $('#name-on-server').val());
    $(dropdownSelector).change(tableSelected);
    $('input.editable-cell').blur(function () { alert('cell focus out'); });
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
        var newRow = '<tr><td><button onclick="deleteRecord(this)" type="button" class="btn btn-danger btn-sm">-</button></td>';
        tableBody.append('<tr>');
        $.each(row, function (cellIndex, cell) {
            if (cell == null)
                cell = 'NULL';
            else
                cell = cell.replace(/"/g, '&quot;').replace(/</g, '&lt;').replace(/'/g, "&#39;");

            newRow += '<td><input onblur="editCell(this)" class="form-control form-control-sm" type="text" data-value-original="' + cell + '"value="' + cell + '"></td>';
        });
        newRow += '</tr>';
        tableBody.append(newRow);
    });

    //final row
    var finalRow = '<tr><td><button onclick="insertRecord(this)" type="button" class="btn btn-info btn-sm">+</button></td>';
    $.each(data.columns, function (index, value) {
        finalRow += '<td><input class="form-control form-control-sm" type="text" value=""></td>';
    });
    finalRow += '</tr>';
    tableBody.append(finalRow);
}

function getNewValue(element) {
    return $(element).val();
}

function getUnmodifiedValue(element) {
    return $(element).data('value-original');
}

function getCell(inputElem, index, getData) {
    return {
        "ColumnName": $('#table-header th').eq(index + 1).text(),
        "Value": getData(inputElem)
    };
}

function getInputValues(trElem, getData) {
    debugger;
    var inputs = $(trElem).find('td input');

    return jQuery.map(inputs, function (item, index) {
        return getCell(item, index, getData);
    });
}

function deleteRecord(btnElem) {
    var trElem = $(btnElem).parents().eq(1);

    var rqBody = {
        "DatabaseName": "AdventureWorks2017",
        "TableName": $('#tableSelector').val(),
        "ValuesUnmodified": getInputValues(trElem, getUnmodifiedValue)
    }

    $.ajax({
        url: '/api/Data/delete',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            alert("success");
        }
    });
}

function insertRecord(btnElem) {
    var trElem = $(btnElem).parents().eq(1);

    var rqBody = {
        "DatabaseName": "AdventureWorks2017",
        "TableName": $('#tableSelector').val(),
        "ValuesNew": getInputValues(trElem, getNewValue)
    }

    $.ajax({
        url: '/api/Data/insert',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            alert("success");
        }
    });
}

function editCell(inputElem) {
    var trElem = $(inputElem).parents().eq(1);
    var inputIndex = trElem.find('td').index($(inputElem).parent()) - 1;

    debugger;
    var cellNew = getCell(inputElem, inputIndex, getNewValue);
    var cellUnmodified = getCell(inputElem, inputIndex, getUnmodifiedValue);

    if (cellNew.Value == cellUnmodified.Value)
        return;

    var rqBody = {
        "DatabaseName": "AdventureWorks2017",
        "TableName": $('#tableSelector').val(),
        "ValuesUnmodified": getInputValues(trElem, getUnmodifiedValue),
        "ValueNew": cellNew
    }

    $.ajax({
        url: '/api/Data/update',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            alert("success");
        }
    });
}