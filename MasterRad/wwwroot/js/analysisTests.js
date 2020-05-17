var testsList = null;
var statusModalSelector = null;
var nameModalSelector = null;

var filterHeaderSelector = '#filter-header';
var tableHeaderSelector = '#table-header';

var nameModal = null;
var statusModal = null;
var deleteModal = null;

$(document).ready(function () {
    setActive("Analysis Tests");
    var paginationConfig = {
        "tableThSelector": tableHeaderSelector,
        "filterThSelector": filterHeaderSelector,
        "reloadFunction": loadTests,
        "displayPagesCnt": 5,
        "sortDefaultKey": "date_created",
    }
    pagination.initUI(paginationConfig);
    testsList = $('#tests-tbody');
    loadTests();

    var dataAttributes = ["id", "timestamp", "name", "status"];
    actionsModal.Init('#actions-modal', dataAttributes, onActionsModalShow);

    nameModal = nameModalBuilder.BuildHandler();
    nameModal.Init('#update-name-modal', onNameModalShow, updateName);

    statusModal = confirmationModalBuilder.BuildHandler();
    statusModal.Init("#status-modal", onStatusModalShow, statusNext);

    deleteModal = confirmationModalBuilder.BuildHandler();
    deleteModal.Init("#confirm-delete-modal", onDeleteModalShow, deleteAnalysis);
});

//LOAD TESTS
function loadTests() {
    drawAnalysisTestsTableMessage('Loading data...');

    getAnalysisTests()
        .then(response => {
            drawTestsList(response.data);
            if (response && response.pageCnt && response.pageNo)
                pagination.drawPagesUI(response.pageCnt, response.pageNo);
        })
        .catch(error => {
            drawAnalysisTestsTableMessage('Error loading data...');
        })
        .then(data => {
            defineNameHoverBehaviour($('td.hover-text-button'));
        });
}
function getAnalysisTests() {
    var apiUrl = '/api/Analysis/Search';
    var rqBody = pagination.buildSearchRQ();
    return promisifyAjaxPost(apiUrl, rqBody);
}

function drawAnalysisTestsTableMessage(message) {
    testsList.html(drawTableMessage(message, 8));
}
function drawTestsList(data) {
    drawAnalysisTestsTableMessage("No records.");

    if (data != null && data.length > 0) {
        testsList.html('');
        $.each(data, function (index, test) {
            var testItem = '<tr>';
            testItem += drawTextCell(test.name, 20);
            testItem += drawTextCell(`${test.synthesisTestName} <br/> <i>${test.student.firstName} ${test.student.lastName}</i> <br/> <i>${test.student.email}</i>`);
            testItem += drawTextCell(test.taskName);
            testItem += drawTextCell(test.templateName);
            testItem += drawAuthorCell(test.createdBy);
            testItem += drawCreatedOnCell(test.dateCreated);
            testItem += drawTextCell(TestStatus.ToString(test.status), 15);
            testItem += drawActionsCell(test);
            testItem += '</tr>'

            testsList.append(testItem);
        });
    }
}
function drawActionsCell(test) {
    var dataAttributes = {
        "id": test.id,
        "timestamp": test.timeStamp,
        "name": test.name,
        "status": test.status
    }
    return '<td>' + actionsModal.drawActionsBtn('#actions-modal', dataAttributes) + '</td>';
}

//MODAL SHOW/HIDE
function onActionsModalShow(element, event) {
    $('#assign-url').attr('href', `/Test/AssignStudents?testId=${actionsModal.id}&testType=${TestType.Analysis}`);
    $('#results-url').attr('href', `/Test/Results?testId=${actionsModal.id}&testType=${TestType.Analysis}`);

    if (actionsModal.status == TestStatus.Scheduled) {
        $('#assign-url').attr('hidden', false);
    }
    else {
        $('#assign-url').attr('hidden', true);
    }

    if (actionsModal.status == TestStatus.Completed) {
        $('#results-url').attr('hidden', false);
        $('#status-next').attr('hidden', true);
    }
    else {
        $('#results-url').attr('hidden', true);
        $('#status-next').attr('hidden', false);
    }
}
function onNameModalShow(element, event) {
    nameModal.SetInputVal(actionsModal.name);
    nameModal.SetTitle(`Update name for '${actionsModal.name}'`);
}
function onStatusModalShow(element, event) {
    statusModal.SetText(`Are you sure you want to change the status of '${actionsModal.name}' ?`);
}
function onDeleteModalShow(element, event) {
    hideModalError("#confirm-delete-modal");
    deleteModal.SetText(`Are you sure you wish to delete analysis test '${actionsModal.name}' ?`);
}

//API CALLERS
function updateName() {
    var rqBody = {
        "Id": actionsModal.id,
        "TimeStamp": actionsModal.timestamp,
        "Name": nameModal.GetInputVal()
    }

    $.ajax({
        url: '/api/Analysis/update/name',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            handleModalAjaxSuccess('#update-name-modal', data, loadTests);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError('#update-name-modal', loadTests);
        }
    });
}
function statusNext() {
    var rqBody = {
        "Id": actionsModal.id,
        "TimeStamp": actionsModal.timestamp
    }

    $.ajax({
        url: '/api/Analysis/status/next',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            handleModalAjaxSuccess('#status-modal', data, loadTests);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError('#status-modal', loadTests);
        }
    });
}
function deleteAnalysis() {
    var rqBody = {
        "Id": actionsModal.id,
        "TimeStamp": actionsModal.timestamp
    }

    $.ajax({
        url: '/api/Analysis/Delete',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            handleModalAjaxSuccess('#confirm-delete-modal', data, loadTests);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError('#confirm-delete-modal', loadTests);
        }
    });
}