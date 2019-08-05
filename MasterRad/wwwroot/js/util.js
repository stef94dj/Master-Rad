function populateDropdown(selector, apiUrl) {
    $.ajax({
        url: apiUrl,
        type: 'GET',
        success: function (data) {
            $('#tableSelector').html('');
            $.each(data, function (index, value) {
                $(selector).append('<option value="' + value + '">' + value + '</option>')
            });
        }
    });
}

function parseTableName(fullName) {
    var tableNameArray = fullName.split('.');
    return {
        schemaName: tableNameArray[0],
        tableName: tableNameArray[1]
    }
}