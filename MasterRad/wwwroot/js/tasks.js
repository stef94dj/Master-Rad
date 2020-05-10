﻿var filterHeaderSelector = '#filter-header';
var tableHeaderSelector = '#table-header';
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
    loadTemplates();

    bindModalOnShow('#create-task-modal', onCreateTaskModalShow);
    bindModalOnShow('#update-name-modal', onNameModalShow);
    bindModalOnShow('#update-description-modal', onDescriptionModalShow);
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
    $('#tasks-tbody').html(drawTableMessage(message, 8));
}
function drawTaskTable(tasks) {
    drawTaskTableMessage('No records.');

    if (tasks != null && tasks.length > 0) {
        tbody = $('#tasks-tbody');
        tbody.html('');
        $.each(tasks, function (index, task) {
            var tableRow = '<tr>';

            tableRow += drawNameCell(task);
            tableRow += drawTemplateCell(task);
            tableRow += drawDescriptionCell(task);
            tableRow += drawDataCell(task);
            tableRow += drawSolutionCell(task);
            tableRow += drawAuthorCell(task);
            tableRow += drawCreatedOnCell(task);
            tableRow += drawDeleteCell(task);

            tableRow += '</tr>'
            tbody.append(tableRow)
        });
    }
}
function drawNameCell(task) {
    var result = '<td class="hover-text-button">';
    result += '<div class="text">' + task.name + '</div>';
    result += drawCellEditModalButton('Modify', 'dark', '#update-name-modal', task.id, task.timeStamp, true, true);
    result += '</td>';
    return result;
}
function drawDescriptionCell(task) {
    var hiddenDescription = `<p style="display:none">${task.description != null ? task.description : ''}</p>`
    var editBtn = drawCellEditModalButton('Modify', 'dark', '#update-description-modal', task.id, task.timeStamp, true);
    return `<td>${hiddenDescription}${editBtn}</td>`;
}
function drawTemplateCell(task) {
    return '<td>' + task.templateName + '</td>';
}
function drawDataCell(task) {
    return `<td>${drawCellEditNavigationButton('Modify', 'dark', 'updateData', task.id, true)}</td>`;
}
function drawSolutionCell(task) {
    return `<td>${drawCellEditNavigationButton('Modify', 'dark', 'updateSolution', task.id, true)}</td>`
}
function drawAuthorCell(task) {
    var author = task.createdBy;
    return drawAuthorCellUtil(author.firstName, author.lastName, author.email)
}
function drawCreatedOnCell(task) {
    var value = toLocaleDateTimeString(task.dateCreated);
    return '<td><div class="text">' + value + '</div></td>'
}
function drawDeleteCell(task) {
    return '<td>' + drawCellEditModalButton('Delete', 'danger', 'deleteTemplate', task.id, task.timestamp, true) + '</td>';
}

//LOAD TEMPLATES
function loadTemplates() {
    getTemplates()
        .then(data => {
            drawTemplatesList(data);
        })
}
function getTemplates() {
    var apiUrl = '/api/Template/Get';
    return promisifyAjaxGet(apiUrl);
}
function drawTemplatesList(data) {
    var createTaskList = $('#create-task-modal').find('#templates-list');
    var editTemplateList = $('#change-template-modal').find('#templates-list');

    $.each(data, function (index, template) {
        var item = '<a data-template-id="' + template.id + '"';
        item += ' style="word-wrap: break-word" class="list-group-item list-group-item-action" id="list-profile-list" data-toggle="list" href="#list-profile" role="tab" aria-controls="profile">';
        item += template.name;
        item += '</a>'

        createTaskList.append(item);
        editTemplateList.append(item);
    });
}

//MODAL SHOW/HIDE
function onCreateTaskModalShow(element, event) {
    var modal = $(element);
    modal.find('#task-name').val('');
    var templateItems = modal.find('a');
    $.each(templateItems, function (index, item) {
        $(item).removeClass('active', false);
    });

    hideModalError(element);
}
function onNameModalShow(element, event) {
    var button = $(event.relatedTarget)

    var name = button.parent().find('div').html();
    var id = button.data('id');
    var timestamp = button.data('timestamp');

    var modal = $(element);

    modal.find('#task-name').val(name);
    modal.find('#task-id').val(id);
    modal.find('#task-timestamp').val(timestamp);
    hideModalError(element);
}
function onDescriptionModalShow(element, event) {
    var button = $(event.relatedTarget)

    var name = button.parent().find('p').html();
    var id = button.data('id');
    var timestamp = button.data('timestamp');

    var modal = $(element)

    modal.find('#template-description').val(name);
    modal.find('#template-id').val(id);
    modal.find('#template-timestamp').val(timestamp);
    hideModalError(element);
}

//API CALLERS
function updateName() {
    var modalBody = $('#update-name-modal').find('.modal-body');

    var rqBody = {
        "Id": parseInt(modalBody.find('#task-id').val()),
        "TimeStamp": modalBody.find('#task-timestamp').val(),
        "Name": modalBody.find('#task-name').val()
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
    var modalBody = $('#update-description-modal').find('.modal-body');

    var rqBody = {
        "Id": parseInt(modalBody.find('#template-id').val()),
        "TimeStamp": modalBody.find('#template-timestamp').val(),
        "Description": modalBody.find('#template-description').val()
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

//NAVIGATION
function updateData(id) {
    var form = $('#hidden-form');
    form.find('#task-id').val(id);
    form.attr('action', '/Task/ModifyTaskData');
    form.submit();
}
function updateSolution(id) {
    var form = $('#hidden-form');
    form.find('#task-id').val(id);
    form.attr('action', '/Task/ModifyTaskSolution');
    form.submit();
}
function deleteTask(id) {

}