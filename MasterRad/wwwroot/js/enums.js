var TestStatus = {
    Scheduled: 1,
    InProgress: 2,
    Completed: 3,
    Graded: 4,
    ToString: function (statusCode) {
        switch (statusCode) {
            case TestStatus.Scheduled:
                return "Scheduled";
            case TestStatus.InProgress:
                return "InProgress";
            case TestStatus.Completed:
                return "Completed";
            case TestStatus.Graded:
                return "Graded";
        }
    },
    ActionName: function (statusCode) {
        switch (statusCode) {
            case TestStatus.Scheduled:
                return "Start";
            case TestStatus.InProgress:
                return "Finish";
            case TestStatus.Completed:
                return "Grade";
        }
    },
    ActionWarningText: function (statusCode) {
        switch (statusCode) {
            case TestStatus.InProgress:
                return "This will enable students start taking the test.";
            case TestStatus.Completed:
                return "This will disable students from submiting new answers.";
            case TestStatus.Graded:
                return "This will start the the process of evaluating answers submited by the student. This may take some time to complete.";
        }
    }
}