var searchResList = null;

$(document).ready(function () {
    searchResList = $('#student-search-res');
    loadAssignedStudents();
});

function loadAssignedStudents() {

}

function searchStudents() {
    searchResList.html('');

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
        var optionHtml = '<option data-id="1">' + student.firstName + ' ' + student.lastName + ' (' + student.email + ')' + '</option>'
        searchResList.append(optionHtml);
    });
}

function assign() {
    //find selected
    //call api to assign
    //on callback:
    //remove from search results
    //add to assigned table
}

function remove() {
    //call api to remove from assigned
    //remove from assigned table
    //add to search result table if maches criteria
}