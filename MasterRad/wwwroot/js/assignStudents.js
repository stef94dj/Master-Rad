var searchResList = null;
var testId = null;
var testType = null;
var assignedStudents = null;
var pagingUl = null;

$(document).ready(function () {
    testId = $('#test-id').val();
    testType = $('#test-type').val();;
    assignedStudents = $('#assigned-students');
    searchResList = $('#student-search-res');
    pagingUl = $('#paging-ul');

    searchResList.on('change', studentSelection);
    initTooltips();
    loadAssignedStudents();
});

function initTooltips() {
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })

}

function loadAssignedStudents() {
    assignedStudents.html(drawTableMessage('Loading data...', 4));

    var apiUrl = '/api/Student/get/assigned/' + testType + '/' + testId;
    $.ajax({
        url: apiUrl,
        type: 'GET',
        success: function (data) {
            displayAssignedStudents(data);
        }
    });
}

//Azure AD
function searchStudents() {
    displaySearchResMessage('Loading data.');
    searchAAD()
        .then(data => {
            populateStudentSearchResult(data);
            drawInitialPages(data);
        })
        .catch(() => {
            displaySearchResMessage('Error loading data.');
        })
}
function searchAAD() {
    var rqBody = {
        "FirstNameStartsWith": $('#search-first-name').val(),
        "LastNameStartsWith": $('#search-last-name').val(),
        "EmailStartsWith": $('#search-email').val(),
        "PageSize": parseInt($('#page-size').val())
    }

    return promisifyAjaxPost('/api/Student/azure/search', rqBody);
}
function getAADPage(pageUrl) {
    var rqBody = {
        "PageUrl": pageUrl
    }

    return promisifyAjaxPost('/api/Student/azure/page', rqBody);
}
function populateStudentSearchResult(data) {
    displaySearchResMessage('No data.');

    if (data != null && data.students != null && data.students.length > 0) {
        searchResList.html('');
        $.each(data.students, function (index, student) {
            var trHtml = `<tr data-ms-id="${student.microsoftId}">`;
            trHtml += `<td>${student.firstName == null ? "" : student.firstName}</td>`;
            trHtml += `<td>${student.lastName == null ? "" : student.lastName}</td>`;
            trHtml += `<td>${student.email == null ? "" : student.email}</td>`;
            trHtml += '<td><input type="checkbox" style="transform:scale(1.5)"></td>';
            trHtml += '</tr>';
            searchResList.append(trHtml);
        });
    }
}
function displaySearchResMessage(message) {
    searchResList.html(drawTableMessage(message, 4));
}

function drawInitialPages(data) {
    var newHtml = '';

    if (data != null && data.students != null) {
        newHtml += drawPageBtn(1, true, null);;
        if (data.nextPageUrl != null)
            newHtml += drawPageBtn(2, false, data.nextPageUrl);
    }

    pagingUl.html(newHtml);
}
function drawPageBtn(number, isCurrent, pageUrl) {
    var pageUrlAttr = pageUrl ? `data-url="${pageUrl}"` : ``;
    var currentClass = isCurrent ? "current" : "";

    var pagehtml = '<li class="page-item">';
    pagehtml += `<a class="page-link ${currentClass}" href="javascript:void(0)" onclick="pageClick(this)" ${pageUrlAttr}>${number}</a>`;
    pagehtml += '</li>';
    return pagehtml;
}

