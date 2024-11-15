// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web ~s.

// Write your JavaScript code.

// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web ~s.

// Write your JavaScript code.


// Get the header and navbar elements
const topHeader = document.querySelector('.top-header');
const navbar = document.getElementById('navbar');

// Get the position where the navbar should stick
const sticky = navbar.offsetTop;

// Add scroll event listener to the window 
window.onscroll = function () {
    handleStickyNavbar();
};

function handleStickyNavbar() {
    if (window.window.scrollY > sticky) {
        navbar.classList.add("sticky");
        topHeader.style.top = "-100px";  // Hide the top header
    } else {
        navbar.classList.remove("sticky");
        topHeader.style.top = "0";  // Show the top header
    }
}

// Function to update the URL without reloading the page
function updateUrl(sectionId) {
    // Use history.pushState to change the URL without reloading the page
    const newUrl = `${window.location.origin}/${sectionId.replace('#', '')}`;

    history.pushState(null, '', newUrl);
}

// Handle back/forward browser navigation
window.onpopstate = function () {
    let section = window.location.pathname.split("/").pop();
    if (section) {
        loadSectionContent(`#${section}`);
    }
};

function updateDropdownText(element) {
    var dropdownButton = document.getElementById("dropdownMenuButton");
    dropdownButton.textContent = element.textContent; // Update the button text with the selected item
}
