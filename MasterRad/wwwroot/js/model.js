﻿$(document).ready(function () {
    $('.save-schema').click(function () {
        saveSchema();
    });
    initSQLTextArea('.schema-text');
});

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

function saveSchema() {

    var rqBody = {
        "Id": $('#template-id').val(),
        "SqlScript": $('#sql-script').val(),
        "DbName": $('#db-name').val()
    };

    $.ajax({
        url: '/api/Template/Update/Model',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            if (data.hasOwnProperty('errors') && data.errors != null && data.errors.length > 0)
                alert("Erorr: " + JSON.stringify(data.errors));
            else
                alert('ok');
        }
    });
}