var dropdownTableSelector = '#tableSelector';

function buildTablesDropDown(databaseName, onChangeHandler, callback) {
    $(dropdownTableSelector).change(onChangeHandler);
    populateDropdown(dropdownTableSelector, '/api/Metadata/tables/' + databaseName, callback);
}

function populateDropdown(selector, apiUrl, callback) {
    $.ajax({
        url: apiUrl,
        type: 'GET',
        success: function (data) {
            $('#tableSelector').html('');
            $.each(data, function (index, value) {
                $(selector).append('<option value="' + value + '">' + value + '</option>')
            });
            if (callback != null)
                callback();
        }
    });
}