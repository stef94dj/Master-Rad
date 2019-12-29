var testId;
var tbody;
var dataToEvaluate;
$(document).ready(function () {
    initializeTooltips();
    testId = $('#test-id').val();
    tbody = $('#evaluation-results-tbody');

    tbody.html(drawEvaluationResultsTableMessage('Loading data...'));
    loadEvaluationResults(`/api/evaluate/get/papers/synthesis/${testId}`)
        .then(data => {
            drawEvaluationResultsTable(tbody, data);
        })
        .then(() => {
            dataToEvaluate = getEvaluationData();
        })
        .catch(error => {
            tbody.html(drawEvaluationResultsTableMessage('Error loading data.'));
            reject(error);
        })
        .then(() => {
            if (dataToEvaluate != null) {
                var connectionObj = {
                    Group: `evaluate_synthesis_${testId}`,
                    Method: "synthesisEvaluationUpdate",
                    HubUrl: "/jobprogress",
                    HubMethod: "AssociateJob"
                };
                signalR_Helper.connect(connectionObj);
            }
        }).then(() => {
            if (dataToEvaluate != null)
                enableButton($('#start-evaluation-btn'));
        });
});

function initializeTooltips() {
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })
}

//DRAW PAPERS TABLE
function loadEvaluationResults(apiUrl) {

    return new Promise((resolve, reject) => {
        $.ajax({
            url: apiUrl,
            type: 'GET',
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error)
            }
        });
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
//function connectToSignalR() {
//    var connection = new signalR.HubConnectionBuilder()
//        .withUrl("/jobprogress")
//        .withAutomaticReconnect()
//        .configureLogging(signalR.LogLevel.Trace)
//        .build();

//    connection.on("synthesisEvaluationUpdate",
//        (data) => {
//            evaluationProgressUI.setCellStatus(data.id, data.secret, data.status)
//        });

//    connection.start()
//        .then(_ => connection.invoke("AssociateJob", `evaluate_synthesis_${testId}`))
//        .catch(err => console.error(err.toString()));

//    connection.onreconnecting(() => {
//        alert('signalR attempting to recconect')
//    })

//    connection.onreconnected(() => {
//        alert('signalR succesfully recconcected')
//    })

//    connection.onclose(() => {
//        alert('signalR connection closed')
//    })
//}
function startProgress() {
    if (dataToEvaluate == null) {
        alert('nothing to evaluate');
        return;
    }

    var data = {
        TestId: testId,
        EvaluationRequests: dataToEvaluate
    }

    $.ajax({
        url: '/api/Evaluate/Start/Evaluation/Synthesis',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function () {
            disableButton($('#start-evaluation-btn'));
        }
    });
}

function getEvaluationData() {
    var tableRows = $('#evaluation-results-tbody tr');
    tableRows = $.grep(tableRows, function (v) {
        return $(v).data('not-submited') === undefined;
    });

    divs = $(tableRows).find('div.test-cell');
    divs = $.grep(divs, function (v) {
        return $(v).data('status') === EvaluationStatus.NotEvaluated;
    });

    if (divs.length < 1) {
        return null;
    }

    var requests = $.map(divs, function (val, i) {
        var res = {
            StudentId: $(val).parent().parent().data('student-id'),
            UseSecretData: $(val).parent().hasClass('secret-data-progress')
        };
        return res;
    });

    return requests;
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
                return this.drawStatusIcon("Not submited", ColorCodes.Grey, ColorCodes.White, Shapes.X_Mark, false, status);
                break;
            case EvaluationStatus.NotEvaluated:
                return this.drawStatusIcon("Not evaluated", ColorCodes.White, ColorCodes.Grey, Shapes.Circle, true, status);
                break;
            case EvaluationStatus.Queued:
                return this.drawStatusIcon("Queued", ColorCodes.Blue, ColorCodes.White, Shapes.Clock, false, status);
                break;
            case EvaluationStatus.Evaluating:
                return this.drawSpinner("text-warning", "Evaluating...", status);
                break;
            case EvaluationStatus.Failed:
                return this.drawStatusIcon("Failed", ColorCodes.Red, ColorCodes.White, Shapes.X_Mark, false, status);
                break;
            case EvaluationStatus.Passed:
                return this.drawStatusIcon("Passed", ColorCodes.Green, ColorCodes.White, Shapes.Checked, false, status);
                break;
        }
    },
    drawStatusIcon: function (tooltip, circleColor, shapeColor, shape, evenOdd, status) {
        var result = `<div data-status="${status}" class="test-cell">`;
        result += `<svg height="24" viewBox="0 0 12 12" width="24" data-toggle="tooltip" data-placement="right" title="${tooltip}">`;
        result += `<circle cx="6" cy="6" r="6" fill="${circleColor}"></circle>`;
        result += '<path';
        if (evenOdd)
            result += ' fill-rule="evenodd"';
        result += ` fill="${shapeColor}" d="${shape}"></path>`;
        result += '</div>';
        return result;
    },
    drawSpinner: function (textColor, tooltip, status) {
        return `<div data-status=${status} class="test-cell spinner-border ${textColor}" role="status" style="width: 24px; height: 24px; font-size: 10px" data-toggle="tooltip" data-placement="right" title="${tooltip}" />`;
    }
}