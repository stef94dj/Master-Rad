$(document).ready(function () {
    loadTemplates($('#templates-tbody'), '/api/Template/Get');
    bindModalOnShow('#update-name-modal', onNameModalShow);
    bindModalOnShow('#update-description-modal', onDescriptionModalShow);
});

//DRAW TEMPLATES TABLE
function loadTemplates(tbody, apiUrl) {
    tbody.html(drawTableMessage('Loading data...'));
    $.ajax({
        url: apiUrl,
        type: 'GET',
        success: function (data) {
            drawTemplateTable(tbody, data);
        },
        error: function () {
            tbody.html(drawTableMessage('Error loading data.'));
        }
    });
}
function drawTableMessage(message) {
    return '<tr><td align="center" colspan="5">' + message + '</td></tr>';
}
function drawTemplateTable(tbody, templates) {
    tbody.html('');
    $.each(templates, function (index, template) {
        var tableRow = '<tr>';

        tableRow += drawNameCell(template);
        tableRow += drawDescriptionCell(template);
        tableRow += drawModelCell(template);
        tableRow += drawDataCell(template);
        tableRow += drawDeleteCell(template);

        tableRow += '</tr>'
        tbody.append(tableRow)
    });
}
function drawButton(buttonName, color, handlerName, id, enabled) {
    var result = '<button ';
    if (!enabled)
        result += 'disabled ';
    result += 'onclick="' + handlerName + '(' + id + ')" type="button" style="float:right" class="btn btn-outline-' + color + ' btn-sm">' + buttonName + '</button>'
    return result;
}
function drawModalTriggerButton(buttonName, color, modalselector, id, timestamp, enabled) {
    var result = '<button ';
    if (!enabled)
        result += 'disabled ';
    result += 'data-toggle="modal" data-target="' + modalselector + '" data-id="' + id + '" data-timestamp="' + timestamp + '" type="button" style="float:right" class="btn btn-outline-' + color + ' btn-sm">' + buttonName + '</button>'
    return result;
}
function drawNameCell(template) {
    var result = '<td><div>';
    result += '<p style="float:left">' + template.name + '</p>';
    result += drawModalTriggerButton('Edit', 'dark', '#update-name-modal', template.id, template.timeStamp, true);
    result += '</div></td>';
    return result;
}
function drawDescriptionCell(template) {
    var result = '<td>';

    result += '<p style="float:left">';
    if (template.modelDescription != null)
        result += template.modelDescription;
    result += '</p>';

    if (template.modelDescription == null || template.modelDescription == '')
        result += drawModalTriggerButton('Set', 'dark', '#update-description-modal', template.id, template.timeStamp, true);
    else
        result += drawModalTriggerButton('Edit', 'dark', '#update-description-modal', template.id, template.timeStamp, true);

    result += '</td>';

    return result;
}
function drawModelCell(template) {
    return '<td>' + drawButton('Edit', 'dark', 'updateModel', template.id, true) + '</td>';
}
function drawDataCell(template) {
    return '<td>' + drawButton('Edit', 'dark', 'updateData', template.id, true) + '</td>';
}
function drawDeleteCell(template) {
    return '<td>' + drawButton('Delete', 'danger', 'deleteTemplate', template.id, true) + '</td>';
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
            $('#create-template-modal').modal('toggle');
            loadTemplates($('#templates-tbody'), '/api/Template/Get');
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
            $('#update-name-modal').modal('toggle');
            loadTemplates($('#templates-tbody'), '/api/Template/Get');
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
            $('#update-description-modal').modal('toggle');
            loadTemplates($('#templates-tbody'), '/api/Template/Get');
        }
    });
}
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
function deleteTemplate(id) {

}