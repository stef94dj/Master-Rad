var testsList = null;
var statusModalSelector = null;
var nameModalSelector = null;
$(document).ready(function () {
    testsList = $('#tests-tbody');
    loadTests();
    statusModalSelector = '#update-staus-modal';
    nameModalSelector = '#update-name-modal';
    bindModalOnShow(nameModalSelector, onNameModalShow);
});

//LOAD TESTS
function loadTests() {
    drawAnalysisTestsTableMessage('Loading data...');

    getAnalysisTests()
        .then(data => {
            drawTestsList(data);
        })
        .catch(error => {
            drawAnalysisTestsTableMessage('Error loading data...');
        })
        .then(data => {
            defineNameHoverBehaviour($('td.hover-text-button'));
        });
}
function getAnalysisTests() {
    var apiUrl = '/api/Analysis/get';
    return promisifyAjaxGet(apiUrl)
}

function drawAnalysisTestsTableMessage(message) {
    testsList.html(drawTableMessage(message, 9));
}
function drawTestsList(data) {
    drawAnalysisTestsTableMessage("No records.");

    if (data != null && data.length > 0) {
        testsList.html('');
        $.each(data, function (index, test) {
            var testItem = '<tr>';
            testItem += drawNameCell(test);
            testItem += drawTextCell(`${test.synthesisTestName} <br/> <i>${test.student.firstName} ${test.student.lastName}</i> <br/> <i>${test.student.email}</i>`);
            testItem += drawTextCell(test.taskName);
            testItem += drawTextCell(test.templateName);
            testItem += drawAuthorCell(test);
            testItem += drawCreatedOnCell(test);
            testItem += drawStudentsCell(test);
            testItem += drawStateCell(test);
            testItem += drawDeleteCell(test);
            testItem += '</tr>'

            testsList.append(testItem);
        });
    }
}
function drawNameCell(test) {
    var result = '<td class="hover-text-button">';
    result += '<div class="text">' + test.name + '</div>';
    result += drawCellEditModalButton('Edit', 'dark', '#update-name-modal', test.id, test.timeStamp, true, true);
    result += '</td>';
    return result;
}
function drawTextCell(templateName) {
    return `<td>${templateName}</td>`;
}
function drawStudentsCell(test) {
    var enabled = test.status < TestStatus.Completed;
    return '<td>' + drawCellEditNavigationButton('Assign', 'dark', 'assignStudents', test.id, enabled) + '</td>';
}
function drawAuthorCell(test) {
    var author = test.createdBy;
    return drawAuthorCellUtil(author.firstName, author.lastName, author.email);
}
function drawCreatedOnCell(test) {
    var value = toLocaleDateTimeString(test.dateCreated);
    return '<td><div class="text">' + value + '</div></td>'
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

//MODAL SHOW/HIDE
function onNameModalShow(element, event) {
    var button = $(event.relatedTarget)

    var name = button.parent().find('div.text').html();
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
function updateName() {
    var modalBody = $(nameModalSelector).find('.modal-body');

    var rqBody = {
        "Id": parseInt(modalBody.find('#test-id').val()),
        "TimeStamp": modalBody.find('#test-timestamp').val(),
        "Name": modalBody.find('#test-name').val()
    }

    $.ajax({
        url: '/api/Analysis/update/name',
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
        url: '/api/Analysis/status/next',
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
    window.location = `/Test/Results?testId=${testId}&testType=${TestType.Analysis}`;
}
function assignStudents(testId) {
    window.location = `/Test/AssignStudents?testId=${testId}&testType=${TestType.Analysis}`
}