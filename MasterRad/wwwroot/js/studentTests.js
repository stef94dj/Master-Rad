var scheduledTests = null;
var inProgressTests = null;
var completedTests = null;
$(document).ready(function () {
    setActive("Tests");
    scheduledTests = $('#scheduled-tests-list');
    inProgressTests = $('#inprogress-tests-list');
    completedTests = $('#completed-tests-list');
    loadTest();
});

function loadTest() {
    scheduledTests.html('<p>Loading data...<p>');

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
    scheduledTests.html('');
    $.each(data, function (index, testDto) {
        var testItem = "";
        var testStatus = testReader.Status(testDto);
        switch (testStatus) {
            case TestStatus.Scheduled:
                testItem = drawScheduled(testDto);
                scheduledTests.append(testItem);
                break;
            case TestStatus.InProgress:
                testItem = drawInProgress(testDto);
                inProgressTests.append(testItem);
                break;
            case TestStatus.Completed:
                testItem = drawCompleted(testDto)
                completedTests.append(testItem);
        }
    });
    initializeTooltips();
}

function drawScheduled(testDto) {
    return `<div class="card bg-light mb-3" style="min-width: 15rem; max-width: 15rem; margin:5px;">
                    <div class="card-header" style="text-align:center;">22/05/2020 17:00</div>
                    <div class="card-body">
                        <h5 class="card-title" style="text-align:center;">${testReader.Name(testDto)}</h5>
                        <p style="text-align:center;">${testReader.TypeText(testDto)}</p>
                        <div class="text-center">
                            <button type="button" class="btn btn-outline-secondary" disabled>Take test</button>
                        </div>
                    </div>
                </div>`;
}
function drawInProgress(testDto) {
    return `<div class="card bg-light mb-3" style="min-width: 15rem; max-width: 15rem; margin:5px;">
                    <div class="card-header" style="text-align:center;">22/05/2020 17:00</div>
                    <div class="card-body">
                        <h5 class="card-title" style="text-align:center;">${testReader.Name(testDto)}</h5>
                        <p style="text-align:center;">${testReader.TypeText(testDto)}</p>
                        <div class="text-center">
                            <button type="button" class="btn btn-outline-primary" onclick="startTest(${testReader.TestId(testDto)},${testReader.Type(testDto)},${testReader.TestStudentTimeStamp(testDto)})">
                                Take test
                            </button>
                        </div>
                    </div>
                </div>`;
}
function drawCompleted(testDto) {
    var size = testReader.CardSize(testDto);
    var takenTest = testReader.TestStudent(testDto).takenTest;
    var testType = testReader.Type(testDto);

    var headerText = "";
    var headerStyle = "";
    if (takenTest) {
        headerText = "Completed";
        headerStyle = "color:green;";
    }
    else {
        headerText = "Missed";
        headerStyle = "color:red;";
    }

    
    var testStudent = testReader.TestStudent(testDto);
    var thead = "";
    var tbody = "";
    if (testStudent) {
        switch (testType) {
            case TestType.Synthesis:
                var thead = "<tr><th>Public</th><th>Secret</th></tr>";
                var tbody = `<tr>
                                <td>${evaluationProgressUI.drawEvaluationStatus(progressReader.PublicData(testStudent))}</td>
                                <td>${evaluationProgressUI.drawEvaluationStatus(progressReader.SecretData(testStudent))}</td>
                             <tr/>`;
                break;
            case TestType.Analysis:
                var thead = "<tr><th>Input &nbsp;</th><th>Query</th><th>Student</th></tr>";
                var tbody = `<tr>
                                <td>${evaluationProgressUI.drawEvaluationStatus(progressReader.FailingInput(testStudent))}</td>
                                <td>${evaluationProgressUI.drawEvaluationStatus(progressReader.QueryOutput(testStudent))}</td>
                                <td>${evaluationProgressUI.drawEvaluationStatus(progressReader.CorrectOutput(testStudent))}</td>
                             <tr/>`;
                break;
        }
    }


    var testResults = `<table class="table table-bordered table-sm">
                            <thead>${thead}</thead> 
                            <tbody>${tbody}</tbody>
                       </table>`;
    
    return `<div class="card bg-light mb-3" style="min-width: ${size}rem; max-width: ${size}rem; margin:5px;">
                    <div class="card-header" style="font-weight:600; text-align:center; ${headerStyle}">${headerText}</div>
                    <div class="card-body">
                        <h5 class="card-title" style="text-align:center;">${testReader.Name(testDto)}</h5>
                        <br/>
                        <div class="float-left-icon">${testResults}<div/>
                    </div>
                </div>`;
}

var testReader = {
    Type: function (testDto) {
        return testDto.testType;
    },
    TypeText: function (testDto) {
        switch (testDto.testType) {
            case TestType.Synthesis:
                return 'Synthesis';
                break;
            case TestType.Analysis:
                return 'Analysis';
                break;
            default: return '';
        };
    },
    CardSize: function (testDto) {
        switch (testDto.testType) {
            case TestType.Synthesis:
                return 16;
                break;
            case TestType.Analysis:
                return 20;
                break;
            default: return 20;
        };
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
