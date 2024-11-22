var departments = {
    CAF: [
        { value: 'BSA', text: 'Bachelor of Science in Accountancy', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSMA', text: 'Bachelor of Science in Management Accounting', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSBAFM', text: 'Bachelor of Science in Business Administration Major in Financial Management', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 }
    ],
    CADBE: [
        { value: 'BS-ARCH', text: 'Bachelor of Science in Architecture', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSID', text: 'Bachelor of Science in Interior Design', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSEP', text: 'Bachelor of Science in Environmental Planning', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 }
    ],
    CAL: [
        { value: 'ABELS', text: 'Bachelor of Arts in English Language Studies', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'ABF', text: 'Bachelor of Arts in Filipinology', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'ABLCS', text: 'Bachelor of Arts in Literary and Cultural Studies', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'AB-PHILO', text: 'Bachelor of Arts in Philosophy', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BPEA', text: 'Bachelor of Performing Arts major in Theater Arts', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 }
    ],
    CBA: [
        { value: 'DBA', text: 'Doctor in Business Administration', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'MBA', text: 'Master in Business Administration', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSBAHRM', text: 'Bachelor of Science in Business Administration major in Human Resource Management', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSBA-MM', text: 'Bachelor of Science in Business Administration major in Marketing Management', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSENTREP', text: 'Bachelor of Science in Entrepreneurship', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSOA', text: 'Bachelor of Science in Office Administration', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },

    ],
    COC: [
        { value: 'BADPR', text: 'Bachelor in Advertising and Public Relations', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BA_Broadcasting', text: 'Bachelor of Arts in Broadcasting', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BAJ', text: 'Bachelor of Arts in Journalism', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 }
    ],
    CCIS: [
        { value: 'BSCS', text: 'Bachelor of Science in Computer Science', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSIT', text: 'Bachelor of Science in Information Technology', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 }
    ],
    COED: [
        { value: 'PhDEM', text: 'Doctor of Philsophy in Education Management', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'MAEM', text: 'Master of Arts in Education Management', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'MBE', text: 'Master in Business Education', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'MLIS', text: 'Master in Library and Information Science', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'MAELT', text: 'Master of Arts in English Language Teaching', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'MAEd-ME', text: 'Master of Arts in Education major in Mathematics Education', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'MAPES', text: 'Master of Arts in Physical Education and Sports', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'MAED-TCA', text: 'Master of Arts in Education major in Teaching in the Challenged Areas', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'PBDE', text: 'Post-Baccalaureate Diploma in Education', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BTLEd', text: 'Bachelor of Technology and Livelihood Education', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BLIS', text: 'Bachelor of Library and Information Science', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSEd', text: 'Bachelor of Secondary Education', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BEEd', text: 'Bachelor of Elementary Education', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BECEd', text: 'Bachelor of Early Childhood Education', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
    ],
    CE: [
        { value: 'BSCE', text: 'Bachelor of Science in Civil Engineering', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSCpE', text: 'Bachelor of Science in Computer Engineering', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSEE', text: 'Bachelor of Science in Electrical Engineering', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSECE', text: 'Bachelor of Science in Electronics Engineering', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSIE', text: 'Bachelor of Science in Industrial Engineering', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSME', text: 'Bachelor of Science in Mechanical Engineering', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSRE', text: 'Bachelor of Science in Railway Engineering', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },

    ],
    CHK: [
        { value: 'BPE', text: 'Bachelor of Physical Education', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSESS', text: 'Bachelor of Science in Exercises and Sports', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 }
    ],
    CL: [
        { value: 'JD', text: 'Juris Doctor', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 }
    ],
    CPSPA: [
        { value: 'DPA', text: 'Doctor in Public Administration', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'MPA', text: 'Master in Public Administration', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BPA', text: 'Bachelor of Public Administration', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BAIS', text: 'Bachelor of Arts in International Studies', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BAPE', text: 'Bachelor of Arts in Political Economy', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BAPS', text: 'Bachelor of Arts in Political Science', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
    ],
    CSSD: [
        { value: 'BAH', text: 'Bachelor of Arts in History', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BAS', text: 'Bachelor of Arts in Sociology', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSC', text: 'Bachelor of Science in Cooperatives', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSE', text: 'Bachelor of Science in Economics', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSPSY', text: 'Bachelor of Science in Psychology', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
    ],
    CS: [
        { value: 'BSFT', text: 'Bachelor of Science Food Technology', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSAPMATH', text: 'Bachelor of Science in Applied Mathematics', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSBIO', text: 'Bachelor of Science in Biology', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSCHEM', text: 'Bachelor of Science in Chemistry', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSMATH', text: 'Bachelor of Science in Mathematics', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSND', text: 'Bachelor of Science in Nutrition and Dietetics', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSPHY', text: 'Bachelor of Science in Physics', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSSTAT', text: 'Bachelor of Science in Statistics', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 }
    ],
    CTHTM: [
        { value: 'BSHM', text: 'Bachelor of Science in Hospitality Management', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSTM', text: 'Bachelor of Science in Tourism Management', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'BSTRM', text: 'Bachelor of Science in Transportation Management', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 }
    ],
    ITECH: [
        { value: 'DCvET', text: 'Diploma in Civil Engineering Technology', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'DCET', text: 'Diploma in Computer Engineering Technology', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'DEET', text: 'Diploma in Electrical Engineering Technology', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'DECET', text: 'Diploma in Electronics Engineering Technology', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'DICT', text: 'Diploma in Information Communication Technology', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'DMET', text: 'Diploma in Mechanical Engineering Technology', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'DOMT', text: 'Diploma in Office Management Technology', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2 },
        { value: 'DRET', text: 'Diploma in Railway Engineering Technology', production: 10, publication: 5, presentation: 7, utilization: 3, patent: 1, citation: 8, copyright: 2  }
    ]


};


const ctx = document.getElementById('rmoacts').getContext('2d');
let rmoacts = new Chart(ctx, {
    type: 'pie',
    data: {
        labels: [],
        datasets: [{
            label: 'Departments',
            data: [],
            backgroundColor: [],
            borderColor: ['#FFDB89'],
            borderWidth: 1
        }]
    },
    options: {
        responsive: true,
        plugins: {
            legend: {
                position: 'top',
                labels: {
                    color: 'white'
                }
            },
            tooltip: { enabled: true }
        }
    }
});


function updatePieChart(college, category) {

    if (college === 'all') {
        const colleges = Object.keys(departments);
        const labels = colleges;
        const data = colleges.map(college => {
            return departments[college].reduce((sum, dept) => sum + dept[category], 0);
        });
        const colors = colleges.map(() => `rgba(${Math.floor(Math.random() * 255)}, ${Math.floor(Math.random() * 255)}, ${Math.floor(Math.random() * 255)}, 0.6)`);

        rmoacts.data.labels = labels;
        rmoacts.data.datasets[0].data = data;
        rmoacts.data.datasets[0].backgroundColor = colors;
        rmoacts.data.datasets[0].label = capitalizeFirstLetter(category) + " by College";
        rmoacts.update();
        return;
    }


    if (!college || !departments[college]) {
        rmoacts.data.labels = [];
        rmoacts.data.datasets[0].data = [];
        rmoacts.data.datasets[0].backgroundColor = [];
        rmoacts.update();
        return;
    }

    const deptData = departments[college];
    const labels = deptData.map(dept => dept.text);
    const data = deptData.map(dept => dept[category]);
    const colors = deptData.map(() => `rgba(${Math.floor(Math.random() * 255)}, ${Math.floor(Math.random() * 255)}, ${Math.floor(Math.random() * 255)}, 0.6)`);

    rmoacts.data.labels = labels;
    rmoacts.data.datasets[0].data = data;
    rmoacts.data.datasets[0].backgroundColor = colors;
    rmoacts.data.datasets[0].label = capitalizeFirstLetter(category) + " by Department";
    rmoacts.update();
}


function capitalizeFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
}


$(document).ready(function () {
    const initialCollege = 'all';
    const initialCategory = 'production';
    $('#college').val(initialCollege);
    $('#category').val(initialCategory);
    updatePieChart(initialCollege, initialCategory);
});


$('#college').change(function () {
    var college = $(this).val();
    var category = $('#category').val();
    updatePieChart(college, category);
});


$('#category').change(function () {
    var college = $('#college').val();
    var category = $(this).val();
    updatePieChart(college, category);
});