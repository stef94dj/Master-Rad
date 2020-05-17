var filterHeaderSelector = '#filter-header';
var tableHeaderSelector = '#table-header';

var nameModal = null;
var createSynthesisTestModal = null;
var deleteModal = null;

$(document).ready(function () {
    setActive("Tasks");
    var paginationConfig = {
        "tableThSelector": tableHeaderSelector,
        "filterThSelector": filterHeaderSelector,
        "reloadFunction": loadTasks,
        "displayPagesCnt": 5,
        "sortDefaultKey": "date_created",
    }
    pagination.initUI(paginationConfig);
    loadTasks();

    var dataAttributes = ["id", "timestamp", "name", "description"];
    actionsModal.Init('#actions-modal', dataAttributes, onActionsModalShow);

    nameModal = nameModalBuilder.BuildHandler();
    nameModal.Init('#update-name-modal', onNameModalShow, updateName);

    descriptionModal.Init('#update-description-modal', updateDescription);

    createSynthesisTestModal = nameModalBuilder.BuildHandler();
    createSynthesisTestModal.Init('#create-synthesis-modal', onCreateSynthesisModalShow, createTest);

    deleteModal = confirmationModalBuilder.BuildHandler();
    deleteModal.Init("#confirm-delete-modal", onDeleteModalShow, deleteTask);
});

//LOAD TASKS
function loadTasks() {
    drawTaskTableMessage('Loading data...');
    getTasks()
        .then(response => {
            drawTaskTable(response.data);
            if (response && response.pageCnt && response.pageNo)
                pagination.drawPagesUI(response.pageCnt, response.pageNo);
        })
        .catch(error => {
            if (error?.status && error.status === HttpCodes.ReloadRequired) {
                drawTaskTableMessage('Reloading...');
                location.reload();
            }
            else {
                drawTaskTableMessage('Error loading data...');
            }
        })
        .then(data => {
            defineNameHoverBehaviour($('td.hover-text-button'));
        });
}
function getTasks() {
    var apiUrl = '/api/Task/Search';
    var rqBody = pagination.buildSearchRQ();
    return promisifyAjaxPost(apiUrl, rqBody);
}

function drawTaskTableMessage(message) {
    $('#tasks-tbody').html(drawTableMessage(message, 6));
}
function drawTaskTable(tasks) {
    drawTaskTableMessage('No records.');

    if (tasks != null && tasks.length > 0) {
        tbody = $('#tasks-tbody');
        tbody.html('');
        $.each(tasks, function (index, task) {
            var tableRow = '<tr>';

            tableRow += drawTextCell(task.name, 20);
            tableRow += drawTextCell(task.templateName, 20);
            tableRow += drawTextCell(task.description, 20);
            tableRow += drawAuthorCell(task.createdBy);
            tableRow += drawCreatedOnCell(task.dateCreated);
            tableRow += drawActionsCell(task);

            tableRow += '</tr>'
            tbody.append(tableRow)
        });
    }
}
function drawActionsCell(task) {
    var dataAttributes = {
        "id": task.id,
        "timestamp": task.timeStamp,
        "name": task.name,
        "description": task.description
    }
    return '<td>' + actionsModal.drawActionsBtn('#actions-modal', dataAttributes) + '</td>';
}

//MODAL SHOW/HIDE
function onActionsModalShow(element, event) {
    $('#data-url').attr('href', `/Task/ModifyTaskData?taskId=${actionsModal.id}`);
    $('#solution-url').attr('href', `/Task/ModifyTaskSolution?taskId=${actionsModal.id}`);
}
function onNameModalShow(element, event) {
    nameModal.SetInputVal(actionsModal.name);
    nameModal.SetTitle(`Update name for '${actionsModal.name}'`);
}
function onCreateSynthesisModalShow(element, event) {
    createSynthesisTestModal.SetInputVal('');
    createSynthesisTestModal.SetTitle(`Create synthesis test from '${actionsModal.name}'`);
}
function onDeleteModalShow(element, event) {
    hideModalError("#confirm-delete-modal");
    deleteModal.SetText(`Are you sure you wish to delete task '${actionsModal.name}' ?`);
}

//API CALLERS
function updateName() {
    var rqBody = {
        "Id": actionsModal.id,
        "TimeStamp": actionsModal.timestamp,
        "Name": nameModal.GetInputVal()
    }

    $.ajax({
        url: '/api/Task/Update/Name',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            handleModalAjaxSuccess('#update-name-modal', data, loadTasks);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError('#update-name-modal', loadTasks);
        }
    });
}
function updateDescription() {
    var rqBody = {
        "Id": actionsModal.id,
        "TimeStamp": actionsModal.timestamp,
        "Description": descriptionModal.GetInputVal()
    }

    $.ajax({
        url: '/api/Task/Update/Description',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            handleModalAjaxSuccess('#update-description-modal', data, loadTasks);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError('#update-description-modal', loadTasks);
        }
    });
}
function createTest() {
    var rqBody = {
        "Name": createSynthesisTestModal.GetInputVal(),
        "TaskId": actionsModal.id
    };

    $.ajax({
        url: '/api/Synthesis/Create/Test',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            handleModalAjaxSuccess('#create-synthesis-modal', data, loadTasks);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError('#create-synthesis-modal', loadTasks);
        }
    });
}
function deleteTask() {
    var rqBody = {
        "Id": actionsModal.id,
        "TimeStamp": actionsModal.timestamp
    }

    $.ajax({
        url: '/api/Task/Delete',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            handleModalAjaxSuccess('#confirm-delete-modal', data, loadTasks);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError('#confirm-delete-modal', loadTasks);
        }
    });
}