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
    testsList.html('');
    $.each(data, function (index, test) {

        var sts = test.synthesisTestStudent;

        var testItem = '<tr>';
        testItem += drawNameCell(test);
        testItem += drawTextCell(`${sts.synthesisTest.name} <br/> <i>student: ${sts.student.firstName} ${sts.student.lastName}</i>`);
        testItem += drawTextCell(sts.synthesisTest.task.name);
        testItem += drawTextCell(sts.synthesisTest.task.template.name);
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
    var form = $('#hidden-form');
    form.find('#test-id').val(testId);
    form.find('#test-type').val(TestType.Analysis);
    form.attr('action', '/Test/Results');
    form.submit();
}
function assignStudents(testId) {
    var form = $('#hidden-form');
    form.find('#test-id').val(testId);
    form.find('#test-type').val(TestType.Analysis);
    form.attr('action', '/Test/AssignStudents');
    form.submit();
}