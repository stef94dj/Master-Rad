var nameOnServer = null;
var editor = null;
var tableHeader = null;
var tableBody = null;
var solutionFormat = null;
var testId = null;
var studentId = null;
var timeStamp = null;

$(document).ready(function () {
    testId = $('#test-id').val();
    studentId = $('#student-id').val();
    tableHeader = $('#table-header');
    tableBody = $('#table-body');
    nameOnServer = $('#name-on-server').val();
    timeStamp = $('#synthesis-test-student-timestamp').val();

    buildSqlEditor(nameOnServer); //sets value for "editor"
    loadSolutionFormat();
});

function loadSolutionFormat() {
    var apiUrl = '/api/Synthesis/Solution/Format/' + testId

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
        "TestId": testId,
        "StudentId": studentId,
        "TimeStamp": timeStamp,
        "SqlScript": editor.getValue()
    };

    $.ajax({
        url: '/api/Synthesis/Submit/Answer',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            if (data == null)
                alert('error')
            else {
                alert('success');
                timeStamp = data;
            }
        }
    });
}

