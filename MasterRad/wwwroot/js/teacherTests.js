var testsList = null;
$(document).ready(function () {
    testsList = $('#tasks-tbody');
    loadTest();
});

function loadTest() {
    testsList.html('<p>Loading data...<p>');

    var apiUrl = '/api/Synthesis/get';
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
    $.each(data, function (index, test) {
        var task = test.task;
        var template = task.template;

        var testItem = '<tr>';
        testItem += drawPathCell(template.name, task.name);
        testItem += drawNameCell(test);
        testItem += drawCreatedAtCell(test);
        testItem += drawCreatedByCell(test);
        testItem += drawStateCell(test);
        testItem += drawDeleteCell(test);
        testItem += '</tr>'


        testsList.append(testItem);
    });
}

function drawPathCell(templateName, taskName) {
    return '<td>' + templateName + ' -> ' + taskName + '</td>';
}

function drawNameCell(test) {
    var result = '<td><div>';
    result += '<p style="float:left">' + test.name + '</p>';
    result += drawCellEditModalButton('Edit', 'dark', '#update-name-modal', test.id, test.timeStamp, true);
    result += '</div></td>';
    return result;
}

function drawCreatedAtCell(test) {
    return '<td>' + test.dateCreated + '</td>';
}

function drawCreatedByCell(test) {
    return '<td>' + test.createdBy + '</td>';
}

function drawStateCell(test) {
    var result = '<td><div>';
    result += '<p style="float:left">' + TestStatus.ToString(test.status) + '</p>';
    if (test.status < TestStatus.Graded) {
        result += drawChangeStatusModalButton('Edit', 'dark', '#update-staus-modal', test, true);
    }
    result += '</div></td>';
    return result;
}

function drawChangeStatusModalButton(buttonName, color, modalselector, test, enabled) {
    var newStatus = test.status + 1;
    var result = '<button ';
    if (!enabled)
        result += 'disabled ';
    result += 'data-toggle="modal" data-target="' + modalselector + '" data-id="' + test.id + '" data-timestamp="' + test.timestamp + '" data-news-tatus-code="' + newStatus + '" type="button" style="float:right" class="btn btn-outline-' + color + ' btn-sm">' + TestStatus.ActionName(test.status) + '</button>'
    return result;
}

function drawDeleteCell(test) {
    return '<td>' + drawCellEditModalButton('Delete', 'danger', 'deleteTest', test.id, test.timeStamp, true) + '</td>';
}

function updateName() {
    var modalBody = $('#update-name-modal').find('.modal-body');

    var rqBody = {
        "Id": parseInt(modalBody.find('#test-id').val()),
        "TimeStamp": modalBody.find('#test-timestamp').val(),
        "Name": modalBody.find('#test-name').val()
    }

    $.ajax({
        url: '/api/Synthesis/update/name',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            $('#update-name-modal').modal('toggle');
            loadTemplates($('#templates-tbody'), '/api/Template/Get');
        }
    });
}