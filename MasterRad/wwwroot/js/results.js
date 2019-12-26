var testId;
$(document).ready(function () {
    initializeTooltips();
    testId = $('#test-id').val();
    loadEvaluationResults($('#evaluation-results-tbody'), `/api/evaluate/get/papers/synthesis/${testId}`);

    $.when(loadEvaluationResults($('#evaluation-results-tbody'), `/api/evaluate/get/papers/synthesis/${testId}`))
        .then(connectToSignalR());
});

function initializeTooltips() {
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })
}

//DRAW PAPERS TABLE
function loadEvaluationResults(tbody, apiUrl) {
    tbody.html(drawEvaluationResultsTableMessage('Loading data...'));
    $.ajax({
        url: apiUrl,
        type: 'GET',
        success: function (data) {
            drawEvaluationResultsTable(tbody, data);
        },
        error: function () {
            tbody.html(drawEvaluationResultsTableMessage('Error loading data.'));
        }
    });
}
function drawEvaluationResultsTableMessage(message) {
    return drawTableMessage(message, 3);
}
function drawEvaluationResultsTable(tbody, studentPapers) {
    tbody.html('');
    $.each(studentPapers, function (index, studentPaper) {
        var student = studentPaper.student;

        var paperNotSubmitedData = "";
        if (studentPaper.synthesisPaper == null)
            paperNotSubmitedData = ' data-not-submited="true"';

        var tableRow = `<tr data-student-id="${student.id}"${paperNotSubmitedData}>`;

        tableRow += drawStudentCell(student);
        tableRow += drawPublicEvaluationCell(studentPaper.synthesisPaper);
        tableRow += drawSecretEvaluationCell(studentPaper.synthesisPaper);

        tableRow += '</tr>'
        tbody.append(tableRow)
    });
}
function drawStudentCell(student) {
    return `<td>${student.firstName} ${student.lastName}</td>`;
};
function drawPublicEvaluationCell(paper) {
    var cellContent = evaluationProgressUI.drawEvaluationStatus(EvaluationStatus.NotSubmited);
    if (paper != null)
        cellContent = evaluationProgressUI.drawEvaluationStatus(paper.publicDataEvaluationStatus)
    return `<td class="public-data-progress">${cellContent}</td>`;
}
function drawSecretEvaluationCell(paper) {
    var cellContent = evaluationProgressUI.drawEvaluationStatus(EvaluationStatus.NotSubmited);
    if (paper != null)
        cellContent = evaluationProgressUI.drawEvaluationStatus(paper.secretDataEvaluationStatus)
    return `<td class="secret-data-progress">${cellContent}</td>`;
}

//SIGNAL R
function connectToSignalR() {
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/jobprogress")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("synthesisEvaluationUpdate",
        (data) => {
            evaluationProgressUI.setCellStatus(data.id, data.secret, data.status)
        });

    connection.start()
        .then(_ => connection.invoke("AssociateJob", `evaluate_synthesis_${testId}`))
        .catch(err => console.error(err.toString()));
}
function startProgress() {
    var tableRows = $('#evaluation-results-tbody tr');

    tableRows = $.grep(tableRows, function (v) {
        return $(v).data('not-submited') === undefined;
    });

    var studentIds = $.map(tableRows, function (val, i) {
        return $(val).data('student-id');
    });

    var data = {
        TestId: testId,
        StudentIds: studentIds
    }

    $.ajax({
        url: '/api/Evaluate/Start/Evaluation/Synthesis',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (data, textStatus, jQxhr) {
            disableButton($('#start-evaluation-btn'));
        }
    });
}

//UI
var evaluationProgressUI = {
    setCellStatus: function (studentId, secret, status) {
        var selector = `tr[data-student-id='${studentId}']`;
        if (secret)
            selector += ' td.secret-data-progress';
        else
            selector += ' td.public-data-progress';

        var tableRow = $(selector);

        if (tableRow.length == 1)
            tableRow.html(this.drawEvaluationStatus(status));
    },
    drawEvaluationStatus: function (status) {
        switch (status) {
            case EvaluationStatus.NotSubmited:
                return this.drawStatusIcon("Not submited", ColorCodes.Grey, ColorCodes.White, Shapes.X_Mark, false);
                break;
            case EvaluationStatus.NotEvaluated:
                return this.drawStatusIcon("Not evaluated", ColorCodes.White, ColorCodes.Grey, Shapes.Circle, true);
                break;
            case EvaluationStatus.Queued:
                return this.drawStatusIcon("Queued", ColorCodes.Blue, ColorCodes.White, Shapes.Clock, false);
                break;
            case EvaluationStatus.Evaluating:
                return this.drawSpinner("text-warning", "Evaluating...");
                break;
            case EvaluationStatus.Failed:
                return this.drawStatusIcon("Failed", ColorCodes.Red, ColorCodes.White, Shapes.X_Mark, false);
                break;
            case EvaluationStatus.Passed:
                return this.drawStatusIcon("Passed", ColorCodes.Green, ColorCodes.White, Shapes.Checked, false);
                break;
        }
    },
    drawStatusIcon: function (tooltip, circleColor, shapeColor, shape, evenOdd) {
        var result = `<svg height="24" viewBox="0 0 12 12" width="24" data-toggle="tooltip" data-placement="right" title="${tooltip}">`;
        result += `<circle cx="6" cy="6" r="6" fill="${circleColor}"></circle>`;
        result += '<path';
        if (evenOdd)
            result += ' fill-rule="evenodd"';
        result += ` fill="${shapeColor}" d="${shape}"></path>`;
        return result;
    },
    drawSpinner: function (textColor, tooltip) {
        return `<div class="spinner-border ${textColor}" role="status" style="width: 24px; height: 24px; font-size: 10px" data-toggle="tooltip" data-placement="right" title="${tooltip}" />`;
    }
}