$(document).ready(function () {
    loadTasks($('#tasks-tbody'), '/api/Task/Get');
    loadTemplates('/api/Template/Get');
    bindModalOnShow('#update-name-modal', onNameModalShow);
    bindModalOnShow('#update-description-modal', onDescriptionModalShow);
    bindModalOnShow('#change-template-modal', onChangeTemplateModalShow);
});

//DRAW TEMPLATES TABLE
function drawTableMessage(message) {
    return '<tr><td align="center" colspan="6">' + message + '</td></tr>';
}
function drawTaskTable(tbody, tasks) {
    tbody.html('');
    $.each(tasks, function (index, task) {
        var tableRow = '<tr>';

        tableRow += drawNameCell(task);
        tableRow += drawDescriptionCell(task);
        tableRow += drawTemplateCell(task);
        tableRow += drawDataCell(task);
        tableRow += drawSolutionCell(task);
        tableRow += drawDeleteCell(task);

        tableRow += '</tr>'
        tbody.append(tableRow)
    });
}
function drawNameCell(task) {
    var result = '<td><div>';
    result += '<p style="float:left">' + task.name + '</p>';
    result += drawCellEditModalButton('Edit', 'dark', '#update-name-modal', task.id, task.timeStamp, true);
    result += '</div></td>';
    return result;
}
function drawDescriptionCell(task) {
    var result = '<td>';

    result += '<p style="float:left">';
    if (task.description != null)
        result += task.description;
    result += '</p>';

    if (task.description == null || task.description == '')
        result += drawCellEditModalButton('Set', 'dark', '#update-description-modal', task.id, task.timeStamp, true);
    else
        result += drawCellEditModalButton('Edit', 'dark', '#update-description-modal', task.id, task.timeStamp, true);

    result += '</td>';

    return result;
}
function drawTemplateCell(task) {
    var result = '<td data-template-id="' + task.template.id + '"><div>';
    result += '<p style="float:left">' + task.template.name + '</p>';
    result += drawCellEditModalButton('Edit', 'dark', '#change-template-modal', task.id, task.timeStamp, true);
    result += '</div></td>';
    return result;
}
function drawDataCell(task) {
    if (!task.isDataSet)
        return '<td>' + drawCellEditNavigationButton('Set', 'dark', 'updateData', task.id, true) + '</td>';
    else
        return '<td>' + drawCellEditNavigationButton('Edit', 'dark', 'updateData', task.id, true) + '</td>';
}
function drawSolutionCell(task) {
    return '<td>' + drawCellEditNavigationButton('Set', 'dark', 'updateSolution', task.id, true) + '</td>';
}
function drawDeleteCell(task) {
    return '<td>' + drawCellEditModalButton('Delete', 'danger', 'deleteTemplate', task.id, task.timestamp, true) + '</td>';
}

//DRAW TEMPLATES LIST
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

//MODAL SHOW
function bindModalOnShow(selector, handler) {
    $(selector).on('show.bs.modal', function (event) {
        handler(this, event);
    })
}
function onNameModalShow(element, event) {
    var button = $(event.relatedTarget)

    var name = button.parent().find('p').html();
    var id = button.data('id');
    var timestamp = button.data('timestamp');

    var modal = $(element)

    modal.find('#template-name').val(name);
    modal.find('#template-id').val(id);
    modal.find('#template-timestamp').val(timestamp);
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
}
function onChangeTemplateModalShow(element, event) {
    var button = $(event.relatedTarget)

    var taskId = button.data('id');
    var taskTimeStamp = button.data('timestamp');
    var templateId = button.parents().eq(1).data('template-id');

    var modal = $(element);

    modal.find('#task-id').val(taskId);
    modal.find('#task-timestamp').val(taskTimeStamp);

    var items = modal.find('#templates-list').find('a');
    $.each(items, function (index, item) {
        $(item).removeClass('active');

        if ($(item).data('template-id') == templateId)
            $(item).addClass('active');
    });

}

//API CALLERS
function loadTasks(tbody, apiUrl) {
    tbody.html(drawTableMessage('Loading data...'));
    $.ajax({
        url: apiUrl,
        type: 'GET',
        success: function (data) {
            drawTaskTable(tbody, data);
        },
        error: function () {
            tbody.html(drawTableMessage('Error loading data.'));
        }
    });
}
function loadTemplates(apiUrl) {
    //tbody.html(drawTableMessage('Loading data...'));
    $.ajax({
        url: apiUrl,
        type: 'GET',
        success: function (data) {
            drawTemplatesList(data);
        },
        error: function () {
            //tbody.html(drawTableMessage('Error loading data.'));
        }
    });
}
function createTask() {
    var modalBody = $('#create-task-modal').find('.modal-body');

    var rqBody = {
        "Name": modalBody.find('#task-name').val(),
        "TemplateId": modalBody.find('#templates-list').find('a.active').data('template-id')
    };

    $.ajax({
        url: '/api/Task/Create',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            $('#create-task-modal').modal('toggle');
            loadTasks($('#tasks-tbody'), '/api/Task/Get');
        }
    });
}
function updateName() {
    var modalBody = $('#update-name-modal').find('.modal-body');

    var rqBody = {
        "Id": parseInt(modalBody.find('#template-id').val()),
        "TimeStamp": modalBody.find('#template-timestamp').val(),
        "Name": modalBody.find('#template-name').val()
    }

    $.ajax({
        url: '/api/Task/Update/Name',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            $('#update-name-modal').modal('toggle');
            loadTasks($('#tasks-tbody'), '/api/Task/Get');
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
            $('#update-description-modal').modal('toggle');
            loadTasks($('#tasks-tbody'), '/api/Task/Get');
        }
    });
}
function updateTemplate() {
    var modalBody = $('#change-template-modal').find('.modal-body');

    var rqBody = {
        "Id": parseInt(modalBody.find('#task-id').val()),
        "TimeStamp": modalBody.find('#task-timestamp').val(),
        "TemplateId": modalBody.find('#templates-list').find('a.active').data('template-id')
    }

    $.ajax({
        url: '/api/Task/Update/Template',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            $('#change-template-modal').modal('toggle');
            loadTasks($('#tasks-tbody'), '/api/Task/Get');
        }
    });
}
function updateData(id) {
    debugger;
    var form = $('#hidden-form');
    form.find('#task-id').val(id);
    form.attr('action', '/Task/ModifyTaskData');
    form.submit();
}
function updateSolution(id) {
    debugger;
    var form = $('#hidden-form');
    form.find('#task-id').val(id);
    form.attr('action', '/Task/ModifyTaskSolution');
    form.submit();
}
function deleteTask(id) {

}