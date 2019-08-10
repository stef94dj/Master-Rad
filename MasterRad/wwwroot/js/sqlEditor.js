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

function defaultExecuteCallback(data) {
    if (data.hasOwnProperty('errors') && data.errors != null && data.errors.length > 0)
        alert("Erorr: " + JSON.stringify(data.errors));
    else {
        alert('ok');
        //populateDropdown(dropdownTableSelector, '/api/Metadata/tables/' + $('#db-name').val());
    }
}