const colors = [
    "#6CE5E8", // Light Cyan
    "#40B8D5", // Sky Blue
    "#2C8BBA", // Soft Blue
    "#2F5F98", // Deep Blue
    "#31356E", // Dark Blue
    "#2B2C5D", // Darker Blue
    "#25254C", // Navy Blue
    "#1F1F3C", // Midnight Blue
    "#19192B", // Deep Midnight
    "#13131B"  // Almost Black-Blue
];

window.renderApexChart = (chartId, chartOptions) => {
    let element = document.querySelector("#" + chartId);

    //// Destroy previous instance if exists
    if (element && element.chartInstance) {
        element.chartInstance.destroy();
    }

    var chart = new ApexCharts(element, chartOptions);
    chart.render();

    // Store chart instance for cleanup
    element.chartInstance = chart;
};

window.renderBarChart = (chartId, data) => {
    let chartOptions = {
        colors: colors,
        series: data?.series || [],
        xaxis: {
            categories: data?.xaxis?.categories
        },
        chart: {
            type: 'bar',
            height: "328px"
        },
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: "25%",
                borderRadius: 5,
                borderRadiusApplication: "end"
            }
        },
        dataLabels: {
            enabled: false,
        },
        legend: {
            show: true,
            position: "top",
        }
    };

    renderApexChart(chartId, chartOptions);
};

window.renderDonutChart = (chartId, data) => {
    let chartOptions = {
        colors: colors,
        series: data?.series,
        labels: data?.labels,
        chart: {
            type: 'donut',
            height: "340px"
        },
        dataLabels: {
            enabled: false,
        },
        legend: {
            show: false,
        }
    };

    renderApexChart(chartId, chartOptions);
};