var editor = null;
var nameOnServer = null;
$(document).ready(function () {
    nameOnServer = $('#name-on-server').val();
    loadTableAndColumnNames();
});

function loadTableAndColumnNames() {
    var apiUrl = '/api/Metadata/table-names/column-names/' + nameOnServer

    $.ajax({
        url: apiUrl,
        type: 'GET',
        success: function (data) {
            prepareTableAndColumNames(data);
        }
    });
}

function prepareTableAndColumNames(data) {
    var res = {};

    $.each(data, function (index, item) {
        res[item.tableFullName] = item.columnNames;
    });

    initSqlEditor(res);
}

function initSqlEditor(tableAndColumnNames) {
    var editorTextArea = document.getElementById('sql-script');
    editor = CodeMirror.fromTextArea(editorTextArea, {
        mode: 'text/x-mssql',
        indentWithTabs: true,
        smartIndent: true,
        lineNumbers: true,
        matchBrackets: true,
        autofocus: true,
        extraKeys: { "Ctrl-Space": "autocomplete" },
        hint: CodeMirror.hint.sql,
        hintOptions: {
            tables: tableAndColumnNames
        }
    });
}

function executeScript() {
    var rqBody = {
        "SQLQuery": editor.getValue(),
        "DatabaseName": nameOnServer
    };

    executeSqlScript(rqBody, drawReadonlyTable);
}

function saveSolution() {
    var rqBody = {
        "Id": $('#task-id').val(),
        "TimeStamp": $('#task-timestamp').val(),
        "SqlScript": $('#sql-script').val()
    };

    $.ajax({
        url: '/api/Template/Update/Solution',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            drawReadonlyTable(data);
        }
    });
}

function drawReadonlyTable(data) {
    var tableData = data.value.tables[0];
    var tableHeader = $('#table-header');
    var tableBody = $('#table-body');

    tableHeader.html('');
    tableBody.html('');

    //columns
    $.each(tableData.columns, function (index, value) {
        tableHeader.append('<th scope="col" data-sql-type="' + value.sqlType + '">' + value.name + '</th>');
    });

    //rows
    $.each(tableData.rows, function (rowIndex, row) {
        var newRow = '<tr>';
        $.each(row, function (cellIndex, cell) {
            if (cell == null)
                cell = 'NULL';
            else
                cell = cell.replace(/"/g, '&quot;').replace(/</g, '&lt;').replace(/'/g, "&#39;");

            newRow += '<td>' + cell + '</td>';
        });
        newRow += '</tr>';
        tableBody.append(newRow);
    });
}