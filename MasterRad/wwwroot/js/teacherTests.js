var testsList = null;
var statusModalSelector = null;
var nameModalSelector = null;
$(document).ready(function () {
    testsList = $('#tests-tbody');
    loadTest();

    statusModalSelector = '#update-staus-modal';
    nameModalSelector = '#update-name-modal';
    bindModalOnShow(nameModalSelector, onNameModalShow);
});

//DRAW TABLE
function loadTest() {
    testsList.html(drawSynthesisTestsTableMessage('Loading data...'));

    var apiUrl = '/api/Synthesis/get';
    $.ajax({
        url: apiUrl,
        type: 'GET',
        success: function (data) {
            drawTestsList(data);
        },
        error: function () {
            testsList.html(drawSynthesisTestsTableMessage('Error loading data.'));
        }
    });
}
function drawSynthesisTestsTableMessage(message) {
    return drawTableMessage(message, 6);
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
        result += drawChangeStatusModalButton('dark', test, true);
    }
    result += '</div></td>';
    return result;
}
function drawChangeStatusModalButton(color, test, enabled) {
    var newStatus = test.status + 1;
    var result = '<button ';
    if (!enabled)
        result += 'disabled ';
    debugger;
    if (test.status == TestStatus.Graded) 
        result += ' onclick="viewResults(' + test.id + ')" type="button" style="float:right" class="btn btn-outline-' + color + ' btn-sm">Results</button>';
    else
        result += 'data-toggle="modal" onclick="openStatusModal(this,' + newStatus + ')" data-id="' + test.id + '" data-timestamp="' + test.timeStamp + '" data-news-tatus-code="' + newStatus + '" type="button" style="float:right" class="btn btn-outline-' + color + ' btn-sm">' + TestStatus.ActionName(test.status) + '</button>'
    return result;
}
function drawDeleteCell(test) {
    return '<td>' + drawCellEditModalButton('Delete', 'danger', 'deleteTest', test.id, test.timeStamp, true) + '</td>';
}

//UPDATE NAME MODAL
function onNameModalShow(element, event) {
    var button = $(event.relatedTarget)

    var name = button.parent().find('p').html();
    var id = button.data('id');
    var timestamp = button.data('timestamp');

    var modal = $(element)

    modal.find('#test-name').val(name);
    modal.find('#test-id').val(id);
    modal.find('#test-timestamp').val(timestamp);
}
function updateName() {
    var modalBody = $(nameModalSelector).find('.modal-body');

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
            $(nameModalSelector).modal('toggle');
            loadTest();
        }
    });
}

//UPDATE STATUS MODAL
function openStatusModal(btn, nextStatus) {
    var modal = $(statusModalSelector);

    var id = $(btn).data('id');
    var timestamp = $(btn).data('timestamp');

    modal.find('#test-id').val(id);
    modal.find('#test-timestamp').val(timestamp);

    modal.find('#change-status-message').html(TestStatus.ActionWarningText(nextStatus));

    modal.modal('show');
    return false;
}

function statusNext() {
    var modalBody = $(statusModalSelector).find('.modal-body');

    var rqBody = {
        "Id": parseInt(modalBody.find('#test-id').val()),
        "TimeStamp": modalBody.find('#test-timestamp').val()
    }

    $.ajax({
        url: '/api/Synthesis/status/next',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            $(statusModalSelector).modal('toggle');
            loadTest();
        }
    });
}