function showManageRole() {
    // Hide the user roles list
    document.getElementById("user-roles-list").style.display = "none";

    // Show the manage roles page
    document.getElementById("manage-roles-page").style.display = "block";
}

const checkboxes = document.querySelectorAll('.role-checkbox');
const cancelButton = document.getElementById('cancel-btn');

// Function to enable cancel button if any checkbox state changes
checkboxes.forEach(checkbox => {
    checkbox.addEventListener('change', () => {
        cancelButton.disabled = false;
    });
});

// Function for the Update button
function showUpdateRole() {
    // Check if any checkbox is checked
    const isChecked = Array.from(checkboxes).some(checkbox => checkbox.checked);

    if (isChecked) {
        alert('Roles updated');
        cancelButton.disabled = true; // Re-disable cancel button
    } else {
        alert('You need to pick roles');
    }
}

// Function for the Cancel button
function showCancelRole() {
    // Clear all checkboxes and disable cancel button
    checkboxes.forEach(checkbox => checkbox.checked = false);
    cancelButton.disabled = true;
}

// back to list 
function goBack() {
    document.getElementById('manage-roles-page').style.display = 'none';

    // Show the list page
    document.getElementById('user-roles-list').style.display = 'block';
}