var editor = null;
var nameOnServer = null;

$(document).ready(function () {
    nameOnServer = $('#db-name').val();
    buildSqlEditor(nameOnServer, sqlExeCallback);
    buildTablesDropDown(nameOnServer, tableSelected, null);
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

