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

function drawButton(buttonName, color, handlerName, id, timestamp) {
    return '<td><button onclick="' + handlerName + '(' + id + ',' + timestamp + ')" type="button" class="btn btn-outline-' + color + ' btn-sm">' + buttonName + '</button></td>'
}

function drawNameCell(template) {
    var result = '<td><div class="description">';
    result += '<p style="float:left">' + template.text + ' &nbsp;&nbsp;&nbsp;</p>';
    result += drawButton('Edit', 'dark', 'updateName', template.id, template.timestamp);
    result += '</div></td>';
    return result;
}

function drawDescriptionCell(template) {
    if (template.modelDescription == null || template.modelDescription == '')
        return '<td>' + drawButton('Set', 'dark', 'updateDescription', template.id, template.timestamp) + '</td>';
    else
        return '<td>' + drawButton('Edit', 'dark', 'updateDescription', template.id, template.timestamp) + '</td>';
}

function drawSqlScriptCell(template) {
    if (template.sqlScript == null || template.sqlScript == '')
        return '<td>' + drawButton('Set', 'dark', 'updateSqlScript', template.id, template.timestamp) + '</td>';
    else
        return '<td>' + drawButton('Edit', 'dark', 'updateSqlScript', template.id, template.timestamp) + '</td>';
}

function drawBaseDataCell(template) {
    if (!template.isBaseDataSet)
        return '<td>' + drawButton('Set', 'dark', 'updateSqlScript', template.id, template.timestamp) + '</td>';
    else
        return '<td>' + drawButton('Edit', 'dark', 'updateSqlScript', template.id, template.timestamp) + '</td>';
}

function drawDeleteCell(template) {
    return '<td>' + drawButton('Delete', 'danger', 'deleteTemplate', template.id, template.timestamp) + '</td>';
}


//ACTION HANDLERS
function updateName(id, timestamp) {

}

function updateDescription(id, timestamp) {

}

function updateSqlScript(id, timestamp) {

}

function deleteTemplate(id, timestamp) {

}