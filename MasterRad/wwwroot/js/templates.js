$(document).ready(function () {
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

//MODAL SHOW
function onNameModalShow(element, event) {
    var button = $(event.relatedTarget)
    var name = button.parent().find('div').html();
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
            hideModalError('#create-template-modal');
            if (data != null && data.isSuccess) {
                $('#create-template-modal').modal('toggle');
                loadTemplates($('#templates-tbody'), '/api/Template/Get');
                modalBody.find('#template-name').val('');
            }
            else if (data.errors != null && data.errors.length > 0) {
                showModalError('#create-template-modal', data.errors[0]);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            showModalError('#create-template-modal', 'Unexpected error occured.');
        }
    });
}
function createTemplateModalClose() {
    var modalSelector = '#create-template-modal';
    var modalBody = $(modalSelector).find('.modal-body');
    modalBody.find('#template-name').val('');
    hideModalError(modalSelector);
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
            loadTemplates();
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
            loadTemplates();
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