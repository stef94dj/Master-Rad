var connection = new signalR.HubConnectionBuilder()
    .withUrl("/jobprogress")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("progress",
    (percent) => {
        if (percent === 100) {
            document.getElementById("job-status").innerText = "Finished!";
        } else {
            document.getElementById("job-status").innerText = `${percent}%`;
        }
    });

connection.start()
    .then(_ => connection.invoke("AssociateJob", "123"))
    .catch(err => console.error(err.toString()));

function startProgress() {
    $.ajax({
        url: '/api/Evaluate/StartProgress',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({}),
        success: function (data, textStatus, jQxhr) {
            alert('started');
        }
    });
}