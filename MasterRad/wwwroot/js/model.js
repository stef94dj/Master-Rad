var editor = null;
var nameOnServer = null;

$(document).ready(function () {
    nameOnServer = $('#db-name').val();
    buildSqlEditor(nameOnServer, sqlExeCallback);
    buildTablesDropDown(nameOnServer, tableSelected);
});

//Script execution
function executeScript() {
    var rqBody = {
        "SQLQuery": editor.getValue(),
        "DatabaseName": $('#db-name').val()
    };

    executeSqlScript(rqBody, sqlExeCallback);
}
function sqlExeCallback(data) {
    if (data.hasOwnProperty('errors') && data.errors != null && data.errors.length > 0)
        alert("Erorr: " + JSON.stringify(data.errors));
    else {
        alert('ok');
        populateDropdown(dropdownTableSelector, '/api/Metadata/tables/' + $('#db-name').val());
    }
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
