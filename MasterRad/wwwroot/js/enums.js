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
    }
}