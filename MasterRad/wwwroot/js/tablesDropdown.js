var dropdownTableSelector = '#tableSelector';

function buildTablesDropDown(databaseName, onChangeHandler) {
    populateDropdown(dropdownTableSelector, '/api/Metadata/tables/' + databaseName);
    $(dropdownTableSelector).change(onChangeHandler);
}

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