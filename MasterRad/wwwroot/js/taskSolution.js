$(document).ready(function () {

});

function executeScript() {

    var rqBody = {
        "Id": $('#task-id').val(),
        "TimeStamp": $('#task-timestamp').val(),
        "SqlScript": $('#sql-script').val(),
        "DbName": $('#name-on-server').val()
    };

    $.ajax({
        url: '/api/Template/Update/Solution',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            if (data.hasOwnProperty('errors') && data.errors != null && data.errors.length > 0)
                alert("Erorr: " + JSON.stringify(data.errors));
            else {
                alert('ok');
            }
        }
    });
}