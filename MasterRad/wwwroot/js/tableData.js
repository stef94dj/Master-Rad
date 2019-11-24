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

function hideTableData() {
    $('#json-display').attr("hidden", true);
}

function showTableData() {
    $('#json-display').removeAttr("hidden");
}