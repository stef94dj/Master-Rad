

//$(document).ready(function () {
//    var myTextArea = $('#code')[0];
//    var sqlEditor = CodeMirror.fromTextArea(myTextArea);
 
//})

function clearSQL(){
    window.editor.setValue("");
}
function setQueryValue(imeTabele) {
    debugger;
    window.editor.setValue("select * from [" + imeTabele + "]");
}

function executeSQL() {
    debugger;
    var qry = window.editor.getValue();
    if ("SELECT" == qry.substring(0, 6) || "select" == qry.substring(0, 6)) {
        executeSelectSQL(qry);
    }
    else {
        executeEditSQL(qry);
    }

}

function executeSelectSQL(qry) {
    debugger;

    $('#ServerOutputID').val("");
    depopulateQryResponseTable();

    $('#loadScreenID').show();

    var dataForController = {
        QueryText: qry
        
    };

    $.ajax({
        type: 'POST',
        url: '/Student/ExecuteSelectQuery',
        dataType: 'json',
        data: dataForController,
        error: function () {
            debugger;
            alert("greska")
        },
        success: function (response) {
            debugger;
            $("#ServerOutputID").val(response.message);
            populateQryResponseTable(response.table);
            $('#loadScreenID').hide();
        }
    });

}

function populateQryResponseTable(table) {
 
    $.each(table.NamesOfCollumns, function (i, colName) {
        var $tr = $('#qryResTblHeadRowId').append(
            $('<th>').text(colName)
        );
    });

    $.each(table.Rows, function (i, row) {
        $('#qryResTblBodyId').append($('<tr id="qryResTblBodyRow' + i + 'Id">'));
        $.each(table.Rows[i], function (j, field) {
            $('#qryResTblBodyRow' + i + 'Id').append($('<td>').text(field));
        });
    });

    $("#dataTableRuntime").DataTable();
}

function depopulateQryResponseTable(){
    $('#qryResTblHeadRowId').empty();
    $('#qryResTblBodyId').empty();
}

function executeEditSQL(qry) {
    debugger;

    $('#ServerOutputID').val("");
    depopulateQryResponseTable();

    $('#loadScreenID').show();

    var dataForController = {
        QueryText: qry
    };

    $.ajax({
        type: 'POST',
        url: '/Student/ExecuteEditQuery',
        dataType: 'json',
        data: dataForController,
        error: function () {
            debugger;
            alert("greska")
        },
        success: function (response) {
            debugger;
            $("#ServerOutputID").val(response.message);
            $('#loadScreenID').hide();
        }
    });

}
