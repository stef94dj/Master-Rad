var DBs = {
    DbUids: null, //user interface ids
    TableUids: null,
    NamesOnServer: null,
    TableDropdowns: null,
    TableSelectors: null,
    TableHeaders: null,
    TableBodies: null,
    Initialise: function () {
        var dbUidElements = $('.uid-db');

        this.DbUids = $.map(dbUidElements, function (item, index) {
            return $(item).val()
        });

        this.NamesOnServer = $.map(dbUidElements, function (item, index) {
            return $(item).data('name-on-server');
        });

        this.TableDropdowns = $.map(this.DbUids, function (item, index) {
            return $(`#table-selector-${item}`);
        });

        this.TableSelectors = $.map(this.DbUids, function (item, index) {
            return `#table-selector-${item}`;
        });

        var tableUidElements = $('.uid'); 

        this.TableUids = $.map(tableUidElements, function (item, index) {
            return $(item).val()
        });

        this.TableHeaders = $.map(this.TableUids, function (uid, index) {
            return $(`#table-header-${uid}`);
        });

        this.TableBodies = $.map(this.TableUids, function (uid, index) {
            return $(`#table-body-${uid}`);
        });
    },
    DbUidIndex: function(dbUid) {
        return this.DbUids.indexOf(dbUid);
    },
    NameOnServer(dbUid) {
        return this.NamesOnServer[this.DbUidIndex(dbUid)];
    },
    TableDropdown: function (dbUid) {
        return this.TableDropdowns[this.DbUidIndex(dbUid)];
    },
    TableSelector: function (dbUid) {
        return this.TableSelectors[this.DbUidIndex(dbUid)];
    },
    TbUidIndex: function (tbUid) {
        return this.TableUids.indexOf(tbUid);
    },
    TableHeader: function (tbUid) {
        return this.TableHeaders[this.TbUidIndex(tbUid)];
    },
    TableBody: function (tbUid) {
        return this.TableBodies[this.TbUidIndex(tbUid)];
    }
}

function initialiseModifyDatabasePartial() {
    DBs.Initialise();

    $.each(DBs.DbUids, function (index, uid) {
        tablesDropdownJS.dropdownSelector = DBs.TableSelector(uid);
        tablesDropdownJS.loadTablesDropdownData(`/api/Metadata/tables/${DBs.NameOnServer(uid)}`)
            .then(data => {
                tablesDropdownJS.drawTablesDropdown(data);
            })
            .then(() => {
                tablesDropdownJS.attachOnChangeHandler(tableSelected);
            })
            .then(() => {
                tableSelected();
            })
    });
};

function tableSelected() {
    var uid = $(this).data('uid');
    var tableDD = DBs.TableDropdown(uid);
    var tnforparse = tableDD.val();
    var tableFullName = parseTableName(tnforparse);

    debugger;
    var tbhead = DBs.TableHeader(uid);
    var tbBod = DBs.TableBody(uid);
    renderTable(DBs.NameOnServer(uid), tableFullName.schemaName, tableFullName.tableName, tbhead, tbBod);
}

//table.js when-then
function renderTable(dbNameOnServer, schemaName, tableName, tbHeader, tbBody) {
    var apiUrl = `/api/Data/read/${dbNameOnServer}/${schemaName}/${tableName}`;

    $.ajax({
        url: apiUrl,
        type: 'GET',
        success: function (data) {
            getIdentityColumns(dbNameOnServer, schemaName, tableName, data, tbHeader, tbBody);
        }
    });
}

//table.js when-then
function getIdentityColumns(databaseName, schemaName, tableName, tbData, tbHeader, tbBody) {
    var apiUrl = `/api/MetaData/identity_columns/${databaseName}/${schemaName}/${tableName}`
    $.ajax({
        url: apiUrl,
        type: 'GET',
        success: function (data) {
            drawTable(tbData, data, tbHeader, tbBody);
        }
    });
}

