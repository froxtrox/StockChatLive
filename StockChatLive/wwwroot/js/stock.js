"use strict";

const ctx = document.getElementById('stockChart').getContext('2d');
const stockData = {
    labels: [], // Time labels
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
                    date: {
                        // Using date-fns adapter
                    }
                }
            },
            y: {
                beginAtZero: false
            }
        }
    }
});

const stockconnection = new signalR.HubConnectionBuilder()
    .withUrl("/stocklisting")
    .withAutomaticReconnect()
    .build();

stockconnection.on("PostStocks", (name, price) => {
    const now = new Date();
    stockData.labels.push(now);
    stockData.datasets[0].data.push(price);
    stockChart.update();
});

stockconnection.onreconnecting((error) => {
    console.log("Stock connection lost. Reconnecting...", error);
    updateStockConnectionStatus("Reconnecting...", "warning");
});

stockconnection.onreconnected((connectionId) => {
    console.log("Stock connection reestablished. Connected with connectionId: " + connectionId);
    updateStockConnectionStatus("Connected", "success");
});

stockconnection.onclose((error) => {
    console.log("Stock connection closed.", error);
    updateStockConnectionStatus("Disconnected", "danger");
});

stockconnection.start()
    .then(() => {
        updateStockConnectionStatus("Connected", "success");
    })
    .catch(err => console.error(err.toString()));

function updateStockConnectionStatus(message, status) {
    const statusElement = document.getElementById("stockConnectionStatus");
    if (statusElement) {
        // Validate status to prevent CSS injection
        const validStatuses = ["info", "success", "warning", "danger"];
        const safeStatus = validStatuses.includes(status) ? status : "info";
        statusElement.textContent = message;
        statusElement.className = `alert alert-${safeStatus} p-2 text-center`;
    }
}