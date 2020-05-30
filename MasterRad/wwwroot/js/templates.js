var filterHeaderSelector = '#filter-header';
var tableHeaderSelector = '#table-header';

var createTemplateModal = null;
var nameModal = null;
var createTaskModal = null;
var deleteModal = null;

$(document).ready(function () {
    setActive("Templates");
    var paginationConfig = {
        "tableThSelector": tableHeaderSelector,
        "filterThSelector": filterHeaderSelector,
        "reloadFunction": loadTemplates,
        "displayPagesCnt": 5,
        "sortDefaultKey": "date_created",
    }
    pagination.initUI(paginationConfig);
    loadTemplates();
    initializeTooltips();

    var dataAttributes = ["id", "timestamp", "name", "description"];
    actionsModal.Init('#actions-modal', dataAttributes, onActionsModalShow);

    createTemplateModal = nameModalBuilder.BuildHandler();
    createTemplateModal.Init('#create-template-modal', onCreateTemplateModalShow, createTemplate);

    nameModal = nameModalBuilder.BuildHandler();
    nameModal.Init('#update-name-modal', onNameModalShow, updateName);

    descriptionModal.Init('#update-description-modal', updateDescription);

    createTaskModal = nameModalBuilder.BuildHandler();
    createTaskModal.Init('#create-task-modal', onCreateTaskModalShow, createTask);

    deleteModal = confirmationModalBuilder.BuildHandler();
    deleteModal.Init("#confirm-delete-modal", onDeleteModalShow, deleteTemplate);
});

//DRAW TEMPLATES TABLE
function loadTemplates() {
    drawTemplateTableMessage('Loading data...');
    getTemplates()
        .then(response => {
            drawTemplateTable($('#templates-tbody'), response.data);
            if (response && response.pageCnt && response.pageNo)
                pagination.drawPagesUI(response.pageCnt, response.pageNo);
        })
        .catch(error => {
            if (error?.status && error.status === HttpCodes.ReloadRequired) {
                drawTemplateTableMessage('Reloading...');
                location.reload();
            }
            else {
                drawTemplateTableMessage('Error loading data...');
            }
        })
        .then(data => {
            defineNameHoverBehaviour($('td.hover-text-button'));
        });
}

function getTemplates() {
    var apiUrl = '/api/Template/Search';
    var rqBody = pagination.buildSearchRQ();
    return promisifyAjaxPost(apiUrl, rqBody);
}
function drawTemplateTableMessage(message) {
    $('#templates-tbody').html(drawTableMessage(message, 6));
}
function drawTemplateTable(tbody, templates) {
    drawTemplateTableMessage('No records.');

    if (templates != null && templates.length > 0) {
        tbody.html('');
        $.each(templates, function (index, template) {
            var tableRow = '<tr>';

            tableRow += drawTextCell(template.name, 20);
            tableRow += drawTextCell(template.description, 20);
            tableRow += drawAuthorCell(template.createdBy);
            tableRow += drawCreatedOnCell(template.dateCreated);
            tableRow += drawPublicCell(template);
            tableRow += drawActionsCell(template);

            tableRow += '</tr>'
            tbody.append(tableRow)
        });
    }
}

function drawPublicCell(template) {
    var checkedHtml = template.isPublic ? 'checked="checked"' : '';
    return `<td><div><input type="checkbox" ${checkedHtml} style="transform:scale(1.5)" onchange="toggleIsPublic(this, ${template.id}, '${template.timeStamp}')"></div></td>`;
}
function drawActionsCell(template) {
    var dataAttributes = {
        "id": template.id,
        "timestamp": template.timeStamp,
        "name": template.name,
        "description": template.description
    }
    return '<td>' + actionsModal.drawActionsBtn('#actions-modal', dataAttributes) + '</td>';
}

//MODAL SHOW CLOSE
function onActionsModalShow(element, event) {
    $('#model-url').attr('href', `/Template/Model?templateId=${actionsModal.id}`);
    $('#data-url').attr('href', `/Template/ModifyTemplateData?templateId=${actionsModal.id}`);
}
function onNameModalShow(element, event) {
    nameModal.SetInputVal(actionsModal.name);
    nameModal.SetTitle(`Update name for '${actionsModal.name}'`);
}
function onCreateTemplateModalShow(element, event) {
    nameModal.SetInputVal('');
}
function onCreateTaskModalShow(element, event) {
    createTaskModal.SetInputVal('');
    createTaskModal.SetTitle(`Create task from '${actionsModal.name}'`);
}
function onDeleteModalShow(element, event) {
    hideModalError("#confirm-delete-modal");
    deleteModal.SetText(`Are you sure you wish to delete template '${actionsModal.name}' ?`);
}

//API CALLERS
function createTemplate() {
    var rqBody = {
        "Name": createTemplateModal.GetInputVal()
    }

    $.ajax({
        url: '/api/Template/Create',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            handleModalAjaxSuccess('#create-template-modal', data, loadTemplates);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError('#create-template-modal', loadTemplates);
        }
    });
}
function updateName() {
    var rqBody = {
        "Id": actionsModal.id,
        "TimeStamp": actionsModal.timestamp,
        "Name": nameModal.GetInputVal()
    }

    $.ajax({
        url: '/api/Template/Update/Name',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            handleModalAjaxSuccess('#update-name-modal', data, loadTemplates);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError('#update-name-modal', loadTemplates);
        }
    });
}
function createTask() {
    var rqBody = {
        "Name": createTaskModal.GetInputVal(),
        "TemplateId": actionsModal.id
    };

    $.ajax({
        url: '/api/Task/Create',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            if (data != null && data.isSuccess)
                window.location.replace('/TeacherMenu/Tasks');

            handleModalAjaxSuccess('#create-task-modal', data, null);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError('#create-task-modal', null);
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
        url: '/api/Template/Update/Description',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            handleModalAjaxSuccess('#update-description-modal', data, loadTemplates);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError('#update-name-modal', loadTemplates);
        }
    });
}
function deleteTemplate() {
    var rqBody = {
        "Id": actionsModal.id,
        "TimeStamp": actionsModal.timestamp
    }

    $.ajax({
        url: '/api/Template/Delete',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            handleModalAjaxSuccess('#confirm-delete-modal', data, loadTemplates);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError('#confirm-delete-modal', loadTemplates);
        }
    });
}
function toggleIsPublic(cbx, id, timestamp) {
    debugger;
    var newVal = $(cbx).is(":checked");

    var rqBody = {
        "Id": id,
        "TimeStamp": timestamp,
        "IsPublic": newVal
    }

    $.ajax({
        url: '/api/Template/Toggle/IsPublic',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            loadTemplates();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            loadTemplates();
        }
    });
}