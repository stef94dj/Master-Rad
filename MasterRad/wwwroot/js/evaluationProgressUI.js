var evaluationProgressUI = {
    drawEvaluationStatus: function (status) {
        switch (status) {
            case EvaluationStatus.NotSubmited:
                return this.drawStatusIcon("Not submited", ColorCodes.Grey, ColorCodes.White, Shapes.X_Mark, false, status);
                break;
            case EvaluationStatus.NotEvaluated:
                return this.drawStatusIcon("Not evaluated", ColorCodes.White, ColorCodes.Grey, Shapes.Circle, true, status);
                break;
            case EvaluationStatus.Queued:
                return this.drawStatusIcon("Queued", ColorCodes.Blue, ColorCodes.White, Shapes.Clock, false, status);
                break;
            case EvaluationStatus.Evaluating:
                return this.drawSpinner("text-warning", "Evaluating...", status);
                break;
            case EvaluationStatus.Failed:
                return this.drawStatusIcon("Failed", ColorCodes.Red, ColorCodes.White, Shapes.X_Mark, false, status);
                break;
            case EvaluationStatus.Passed:
                return this.drawStatusIcon("Passed", ColorCodes.Green, ColorCodes.White, Shapes.Checked, false, status);
                break;
        }
    },
    drawStatusIcon: function (tooltip, circleColor, shapeColor, shape, evenOdd, status) {
        var result = `<div data-status="${status}" class="test-cell">`;
        result += `<svg height="24" viewBox="0 0 12 12" width="24" data-toggle="tooltip" data-placement="right" title="${tooltip}">`;
        result += `<circle cx="6" cy="6" r="6" fill="${circleColor}"></circle>`;
        result += '<path';
        if (evenOdd)
            result += ' fill-rule="evenodd"';
        result += ` fill="${shapeColor}" d="${shape}"></path>`;
        result += '</div>';
        return result;
    },
    drawSpinner: function (textColor, tooltip, status) {
        return `<div data-status=${status} class="test-cell spinner-border ${textColor}" role="status" style="width: 24px; height: 24px; font-size: 10px" data-toggle="tooltip" data-placement="right" title="${tooltip}" />`;
    }
}