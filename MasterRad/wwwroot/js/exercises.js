var templateId = null;
var instanceId = null;
var instanceTimeStamp = null;

var nameModal = null;
var deleteModal = null;

$(document).ready(function () {
    setActive("Exercises");

    loadTemplates();
    loadInstances();

    nameModal = nameModalBuilder.BuildHandler();
    nameModal.Init('#create-instance-modal', onInstanceModalShow, createInstance);

    deleteModal = confirmationModalBuilder.BuildHandler();
    deleteModal.Init("#confirm-delete-modal", onDeleteModalShow, deleteInstance);
});


function loadTemplates() {
    getTemplates()
        .then(response => {
            renderTemplates(response);
            initializeTooltips();
        })
        .catch(error => {
            debugger;
            alert('Error loading templates...');
        })
}
function loadInstances() {
    getInstances()
        .then(response => {
            renderInstances(response);
            initializeTooltips();
        })
        .catch(error => {
            alert('Error loading instances...');
        })
}

//DOM HANDLERS
function renderTemplates(data) {
    var templateCards = $('#template-cards');
    templateCards.html('');

    if (data == null || data.length == 0) {
        templateCards.append('<p>Sorry, no templates available at this time.</p>')
    }
    else {
        $.each(data, function (index, template) {
            var cardHtml = renderTemplateCard(template);
            templateCards.append(cardHtml)
        });
    }
}
function renderInstances(data) {
    var instanceCards = $('#instance-cards');
    instanceCards.html('');

    if (data == null || data.length == 0) {
        instanceCards.append('<p>No instances. Go to Templates to create some.</p>')
    }
    else {
        $.each(data, function (index, instance) {
            var cardHtml = renderInstanceCard(instance);
            instanceCards.append(cardHtml)
        });
    }
}
function renderTemplateCard(template) {
    var descriptionPreview = "";
    var descriptionTooltip = "";

    if (template.description == null || template.description == "")
        template.description = "-";

    if (template.description == "")
        template.description = "-";
    descriptionPreview = textPreview(template.description, 25);
    if (template.description.length > descriptionPreview.length) {
        descriptionTooltip = `data-toggle="tooltip" data-placement="top" title="${template.description}"`;
    }

    namePreview = "";
    nameTooltip = "";

    if (template.name != null) {
        namePreview = textPreview(template.name, 15);
        if (template.name.length > namePreview.length) {
            nameTooltip = `data-toggle="tooltip" data-placement="top" title="${template.name}"`;
        }
    }

    var dateLabel = toLocaleDateTimeString(template.dateCreated);

    return `<div class="card bg-light mb-3" style="min-width: 15rem; max-width: 15rem; margin:5px;">
                    <div class="card-header" style="text-align:center;">${dateLabel}</div>
                    <div class="card-body">
                        <h5 class="card-title" ${nameTooltip} style="text-align:center;">${namePreview}</h5>
                        <p style="text-align:center;" class="card-text" ${descriptionTooltip}>${descriptionPreview}</p>
                        <div class="text-center">
                            <button type="button" class="btn btn-outline-primary" data-toggle="modal" data-target="#create-instance-modal" data-template-id="${template.id}">
                                + Create instance
                            </button>
                        </div>
                    </div>
                </div>`;
}
function renderInstanceCard(instance) {

    templateNamePreview = "";
    templateNameTooltip = "";

    if (instance.name != null) {
        templateNamePreview = textPreview(instance.name, 20);
        if (instance.name.length > templateNamePreview.length) {
            templateNameTooltip = `data-toggle="tooltip" data-placement="top" title="${instance.name}"`;
        }
    }

    namePreview = "";
    nameTooltip = "";

    if (instance.name != null) {
        namePreview = textPreview(instance.name, 20);
        if (instance.name.length > namePreview.length) {
            nameTooltip = `data-toggle="tooltip" data-placement="top" title="${instance.name}"`;
        }
    }

    var dateLabel = toLocaleDateTimeString(instance.dateCreated);
    return `<div class="card bg-light mb-3" style="max-width: 15rem; min-width: 15rem; margin:5px;">
                    <div class="card-header" style="text-align:center;">${dateLabel}</div>
                    <div class="card-body" data-instance-id="${instance.id}" data-instance-timestamp="${instance.timeStamp}">
                        <h5 style="text-align:center;" class="card-title" ${nameTooltip}>${namePreview}</h5>
                        <p style="text-align:center;" class="card-text" ${templateNameTooltip}>${templateNamePreview}</p>
                        <a class="btn btn-outline-primary" href="/Exercise/Instance?exerciseId=${instance.id}" role="button">Open</a>
                        <button type="button" class="btn btn-outline-danger" data-toggle="modal" data-target="#confirm-delete-modal"style="float:right">Delete</button>
                    </div>
                </div>`;
}

//MODAL SHOW/HIDE
function onInstanceModalShow(element, event) {
    var button = $(event.relatedTarget);
    templateId = button.data('template-id');
}
function onDeleteModalShow(element, event) {
    hideModalError("#confirm-delete-modal");
    var buttonParent = $(event.relatedTarget).parent();
    instanceId = buttonParent.data('instance-id');
    instanceTimeStamp = buttonParent.data('instance-timestamp');
}

//API CALLERS
function getTemplates() {
    var apiUrl = '/api/Exercise/get/templates';
    return promisifyAjaxGet(apiUrl);
}
function getInstances() {
    var apiUrl = '/api/Exercise/get/instances';
    return promisifyAjaxGet(apiUrl);
}
function createInstance() {
    var rqBody = {
        "Name": nameModal.GetInputVal(),
        "TemplateId": templateId
    }

    $.ajax({
        url: '/api/Exercise/create/instance',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            handleModalAjaxSuccess('#create-instance-modal', data, loadInstances);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError('#create-instance-modal', loadInstances);
        }
    });
}
function deleteInstance() {
    var rqBody = {
        "Id": instanceId,
        "TimeStamp": instanceTimeStamp
    }

    $.ajax({
        url: '/api/Exercise/delete/instance',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            handleModalAjaxSuccess('#confirm-delete-modal', data, loadInstances);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleModalAjaxError('#confirm-delete-modal', loadInstances);
        }
    });
}