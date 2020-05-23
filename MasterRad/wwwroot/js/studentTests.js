var testsList = null;
$(document).ready(function () {
    setActive("Tests");
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

        testItem += drawTestHeader(testReader.Status(testDto), testReader.TestStudent(testDto).takenTest);
        testItem += drawTestName(testReader.Name(testDto));
        testItem += drawTestContent(testReader.Status(testDto), testReader.StudentId(testDto), testReader.TestId(testDto), testReader.Type(testDto), testReader.TestStudent(testDto), testReader.TestStudentTimeStamp(testDto));

        testItem += '</div>';
        //testItem += drawTestFooter('footer text');
        testItem += '</div><br/>';

        testsList.append(testItem);
    });
    initializeTooltips();
}

function drawTestHeader(testStatus, takenTest) {
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
            if (takenTest)
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
function drawTestContent(status, studentId, testId, testType, testStudent, testStudentTimeStamp) {
    switch (status) {
        case TestStatus.Scheduled:
            return '<a href="javascript:void(0)" class="btn btn-dark disabled">' + 'Take test' + '</a>';
        case TestStatus.InProgress:
            return `<a href="javascript:void(0)" class="btn btn-dark" onclick="startTest(${testId},${testType},${testStudentTimeStamp})">Take test</a>`;
        case TestStatus.Completed:
            if (testStudent.takenTest) {
                switch (testType) {
                    case TestType.Synthesis:
                        return testStudent == null ? '' :
                            `<div>Public data: ${evaluationProgressUI.drawEvaluationStatus(progressReader.PublicData(testStudent))}</div>
                             <div>Secret data: ${evaluationProgressUI.drawEvaluationStatus(progressReader.SecretData(testStudent))}</div>`;
                    case TestType.Analysis:
                        return testStudent == null ? '' :
                            `<div>Failing Input: ${evaluationProgressUI.drawEvaluationStatus(progressReader.FailingInput(testStudent))}</div>
                             <div>Query Output: ${evaluationProgressUI.drawEvaluationStatus(progressReader.QueryOutput(testStudent))}</div>
                             <div>Correct Output: ${evaluationProgressUI.drawEvaluationStatus(progressReader.CorrectOutput(testStudent))}</div>`
                }
            }
            else
                return "";

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
    TestStudent: function (testDto) {
        return testDto.testStudent;
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
    TestStudentTimeStamp: function (testDto) {
        return `'${testDto.testStudent.timeStamp}'`;
    }
}

function startTest(testId, testType, testStudentTimeStamp) {
    var form = $('#hidden-form');
    form.find('#test-id').val(testId);
    form.find('#test-student-timestamp').val(testStudentTimeStamp);

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
