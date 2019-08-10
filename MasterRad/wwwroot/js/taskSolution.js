var editor = null;
$(document).ready(function () {
    var editorTextArea = document.getElementById('sql-script');
    editor = CodeMirror.fromTextArea(editorTextArea, {
        mode: 'text/x-mssql',
        indentWithTabs: true,
        smartIndent: true,
        lineNumbers: true,
        matchBrackets: true,
        autofocus: true,
        extraKeys: { "Ctrl-Space": "autocomplete" }//,
        //hint: CodeMirror.hint.sql,
        //hintOptions: {
        //    tables: {
        //        "table1": ["col_A", "col_B", "col_C"],
        //        "table2": ["other_columns1", "other_columns2"]
        //    }
        //}
    });
});

function executeScript() {
    var rqBody = {
        "SQLQuery": editor.getValue(),
        "DatabaseName": $('#name-on-server').val()
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