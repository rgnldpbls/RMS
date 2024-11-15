
 const data = {
    labels: [
        'Submitted Applications',
        'Approved Applications',
        'Rejected Applications'
    ],
    datasets: [{
        label: 'Applications Data',
        data: [300, 150, 50],  
        backgroundColor: [
            '#6a040f', 
            '#9d0208', 
            '#d00000'  
        ],
        hoverOffset: 4
    }]
};


const config = {
    type: 'pie',
    data: data,
    options: {
        responsive: true,
        plugins: {
            legend: {
                position: 'top',
                labels: {
                    color: 'black'  
                }
            },
            tooltip: {
                callbacks: {
                    label: function(context) {
                        const label = context.label || '';
                        const value = context.raw || 0;
                        return `${label}: ${value}`;
                    }
                }
            }
        }
    }
};


const ctx = document.getElementById('applicationsPieChart').getContext('2d');
new Chart(ctx, config);


document.getElementById("interactionBtn").addEventListener("click", function() {
    document.getElementById("selectedInteraction").style.display = "block";
    document.getElementById("reply").style.display = "block";
});