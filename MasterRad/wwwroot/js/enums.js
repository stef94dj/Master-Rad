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

var TestType = {
    Synthesis: 1,
    Analysis: 2
}

var EvaluationStatus = {
    NotSubmited : 0,
    Ready : 1,
    Queued : 2,
    Evaluating : 3,
    Failed : 4,
    Passed : 5
}

var Shapes = {
    X_Mark: "M3.64 3.64a.75.75 0 0 1 1.06 0l1.294 1.294L7.288 3.64a.75.75 0 0 1 1.06 1.06L7.056 5.994l1.292 1.292a.75.75 0 0 1-1.06 1.06l-1.295-1.29-1.291 1.291a.75.75 0 1 1-1.06-1.06l1.292-1.293L3.64 4.7a.75.75 0 0 1 0-1.06z",
    Checked: "M4.74 8.19l-.002-.002-1.29-1.29a.677.677 0 1 1 .958-.957l.813.812 2.804-2.805a.678.678 0 0 1 .959.958L5.7 8.188l-.002.002a.678.678 0 0 1-.958 0z",
    Clock: "M6 2.6a.75.75 0 0 1 .75.75v2.439L8.122 7.16a.75.75 0 1 1-1.06 1.06L5.487 6.648A.747.747 0 0 1 5.25 6.1V3.35A.75.75 0 0 1 6 2.6z",
    Circle: "M6 12A6 6 0 1 0 6 0a6 6 0 0 0 0 12zm0-1.25a4.75 4.75 0 1 0 0-9.5 4.75 4.75 0 0 0 0 9.5z",
}

var ColorCodes = {
    White: "#fff",
    Grey: "#b2b2b2",
    Blue: "#007bfe",
    Green: "#27a745",
    Red: "#DC3545"
}