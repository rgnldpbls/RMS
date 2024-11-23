// list buttons

function editApplication() {
    // Hide the details page and show the edit page
    document.getElementById('listPage').style.display = 'none';
    document.getElementById('detailsPage').style.display = 'none';
    document.getElementById('editPage').style.display = 'block';

    // Logic for populating the edit form
    console.log('Edit Application function called');
}

function viewDetails() {
    // Hide the edit page and show the details page
    document.getElementById('listPage').style.display = 'none';
    document.getElementById('editPage').style.display = 'none';
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


// SAVE CHANGES MODAL

window.openSaveChangesModal = function() {
    if (validateForm()) {  // Run validation before showing modal
        const modal = new bootstrap.Modal(document.getElementById('saveChangesModal'));
        modal.show();
    } else {
        alert("Please fill out all required fields before saving.");  // Feedback to user
    }
};

// Validation function for required fields
function validateForm() {
    let isValid = true;
    document.querySelectorAll('.form-control').forEach(input => {
        if (input.hasAttribute('required') && input.value.trim() === '') {
            isValid = false;
            input.classList.add('is-invalid');  // Optional: add visual feedback for invalid fields
        } else {
            input.classList.remove('is-invalid');  // Reset validation state if corrected
        }
    });
    return isValid;
}


// Confirm save and set fields as read-only after saving
window.confirmSave = function() {
    const visibleForm = document.querySelector('.form:not([style*="display: none"])');
    if (visibleForm) {
        visibleForm.querySelectorAll('input').forEach(input => input.setAttribute('readonly', true));
        visibleForm.querySelector('.btn-danger').style.display = 'inline-block';
        visibleForm.querySelector('.btn-success').style.display = 'none';
        visibleForm.querySelector('.btn-secondary').style.display = 'none';
    }
    
    closeModal();
    resetForm();
};

// Close modal after saving
window.closeModal = function() {
    const modal = bootstrap.Modal.getInstance(document.getElementById('saveChangesModal'));
    if (modal) {
        modal.hide();
    }
    alert('Changes saved successfully!');
};

// back to list 
function goBack() {
    // Navigate to the list page HTML file
    window.location.href = 'accomplishments-list.html';
}

// Example usage for going to the details or edit pages
function goToDetailsPage() {
    window.location.href = 'accomplishment-details.html';
}

function goToEditPage() {
    window.location.href = 'edit-accomplishment.html';
}

// filter 

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


// TAG INPUT

document.addEventListener("DOMContentLoaded", function () {
    // Reference to tag container and input field
    const tagContainer = document.getElementById("tagContainer");
    const tagInput = document.getElementById("edit-researcher-input");

    // Function to create and display a tag
    function createTag(label) {
        const div = document.createElement("div");
        div.classList.add("tag");
        div.textContent = label;

        const removeBtn = document.createElement("span");
        removeBtn.classList.add("remove-btn");
        removeBtn.textContent = "✕";
        removeBtn.onclick = function () {
            tagContainer.removeChild(div);
        };

        div.appendChild(removeBtn);
        tagContainer.insertBefore(div, tagInput);
    }

    // Event listener for adding tags
    tagInput.addEventListener("keydown", function (event) {
        if (event.key === "Enter" && tagInput.value.trim() !== "") {
            event.preventDefault(); // Prevent form submission on Enter
            createTag(tagInput.value.trim()); // Add the entered text as a tag
            tagInput.value = ""; // Clear the input
        }
    });
});

document.addEventListener("DOMContentLoaded", function () {
    // Reference to tag container and input field
    const tagContainer = document.getElementById("keyword-tags-container");
    const tagInput = document.getElementById("edit-keyword-input");

    // Function to create and display a tag
    function createTag(label) {
        const div = document.createElement("div");
        div.classList.add("tag");
        div.textContent = label;

        const removeBtn = document.createElement("span");
        removeBtn.classList.add("remove-btn");
        removeBtn.textContent = "✕";
        removeBtn.onclick = function () {
            tagContainer.removeChild(div);
        };

        div.appendChild(removeBtn);
        tagContainer.insertBefore(div, tagInput);
    }

    // Event listener for adding tags with a limit of 5
    tagInput.addEventListener("keydown", function (event) {
        const existingTags = tagContainer.querySelectorAll(".tag");
        
        // Check if the number of tags is already 5
        if (existingTags.length >= 5) {
            alert("You can only add up to 5 keywords.");
            return;
        }
        
        if (event.key === "Enter" && tagInput.value.trim() !== "") {
            event.preventDefault(); // Prevent form submission on Enter
            createTag(tagInput.value.trim()); // Add the entered text as a tag
            tagInput.value = ""; // Clear the input
        }
    });
});

// upload files, limit up to 10 mb

const uploadContainer = document.getElementById('upload-container');
const fileInput = document.getElementById('file-upload');
const maxSize = 10 * 1024 * 1024; // 10 MB
const fileError = document.getElementById('file-error');
const fileList = document.getElementById('file-list');
let filesArray = []; // To store selected files

// Drag-and-drop events
uploadContainer.addEventListener('dragover', (e) => {
    e.preventDefault();
    uploadContainer.style.backgroundColor = "#f0f0f0";
});

uploadContainer.addEventListener('dragleave', () => {
    uploadContainer.style.backgroundColor = "";
});

uploadContainer.addEventListener('drop', (e) => {
    e.preventDefault();
    uploadContainer.style.backgroundColor = "";
    handleFiles(e.dataTransfer.files);
});

// Handle file input change
fileInput.addEventListener('change', () => handleFiles(fileInput.files));

function handleFiles(selectedFiles) {
    fileError.style.display = "none";

    for (const file of selectedFiles) {
        if (file.size > maxSize) {
            fileError.style.display = "block";
            return;
        } else {
            filesArray.push(file);
            displayFiles();
        }
    }
}

function displayFiles() {
    fileList.innerHTML = "";

    filesArray.forEach((file, index) => {
        const fileItem = document.createElement("div");
        fileItem.classList.add("d-flex", "justify-content-between", "align-items-center", "mb-2");

        fileItem.innerHTML = `
            <span>${file.name} (${(file.size / 1024 / 1024).toFixed(2)} MB)</span>
            <button class="btn btn-sm remove-file-btn ms-2" onclick="removeFile(${index})"><i class='bx bx-x'></i></button>
        `;
        fileList.appendChild(fileItem);
    });
}

function removeFile(index) {
    filesArray.splice(index, 1);
    displayFiles();
}