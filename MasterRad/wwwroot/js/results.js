$(document).ready(function () {
    initializeTooltips();
    connectToSignalR();
});

function initializeTooltips() {
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })
}

function connectToSignalR() {
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/jobprogress")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("progress",
        (data) => {
            evaluationProgressUI.setCellStatus(data.id, data.secret, data.status)
        });

    connection.start()
        .then(_ => connection.invoke("AssociateJob", "123"))
        .catch(err => console.error(err.toString()));
}

function startProgress() {
    $.ajax({
        url: '/api/Evaluate/StartProgress',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({}),
        success: function (data, textStatus, jQxhr) {
            disableButton($('#start-evaluation-btn'));
        }
    });
}

var evaluationProgressUI = {
    setCellStatus: function (studentId, secret, status) {
        var selector = `tr[data-student-id='${studentId}']`;
        if (secret)
            selector += ' td.secret-data-progress';
        else
            selector += ' td.public-data-progress';

        var tableRow = $(selector);

        if (tableRow.length == 1)
            tableRow.html(this.drawEvaluationStatus(status));
    },
    drawEvaluationStatus: function (status) {
        switch (status) {
            case EvaluationStatus.NotSubmited:
                return this.drawStatusIcon("Not submited", ColorCodes.Grey, ColorCodes.White, Shapes.X_Mark, false);
                break;
            case EvaluationStatus.Ready:
                return this.drawStatusIcon("Ready", ColorCodes.White, ColorCodes.Grey, Shapes.Circle, true);
                break;
            case EvaluationStatus.Queued:
                return this.drawStatusIcon("Queued", ColorCodes.Blue, ColorCodes.White, Shapes.Clock, false);
                break;
            case EvaluationStatus.Evaluating:
                return this.drawSpinner("text-warning", "Evaluating...");
                break;
            case EvaluationStatus.Failed:
                return this.drawStatusIcon("Failed", ColorCodes.Red, ColorCodes.White, Shapes.X_Mark, false);
                break;
            case EvaluationStatus.Passed:
                return this.drawStatusIcon("Passed", ColorCodes.Green, ColorCodes.White, Shapes.Checked, false);
                break;
        }
    },
    drawStatusIcon: function (tooltip, circleColor, shapeColor, shape, evenOdd) {
        var result = `<svg height="24" viewBox="0 0 12 12" width="24" data-toggle="tooltip" data-placement="right" title="${tooltip}">`;
        result += `<circle cx="6" cy="6" r="6" fill="${circleColor}"></circle>`;
        result += '<path';
        if (evenOdd)
            result += ' fill-rule="evenodd"';
        result += ` fill="${shapeColor}" d="${shape}"></path>`;
        return result;
    },
    drawSpinner: function (textColor, tooltip) {
        return `<div class="spinner-border ${textColor}" role="status" style="width: 24px; height: 24px; font-size: 10px" data-toggle="tooltip" data-placement="right" title="${tooltip}" />`;
    }
}