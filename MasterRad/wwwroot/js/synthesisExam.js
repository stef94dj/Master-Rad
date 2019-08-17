var nameOnServer = null;
var editor = null;
var tableHeader = null;
var tableBody = null;
var solutionFormat = null;

$(document).ready(function () {
    tableHeader = $('#table-header');
    tableBody = $('#table-body');
    nameOnServer = $('#name-on-server').val();
    buildSqlEditor(nameOnServer); //sets value for "editor"
    loadSolutionFormat();
});

function loadSolutionFormat() {
    var apiUrl = '/api/Synthesis/Solution/Format/' + $('#test-id').val()

    $.ajax({
        url: apiUrl,
        type: 'GET',
        success: function (data) {
            solutionFormat = data;
        }
    });
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
    submitScriptUI.checkDisableSubmit();
}
function submitAnswer() {

    //upit, test id, (student id sa servera)
    //api da odradi validaciju

    var rqBody = {
        "Id": $('#task-id').val(),
        "TimeStamp": $('#task-timestamp').val(),
        "SolutionSqlScript": editor.getValue(),
        "ColumnNames": columnNames
    };

    $.ajax({
        url: '/api/Syntheis/Submit/Answer',
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

