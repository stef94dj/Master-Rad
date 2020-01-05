var testId;
var tbody;
var dataToEvaluate;
$(document).ready(function () {
    testId = $('#test-id').val();
    tbody = $('#evaluation-results-tbody');
    bindModalOnShow('#create-analysis-test-modal', onCreateAnalysisModalShow);

    tbody.html(drawEvaluationResultsTableMessage('Loading data...'));
    loadEvaluationResults(`/api/evaluate/get/papers/synthesis/${testId}`)
        .then(data => {
            drawEvaluationResultsTable(tbody, data);
        })
        .then(() => {
            initializeTooltips();
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
    return drawTableMessage(message, 4);
}
function drawEvaluationResultsTable(tbody, studentPapers) {
    tbody.html('');
    $.each(studentPapers, function (index, studentPaper) {
        var student = studentPaper.student;

        var paperData = "";
        if (studentPaper.synthesisPaper == null)
            paperData = 'data-not-submited="true"';
        else
            paperData = `data-paper-id="${studentPaper.synthesisPaper.id}"`

        var tableRow = `<tr data-student-id="${student.id}" ${paperData}>`;

        tableRow += drawStudentCell(student);
        tableRow += drawPublicEvaluationCell(studentPaper.synthesisPaper);
        tableRow += drawSecretEvaluationCell(studentPaper.synthesisPaper);
        tableRow += drawCreateAnalysisCell(studentPaper.synthesisPaper);

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
function drawCreateAnalysisCell(paper) {
    var enabled = paper != null && (paper.publicDataEvaluationStatus == EvaluationStatus.Failed || paper.secretDataEvaluationStatus == EvaluationStatus.Failed);
    var dynamic = `disabled`;
    if (enabled)
        dynamic = `data-paper-id="${paper.id}"`;

    return `<td class="secret-data-progress"><button type="button" data-toggle="modal" data-target="#create-analysis-test-modal" ${dynamic} class="btn btn-outline-primary" style="float:right">Create analysis test</button></td>`;
}

//API
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
function createAnalysisTest() {
    var modalBody = $('#create-analysis-test-modal').find('.modal-body');

    var rqBody = {
        "Name": modalBody.find('#analysis-test-name').val(),
        "SynthesisPaperId": modalBody.find('#synthesis-paper-id').val(),
    }

    debugger;
    $.ajax({
        url: '/api/Analysis/Create/Test',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            $('#create-analysis-test-modal').modal('toggle');
            window.location.replace('/TeacherMenu/AnalysisTests');
        }
    });
}

//UI
function setCellStatus(studentId, secret, status) {
    var selector = `tr[data-student-id='${studentId}']`;
    if (secret)
        selector += ' td.secret-data-progress';
    else
        selector += ' td.public-data-progress';

    var tableRow = $(selector);

    if (tableRow.length == 1)
        tableRow.html(evaluationProgressUI.drawEvaluationStatus(status));
}


function onCreateAnalysisModalShow(element, event) {
    var button = $(event.relatedTarget)

    var id = button.data('paper-id');

    var modal = $(element)

    modal.find('#analysis-test-name').val("");
    modal.find('#synthesis-paper-id').val(id);
}