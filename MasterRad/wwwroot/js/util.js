function parseTableName(fullName) {
    var tableNameArray = fullName.split('.');
    return {
        schemaName: tableNameArray[0],
        tableName: tableNameArray[1]
    }
}

function drawCellEditNavigationButton(buttonName, color, handlerName, id, enabled) {
    var result = '<button ';
    if (!enabled)
        result += 'disabled ';
    result += 'onclick="' + handlerName + '(' + id + ')" type="button" style="float:right" class="btn btn-outline-' + color + ' btn-sm">' + buttonName + '</button>'
    return result;
}

function drawCellEditModalButton(buttonName, color, modalselector, id, timestamp, enabled) {
    var result = '<button ';
    if (!enabled)
        result += 'disabled ';
    result += 'data-toggle="modal" data-target="' + modalselector + '" data-id="' + id + '" data-timestamp="' + timestamp + '" type="button" style="float:right" class="btn btn-outline-' + color + ' btn-sm">' + buttonName + '</button>'
    return result;
}