//table.js when-then
function drawTable(tbData, identityColumns, tbHeader, tbBody) {
    tbHeader.html('');
    tbBody.html('');

    //columns
    tbHeader.append('<th scope="col"></th>');
    $.each(tbData.columns, function (index, value) {
        tbHeader.append('<th scope="col" data-sql-type="' + value.sqlType + '">' + value.name + '</th>');
    });

    //rows
    $.each(tbData.rows, function (rowIndex, row) {
        var newRow = '<tr><td><button onclick="deleteRecord(this)" type="button" class="btn btn-danger btn-sm">-</button></td>';
        tbBody.append('<tr>');
        $.each(row, function (cellIndex, cell) {
            if (cell == null)
                cell = 'NULL';
            else
                cell = cell.replace(/"/g, '&quot;').replace(/</g, '&lt;').replace(/'/g, "&#39;");

            newRow += '<td><input ';
            if (identityColumns.includes(tbData.columns[cellIndex].name))
                newRow += 'disabled ';
            newRow += 'onblur="editCell(this)" class="form-control form-control-sm" type="text" data-value-original="' + cell + '"value="' + cell + '"></td>';
        });
        newRow += '</tr>';
        tbBody.append(newRow);
    });

    //final row
    var finalRow = '<tr><td><button onclick="insertRecord(this)" type="button" class="btn btn-info btn-sm">+</button></td>';
    $.each(tbData.columns, function (index, value) {
        finalRow += '<td><input ';
        if (identityColumns.includes(tbData.columns[index].name))
            finalRow += 'disabled ';
        finalRow += 'class="form-control form-control-sm" type="text" value=""></td>';
    });
    finalRow += '</tr>';
    tbBody.append(finalRow);
}

function getNewValue(element) {
    return $(element).val();
}

function getUnmodifiedValue(element) {
    return $(element).data('value-original');
}

function getCell(inputElem, index, getData) {
    var th = $('#table-header th').eq(index + 1);
    return {
        "ColumnName": th.text(),
        "ColumnType": th.data('sql-type'),
        "Value": getData(inputElem)
    };
}

function getInputValues(trElem, getData, includeDisabled) {
    var inputs = $(trElem).find('td input');

    return jQuery.map(inputs, function (item, index) {
        if (includeDisabled || !$(item).prop('disabled'))
            return getCell(item, index, getData);
    });
}

function deleteRecord(btnElem) {
    var trElem = $(btnElem).parents().eq(1);
    var tableFullName = parseTableName($('#table-selector').val());

    var rqBody = {
        "DatabaseName": $('#name-on-server').val(),
        "TableName": tableFullName.tableName,
        "SchemaName": tableFullName.schemaName,
        "ValuesUnmodified": getInputValues(trElem, getUnmodifiedValue, true)
    }

    $.ajax({
        url: '/api/Data/delete',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            tableSelected();
        }
    });
}

function insertRecord(btnElem) {
    var trElem = $(btnElem).parents().eq(1);
    var tableFullName = parseTableName($('#table-selector').val());

    var rqBody = {
        "DatabaseName": $('#name-on-server').val(),
        "TableName": tableFullName.tableName,
        "SchemaName": tableFullName.schemaName,
        "ValuesNew": getInputValues(trElem, getNewValue, false)
    };

    $.ajax({
        url: '/api/Data/insert',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            tableSelected();
        }
    });
}

function editCell(inputElem) {
    var trElem = $(inputElem).parents().eq(1);
    var tableFullName = parseTableName($('#table-selector').val());
    var inputIndex = trElem.find('td').index($(inputElem).parent()) - 1;

    var cellNew = getCell(inputElem, inputIndex, getNewValue);
    var cellUnmodified = getCell(inputElem, inputIndex, getUnmodifiedValue);

    if (cellNew.Value == cellUnmodified.Value)
        return;

    var rqBody = {
        "DatabaseName": $('#name-on-server').val(),
        "TableName": tableFullName.tableName,
        "SchemaName": tableFullName.schemaName,
        "ValuesUnmodified": getInputValues(trElem, getUnmodifiedValue, true),
        "ValueNew": cellNew
    }

    $.ajax({
        url: '/api/Data/update',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            tableSelected();
        }
    });
}