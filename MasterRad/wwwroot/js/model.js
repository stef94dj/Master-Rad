﻿var dropdownTableSelector = '#tableSelector';

$(document).ready(function () {
    initSQLTextArea('.schema-text');
    populateDropdown(dropdownTableSelector, '/api/Metadata/tables/' + $('#db-name').val());
    $(dropdownTableSelector).change(tableSelected);
});

//Script execution
function executeScript() {
    var rqBody = {
        "SQLQuery": $('#sql-script').val(),
        "DatabaseName": $('#db-name').val()
    };

    executeSqlScript(rqBody);
}

//Table info
function tableSelected() {
    var dbName = $('#db-name').val();
    var tableFullName = parseTableName($(this).val());
    loadJson(dbName, tableFullName.schemaName, tableFullName.tableName);
}
function loadJson(dbName, schemaName, tableName) {
    var apiUrl = '/api/Metadata/explore/' + dbName + '/' + schemaName + '/' + tableName;

    $.ajax({
        url: apiUrl,
        type: 'GET',
        success: function (data) {
            displayJson(data);
        }
    });
}
function displayJson(jsonData) {
    var input = eval('(' + JSON.stringify(jsonData) + ')');
    $('#json-display').jsonViewer(input, {
        collapsed: true,
        rootCollapsable: false
    });
}

//Text-area
function initSQLTextArea(selector) {
    $(selector).keydown(keyHandler);
}
function keyHandler(e) {
    var TABKEY = 9;
    if (e.keyCode == TABKEY) {
        //this.value += "    ";
        insertAtCursor(this, "    ")
        if (e.preventDefault) {
            e.preventDefault();
        }
        return false;
    }
}
function insertAtCursor(myField, myValue) {
    //IE support
    if (document.selection) {
        myField.focus();
        sel = document.selection.createRange();
        sel.text = myValue;
    }
    //MOZILLA and others
    else if (myField.selectionStart || myField.selectionStart == '0') {
        var startPos = myField.selectionStart;
        var endPos = myField.selectionEnd;
        myField.value = myField.value.substring(0, startPos)
            + myValue
            + myField.value.substring(endPos, myField.value.length);
        myField.selectionStart = startPos + myValue.length;
        myField.selectionEnd = startPos + myValue.length;
    } else {
        myField.value += myValue;
    }
}