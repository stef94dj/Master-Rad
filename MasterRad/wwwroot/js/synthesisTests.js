var testsList = null;

var createTestModalSelector = null;
var nameModalSelector = null;
var statusModalSelector = null;


$(document).ready(function () {
    testsList = $('#tests-tbody');

    createTestModalSelector = '#create-test-modal';
    nameModalSelector = '#update-name-modal';
    statusModalSelector = '#update-staus-modal';
    
    loadTests();
    loadTasks();

    bindModalOnShow(createTestModalSelector, onCreateSynthesisTestModalShow);
    bindModalOnShow(nameModalSelector, onNameModalShow);
});

//LOAD TESTS
function loadTests() {
    drawSynthesisTestsTableMessage('Loading data...');
    getSynthesisTests()
        .then(data => {
            drawTestsList(data);
        })
        .catch(error => {
            drawSynthesisTestsTableMessage('Error loading data...');
        })
        .then(data => {
            defineNameHoverBehaviour($('td.hover-text-button'));
        });
}
function getSynthesisTests() {
    var apiUrl = '/api/Synthesis/get';
    return promisifyAjaxGet(apiUrl);
}

function drawSynthesisTestsTableMessage(message) {
    testsList.html(drawTableMessage(message, 8));
}
function drawTestsList(data) {
    testsList.html('');
    $.each(data, function (index, test) {
        var task = test.task;
        var template = task.template;

        var testItem = '<tr>';
        testItem += drawNameCell(test);
        testItem += drawTaskCell(test);
        testItem += drawTemplateCell(test);
        testItem += drawAuthorCell(test);
        testItem += drawCreatedOnCell(test);
        testItem += drawStudentsCell(test);
        testItem += drawStateCell(test);
        testItem += drawDeleteCell(test);
        testItem += '</tr>'


        testsList.append(testItem);
    });
}
function drawNameCell(test) {
    var result = '<td class="hover-text-button">';
    result += '<div class="text">' + test.name + '</div>';
    result += drawCellEditModalButton('Modify', 'dark', '#update-name-modal', test.id, test.timeStamp, true, true);
    result += '</td>';
    return result;
}
function drawStudentsCell(test) {
    var enabled = test.status < TestStatus.Completed;
    return '<td>' + drawCellEditNavigationButton('Assign', 'dark', 'assignStudents', test.id, enabled) + '</td>';
}
function drawTemplateCell(test) {
    return '<td>' + test.task.template.name + '</td>';
}
function drawTaskCell(test) {
    return '<td>' + test.task.name + '</td>';
}
function drawAuthorCell(test) {
    return '<td><div class="text">' + 'cmilos@etf.bg.ac.rs' + '</div></td>'
}
function drawCreatedOnCell(test) {
    return '<td><div class="text">' + '08/03/2020 20:22' + '</div></td>'
}
function drawStateCell(test) {
    var result = '<td class="hover-text-button">';
    result += '<div class="text">' + TestStatus.ToString(test.status) + '</div>';
    result += drawChangeStatusModalButton('dark', test, true, true);
    result += '</td>';
    return result;
}
function drawChangeStatusModalButton(color, test, enabled, hidden) {
    var newStatus = test.status + 1;
    var result = '<button ';
    if (!enabled)
        result += 'disabled ';

    if (hidden)
        result += 'hidden="true" ';

    if (test.status == TestStatus.Completed)
        result += ' onclick="viewResults(' + test.id + ')" type="button" class="btn btn-outline-' + color + ' btn-sm">Results</button>';
    else
        result += 'data-toggle="modal" onclick="openStatusModal(this,' + newStatus + ')" data-id="' + test.id + '" data-timestamp="' + test.timeStamp + '" data-news-tatus-code="' + newStatus + '" type="button" class="btn btn-outline-' + color + ' btn-sm">' + TestStatus.ActionName(test.status) + '</button>'
    return result;
}
function drawDeleteCell(test) {
    return '<td>' + drawCellEditModalButton('Delete', 'danger', 'deleteTest', test.id, test.timeStamp, true) + '</td>';
}

//LOAD TASKS
function loadTasks() {
    getTasks()
        .then(data => {
            drawTaskList(data);
        });
}
function getTasks() {
    var apiUrl = '/api/Task/Get';
    return promisifyAjaxGet(apiUrl);
}
function drawTaskList(data) {
    var taskList = $('#create-test-modal').find('#task-list');

    $.each(data, function (index, task) {
        var item = '<a data-task-id="' + task.id + '"';
        item += ' style="word-wrap: break-word" class="list-group-item list-group-item-action" id="list-profile-list" data-toggle="list" href="#list-profile" role="tab" aria-controls="profile">';
        item += task.name;
        item += '</a>'

        taskList.append(item);
    });
}

//MODAL SHOW/HIDE
function onCreateSynthesisTestModalShow(element, event) {
    var button = $(event.relatedTarget)

    var modal = $(element)

    modal.find('#test-name').val('');
    modal.find('#test-id').val('');
    modal.find('#test-timestamp').val('');
    var taskItems = modal.find('a');
    $.each(taskItems, function (index, item) {
        $(item).removeClass('active', false);
    });

    hideModalError(element);
}
function onNameModalShow(element, event) {
    var button = $(event.relatedTarget)

    var name = button.parent().find('div').html();
    var id = button.data('id');
    var timestamp = button.data('timestamp');

    var modal = $(element)

    modal.find('#test-name').val(name);
    modal.find('#test-id').val(id);
    modal.find('#test-timestamp').val(timestamp);
    hideModalError(element);
}
function openStatusModal(btn, nextStatus) {
    var modal = $(statusModalSelector);

    var id = $(btn).data('id');
    var timestamp = $(btn).data('timestamp');

    modal.find('#test-id').val(id);
    modal.find('#test-timestamp').val(timestamp);

    modal.find('#change-status-message').html(TestStatus.ActionWarningText(nextStatus));

    modal.modal('show');
    hideModalError(statusModalSelector);
    return false;
}

//API CALLERS
function createTest() {
    var modalBody = $('#create-test-modal').find('.modal-body');

    var rqBody = {
        "Name": modalBody.find('#test-name').val(),
        "TaskId": modalBody.find('#task-list').find('a.active').data('task-id')
    };

    $.ajax({
        url: '/api/Synthesis/Create/Test',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            handleModalAjaxSuccess(createTestModalSelector, data, loadTests);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError(createTestModalSelector, loadTests);
        }
    });
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
            handleModalAjaxSuccess(nameModalSelector, data, loadTests);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError(nameModalSelector, loadTests);
        }
    });
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
            handleModalAjaxSuccess(statusModalSelector, data, loadTests);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError(statusModalSelector, loadTests);
        }
    });
}

//NAVIGATION
function viewResults(testId) {
    var form = $('#hidden-form');
    form.find('#test-id').val(testId);
    form.find('#test-type').val(TestType.Synthesis);
    form.attr('action', '/Test/Results');
    form.submit();
}
function assignStudents(testId) {
    window.location = `/Test/AssignStudents?testId=${testId}&testType=${TestType.Synthesis}`

    //var form = $('#hidden-form');
    //form.find('#test-id').val(testId);
    //form.find('#test-type').val(TestType.Synthesis);
    //form.attr('action', '');
    //form.submit();
}