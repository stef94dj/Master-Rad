var testId = null;
var tbody = null;
var dataToEvaluate = null;
var testType = null;

$(document).ready(function () {
    testId = $('#test-id').val();
    tbody = $('#evaluation-results-tbody');
    testType = parseInt($('#test-type').val());

    bindModalOnShow('#create-analysis-test-modal', onCreateAnalysisModalShow);

    tbody.html(drawEvaluationResultsTableMessage('Loading data...'));

    var apiUrl = getApiUrlToGetPapers(testId);
    promisifyAjaxGet(apiUrl)
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
                var connectionObj = getSignalRConnectionObj();
                signalR_Helper.connect(connectionObj);
            }
        }).then(() => {
            if (dataToEvaluate != null)
                enableButton($('#start-evaluation-btn'));
        });
});

function getApiUrlToGetPapers(testId) {
    switch (testType) {
        case TestType.Synthesis:
            return `/api/evaluate/get/papers/synthesis/${testId}`;
        case TestType.Analysis:
            return `/api/evaluate/get/papers/analysis/${testId}`;
    }
    console.log(`test type '${testType}' not supported`);
    return null;
}
function getSignalRConnectionObj() {
    switch (testType) {
        case TestType.Synthesis:
            return getSignalRConnectionObjSynthesis();
        case TestType.Analysis:
            return getSignalRConnectionObjAnalysis();
    }
    console.log(`test type '${testType}' not supported`);
    return null;
}
function getSignalRConnectionObjSynthesis() {
    var connectionObj = {
        Group: `evaluate_synthesis_${testId}`,
        Method: "synthesisEvaluationUpdate",
        HubUrl: "/synthesisprogress",
        HubMethod: "AssociateJob"
    };
    return connectionObj;
}
function getSignalRConnectionObjAnalysis() {
    var connectionObj = {
        Group: `evaluate_analysis_${testId}`,
        Method: "analysisEvaluationUpdate",
        HubUrl: "/analysisprogress",
        HubMethod: "AssociateJob"
    };
    return connectionObj;
}

//DRAW PAPERS TABLE
function drawEvaluationResultsTableMessage(message, testType) {
    switch (testType) {
        case TestType.Synthesis:
            return drawTableMessage(message, 4);
        case TestType.Analysis:
            return drawTableMessage(message, 6);
    }
    console.log(`test type '${testType}' not supported`);
    return null;
}

function drawEvaluationResultsTable(tbody, testStudents) {
    switch (testType) {
        case TestType.Synthesis:
            drawTableSynthesis(tbody, testStudents);
            break;
        case TestType.Analysis:
            drawTableAnalysis(tbody, testStudents);
            break;
        default: console.log(`test type '${testType}' not supported`);
    }
}

function drawTableSynthesis(tbody, testStudents) {
    tbody.html('');
    $.each(testStudents, function (index, testStudent) {
        var student = testStudent.studentDetail;

        var takenTest = testStudent.takenTest;
        var notSubmitedFlag = takenTest ? '' : 'data-not-submited="true"';
        var tableRow = `<tr data-student-id="${student.microsoftId}" ${notSubmitedFlag}>`;

        tableRow += drawStudentCell(student);

        var progressPublic = testStudent.publicDataProgress;

        var progressSecret = testStudent.secretDataProgress;

        tableRow += drawEvaluationProgressCell(takenTest, takenTest ? progressPublic : null, "public-data-progress");
        tableRow += drawEvaluationProgressCell(takenTest, takenTest ? progressSecret : null, "secret-data-progress");
        tableRow += drawCreateAnalysisCell(testStudent, progressPublic, progressSecret);

        tableRow += '</tr>'
        tbody.append(tableRow)
    });
}
function drawStudentCell(student) {
    return `<td>${student.firstName} ${student.lastName} <br/> ${student.email}</td>`;
};
function drawEvaluationProgressCell(takenTest, progress, cssClass) {
    var cellContent = evaluationProgressUI.drawEvaluationStatus(EvaluationStatus.NotSubmited);
    if (takenTest)
        cellContent = evaluationProgressUI.drawEvaluationStatus(progress)
    return `<td class="${cssClass}">${cellContent}</td>`;
}
function drawCreateAnalysisCell(testStudent, publicDataStatus, secretDataStatus) {
    var enabled = testStudent != null && (publicDataStatus == EvaluationStatus.Failed || secretDataStatus == EvaluationStatus.Failed);
    var dynamic = `disabled`;
    if (enabled)
        dynamic = `data-student-id="${testStudent.studentId}"`;

    return `<td><button type="button" data-toggle="modal" data-target="#create-analysis-test-modal" ${dynamic} class="btn btn-outline-primary">Create analysis test</button></td>`;
}

