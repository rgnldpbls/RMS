function viewDetails() {
    // Hide the edit page and show the details page
    document.getElementById('listPage').style.display = 'none';
    document.getElementById('detailsPage').style.display = 'block';

    // Logic for viewing application details
    console.log('View Details function called');
}

// MODALS

// delete modal
function showDeleteModal() {
    var modal = new bootstrap.Modal(document.getElementById('deleteModal'));
    modal.show();
}

function confirmDelete() {
    // Perform the delete action
    console.log('Accomplishment deleted');
    // Hide the modal
    var modal = bootstrap.Modal.getInstance(document.getElementById('deleteModal'));
    modal.hide();
}

// back to list 
function goBack() {
    // Navigate to the list page HTML file
    window.location.href = 'accomplishments-list.html';
}

// Example usage for going to the details or edit pages
function goToDetailsPage() {
    window.location.href = 'accomplishment-details.html';
}

// filter 

// filter for date

document.getElementById('dateFrom').addEventListener('change', function() {
    const fromYear = parseInt(this.value);
    const toYearDropdown = document.getElementById('dateTo');
    
    // Enable only the years greater than or equal to the selected "From" year
    Array.from(toYearDropdown.options).forEach(option => {
        if (option.value && parseInt(option.value) < fromYear) {
            option.disabled = true;
        } else {
            option.disabled = false;
        }
    });
});


document.getElementById('dateTo').addEventListener('change', function() {
    const toYear = parseInt(this.value);
    const fromYearDropdown = document.getElementById('dateFrom');
    
    // Enable only the years less than or equal to the selected "To" year
    Array.from(fromYearDropdown.options).forEach(option => {
        if (option.value && parseInt(option.value) > toYear) {
            option.disabled = true;
        } else {
            option.disabled = false;
        }
    });
});

// Campus list
const campuses = [
    { id: "campus1", name: "Sta. Mesa, Manila" },
    { id: "campus2", name: "Taguig" },
    { id: "campus3", name: "Quezon City" },
    { id: "campus4", name: "San Juan City" },
    { id: "campus5", name: "Parañaque City" },
    { id: "campus6", name: "Bataan" },
    { id: "campus7", name: "Sta. Maria, Bulacan" },
    { id: "campus8", name: "Pulilan, Bulacan" },
    { id: "campus9", name: "Cabiao, Nueva Ecija" },
    { id: "campus10", name: "Lopez, Quezon" },
    { id: "campus11", name: "Mulanay, Quezon" },
    { id: "campus12", name: "Unisan, Quezon" },
    { id: "campus13", name: "Ragay, Camarines Sur" },
    { id: "campus14", name: "Sto. Tomas, Batangas" },
    { id: "campus15", name: "Maragondon, Cavite" },
    { id: "campus16", name: "Bansud, Oriental Mindoro" },
    { id: "campus17", name: "Sablayan, Occidental Mindoro" },
    { id: "campus18", name: "Biñan, Laguna" },
    { id: "campus19", name: "San Pedro, Laguna" },
    { id: "campus20", name: "Sta. Rosa, Laguna" },
    { id: "campus21", name: "Calauan, Laguna" }
];

// College list
const colleges = [
    { id: "college1", name: "(CAF) College of Accountancy and Finances" },
    { id: "college2", name: "(CADBE) College of Architecture, Design and the Built Environment" },
    { id: "college3", name: "(CAL) College of Arts and Letters" },
    { id: "college4", name: "(CBA) College of Business Administration" },
    { id: "college5", name: "(COC) College of Communication " },
    { id: "college6", name: "(CCIS) College of Computer and Information Sciences " },
    { id: "college7", name: "(COED) College of Education " },
    { id: "college8", name: "(CE) College of Engineering " },
    { id: "college9", name: "(CHK) College of Human Kinetics " },
    { id: "college10", name: "(CL) College of Law " },
    { id: "college11", name: "(CPSPA) College of Political Science and Public Administration " },
    { id: "college12", name: "(CSSD) College of Social Sciences and Development " },
    { id: "college13", name: "(CS) College of Science " },
    { id: "college14", name: "(CTHTM) College of Tourism, Hospitality and Transportation Management " },
    { id: "college15", name: "(ITech) Institute of Technology " },
    { id: "college16", name: "(OUS) Open University System " }
];

// Function to populate filter checkboxes
function populateFilter(containerId, dataList) {
    const container = document.getElementById(containerId);
    dataList.forEach(item => {
        const checkbox = document.createElement('input');
        checkbox.type = 'checkbox';
        checkbox.id = item.id;
        checkbox.value = item.name;
        checkbox.className = 'form-check-input';

        const label = document.createElement('label');
        label.htmlFor = item.id;
        label.className = 'form-check-label';
        label.appendChild(document.createTextNode(item.name));

        const div = document.createElement('div');
        div.className = 'form-check';

        div.appendChild(checkbox);
        div.appendChild(label);

        container.appendChild(div);
    });
}

// Populate campus and college checkboxes
populateFilter('campus-filter-container', campuses);
populateFilter('college-filter-container', colleges);

