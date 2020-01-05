var testsList = null;
$(document).ready(function () {
    testsList = $('#tests-list');
    loadTest();
});

function loadTest() {
    testsList.html('<p>Loading data...<p>');

    var apiUrl = '/api/Test/get/student/assigned';
    $.ajax({
        url: apiUrl,
        type: 'GET',
        success: function (data) {
            drawTestsList(data);
        },
        error: function () {
            tbody.html(drawTableMessage('Error loading data.'));
        }
    });
}

function drawTestsList(data) {
    testsList.html('');
    $.each(data, function (index, synthesisTestStudent) {
        var testItem = '<div class="card text-center">';

        var stsEntity = synthesisTestStudent;
        var testEntity = stsEntity.synthesisTest;
        var paperEntity = stsEntity.synthesisPaper;

        testItem += drawTestHeader(testEntity.status, paperEntity != null);

        testItem += drawTestName(testEntity.name);

        testItem += drawTestContent(testEntity.status, stsEntity.studentId, stsEntity.synthesisTestId, TestType.Synthesis, paperEntity);

        testItem += '</div>';
        //testItem += drawTestFooter('footer text');
        testItem += '</div><br/>';

        testsList.append(testItem);
    });
    initializeTooltips();
}
function drawTestHeader(testStatus, hasPaper) {
    var color = '';
    var text = TestStatus.ToString(testStatus);
    switch (testStatus) {
        case TestStatus.Scheduled:
            color = 'bg-light text-primary';
            break;
        case TestStatus.InProgress:
            color = 'text-white bg-primary';
            break;
        case TestStatus.Completed:
            if (hasPaper)
                color = 'bg-light text-dark';
            else {
                color = 'font-weight-bold text-danger';
                text = 'Missed';
            }

    }
    return `<div class="card-header font-weight-bold ${color}">${text}</div>`;
}

function drawTestName(testName, testDescription) {
    var res = `<div class="card-body bg-light text-dark"><h5 class="card-title">${testName}</h5>`;
    if (arguments.length == 2 && testDescription != null && testDescription != '')
        res += '<p class="card-text">' + 'Test description text' + '</p>';
    return res;
}

function drawTestContent(status, studentId, testId, testType, paper) {
    switch (status) {
        case TestStatus.Scheduled:
            return '<a href="javascript:;" class="btn btn-dark disabled">' + 'Take test' + '</a>';
        case TestStatus.InProgress:
            return `<a href="javascript:;" class="btn btn-dark" onclick="startTest(${studentId},${testId},${testType})">Take test</a>`;
        case TestStatus.Completed:
            switch (testType) {
                case TestType.Synthesis:
                    return paper == null ? '' :
                        `<div>Public data: ${evaluationProgressUI.drawEvaluationStatus(paper.publicDataEvaluationStatus)}</div>
                         <div>Secret data: ${evaluationProgressUI.drawEvaluationStatus(paper.secretDataEvaluationStatus)}</div>`;
                case testType.Analysis:
                    return 'not implemented';
            }
    }
}

function drawTestFooter(text) {
    return `<div class="card-footer text-muted bg-dark">${text}</div>`
}

function startTest(studentId, synthesisTestId, testType) {
    var form = $('#hidden-form');
    form.find('#test-id').val(synthesisTestId);

    switch (testType) {
        case TestType.Synthesis:
            form.attr('action', '/Test/SynthesisExam');
            break;
        case testType.Analysis:
            form.attr('action', 'not implemented');
            break;
    }

    form.submit();
}
