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

function drawTextCell(text, maxChars) {
    if (!text)
        text = "-";

    if (text.length > maxChars) {
        text = text.substring(0, maxChars);
    }

    if (text.length == maxChars) {
        text = text.substring(0, text.length - 3) + '...';
    }

    return `<td><div class="text">${text}</div></td>`;
}

function bindModalOnClose(selector, handler) {
    $(selector).on('hidden.bs.modal', function (event) {
        handler(this, event);
        hideModalError(selector);
    });
}

function showModalError(modal, message) {
    var errorDiv = findErrorDiv(modal);
    if (errorDiv != null) {
        $(errorDiv).html(message);
        $(errorDiv).show();
    }
}

function hideModalError(modal) {
    var errorDiv = findErrorDiv(modal);
    if (errorDiv != null) {
        $(errorDiv).html('');
        $(errorDiv).hide();
    }
}

function findErrorDiv(modal) {
    var divs = $(modal).find('div.modal-error');
    if (divs == null || divs.length != 1)
        return null;
    else
        return divs[0];
}

function handleModalAjaxSuccess(modalSelector, data, reloadFunc) {
    var modal = $(modalSelector);
    hideModalError(modal);
    if (data != null && data.isSuccess) {
        modal.modal('toggle');
        modal.find('.modal-body').find(modalSelector).val('');
    }
    else if (data.errors != null && data.errors.length > 0) {
        showModalError(modal, data.errors[0]);
    }

    if (reloadFunc != null)
        reloadFunc();
}
function handleModalAjaxError(modalSelector, reloadFunc) {
    showModalError(modalSelector, 'Unexpected error occured.');
    if (reloadFunc != null)
        reloadFunc();
}

function defineNameHoverBehaviour(collection) {
    $.each(collection, function (index, item) {
        $(item).hover(
            function () { //on hover in
                textButtonSwap(this, false);
            },
            function () { //on hover out
                textButtonSwap(this, true);
            })
    });
}

function textButtonSwap(td, showText) {
    var p = $(td).find('div.text')[0];
    var btn = $(td).find('button')[0];

    $(p).attr('hidden', !showText);
    $(btn).attr('hidden', showText);
}

function disableButton(button) {
    if (button.length == 1)
        $(button).attr('disabled', 'disabled').addClass('disabled');
}

function enableButton(button) {
    if (button.length == 1)
        $(button).removeAttr('disabled').removeClass('disabled');
}

function promisifyAjaxPost(apiUrl, rqBody) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: apiUrl,
            dataType: 'json',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(rqBody),
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error);
            }
        })
    });
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

function toLocaleDateTimeString(serverUtcDateString) {
    try {
        var dateStringArray = serverUtcDateString.split('-');
        var dateNoArray = $.map(dateStringArray, function (item, index) {
            return parseInt(item)
        });
        var utcDate = Date.UTC(dateNoArray[0], dateNoArray[1], dateNoArray[2], dateNoArray[3], dateNoArray[4], dateNoArray[5], 0);
        var dateObj = new Date(utcDate);
        return dateObj.toLocaleDateString() + " " + dateObj.toLocaleTimeString();
    }
    catch (err) {
        return "error"
    }
}

function drawAuthorCellUtil(firstName, lastName, email) {
    var cellContent = `${firstName} ${lastName}<br/>${email}`;
    return `<td><div class="text">${cellContent}</div></td>`
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