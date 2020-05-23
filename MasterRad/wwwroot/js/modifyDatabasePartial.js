var UI = {
    DbUids: null,
    TableDropdowns: null,
    TableSelectors: null,
    TableUids: null,
    NamesOnServer: null,
    TableNames: null,
    ReadOnlyFlags: null,
    TableHeaders: null,
    TableBodies: null,
    Initialise: function () {
        //edit whole database UIs (1)
        var dbUidElements = $('.uid-db');

        this.DbUids = $.map(dbUidElements, function (item, index) {
            return $(item).val()
        });

        this.TableDropdowns = $.map(this.DbUids, function (item, index) {
            return $(`#table-selector-${item}`);
        });

        this.TableSelectors = $.map(this.DbUids, function (item, index) {
            return `#table-selector-${item}`;
        });

        //edit table UIs - contains 1
        var tableUidElements = $('.uid');

        this.TableUids = $.map(tableUidElements, function (item, index) {
            return $(item).val()
        });

        this.NamesOnServer = $.map(tableUidElements, function (item, index) {
            return $(item).data('name-on-server');
        });

        this.TableNames = $.map(tableUidElements, function (item, index) {
            var tableName = $(item).data('table-name');
            return (tableName != undefined) ? tableName : "";
        });

        this.ReadOnlyFlags = $.map(tableUidElements, function (item, index) {
            var tableName = $(item).data('readonly');
            return (tableName != undefined) ? tableName : false;
        });

        this.TableHeaders = $.map(this.TableUids, function (uid, index) {
            return $(`#table-header-${uid}`);
        });

        this.TableBodies = $.map(this.TableUids, function (uid, index) {
            return $(`#table-body-${uid}`);
        });
    },
    DbUidIndex: function (dbUid) {
        return this.DbUids.indexOf(dbUid);
    },
    NameOnServer(tbUid) {
        return this.NamesOnServer[this.TbUidIndex(tbUid)];
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
    },
    TableName: function (tbUid) {
        return this.TableNames[this.TbUidIndex(tbUid)];
    },
    IsTableOnly: function (uid) {
        return this.DbUidIndex(uid) === -1;
    },
    IsReadOnly: function (uid) {
        return UI.ReadOnlyFlags[this.TbUidIndex(uid)]
    }
}

function initialiseModifyDatabasePartial() {
    UI.Initialise();

    $.each(UI.DbUids, function (index, uid) {
        tablesDropdownJS.dropdownSelector = UI.TableSelector(uid);
        tablesDropdownJS.loadTablesDropdownData(`/api/Metadata/tables/${UI.NameOnServer(uid)}`)
            .then(data => {
                tablesDropdownJS.drawTablesDropdown(data);
            })
            .then(() => {
                tablesDropdownJS.attachOnChangeHandler(tableSelected);
            })
            .then(() => {
                reloadTable(uid);
            })
    });

    var tableOnlyUids = jQuery.map(UI.TableUids, function (tbUid, index) {
        if (!UI.DbUids.includes(tbUid))
            return tbUid;
    });

    $.each(tableOnlyUids, function (index, uid) {
        reloadTable(uid);
    });
};

function reloadTable(uid) {
    if (UI.IsTableOnly(uid))
        refreshSingleTableUI(uid);
    else
        tableSelected(uid);
}

function tableSelected(uid = null) {
    var uidFromClick = $(this).data('uid');
    if (uidFromClick != undefined)
        uid = uidFromClick;

    var tableDD = UI.TableDropdown(uid);
    var tableFullName = parseTableName(tableDD.val());

    var tbhead = UI.TableHeader(uid);
    var tbBody = UI.TableBody(uid);

    renderTable(UI.NameOnServer(uid), tableFullName.schemaName, tableFullName.tableName, tbhead, tbBody, UI.IsReadOnly(uid));
}

function refreshSingleTableUI(uid) {
    var databaseName = UI.NameOnServer(uid);
    var schemaName = "dbo";
    var tableName = UI.TableName(uid);
    var tbHead = UI.TableHeader(uid);
    var tbBody = UI.TableHeader(uid);

    renderTable(databaseName, schemaName, tableName, tbHead, tbBody, UI.IsReadOnly(uid));
}

function renderTable(databaseName, schemaName, tableName, tHead, tBody, readonly) {
    var tableData = null;
    readTable(databaseName, schemaName, tableName)
        .then(data => {
            tableData = data;
            return getIdentityColumns(databaseName, schemaName, tableName);
        })
        .then(identityColumns => {
            drawTable(tableData, identityColumns, tHead, tBody, readonly);
        });
}

function readTable(dbNameOnServer, schemaName, tableName) {
    var apiUrl = `/api/Data/read/${dbNameOnServer}/${schemaName}/${tableName}`;
    return promisifyAjaxGet(apiUrl);
}

