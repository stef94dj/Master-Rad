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
        var testEntity = synthesisTestStudent.synthesisTest;

        //header
        switch (testEntity.status) {
            case TestStatus.Scheduled:
                testItem += '<div class="card-header font-weight-bold bg-light text-primary">' + 'Scheduled' + '</div>';
                break;
            case TestStatus.InProgress:
                testItem += '<div class="card-header font-weight-bold text-white bg-primary">' + 'In progress' + '</div>';
                break;
            case TestStatus.Completed:
            case TestStatus.Graded:
                if (stsEntity.synthesisPaper == null)
                    testItem += '<div class="card-header font-weight-bold text-danger">' + 'Missed' + '</div>';
                else if (testEntity.status == TestStatus.Completed)
                    testItem += '<div class="card-header font-weight-bold bg-dark text-light">' + 'Pending review' + '</div>';
                else if (testEntity.status == TestStatus.Graded)
                    testItem += '<div class="card-header font-weight-bold bg-dark text-light">' + 'Completed' + '</div>';
                break;
        }

        //body
        testItem += '<div class="card-body bg-light text-dark">';
        testItem += '<h5 class="card-title">' + testEntity.name + '</h5>';
        testItem += '<p class="card-text">' + 'Test description text' + '</p>';

        switch (testEntity.status) {
            case TestStatus.Scheduled:
                testItem += '<a href="javascript:;" class="btn btn-dark disabled">' + 'Take test' + '</a>';
                break;
            case TestStatus.InProgress:
                testItem += '<a href="javascript:;" class="btn btn-dark" onclick="startTest(' + stsEntity.studentId + ',' + stsEntity.synthesisTestId +')">' + 'Take test' + '</a>';
                break;
            case TestStatus.Completed:
            case TestStatus.Graded:
                if (testEntity.synthesisPaper == null)
                    break
                else if (test.status == TestStatus.Completed)
                    testItem += '<a href="#" class="btn btn-primary disabled">' + 'vidi rez ili da pise da nije polagao' + '</a>';
                else if (test.status == TestStatus.Graded)
                    testItem += '<a href="#" class="btn btn-primary">' + 'Results' + '</a>';
                break;
        }
        testItem += '</div>';

        //footer
        //testItem += '<div class="card-footer text-muted bg-dark">' + '2 days ago' + '</div>';

        testItem += '</div><br/>';
        testsList.append(testItem);
    });
}

function startTest(studentId, synthesisTestId) {
    var form = $('#hidden-form');
    form.find('#test-id').val(synthesisTestId);
    form.attr('action', '/Test/SynthesisExam');
    form.submit();
}