function drawTableAnalysis(tbody, analysisTestStudents) {
    tbody.html('');
    $.each(analysisTestStudents, function (index, ats) {
        var notSubmitedFlag = ats.takenTest ? '' : 'data-not-submited="true"';
        var tableRow = `<tr data-student-id="${ats.studentId}" ${notSubmitedFlag}>`;

        tableRow += drawStudentCell(ats.student);
        tableRow += drawEvaluationProgressCell(ats.takenTest, progressReader.PrepareData(ats), "prepare-data-progress");
        tableRow += drawEvaluationProgressCell(ats.takenTest, progressReader.FailingInput(ats), "failing-input-progress");
        tableRow += drawEvaluationProgressCell(ats.takenTest, progressReader.QueryOutput(ats), "student-output-progress");
        tableRow += drawEvaluationProgressCell(ats.takenTest, progressReader.CorrectOutput(ats), "teacher-output-progress");

        tableRow += '</tr>'
        tbody.append(tableRow)
    });
}

//API
function startProgress() {
    if (dataToEvaluate == null) {
        alert('nothing to evaluate');
        return;
    }

    var data = {
        TestId: testId,
    }

    var finalEndpoint;
    switch (testType) {
        case TestType.Synthesis:
            finalEndpoint = "Synthesis";
            data.EvaluationRequests = dataToEvaluate;
            break;
        case TestType.Analysis:
            finalEndpoint = "Analysis";
            data.StudentIds = dataToEvaluate;
            break;
        default: console.log(`test type '${testType}' not supported`);
    }

    $.ajax({
        url: `/api/Evaluate/Start/Evaluation/${finalEndpoint}`,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function () {
            disableButton($('#start-evaluation-btn'));
        }
    });
}

function getEvaluationData() {
    switch (testType) {
        case TestType.Synthesis:
            return getEvaluationDataSynthesis();
        case TestType.Analysis:
            return getEvaluationDataAnalysis();
    }
    return null;
}

function getEvaluationDataSynthesis() {
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

function getEvaluationDataAnalysis() {
    var tableRows = $('#evaluation-results-tbody tr');
    tableRows = $.grep(tableRows, function (v) {
        return $(v).data('not-submited') === undefined;
    });

    divs = $(tableRows).find('td.prepare-data-progress').find('div.test-cell');
    divs = $.grep(divs, function (v) {
        return $(v).data('status') === EvaluationStatus.NotEvaluated;
    });

    if (divs.length < 1) {
        return null;
    }

    var requests = $.map(divs, function (val, i) {
        return $(val).parent().parent().data('student-id');
    });

    return requests;
}

function createAnalysisTest() {
    var modalBody = $('#create-analysis-test-modal').find('.modal-body');

    var rqBody = {
        "Name": modalBody.find('#analysis-test-name').val(),
        "StudentId": modalBody.find('#student-id').val(),
        "SynthesisTestId": testId,
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
function setCellStatus(data) {
    switch (testType) {
        case TestType.Synthesis:
            return setCellStatusSynthesis(data);
        case TestType.Analysis:
            return setCellStatusAnalysis(data);
    }
    return null;
}
function setCellStatusSynthesis(data) {
    debugger;
    var selector = `tr[data-student-id='${data.id}']`;
    if (data.secret)
        selector += ' td.secret-data-progress';
    else
        selector += ' td.public-data-progress';

    var tableRow = $(selector);

    if (tableRow.length == 1)
        tableRow.html(evaluationProgressUI.drawEvaluationStatus(data.status));
}

function setCellStatusAnalysis(data) {
    var selector = `tr[data-student-id='${data.id}']`;

    switch (data.type) {
        case AnalysisEvaluationType.PrepareData:
            selector += ' td.prepare-data-progress';
            break;
        case AnalysisEvaluationType.FailingInput:
            selector += ' td.failing-input-progress';
            break;
        case AnalysisEvaluationType.CorrectOutput:
            selector += ' td.teacher-output-progress';
            break;
        case AnalysisEvaluationType.QueryOutput:
            selector += ' td.student-output-progress';
            break;
        default:
    }

    var tableRow = $(selector);

    if (tableRow.length == 1)
        tableRow.html(evaluationProgressUI.drawEvaluationStatus(data.status));
}

function onCreateAnalysisModalShow(element, event) {
    var button = $(event.relatedTarget)

    var id = button.data('student-id');

    var modal = $(element)

    modal.find('#analysis-test-name').val("");
    modal.find('#student-id').val(id);
}