function parseTableName(fullName) {
    var tableNameArray = fullName.split('.');
    return {
        schemaName: tableNameArray[0],
        tableName: tableNameArray[1]
    }
}