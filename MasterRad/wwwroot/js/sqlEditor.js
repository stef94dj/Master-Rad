function buildSqlEditor(databaseNameOnServer) {
    var apiUrl = '/api/Metadata/table-names/column-names/' + databaseNameOnServer

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
    editor.on("change", function (cm, change) {
        saveScriptUI.checkDisableSave();
    });
}

function executeSqlScript(dto, callback) {
    $.ajax({
        url: '/api/Query/Execute',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(dto),
        success: function (data, textStatus, jQxhr) {
            if (callback == null || callback == undefined)
                defaultExecuteCallback(data);
            else
                callback(data);
        }
    });
}

function drawReadonlyTable(data) {
    var tableData = data.value.tables[0];

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

function defaultExecuteCallback(data) {
    if (data.hasOwnProperty('errors') && data.errors != null && data.errors.length > 0)
        alert("Erorr: " + JSON.stringify(data.errors));
    else {
        alert('ok');
        //populateDropdown(dropdownTableSelector, '/api/Metadata/tables/' + $('#db-name').val());
    }
}

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

var submitScriptUI = {
    isResultFormatValid: function () {

        var qryResultColumnNames = queryResultTable.getColumnNames();
        if (qryResultColumnNames == null || qryResultColumnNames.length == 0 || solutionFormat == null || solutionFormat.length == 0)
            return false;

        if (solutionFormat.length != qryResultColumnNames.length)
            return false;

        for (i = 0; i < solutionFormat.length; i++) {
            if (!qryResultColumnNames.includes(solutionFormat[i]))
                return false;
        }

        return true;
    },
    checkDisableSubmit: function () {
        var saveBtn = $('#save-sln-btn');
        if (saveScriptUI.isScriptRepresentedByResult() && submitScriptUI.isResultFormatValid())
            saveBtn.removeAttr('disabled');
        else
            saveBtn.attr('disabled', true);
    }
}

var queryResultTable = {
    getColumnNames: function () {
        return $.map($('#table-header th'), function (item, index) {
            return item.innerText;
        });
    }
}