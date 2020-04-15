var searchResList = null;
var testId = null;
var testType = null;
var assignedStudents = null;
var assignBtn = null;
var pagingUl = null;
var assignedStudentsHeader = null;

$(document).ready(function () {
    testId = $('#test-id').val();
    testType = $('#test-type').val();;
    assignedStudents = $('#assigned-students');
    searchResList = $('#student-search-res');
    assignBtn = $('#assign-btn');
    pagingUl = $('#paging-ul');
    assignedStudentsHeader = $('#assigned-students-header');

    //searchResList.on('change', studentSelection);
    initTooltips();
    searchStudents();
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
            disableCbxForAssigned();
            setAssignedCount();
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
            disableAssignBtn();
            disableCbxForAssigned();
        })
        .catch(error => {
            handleMsGraphAjaxError(error);
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
function populateStudentSearchResult(data) {
    displaySearchResMessage('No data.');

    if (data != null && data.students != null && data.students.length > 0) {
        searchResList.html('');
        $.each(data.students, function (index, student) {
            var trHtml = `<tr data-ms-id="${student.microsoftId}">`;
            trHtml += `<td>${student.firstName == null ? "" : student.firstName}</td>`;
            trHtml += `<td>${student.lastName == null ? "" : student.lastName}</td>`;
            trHtml += `<td>${student.email == null ? "" : student.email}</td>`;
            trHtml += '<td><div><input type="checkbox" style="transform:scale(1.5)" onchange="onMarkCbxChange(this)"></div></td>';
            trHtml += '</tr>';
            searchResList.append(trHtml);
        });
    }
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
function displaySearchResMessage(message) {
    searchResList.html(drawTableMessage(message, 4));
}

//page click
function pageClick(pageBtn) {
    pageBtn = $(pageBtn);
    var allPageBtns = pagingUl.find('a');
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
                    afterRenderSearchResUI(pageNo);
                })
                .catch(error => {
                    handleMsGraphAjaxError(error);
                })
        }
        else {
            getAADPage(pageUrl)
                .then(data => {
                    populateStudentSearchResult(data);
                    if (isLastPage && data != null && data.nextPageUrl) {
                        var newBtnHtml = drawPageBtn(pageNo + 1, false, data.nextPageUrl);
                        pagingUl.html(pagingUl.html() + newBtnHtml);

                    }
                    afterRenderSearchResUI(pageNo);
                })
                .catch(error => {
                    handleMsGraphAjaxError(error);
                })
        }
    }
}
function afterRenderSearchResUI(currentPageBtnNo) {
    var allPageBtns = pagingUl.find('a');
    var pageBtn = $.map(allPageBtns, function (item, index) {
        var pageNo = parseInt($(item).html());
        if (currentPageBtnNo == pageNo)
            return $(item)
    })[0];

    updateCurrentPageBtn(allPageBtns, pageBtn);
    updateVisiblePageBtns(allPageBtns, pageBtn);
    disableAssignBtn();
    disableCbxForAssigned();
}
function getAADPage(pageUrl) {
    var rqBody = {
        "PageUrl": pageUrl
    }

    return promisifyAjaxPost('/api/Student/azure/page', rqBody);
}
function updateCurrentPageBtn(allPageBtns, pageBtn) {
    $.each(allPageBtns, function (index, item) {
        $(item).removeClass('current');
    });
    pageBtn.addClass('current');
}
function updateVisiblePageBtns(allPageBtns, pageBtn, toDisplay) {
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
function handleMsGraphAjaxError(error) {
    if (error?.status && error.status === HttpCodes.ReloadRequired) {
        displaySearchResMessage('Reloading...');
        location.reload();
    }
    else {
        displaySearchResMessage('Error loading data...');
    }
}

//marked and assigned
function getSearchedRows() {
    return searchResList.find('tr');
}
function getMarkedRows() {
    var searched = getSearchedRows();
    var marked = $.map(searched, function (item, index) {
        var cbx = $(item).find('input:checkbox')[0];
        if ($(cbx).is(':checked'))
            return item;
    });

    return marked;
}
function getAssignedRows() {
    return assignedStudents.find('tr');
}
function onMarkCbxChange() {
    var markedCnt = getMarkedRows().length;
    if (markedCnt > 0)
        enableAssignBtn(markedCnt);
    else
        disableAssignBtn();
}
function enableAssignBtn(markedCnt) {
    setAssignBtnLabel(markedCnt);
    $(assignBtn).attr('disabled', false);
}
function disableAssignBtn() {
    setAssignBtnLabel(0);
    $(assignBtn).attr('disabled', true);
}
function setAssignBtnLabel(markedCnt) {
    $(assignBtn).html(`Assign marked (${markedCnt})`);
}

function disableCbxForAssigned() {
    var searched = getSearchedRows();
    $.each(searched, function (index, item) {
        enableSearchResult(item);
    });

    var assigned = getAssignedRows();
    var assignedMsIds = $.map(assigned, function (item, index) {
        return $(item).data('ms-id');
    });

    $.each(searched, function (index, item) {
        var msid = $(item).data('ms-id');
        if (assignedMsIds.includes(msid))
            disableSearchResult(item);
    });

    initTooltips();
}
function enableSearchResult(tr) {
    $(tr).attr('style', 'color:black');
    cbx = $(tr).find('input:checkbox')[0];
    cbx = $(cbx);
    cbx.attr('disabled', false);
}
function disableSearchResult(tr) {
    $(tr).attr('style', 'color:grey');
    cbx = $(tr).find('input:checkbox')[0];
    cbx = $(cbx);
    cbx.attr('disabled', true);

    if (cbx.is(':checked'))
        cbx.prop('checked', false);
}
function setAssignedCount() {
    debugger;
    var assignedCnt = assignedStudents.find('tr').length;
    var text = `Assigned students (${assignedCnt})`;
    assignedStudentsHeader.html(text);
}

function assign() {
    var markedRows = getMarkedRows();
    var markedIds = $.map(markedRows, function (item, index) {
        return $(item).data('ms-id')
    });

    var rqBody = {
        "TestType": testType,
        "TestId": testId,
        "StudentIds": markedIds
    }

    $.ajax({
        url: '/api/Student/assign',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            loadAssignedStudents();
            disableAssignBtn();
        }
    });
}
function renderAssignedListItem(msid, timestamp, email, firstName, lastName) {
    var result = '<tr data-ms-id="' + msid + '">'

    result += '<td>' + firstName + '</td>';
    result += '<td>' + lastName + '</td>';
    result += '<td>' + email + '</td>';
    result += '<td><button onclick="removeStudent(' + msid + ',' + "'" + timestamp + "'" + ')" type="button" class="btn btn-outline-danger btn-sm">Remove</button></td>';
    result += '</tr>';

    return result;
}
function displayAssignedStudents(data) {
    assignedStudents.html('');

    $.each(data, function (index, sts) {
        var studentRow = renderAssignedListItem(sts.studentId, sts.timeStamp, sts.email, sts.firstName, sts.lastName);
        assignedStudents.append(studentRow);
    });
}
function removeStudent(msid, timestamp) {
    var rqBody = {
        "TestType": testType,
        "TestId": testId,
        "StudentId": msid,
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