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

// Cache DOM elements
var currentUserElement = document.getElementById("currentUser");
var logoutBtn = document.getElementById("logoutBtn");
var sendButton = document.getElementById("sendButton");
var messageInput = document.getElementById("messageInput");
var messagesList = document.getElementById("messagesList");
var errorAlert = document.getElementById("errorAlert");

currentUserElement.textContent = getUsernameFromToken();

logoutBtn.addEventListener("click", function() {
    localStorage.removeItem("jwt_token");
    localStorage.removeItem("jwt_expiration");
    window.location.href = "/Login";
});

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/livechat", {
        accessTokenFactory: function() { return getToken(); }
    })
    .withAutomaticReconnect()
    .build();

sendButton.disabled = true;

function showError(message) {
    errorAlert.textContent = message;
    errorAlert.classList.remove("d-none");
    setTimeout(function() {
        errorAlert.classList.add("d-none");
    }, 5000);
}

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    messagesList.appendChild(li);
    var now = new Date();
    var timeString = now.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    li.textContent = `[${timeString}] ${user} says: ${message}`;
    scrollToBottom();
});

connection.onreconnecting((error) => {
    console.log("Connection lost. Reconnecting...", error);
    sendButton.disabled = true;
    updateChatConnectionStatus("Reconnecting to live chat...", "warning");
});

connection.onreconnected((connectionId) => {
    console.log("Connection reestablished. Connected with connectionId: " + connectionId);
    sendButton.disabled = false;
    updateChatConnectionStatus("Connected to live chat", "success");
});

connection.onclose((error) => {
    console.log("Connection closed.", error);
    sendButton.disabled = true;
    updateChatConnectionStatus("Disconnected", "danger");
    if (error) {
        showError("Connection lost. Please refresh the page.");
    }
});

connection.start().then(function () {
    sendButton.disabled = false;
    updateChatConnectionStatus("Connected to live chat", "success");
}).catch(function (err) {
    if (err.message && err.message.includes("401")) {
        localStorage.removeItem("jwt_token");
        window.location.href = "/Login";
        return;
    }
    showError("Failed to connect to live chat. Please refresh the page.");
    console.error(err.toString());
});

function sendMessage() {
    var message = messageInput.value.trim();

    if (!message) {
        showError("Message cannot be empty.");
        return;
    }

    if (message.length > 500) {
        showError("Message too long (max 500 characters).");
        return;
    }

    // Disable send button during message send
    sendButton.disabled = true;

    connection.invoke("SendMessage", message).then(function() {
        messageInput.value = "";
        sendButton.disabled = false;
    }).catch(function (err) {
        showError(err.message || "Failed to send message.");
        console.error(err.toString());
        sendButton.disabled = false;
    });
}

sendButton.addEventListener("click", function (event) {
    event.preventDefault();
    sendMessage();
});

messageInput.addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        event.preventDefault();
        sendMessage();
    }
});

function scrollToBottom() {
    var isScrolledToBottom = messagesList.scrollHeight - messagesList.clientHeight <= messagesList.scrollTop + 1;
    if (isScrolledToBottom) {
        messagesList.scrollTop = messagesList.scrollHeight - messagesList.clientHeight;
    }
}

function updateChatConnectionStatus(message, status) {
    const statusElement = document.getElementById("chatConnectionStatus");
    if (statusElement) {
        // Validate status to prevent CSS injection
        const validStatuses = ["info", "success", "warning", "danger"];
        const safeStatus = validStatuses.includes(status) ? status : "info";
        statusElement.textContent = message;
        statusElement.className = `alert alert-${safeStatus} p-2 text-center`;
    }
}