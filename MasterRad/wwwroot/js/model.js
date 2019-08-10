var dropdownTableSelector = '#tableSelector';
var editor = null;

$(document).ready(function () {
    buildSqlEditor($('#db-name').val());
    populateDropdown(dropdownTableSelector, '/api/Metadata/tables/' + $('#db-name').val());
    $(dropdownTableSelector).change(tableSelected);
});

//Script execution
function executeScript() {
    var rqBody = {
        "SQLQuery": editor.getValue(),
        "DatabaseName": $('#db-name').val()
    };

    executeSqlScript(rqBody);
}

//Table info
function tableSelected() {
    var dbName = $('#db-name').val();
    var tableFullName = parseTableName($(this).val());
    loadJson(dbName, tableFullName.schemaName, tableFullName.tableName);
}
function loadJson(dbName, schemaName, tableName) {
    var apiUrl = '/api/Metadata/explore/' + dbName + '/' + schemaName + '/' + tableName;

    $.ajax({
        url: apiUrl,
        type: 'GET',
        success: function (data) {
            displayJson(data);
        }
    });
}
function displayJson(jsonData) {
    var input = eval('(' + JSON.stringify(jsonData) + ')');
    $('#json-display').jsonViewer(input, {
        collapsed: true,
        rootCollapsable: false
    });
}
