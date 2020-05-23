var nameModal = null;
var templateId = null;

$(document).ready(function () {
    setActive("Exercises");
    
    loadTemplates();
    loadInstances();

    nameModal = nameModalBuilder.BuildHandler();
    nameModal.Init('#create-instance-modal', onInstanceModalShow, createInstance);
});


function loadTemplates() {
    getTemplates()
        .then(response => {
            renderTemplates(response);
            initializeTooltips();
        })
        .catch(error => {
            alert('Error loading data...');
        })
}

function loadInstances() {

}

//DOM HANDLERS
function renderTemplates(data) {
    var templateCards = $('#template-cards');
    templateCards.html('');

    $.each(data, function (index, template) {
        var cardHtml = renderTemplateCard(template);
        templateCards.append(cardHtml)
    });
}

function renderTemplateCard(template) {
    debugger;
    var descriptionPreview = "";
    var descriptionTooltip = "";

    if (template.description != null) {
        descriptionPreview = textPreview(template.description, 25);
        if (template.description.length > descriptionPreview.length) {
            descriptionTooltip = `data-toggle="tooltip" data-placement="top" title="${template.description}"`;
        }
    }

    var dateLabel = toLocaleDateTimeString(template.dateCreated);

    return `<div class="card bg-light mb-3" style="max-width: 15rem; margin:5px;">
                    <div class="card-header">${dateLabel}</div>
                    <div class="card-body">
                        <h5 class="card-title">${template.name}</h5>
                        <p class="card-text" ${descriptionTooltip}>${descriptionPreview}</p>
                        <button type="button" class="btn btn-outline-primary" data-toggle="modal" data-target="#create-instance-modal" data-template-id="${template.id}">
                            + Create instance
                        </button>
                    </div>
                </div>`;
}

//MODAL SHOW/HIDE
function onInstanceModalShow(element, event) {
    var button = $(event.relatedTarget);
    templateId = button.data('template-id');
}


//API CALLERS
function getTemplates() {
    var apiUrl = '/api/Exercise/get/templates';
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