function getIdentityColumns(databaseName, schemaName, tableName) {
    var apiUrl = `/api/MetaData/identity_columns/${databaseName}/${schemaName}/${tableName}`
    return promisifyAjaxGet(apiUrl);
}

function drawTable(tbData, identityColumns, tbHeader, tbBody, readonly) {
    tbHeader.html('');
    tbBody.html('');

    //columns
    if (!readonly) {
        tbHeader.append('<th scope="col"></th>');
    }
    
    $.each(tbData.columns, function (index, value) {
        tbHeader.append('<th scope="col" data-sql-type="' + value.sqlType + '">' + value.name + '</th>');
    });

    //rows
    $.each(tbData.rows, function (rowIndex, row) {
        if (!readonly) {
            var newRow = '<tr><td><button onclick="deleteRecord(this)" type="button" class="btn btn-danger btn-sm">-</button></td>';
        }
        
        tbBody.append('<tr>');
        $.each(row, function (cellIndex, cell) {
            if (cell == null)
                cell = 'NULL';
            else
                cell = cell.replace(/"/g, '&quot;').replace(/</g, '&lt;').replace(/'/g, "&#39;");

            newRow += '<td><input ';
            if (identityColumns.includes(tbData.columns[cellIndex].name))
                newRow += 'disabled ';

            var cellOnBlur = readonly ? '' : 'onblur="editCell(this)" ';
            newRow += cellOnBlur + 'class="form-control form-control-sm" type="text" data-value-original="' + cell + '"value="' + cell + '"></td>';
        });
        newRow += '</tr>';
        tbBody.append(newRow);
    });

    if (!readonly) {
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
}

function getNewValue(element) {
    return $(element).val();
}

function getUnmodifiedValue(element) {
    return $(element).data('value-original');
}

function getCell(inputElem, index, getData, uid) {
    var th = $(`#table-header-${uid} th`).eq(index + 1);
    return {
        "ColumnName": th.text(),
        "ColumnType": th.data('sql-type'),
        "Value": getData(inputElem)
    };
}

function getInputValues(trElem, getData, includeDisabled, uid) {
    var inputs = $(trElem).find('td input');

    return jQuery.map(inputs, function (item, index) {
        if (includeDisabled || !$(item).prop('disabled'))
            return getCell(item, index, getData, uid);
    });
}

function getCommonOperationInfo(startingPoint) {
    var trElem = $(startingPoint).parents().eq(1);
    var tbElem = trElem.parents().eq(1);
    var uid = tbElem.data('uid');

    var tableNameForParse = UI.IsTableOnly(uid) ? `dbo.${UI.TableName(uid)}` : UI.TableDropdown(uid).val();
    var tableFullName = parseTableName(tableNameForParse);

    var nameOnServer = UI.NameOnServer(uid);

    var rqBody = {
        "DatabaseName": nameOnServer,
        "TableName": tableFullName.tableName,
        "SchemaName": tableFullName.schemaName,
    };

    var result = { rqBody, trElem, uid };
    return result;
}

function deleteRecord(btnElem) {
    var coi = getCommonOperationInfo(btnElem);

    var rqBody = coi.rqBody;
    rqBody.ValuesUnmodified = getInputValues(coi.trElem, getUnmodifiedValue, true, coi.uid);

    $.ajax({
        url: '/api/Data/delete',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            if (data.isSuccess != true)
                alert('error');
            reloadTable(coi.uid);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('error');
            reloadTable(coi.uid);
        }
    });
}

function insertRecord(btnElem) {
    var coi = getCommonOperationInfo(btnElem);

    var rqBody = coi.rqBody;
    rqBody.ValuesNew = getInputValues(coi.trElem, getNewValue, false, coi.uid);

    $.ajax({
        url: '/api/Data/insert',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            if (data.isSuccess != true)
                alert('error');
            reloadTable(coi.uid);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('error');
            reloadTable(coi.uid);
        }
    });
}

function editCell(inputElem) {
    var coi = getCommonOperationInfo(inputElem);

    var inputIndex = coi.trElem.find('td').index($(inputElem).parent()) - 1
    var cellNew = getCell(inputElem, inputIndex, getNewValue, coi.uid);
    var cellUnmodified = getCell(inputElem, inputIndex, getUnmodifiedValue, coi.uid);
    if (cellNew.Value == cellUnmodified.Value)
        return;

    var rqBody = coi.rqBody;
    rqBody.ValueNew = cellNew;
    rqBody.ValuesUnmodified = getInputValues(coi.trElem, getUnmodifiedValue, true, coi.uid)

    $.ajax({
        url: '/api/Data/update',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            if (data.isSuccess != true)
                alert('error');
            reloadTable(coi.uid);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('error');
            reloadTable(coi.uid);
        }
    });
}