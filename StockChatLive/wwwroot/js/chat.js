"use strict";

function getToken() {
    return localStorage.getItem("jwt_token");
}

function getUsernameFromToken() {
    var token = getToken();
    if (!token) return null;
    try {
        var payload = JSON.parse(atob(token.split('.')[1]));
        return payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || payload.name || "User";
    } catch (e) {
        return "User";
    }
}

if (!getToken()) {
    window.location.href = "/Login";
}

document.getElementById("currentUser").textContent = getUsernameFromToken();

document.getElementById("logoutBtn").addEventListener("click", function() {
    localStorage.removeItem("jwt_token");
    localStorage.removeItem("jwt_expiration");
    window.location.href = "/Login";
});

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/livechat", {
        accessTokenFactory: function() { return getToken(); }
    })
    .build();

document.getElementById("sendButton").disabled = true;

function showError(message) {
    var errorAlert = document.getElementById("errorAlert");
    errorAlert.textContent = message;
    errorAlert.classList.remove("d-none");
    setTimeout(function() {
        errorAlert.classList.add("d-none");
    }, 5000);
}

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
    if (err.message && err.message.includes("401")) {
        localStorage.removeItem("jwt_token");
        window.location.href = "/Login";
        return;
    }
    showError("Failed to connect to chat. Please refresh the page.");
    console.error(err.toString());
});

connection.onclose(function (error) {
    document.getElementById("sendButton").disabled = true;
    if (error) {
        showError("Connection lost. Please refresh the page.");
    }
});

function sendMessage() {
    var message = document.getElementById("messageInput").value.trim();
    
    if (!message) {
        showError("Message cannot be empty.");
        return;
    }
    
    if (message.length > 500) {
        showError("Message too long (max 500 characters).");
        return;
    }
    
    connection.invoke("SendMessage", message).then(function() {
        document.getElementById("messageInput").value = "";
    }).catch(function (err) {
        showError(err.message || "Failed to send message.");
        console.error(err.toString());
    });
}

document.getElementById("sendButton").addEventListener("click", function (event) {
    event.preventDefault();
    sendMessage();
});

document.getElementById("messageInput").addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        event.preventDefault();
        sendMessage();
    }
});

function scrollToBottom() {
    var messagesList = document.getElementById("messagesList");
    var isScrolledToBottom = messagesList.scrollHeight - messagesList.clientHeight <= messagesList.scrollTop + 1;
    if (isScrolledToBottom) {
        messagesList.scrollTop = messagesList.scrollHeight - messagesList.clientHeight;
    }
}