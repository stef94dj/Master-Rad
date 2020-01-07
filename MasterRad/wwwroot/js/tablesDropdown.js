var tablesDropdownJS = {
    loadTablesDropdownData: function (apiUrl) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: apiUrl,
                type: 'GET',
                success: function (data) {
                    resolve(data);
                },
                error: function (error) {
                    reject(error);
                }
            })
        });
    },
    dropdownSelector: null,
    drawTablesDropdown: function (data) {
        var dropDown = $(this.dropdownSelector);
        dropDown.html('');
        $.each(data, function (index, value) {
            dropDown.append('<option value="' + value + '">' + value + '</option>')
        });
    },
    attachOnChangeHandler: function (handlerFunction) {
        $(this.dropdownSelector).change(handlerFunction);
    }
}