"use strict";

function getToken() {
    return localStorage.getItem("jwt_token");
}

var stockStatus = document.createElement("div");
stockStatus.id = "stockStatus";
stockStatus.className = "alert alert-info";
stockStatus.textContent = "Connecting to stock feed...";
document.querySelector(".col-md-8").insertBefore(stockStatus, document.getElementById("stockChart"));

const ctx = document.getElementById('stockChart').getContext('2d');
const stockData = {
    labels: [],
    datasets: [{
        label: 'Stock Price',
        data: [],
        borderColor: 'rgba(20, 150, 150)',
        borderWidth: 1,
        fill: true
    }]
};

const stockChart = new Chart(ctx, {
    type: 'line',
    data: stockData,
    options: {
        scales: {
            x: {
                type: 'time',
                time: {
                    unit: 'second'
                },
                adapters: {
                    date: {}
                }
            },
            y: {
                beginAtZero: false
            }
        }
    }
});

const stockconnection = new signalR.HubConnectionBuilder()
    .withUrl("/stocklisting", {
        accessTokenFactory: function() { return getToken(); }
    })
    .withAutomaticReconnect()
    .build();

stockconnection.on("PostStocks", (name, price) => {
    const now = new Date();
    stockData.labels.push(now);
    stockData.datasets[0].data.push(price);
    stockChart.update();
});

function updateStockConnectionStatus(message, status) {
    stockStatus.textContent = message;
    stockStatus.className = "alert alert-" + status;
    if (!stockStatus.parentNode) {
        document.querySelector(".col-md-8").insertBefore(stockStatus, document.getElementById("stockChart"));
    }
}

stockconnection.onreconnecting((error) => {
    updateStockConnectionStatus("Reconnecting to stock feed...", "warning");
});

stockconnection.onreconnected((connectionId) => {
    updateStockConnectionStatus("Reconnected to stock feed", "success");
    setTimeout(function() { stockStatus.remove(); }, 3000);
});

stockconnection.start().then(function() {
    updateStockConnectionStatus("Connected to stock feed", "success");
    setTimeout(function() { stockStatus.remove(); }, 3000);
}).catch(function(err) {
    if (err.message && err.message.includes("401")) {
        localStorage.removeItem("jwt_token");
        window.location.href = "/Login";
        return;
    }
    updateStockConnectionStatus("Failed to connect to stock feed", "danger");
    console.error(err.toString());
});

stockconnection.onclose(function(error) {
    updateStockConnectionStatus("Stock feed disconnected", "warning");
});
