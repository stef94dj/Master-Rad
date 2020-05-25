var editor = null;

var nameOnServer = null;
var tableHeader = null;
var tableBody = null;

var instanceId = null;
var instanceTimeStamp = null;

$(document).ready(function () {
    nameOnServer = $('#db-name').val();
    tableHeader = $('#table-header');
    tableBody = $('#table-body');

    instanceId = parseInt($('#instance-id').val());
    instanceTimeStamp = $('#instance-timeStamp').val();

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

function saveSql() {
    var rqBody = {
        "SQLQuery": editor.getValue(),
        "Id": instanceId,
        "TimeStamp": instanceTimeStamp,
    };

    $.ajax({
        url: '/api/Exercise/save/query',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            if (data == null)
                alert('error')
            else {
                timeStamp = data;
            }
        }
    });
}