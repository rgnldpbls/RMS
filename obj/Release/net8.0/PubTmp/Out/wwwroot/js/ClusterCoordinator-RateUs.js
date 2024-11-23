
    const stars = document.querySelectorAll('.star-icon');
    let currentRating = 0;

    // Add event listeners to each star
    stars.forEach(star => {
        // Highlight stars on hover
        star.addEventListener('mouseover', () => {
            resetStars();
            highlightStars(star.dataset.value);
        });

        // Reset stars when hover ends
        star.addEventListener('mouseout', () => {
            resetStars();
            highlightStars(currentRating); // Keep the selected rating highlighted
        });

        // Set the rating on click
        star.addEventListener('click', () => {
            currentRating = star.dataset.value;
            highlightStars(currentRating);
        });
    });

    // Function to reset stars
    function resetStars() {
        stars.forEach(star => {
            star.classList.remove('selected', 'hovered');
            star.classList.replace('bxs-star', 'bx-star');
        });
    }

    // Function to highlight stars
    function highlightStars(rating) {
        stars.forEach(star => {
            if (star.dataset.value <= rating) {
                star.classList.add('hovered');
                star.classList.replace('bx-star', 'bxs-star');
            }
        });
    }

    document.querySelectorAll('.rate-nav-link').forEach(function(tab) {
        tab.addEventListener('click', function(e) {
            e.preventDefault(); // Prevent the default anchor behavior

            // Remove 'active' class from all tabs
            document.querySelectorAll('.rate-nav-link').forEach(function(item) {
                item.classList.remove('active');
            });

            // Hide all content sections
            document.querySelectorAll('.tab-content').forEach(function(content) {
                content.style.display = 'none';
            });

            // Add 'active' class to clicked tab
            this.classList.add('active');

            // Show the relevant content section
            const targetId = this.getAttribute('data-target');
            document.getElementById(targetId).style.display = 'flex'; // Display flex for your container
        });
    });

    document.querySelectorAll('.edit-button').forEach(function(editButton) {
        editButton.addEventListener('click', function() {
            // Get the parent container of the clicked Edit button
            const container = this.closest('.d-flex.container');
            
            // Get the comment elements
            const commentText = container.querySelector('.comment-text');
            const textarea = container.querySelector('.edit-comment-textarea');
            
            // Switch to edit mode: Hide comment text and show the textarea
            commentText.style.display = 'none';
            textarea.style.display = 'block';
            textarea.value = commentText.textContent; // Copy current comment text into textarea
            
            // Show the Save button and hide the Edit button
            this.style.display = 'none';
            const saveButton = container.querySelector('.save-button');
            saveButton.style.display = 'inline-block';
        });
    });

    document.querySelectorAll('.save-button').forEach(function(saveButton) {
        saveButton.addEventListener('click', function() {
            // Get the parent container of the clicked Save button
            const container = this.closest('.d-flex.container');
            
            // Get the comment elements
            const commentText = container.querySelector('.comment-text');
            const textarea = container.querySelector('.edit-comment-textarea');
            
            // Save the edited comment: Hide the textarea and update the comment text
            commentText.textContent = textarea.value;
            textarea.style.display = 'none';
            commentText.style.display = 'block';
            
            // Show the Edit button and hide the Save button
            this.style.display = 'none';
            const editButton = container.querySelector('.edit-button');
            editButton.style.display = 'inline-block';
        });
    });

