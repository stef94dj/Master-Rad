var filterHeaderSelector = '#filter-header';
var tableHeaderSelector = '#table-header';

var createTemplateModal = null;
var nameModal = null;
var createTaskModal = null;

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

    createTaskModal = nameModalBuilder.BuildHandler();
    createTaskModal.Init('#create-task-modal', onCreateTaskModalShow, createTask);

    descriptionModal.Init('#update-description-modal', updateDescription);

    bindModalOnClose('#create-template-modal', createTemplateModalClose); //obsolete
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
    $('#templates-tbody').html(drawTableMessage(message, 4));
}
function drawTemplateTable(tbody, templates) {
    drawTemplateTableMessage('No records.');

    if (templates != null && templates.length > 0) {
        tbody.html('');
        $.each(templates, function (index, template) {
            var tableRow = '<tr>';

            tableRow += drawNameCell(template);
            //tableRow += drawDescriptionCell(template);
            //tableRow += drawModelCell(template);
            //tableRow += drawDataCell(template);
            tableRow += drawAuthorCell(template);
            tableRow += drawCreatedOnCell(template);
            //tableRow += drawDeleteCell(template);
            tableRow += drawActionsCell(template);

            tableRow += '</tr>'
            tbody.append(tableRow)
        });
    }
}
function drawNameCell(template) {
    return `<td><div class="text">${template.name}</div></td>`;
}

function drawDescriptionCell(template) {
    var result = '<td>';

    result += '<p style="float:left" hidden>';
    if (template.description != null)
        result += template.description;
    result += '</p>';

    result += drawCellEditModalButton('Modify', 'dark', '#update-description-modal', template.id, template.timeStamp, true);

    result += '</td>';

    return result;
}
function drawModelCell(template) {
    return '<td>' + drawCellEditNavigationButton('Modify', 'dark', 'updateModel', template.id, true) + '</td>';
}
function drawDataCell(template) {
    return '<td>' + drawCellEditNavigationButton('Modify', 'dark', 'updateData', template.id, true) + '</td>';
}
function drawAuthorCell(template) {
    var author = template.createdBy;
    return drawAuthorCellUtil(author.firstName, author.lastName, author.email)
}
function drawCreatedOnCell(template) {
    var value = toLocaleDateTimeString(template.dateCreated);
    return '<td><div class="text">' + value + '</div></td>'
}
function drawDeleteCell(template) {
    return '<td>' + drawCellEditModalButton('Delete', 'danger', 'deleteTemplate', template.id, template.timeStamp, true) + '</td>';
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
function createTemplateModalClose(element, event) {
    var modalBody = $(element).find('.modal-body');
    modalBody.find('#template-name').val('');
    hideModalError(element);
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

    debugger;
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
function deleteTemplate(id) {

}

//NAVIGATION
function updateModel(id) {
    var form = $('#hidden-form');
    form.find('#template-id').val(id);
    form.attr('action', '/Template/Model');
    form.submit();
}
function updateData(id) {
    var form = $('#hidden-form');
    form.find('#template-id').val(id);
    form.attr('action', '/Template/ModifyTemplateData');
    form.submit();
}

//ACTIONS
function nameAction(key) {
    alert('ciclked ' + key);
}
function descriptionAction(key) {
    alert('ciclked ' + key);
}
function templateAction(key) {
    alert('ciclked ' + key);
}
function deleteAction(key) {
    alert('ciclked ' + key);
}