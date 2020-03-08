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
    result += 'onclick="' + handlerName + '(' + id + ')" type="button" class="btn btn-outline-' + color + ' btn-sm center">' + buttonName + '</button>'
    return result;
}

function drawCellEditModalButton(buttonName, color, modalselector, id, timestamp, enabled, hidden) {
    var result = '<button ';

    if (!enabled)
        result += 'disabled ';

    if (hidden)
        result += 'hidden="true" ';

    result += 'data-toggle="modal" data-target="' + modalselector + '" data-id="' + id + '" data-timestamp="' + timestamp + '" type="button" class="btn btn-outline-' + color + ' btn-sm center">' + buttonName + '</button>'
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

function bindModalOnClose(selector, handler) {
    $(selector).on('hidden.bs.modal', function (event) {
        handler(this, event);
        hideModalError(selector);
    });
}

function showModalError(modalSelector, message){
    var errorDiv = findErrorDiv(modalSelector);
    if (errorDiv != null) {
        $(errorDiv).html(message);
        $(errorDiv).show();
    }
}
function hideModalError(modalSelector) {
    var errorDiv = findErrorDiv(modalSelector);
    if (errorDiv != null) {
        $(errorDiv).html('');
        $(errorDiv).hide();
    }
}
function findErrorDiv(modalSelector) {
    var divs = $(modalSelector).find('div.modal-error');
    if (divs == null || divs.length != 1)
        return null;
    else
        return divs[0];
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