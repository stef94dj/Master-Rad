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

function executeSqlScript(dto, callback) {
    $.ajax({
        url: '/api/Query/Execute',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(dto),
        success: function (data, textStatus, jQxhr) {
            if (callback == null || callback == undefined)
                scriptExecuteSuccess(data);
            else
                callback(data);
        }
    });
}

function scriptExecuteSuccess(data) {
    if (data.hasOwnProperty('errors') && data.errors != null && data.errors.length > 0)
        alert("Erorr: " + JSON.stringify(data.errors));
    else {
        alert('ok');
        populateDropdown(dropdownTableSelector, '/api/Metadata/tables/' + $('#db-name').val());
    }
}