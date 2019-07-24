$(document).ready(function () {
    var templateTableBody = $('#templates-tbody');
    populateTemplatesTable(templateTableBody, '/api/Template/Get');
});

//DRAW TEMPLATES TABLE
function populateTemplatesTable(tbody, apiUrl) {
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
        tableRow += drawSqlScriptCell(template);
        tableRow += drawBaseDataCell(template);
        tableRow += drawDeleteCell(template);

        tableRow += '</tr>'
        tbody.append(tableRow)
    });
}

function drawButton(buttonName, color, handlerName, id, timestamp, enabled) {
    var result = '<button ';
    if (!enabled)
        result += 'disabled ';
    result += 'onclick="' + handlerName + '(' + id + ',' + timestamp + ')" type="button" class="btn btn-outline-' + color + ' btn-sm">' + buttonName + '</button>'
    return result;
}

function drawNameCell(template) {
    var result = '<td><div class="description">';
    result += '<p style="float:left">' + template.name + ' &nbsp;&nbsp;&nbsp;</p>';
    result += drawButton('Edit', 'dark', 'updateName', template.id, template.timestamp, true);
    result += '</div></td>';
    return result;
}

function drawDescriptionCell(template) {
    if (template.modelDescription == null || template.modelDescription == '')
        return '<td>' + drawButton('Set', 'dark', 'updateDescription', template.id, template.timestamp, true) + '</td>';
    else
        return '<td>' + drawButton('Edit', 'dark', 'updateDescription', template.id, template.timestamp, true) + '</td>';
}

function drawSqlScriptCell(template) {
    if (template.sqlScript == null || template.sqlScript == '')
        return '<td>' + drawButton('Set', 'dark', 'updateSqlScript', template.id, template.timestamp, true) + '</td>';
    else
        return '<td>' + drawButton('Edit', 'dark', 'updateSqlScript', template.id, template.timestamp, true) + '</td>';
}

function drawBaseDataCell(template) {
    if (!template.isBaseDataSet) {
        var enabled = true;
        if (template.sqlScript == null || template.sqlScript == '')
            enabled = false;

        return '<td>' + drawButton('Set', 'dark', 'updateSqlScript', template.id, template.timestamp, enabled) + '</td>';
    }
    else
        return '<td>' + drawButton('Edit', 'dark', 'updateSqlScript', template.id, template.timestamp, true) + '</td>';
}

function drawDeleteCell(template) {
    return '<td>' + drawButton('Delete', 'danger', 'deleteTemplate', template.id, template.timestamp, true) + '</td>';
}


//ACTION HANDLERS
function createTemplate() {

}

function updateName(id, timestamp) {

}

function updateDescription(id, timestamp) {

}

function updateSqlScript(id, timestamp) {

}

function deleteTemplate(id, timestamp) {

}