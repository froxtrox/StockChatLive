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
    .build();

stockconnection.on("PostStocks", (name, price) => {
    const now = new Date();
    stockData.labels.push(now);
    stockData.datasets[0].data.push(price);
    stockChart.update();
});

stockconnection.start().catch(err => console.error(err.toString()));