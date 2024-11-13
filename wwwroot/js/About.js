document.addEventListener("DOMContentLoaded", function () {

    // Get all the profile-nav items
    const profileItems = document.querySelectorAll('.abt-nav-pills a');
    // Define sections by ID
    const sections = {
        'rmo-tab': 'rmo',
        'objectives-tab': 'objectives',
        'logo-tab': 'logo',
        'services-tab': 'services',
        'officials-tab': 'officials-and-staff'
    };

    profileItems.forEach(item => {
        item.addEventListener('click', function (event) {
            event.preventDefault();

            // Remove active class from all items
            profileItems.forEach(i => i.classList.remove('active'));

            // Add active class to the clicked item
            this.classList.add('active');

            // Hide all sections
            for (const sectionId in sections) {
                document.getElementById(sections[sectionId]).style.display = 'none';
            }

            // Show the relevant section
            const targetId = sections[this.id]; // Corrected class reference
            document.getElementById(targetId).style.display = 'block';
        });
    });
});