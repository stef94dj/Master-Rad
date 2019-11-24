var nameOnServer = null;
var tableDropdown = null;
$(document).ready(function () {
    var nameOnServer = $('#name-on-server').val();
    tableDropdown = $('#tableSelector');
    buildTablesDropDown(nameOnServer, tableSelected, afterPopulate);
    //$('input.editable-cell').blur(function () { alert('cell focus out'); });
});

function afterPopulate() {
    tableSelected();
}

function tableSelected() {
    var tableFullName = parseTableName(tableDropdown.val());
    var databaseName = $('#name-on-server').val();
    var apiUrl = '/api/Data/read/' + databaseName + '/' + tableFullName.schemaName + '/' + tableFullName.tableName

    $.ajax({
        url: apiUrl,
        type: 'GET',
        success: function (data) {
            getIdentityColumns(databaseName, tableFullName, data);
        }
    });
}

function getIdentityColumns(databaseName, tableFullName, tableData) {
    var apiUrl = '/api/MetaData/identity_columns/' + databaseName + '/' + tableFullName.schemaName + '/' + tableFullName.tableName
    $.ajax({
        url: apiUrl,
        type: 'GET',
        success: function (data) {
            drawTable(tableData, data);
        }
    });
}

function drawTable(tableData, identityColumns) {
    var tableHeader = $('#table-header');
    var tableBody = $('#table-body');

    tableHeader.html('');
    tableBody.html('');

    //columns
    tableHeader.append('<th scope="col"></th>');
    $.each(tableData.columns, function (index, value) {
        tableHeader.append('<th scope="col" data-sql-type="' + value.sqlType + '">' + value.name + '</th>');
    });

    //rows
    $.each(tableData.rows, function (rowIndex, row) {
        var newRow = '<tr><td><button onclick="deleteRecord(this)" type="button" class="btn btn-danger btn-sm">-</button></td>';
        tableBody.append('<tr>');
        $.each(row, function (cellIndex, cell) {
            if (cell == null)
                cell = 'NULL';
            else
                cell = cell.replace(/"/g, '&quot;').replace(/</g, '&lt;').replace(/'/g, "&#39;");

            newRow += '<td><input ';
            if (identityColumns.includes(tableData.columns[cellIndex].name))
                newRow += 'disabled ';
            newRow += 'onblur="editCell(this)" class="form-control form-control-sm" type="text" data-value-original="' + cell + '"value="' + cell + '"></td>';
        });
        newRow += '</tr>';
        tableBody.append(newRow);
    });

    //final row
    var finalRow = '<tr><td><button onclick="insertRecord(this)" type="button" class="btn btn-info btn-sm">+</button></td>';
    $.each(tableData.columns, function (index, value) {
        finalRow += '<td><input ';
        if (identityColumns.includes(tableData.columns[index].name))
            finalRow += 'disabled ';
        finalRow += 'class="form-control form-control-sm" type="text" value=""></td>';
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
    var th = $('#table-header th').eq(index + 1);
    return {
        "ColumnName": th.text(),
        "ColumnType": th.data('sql-type'),
        "Value": getData(inputElem)
    };
}

function getInputValues(trElem, getData, includeDisabled) {
    var inputs = $(trElem).find('td input');

    return jQuery.map(inputs, function (item, index) {
        if (includeDisabled || !$(item).prop('disabled'))
            return getCell(item, index, getData);
    });
}

function deleteRecord(btnElem) {
    var trElem = $(btnElem).parents().eq(1);
    var tableFullName = parseTableName($('#tableSelector').val());

    var rqBody = {
        "DatabaseName": $('#name-on-server').val(),
        "TableName": tableFullName.tableName,
        "SchemaName": tableFullName.schemaName,
        "ValuesUnmodified": getInputValues(trElem, getUnmodifiedValue, true)
    }

    $.ajax({
        url: '/api/Data/delete',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            tableSelected();
        }
    });
}

function insertRecord(btnElem) {
    var trElem = $(btnElem).parents().eq(1);
    var tableFullName = parseTableName($('#tableSelector').val());

    var rqBody = {
        "DatabaseName": $('#name-on-server').val(),
        "TableName": tableFullName.tableName,
        "SchemaName": tableFullName.schemaName,
        "ValuesNew": getInputValues(trElem, getNewValue, false)
    }

    debugger;

    $.ajax({
        url: '/api/Data/insert',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            tableSelected();
        }
    });
}

function editCell(inputElem) {
    var trElem = $(inputElem).parents().eq(1);
    var tableFullName = parseTableName($('#tableSelector').val());
    var inputIndex = trElem.find('td').index($(inputElem).parent()) - 1;

    var cellNew = getCell(inputElem, inputIndex, getNewValue);
    var cellUnmodified = getCell(inputElem, inputIndex, getUnmodifiedValue);

    if (cellNew.Value == cellUnmodified.Value)
        return;

    var rqBody = {
        "DatabaseName": $('#name-on-server').val(),
        "TableName": tableFullName.tableName,
        "SchemaName": tableFullName.schemaName,
        "ValuesUnmodified": getInputValues(trElem, getUnmodifiedValue, true),
        "ValueNew": cellNew
    }

    $.ajax({
        url: '/api/Data/update',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            tableSelected();
        }
    });
}