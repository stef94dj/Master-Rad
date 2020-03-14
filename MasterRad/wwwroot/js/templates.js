﻿$(document).ready(function () {
    //loadTemplates($('#templates-tbody'), '/api/Template/Get');

    loadTemplates();

    bindModalOnShow('#update-name-modal', onNameModalShow);
    bindModalOnShow('#update-description-modal', onDescriptionModalShow);
    bindModalOnClose('#create-template-modal', createTemplateModalClose);

    $.each($('td.hover-text-button'), function (index, item) {
        debugger;
        $(item).hover(
            function () { //on hover in
                debugger;
                textButtonSwap(this, false);
            },
            function () { //on hover out
                textButtonSwap(this, true);
            })
    });
});

//DRAW TEMPLATES TABLE
function loadTemplates() {
    getTemplates()
        .then(data => {
            drawTemplateTable($('#templates-tbody'), data);
        })
        .then(data => {
            defineNameHoverBehaviour();
        });
}

function getTemplates() {
    var apiUrl = '/api/Template/Get';
    return promisifyAjaxGet(apiUrl);
}
function drawTemplateTableMessage(message) {
    return drawTableMessage(message, 5);
}
function drawTemplateTable(tbody, templates) {
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
function drawNameCell(template) {
    var result = '<td class="hover-text-button" onclick="clickModifyButton(this)">';
    result += '<div class="text">' + template.name + '</div>';
    result += drawCellEditModalButton('Modify', 'dark', '#update-name-modal', template.id, template.timeStamp, true, true);
    result += '</td>';
    return result;
}
function clickModifyButton(td) {
    $(td).find('button')[0].click();
}

function drawDescriptionCell(template) {
    var result = '<td>';

    result += '<p style="float:left" hidden>';
    if (template.modelDescription != null)
        result += template.modelDescription;
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
    return '<td><div class="text">' + 'cmilos@etf.bg.ac.rs' + '</div></td>'
}
function drawCreatedOnCell(template) {
    return '<td><div class="text">' + '08/03/2020 20:22' + '</div></td>'
}
function drawDeleteCell(template) {
    return '<td>' + drawCellEditModalButton('Delete', 'danger', 'deleteTemplate', template.id, template.timeStamp, true) + '</td>';
}


function defineNameHoverBehaviour() {
    $.each($('td.hover-text-button'), function (index, item) {
        $(item).hover(
            function () { //on hover in
                textButtonSwap(this, false);
            },
            function () { //on hover out
                textButtonSwap(this, true);
            })
    });
}
function textButtonSwap(td, showText) {
    var p = $(td).find('div.text')[0];
    var btn = $(td).find('button')[0];

    $(p).attr('hidden', !showText);
    $(btn).attr('hidden', showText);
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
    hideModalError(element);
    var button = $(event.relatedTarget)

    var name = button.parent().find('p').html();
    var id = button.data('id');
    var timestamp = button.data('timestamp');

    var modal = $(element)

    modal.find('#template-description').val(name);
    modal.find('#template-id').val(id);
    modal.find('#template-timestamp').val(timestamp);
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
            handleModalAjaxSuccess('#create-template-modal', data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError('#create-template-modal');
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
            handleModalAjaxSuccess('#update-name-modal', data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError('#update-name-modal');
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
            handleModalAjaxSuccess('#update-description-modal', data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError('#update-name-modal');
        }
    });
}
function deleteTemplate(id) {

}

function handleModalAjaxSuccess(modalSelector, data) {
    var modal = $(modalSelector);
    hideModalError(modal);
    if (data != null && data.isSuccess) {
        modal.modal('toggle');
        modal.find('.modal-body').find(modalSelector).val('');
    }
    else if (data.errors != null && data.errors.length > 0) {
        showModalError(modal, data.errors[0]);
    }

    loadTemplates();
}
function handleModalAjaxError(modalSelector) {
    showModalError(modalSelector, 'Unexpected error occured.');
    loadTemplates();
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