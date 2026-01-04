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
    .build();

stockconnection.on("PostStocks", (name, price) => {
    const now = new Date();
    stockData.labels.push(now);
    stockData.datasets[0].data.push(price);
    stockChart.update();
});

stockconnection.start().then(function() {
    stockStatus.className = "alert alert-success";
    stockStatus.textContent = "Connected to stock feed";
    setTimeout(function() { stockStatus.remove(); }, 3000);
}).catch(function(err) {
    if (err.message && err.message.includes("401")) {
        localStorage.removeItem("jwt_token");
        window.location.href = "/Login";
        return;
    }
    stockStatus.className = "alert alert-danger";
    stockStatus.textContent = "Failed to connect to stock feed";
    console.error(err.toString());
});

stockconnection.onclose(function(error) {
    stockStatus.className = "alert alert-warning";
    stockStatus.textContent = "Stock feed disconnected";
    document.querySelector(".col-md-8").insertBefore(stockStatus, document.getElementById("stockChart"));
});
