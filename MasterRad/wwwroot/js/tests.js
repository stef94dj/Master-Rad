var testsList = null;
$(document).ready(function () {
    testsList = $('#tests-list');
    loadTest();
});

function loadTest() {
    testsList.html('<p>Loading data...<p>');

    var apiUrl = '/api/Test/get/assigned';
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
        if (testEntity.isActive)
            testItem += '<div class="card-header font-weight-bold text-white bg-success">' + 'In progress' + '</div>';
        else
            testItem += '<div class="card-header font-weight-bold">' + 'Scheduled' + '</div>';

        //body
        testItem += '<div class="card-body">';
        testItem += '<h5 class="card-title">' + testEntity.name + '</h5>';
        testItem += '<p class="card-text">' + 'With supporting text below as a natural lead-in to additional content.' + '</p>';

        if (testEntity.isActive)
            testItem += '<a href="#" class="btn btn-primary" onclick="startTest(' + stsEntity.studentId + ',' + stsEntity.synthesisTestId + ')">' + 'Start test' + '</a>';
        else
            testItem += '<a href="#" class="btn btn-primary disabled">' + 'Start test' + '</a>';
        testItem += '</div>';

        //footer
        testItem += '<div class="card-footer text-muted">' + '2 days ago' + '</div>';

        testItem += '</div><br/>';
        testsList.append(testItem);
    });

    //testsList.html(JSON.stringify(data));
}
