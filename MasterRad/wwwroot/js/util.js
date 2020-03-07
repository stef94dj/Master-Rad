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


function initializeTooltips() {
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })
}

function drawTableMessage(message, colspan) {
    return '<tr><td align="center" colspan="' + colspan + '">' + message + '</td></tr>';
}

function bindModalOnShow(selector, handler) {
    $(selector).on('show.bs.modal', function (event) {
        handler(this, event);
    })
}

function disableButton(button) {
    if (button.length == 1)
        $(button).attr('disabled', 'disabled').addClass('disabled');
}

function enableButton(button) {
    if (button.length == 1)
        $(button).removeAttr('disabled').removeClass('disabled');
}

function promisifyAjaxGet(apiUrl) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: apiUrl,
            type: 'GET',
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        })
    });
}

var progressReader = {
    //sunthesis
    PublicData: function (testStudentDto) {
        return testStudentDto.evaluationProgress.find(function (progress) {
            return !progress.isSecretDataUsed
        }).progress;
    },
    SecretData: function (testStudentDto) {
        return testStudentDto.evaluationProgress.find(function (progress) {
            return progress.isSecretDataUsed
        }).progress;
    },
    //analysis
    PrepareData: function (testStudentDto) {
        return testStudentDto.evaluationProgress.find(function (progress) {
            return progress.type == AnalysisEvaluationType.PrepareData;
        }).progress;
    },
    FailingInput: function (testStudentDto) {
        return testStudentDto.evaluationProgress.find(function (progress) {
            return progress.type == AnalysisEvaluationType.FailingInput;
        }).progress;
    },
    QueryOutput: function (testStudentDto) {
        return testStudentDto.evaluationProgress.find(function (progress) {
            return progress.type == AnalysisEvaluationType.QueryOutput;
        }).progress;
    },
    CorrectOutput: function (testStudentDto) {
        return testStudentDto.evaluationProgress.find(function (progress) {
            return progress.type == AnalysisEvaluationType.CorrectOutput;
        }).progress;
    },
}