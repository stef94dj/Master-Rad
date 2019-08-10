var nameOnServer = null;
var editor = null;
var saveScriptUI = {
    lastRepresentedScript: null,
    isScriptRepresentedByResult: function () {
        if (editor === null || this.lastRepresentedScript === null || this.lastRepresentedScript === "")
            return false;

        return editor.getValue() === this.lastRepresentedScript;
    },
    checkDisableSave: function () {
        var saveBtn = $('#save-sln-btn');
        if (saveScriptUI.isScriptRepresentedByResult())
            saveBtn.removeAttr('disabled');
        else
            saveBtn.attr('disabled', true);
    }
}

$(document).ready(function () {
    nameOnServer = $('#name-on-server').val();
    buildSqlEditor(nameOnServer); //sets value for "editor"
});

function executeScript() {
    var rqBody = {
        "SQLQuery": editor.getValue(),
        "DatabaseName": nameOnServer
    };

    executeSqlScript(rqBody, executeScriptCallback);
}
function executeScriptCallback(data) {
    saveScriptUI.lastRepresentedScript = editor.getValue();
    drawReadonlyTable(data);
    saveScriptUI.checkDisableSave();
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

function saveSolution() {
    var columnNames = $.map($('#table-header th'), function (item, index) {
        return item.innerText;
    });

    if (columnNames === null || columnNames.length < 1) {
        alert("Solution must define at least one column");
        return;
    }

    var rqBody = {
        "Id": $('#task-id').val(),
        "TimeStamp": $('#task-timestamp').val(),
        "SolutionSqlScript": editor.getValue(),
        "ColumnNames": columnNames
    };

    $.ajax({
        url: '/api/Task/Update/Solution',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            alert('success');
            window.location.replace('/Setup/Tasks');
        }
    });
}

