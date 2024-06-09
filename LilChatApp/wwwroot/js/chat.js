"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/livechat").build();

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

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
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