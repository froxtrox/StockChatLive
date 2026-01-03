"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/livechat")
    .withAutomaticReconnect()
    .build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
     var now = new Date();
    var timeString = now.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

    li.textContent = `[${timeString}] ${user} says: ${message}`;


    scrollToBottom();
});

connection.onreconnecting((error) => {
    console.log("Connection lost. Reconnecting...", error);
    document.getElementById("sendButton").disabled = true;
    updateChatConnectionStatus("Reconnecting...", "warning");
});

connection.onreconnected((connectionId) => {
    console.log("Connection reestablished. Connected with connectionId: " + connectionId);
    document.getElementById("sendButton").disabled = false;
    updateChatConnectionStatus("Connected", "success");
});

connection.onclose((error) => {
    console.log("Connection closed.", error);
    document.getElementById("sendButton").disabled = true;
    updateChatConnectionStatus("Disconnected", "danger");
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    updateChatConnectionStatus("Connected", "success");
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});


function scrollToBottom() {
    var messagesList = document.getElementById("messagesList");
    var isScrolledToBottom = messagesList.scrollHeight - messagesList.clientHeight <= messagesList.scrollTop + 1;
    if (isScrolledToBottom) {
        messagesList.scrollTop = messagesList.scrollHeight - messagesList.clientHeight;
    }
}

function updateChatConnectionStatus(message, status) {
    const statusElement = document.getElementById("chatConnectionStatus");
    if (statusElement) {
        statusElement.textContent = message;
        statusElement.className = `alert alert-${status} p-2 text-center`;
    }
}