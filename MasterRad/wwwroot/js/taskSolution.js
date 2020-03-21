var nameOnServer = null;
var editor = null;
var tableHeader = null;
var tableBody = null;

$(document).ready(function () {
    tableHeader = $('#table-header');
    tableBody = $('#table-body');
    nameOnServer = $('#name-on-server').val();
    buildSqlEditor(nameOnServer); //sets value for "editor"

    tablesDropdownJS.dropdownSelector = '#tableSelector';
    tablesDropdownJS.loadTablesDropdownData(`/api/Metadata/tables/${nameOnServer}`)
        .then(data => {
            tablesDropdownJS.drawTablesDropdown(data);
        })
        .then(() => {
            tablesDropdownJS.attachOnChangeHandler(tableSelected);
        })
        .then(() => {
            tableSelected();
        });
});

//Table info
function tableSelected() {
    var dbName = $('#name-on-server').val();
    var tableFullName = parseTableName($('#tableSelector').val());
    loadJson(dbName, tableFullName.schemaName, tableFullName.tableName);
}

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

function saveSolution() {
    var columns = queryResultTable.getColumnDescriptions();

    if (columns === null || columns.length < 1) {
        alert("Solution must define at least one column");
        return;
    }

    var rqBody = {
        "Id": $('#task-id').val(),
        "TimeStamp": $('#task-timestamp').val(),
        "SolutionSqlScript": editor.getValue(),
        "Columns": columns
    };

    $.ajax({
        url: '/api/Task/Update/Solution',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            alert('success');
            window.location.replace('/TeacherMenu/Tasks');
        }
    });
}

