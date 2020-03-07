var signalR_Helper = {
    MessageIDs: {
        Reconnecting: "signalR-message-reconnecting",
        Reconnected: "signalR-message-reconnected",
        Disconnected: "signalR-message-disconnected",
    },
    connect: function (model) {
        var connection = new signalR.HubConnectionBuilder()
            .withUrl(model.HubUrl)
            .withAutomaticReconnect([1000, 3000, 5000, null]) //default is [0, 2000, 10000, 30000, null]
            .configureLogging(signalR.LogLevel.Information)
            .build();

        connection.on(model.Method,
            (data) => {
                setCellStatus(data)
            });

        connection.start()
            .then(_ => connection.invoke(model.HubMethod, model.Group))
            .catch(err => console.error(err.toString()));

        connection.onreconnecting(() => {
            this.hideAllMessages();
            this.showMessage(this.MessageIDs.Reconnecting);
        })

        connection.onreconnected(() => {
            this.hideAllMessages();
            this.showMessage(this.MessageIDs.Reconnected);
            setTimeout(() => this.hideMessage(this.MessageIDs.Reconnected), 3000);
        })

        connection.onclose(() => {
            this.hideAllMessages();
            this.showMessage(this.MessageIDs.Disconnected);
        })
    },
    hideAllMessages: function () {
        this.hideMessage(this.MessageIDs.Reconnecting);
        this.hideMessage(this.MessageIDs.Reconnected);
        this.hideMessage(this.MessageIDs.Disconnected);
    },
    hideMessage: function (id) {
        $(`#${id}`).attr('hidden', 'hidden');
    },
    showMessage: function (id) {
        $(`#${id}`).removeAttr('hidden');
    }
}