$(document).ready(function () {
    setActive("Templates");
    loadTemplates();

    bindModalOnShow('#update-name-modal', onNameModalShow);
    bindModalOnShow('#update-description-modal', onDescriptionModalShow);
    bindModalOnClose('#create-template-modal', createTemplateModalClose);
});

//DRAW TEMPLATES TABLE
function loadTemplates() {
    drawTemplateTableMessage('Loading data...');
    getTemplates()
        .then(data => {
            drawTemplateTable($('#templates-tbody'), data);
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
    var apiUrl = '/api/Template/Get';
    return promisifyAjaxGet(apiUrl);
}
function drawTemplateTableMessage(message) {
    $('tbody').html(drawTableMessage(message, 7));
}
function drawTemplateTable(tbody, templates) {
    drawTemplateTableMessage('No records.');

    if (templates != null && templates.length > 0) {
        tbody.html('');
        $.each(templates, function (index, template) {
            var tableRow = '<tr>';

            tableRow += drawNameCell(template);
            tableRow += drawDescriptionCell(template);
            tableRow += drawModelCell(template);
            tableRow += drawDataCell(template);
            tableRow += drawAuthorCell(template);
            tableRow += drawCreatedOnCell(template);
            tableRow += drawDeleteCell(template);

            tableRow += '</tr>'
            tbody.append(tableRow)
        });
    }
}
function drawNameCell(template) {
    var result = '<td class="hover-text-button">';
    result += '<div class="text">' + template.name + '</div>';
    result += drawCellEditModalButton('Modify', 'dark', '#update-name-modal', template.id, template.timeStamp, true, true);
    result += '</td>';
    return result;
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

//MODAL SHOW CLOSE
function onNameModalShow(element, event) {
    var button = $(event.relatedTarget)
    var name = button.parent().find('div').html();
    var id = button.data('id');
    var timestamp = button.data('timestamp');

    var modal = $(element)

    modal.find('#template-name').val(name);
    modal.find('#template-id').val(id);
    modal.find('#template-timestamp').val(timestamp);
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
function createTemplateModalClose(element, event) {
    var modalBody = $(element).find('.modal-body');
    modalBody.find('#template-name').val('');
    hideModalError(element);
}

//API CALLERS
function createTemplate() {
    var modalBody = $('#create-template-modal').find('.modal-body');

    var rqBody = {
        "Name": modalBody.find('#template-name').val()
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
    var modalBody = $('#update-name-modal').find('.modal-body');

    var rqBody = {
        "Id": parseInt(modalBody.find('#template-id').val()),
        "TimeStamp": modalBody.find('#template-timestamp').val(),
        "Name": modalBody.find('#template-name').val()
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
function updateDescription() {
    var modalBody = $('#update-description-modal').find('.modal-body');

    var rqBody = {
        "Id": parseInt(modalBody.find('#template-id').val()),
        "TimeStamp": modalBody.find('#template-timestamp').val(),
        "Description": modalBody.find('#template-description').val()
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