function pageClick(pageBtn) {
    pageBtn = $(pageBtn);
    var allPageBtns = pageBtn.parent().parent().find('a');
    var allPageNos = $.map(allPageBtns, function (item, index) {
        return parseInt($(item).html())
    });
    var pageNo = parseInt(pageBtn.html());
    var pageUrl = pageBtn.data('url');

    var isCurrent = pageBtn.hasClass('current');
    var isFirstPage = pageNo == allPageNos[0];
    var isLastPage = pageNo == allPageNos[allPageNos.length - 1];

    if (!isCurrent && !(isFirstPage && isLastPage)) {
        if (isFirstPage) {
            searchAAD()
                .then(data => {
                    populateStudentSearchResult(data);
                })
        }
        else {
            getAADPage(pageUrl)
                .then(data => {
                    populateStudentSearchResult(data)
                    if (isLastPage && data != null && data.nextPageUrl) {
                        var newBtnHtml = drawPageBtn(pageNo + 1, false, data.nextPageUrl);
                        pagingUl.html(pagingUl.html() + newBtnHtml);
                    }
                })
        }

        updateCurrentPageBtn(allPageBtns, pageBtn);
        updateVisiblePageBtns(allPageBtns, pageBtn);
    }
}

function updateCurrentPageBtn(allPageBtns, pageBtn) {
    $.each(allPageBtns, function (index, item) {
        $(item).removeClass('current');
    });
    pageBtn.addClass('current');
}

function updateVisiblePageBtns(allPageBtns, pageBtn) {
    //hide all
    $.each(allPageBtns, function (index, item) {
        hidePageBtn($(item));
    });

    //display current
    showPageBtn(pageBtn);

    //display left & right of current
    var currentIndex = allPageBtns.index(pageBtn);
    var leftIndex = currentIndex - 1;
    var rightIndex = currentIndex + 1;

    var toDisplay = 10;
    while (toDisplay > 0) {
        var leftInBounds = leftIndex >= 0;
        var rightInBounds = rightIndex <= (allPageBtns.length - 1);

        if (!leftInBounds && !rightInBounds)
            break;

        if (leftInBounds) {
            showPageBtn(allPageBtns[leftIndex]);
            toDisplay--;
            leftIndex--;
        }

        if (rightInBounds) {
            showPageBtn(allPageBtns[rightIndex]);
            toDisplay--;
            rightIndex++;
        }
    }
}
function showPageBtn(pageBtn) {
    $(pageBtn).parent().attr('hidden', false);
}
function hidePageBtn(pageBtn) {
    $(pageBtn).parent().attr('hidden', true);
}

function assign() {
    var selectedStudents = searchResList.find('option:selected');

    var selectedStudentIds = $.map(selectedStudents, function (student, index) {
        return $(student).data('id');
    });

    var rqBody = {
        "TestType": testType,
        "TestId": testId,
        "StudentIds": selectedStudentIds
    }

    $.ajax({
        url: '/api/Student/assign',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            loadAssignedStudents();
            selectedStudents.remove();
        }
    });
}
function renderAssignedListItem(id, timestamp, email, firstName, lastName) {
    var result = '<tr data-id="' + id + '">'

    result += '<td>' + firstName + '</td>';
    result += '<td>' + lastName + '</td>';
    result += '<td>' + email + '</td>';
    result += '<td><button onclick="removeStudent(' + id + ',' + "'" + timestamp + "'" + ')" type="button" class="btn btn-outline-danger btn-sm">Remove</button></td>';
    result += '</tr>';

    return result;
}
function displayAssignedStudents(data) {
    assignedStudents.html('');

    $.each(data, function (index, sts) {
        var studentRow = renderAssignedListItem(sts.studentId, sts.timeStamp, sts.student.email, sts.student.firstName, sts.student.lastName);
        assignedStudents.append(studentRow);
    });
}
function removeFromSearchResultsList(students) {
    searchResList.remove(students);
}
function removeStudent(studentId, timestamp) {
    var rqBody = {
        "TestType": testType,
        "TestId": testId,
        "StudentId": studentId,
        "TimeStamp": timestamp
    };

    $.ajax({
        url: '/api/Student/remove/assigned',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            loadAssignedStudents();
        }
    });
}
function studentSelection() {
    var selectedStudents = searchResList.find('option:selected');
    var assignBtn = $('#assign-btn');
    if (selectedStudents.length > 0)
        assignBtn.removeAttr('disabled');
    else
        assignBtn.attr('disabled', true);
}