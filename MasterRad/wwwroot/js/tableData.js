function loadJson(dbName, schemaName, tableName) {
    getJson(dbName, schemaName, tableName)
        .then(data => {
            displayJson(data);
        })
}

function getJson(dbName, schemaName, tableName) {
    var apiUrl = '/api/Metadata/explore/' + dbName + '/' + schemaName + '/' + tableName;
    return promisifyAjaxGet(apiUrl)
}

function displayJson(jsonData) {
    var input = eval('(' + JSON.stringify(jsonData) + ')');

    input = mapToDisplay(input);

    $('#json-display').jsonViewer(input, {
        collapsed: true,
        rootCollapsable: false,
        hideCommas: true,
        hideCurlyBraces: true,
        noItemsMessage: '0 items'
    });
}

function mapToDisplay(inputJson) {
    var res = {
        columns: {},
        //constraints: {}
    };

    if (inputJson == null)
        return res;

    if (inputJson.columns != null) {
        $.each(inputJson.columns, function (index, item) {
            var colObj = {
                type: item.type,
                isNullable: item.isNullable,
                defaultValue: item.defaultValue,
                maxLength: item.maxLength
            };
            res.columns[item.name] = colObj;
        });
    }

    if (inputJson.constraints != null) {
        $.each(inputJson.constraints, function (index, item) {
            var consObj = {
                type: item.type,
                description: item.description,
            };
            //res.constraints[item.name] = consObj;
        });
    }

    return res;
}

function hideTableData() {
    $('#json-display').attr("hidden", true);
}

function showTableData() {
    $('#json-display').removeAttr("hidden");
}

function clearTableData() {
    $('#json-display').html('');
}