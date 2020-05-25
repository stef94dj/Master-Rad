var editor = null;

var nameOnServer = null;
var tableHeader = null;
var tableBody = null;

$(document).ready(function () {
    nameOnServer = $('#db-name').val();
    tableHeader = $('#table-header');
    tableBody = $('#table-body');

    buildSqlEditor(nameOnServer);
    initialiseModifyDatabasePartial();
});

function executeScript() {
    var rqBody = {
        "SQLQuery": editor.getValue(),
        "DatabaseName": $('#db-name').val()
    };

    executeSqlScript(rqBody, executeScriptCallback);
}

function executeScriptCallback(data) {
    saveScriptUI.lastRepresentedScript = editor.getValue();
    drawReadonlyTable(data);
    submitScriptUI.checkDisableSubmit();
    initialiseModifyDatabasePartial();
}