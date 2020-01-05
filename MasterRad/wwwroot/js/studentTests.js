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
    $.each(data, function (index, testDto) {
        var testItem = '<div class="card text-center">';

        testItem += drawTestHeader(testReader.Status(testDto), testReader.Paper(testDto) != null);
        testItem += drawTestName(testReader.Name(testDto));
        testItem += drawTestContent(testReader.Status(testDto), testReader.StudentId(testDto), testReader.TestId(testDto), testReader.Type(testDto), testReader.Paper(testDto));

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
                case TestType.Analysis:
                    return paper == null ? '' :
                        'not implemented';
            }
    }
}
function drawTestFooter(text) {
    return `<div class="card-footer text-muted bg-dark">${text}</div>`
}

var testReader = {
    Type: function (testDto) {
        return testDto.testType;
    },
    Status: function (testDto) {
        switch (testDto.testType) {
            case TestType.Synthesis:
                return testDto.testStudent.synthesisTest.status;
            case TestType.Analysis:
                return testDto.testStudent.analysisTest.status;
            default: return null;
        }
    },
    Name: function (testDto) {
        switch (testDto.testType) {
            case TestType.Synthesis:
                return testDto.testStudent.synthesisTest.name;
            case TestType.Analysis:
                return testDto.testStudent.analysisTest.name;
            default: return null;
        }
    },
    StudentId: function (testDto) {
        return testDto.testStudent.studentId;
    },
    Paper: function (testDto) {
        switch (testDto.testType) {
            case TestType.Synthesis:
                return testDto.testStudent.synthesisPaper;
            case TestType.Analysis:
                return testDto.testStudent.analysisPaper;
            default: return null;
        }
    },
    TestId: function (testDto) {
        switch (testDto.testType) {
            case TestType.Synthesis:
                return testDto.testStudent.synthesisTestId;
            case TestType.Analysis:
                return testDto.testStudent.analysisTestId;
            default: return null;
        }
    },
}

function startTest(studentId, synthesisTestId, testType) {
    var form = $('#hidden-form');
    form.find('#test-id').val(synthesisTestId);

    switch (testType) {
        case TestType.Synthesis:
            form.attr('action', '/Test/SynthesisExam');
            break;
        case TestType.Analysis:
            form.attr('action', '/Test/AnalysisExam');
            break;
    }

    form.submit();
}
