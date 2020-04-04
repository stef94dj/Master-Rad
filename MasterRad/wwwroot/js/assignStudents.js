var searchResList = null;
var testId = null;
var testType = null;
var assignedStudents = null;

$(document).ready(function () {
    testId = $('#test-id').val();
    testType = $('#test-type').val();;
    assignedStudents = $('#assigned-students');
    searchResList = $('#student-search-res');
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
function searchStudents() {
    searchResList.html('');;

    var assignedStudents = $('#assigned-students').find('tr');

    var excludeIds = $.map(assignedStudents, function (student, index) {
        return $(student).data('id');
    });

    var rqBody = {
        "FirstName": $('#search-first-name').val(),
        "LastName": $('#search-last-name').val(),
        "Email": $('#search-email').val(),
        "ExcludeIds": excludeIds
    }

    $.ajax({
        url: '/api/Student/search',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(rqBody),
        success: function (data, textStatus, jQxhr) {
            populateStudentSearchResult(data);
        }
    });
}
function populateStudentSearchResult(data) {
    searchResList.html('');

    $.each(data, function (index, student) {
        var optionHtml = '<option';
        optionHtml += ' data-id="' + student.id + '"';
        optionHtml += ' data-first-name="' + student.firstName + '"';
        optionHtml += ' data-last-name="' + student.lastName + '"';
        optionHtml += ' data-email="' + student.email + '">';
        optionHtml += student.firstName + ' ' + student.lastName + ' (' + student.email + ')';
        optionHtml += '</option>';
        searchResList.append(optionHtml);
    });
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