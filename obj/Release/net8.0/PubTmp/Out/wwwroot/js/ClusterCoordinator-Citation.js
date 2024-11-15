// Author section
let authoforiginaltitlecount = 1;

document.getElementById('add-button').addEventListener('click', function() {
    
    const newAuthorRow = document.createElement('div');
    newAuthorRow.classList.add('row', 'author-fields');

    newAuthorRow.innerHTML = `
        <div class="col">
            <label class="form-label" for="author_${authoforiginaltitlecount}">Author of (Original) Article</label>
            <input class="form-control mb-3" type="text" id="author_${authoforiginaltitlecount}" placeholder="Author of (Original) Article">
        </div>
        <div class="col d-flex align-items-center mt-3">
            <span class="action remove-button"><i class="fa-solid fa-trash fa-2xl" style="color: #850000;"></i></span>
        </div>
    `;

    document.getElementById('author-container').appendChild(newAuthorRow);

    newAuthorRow.querySelector('.remove-button').addEventListener('click', function() {
        newAuthorRow.remove();
    });

    authoforiginaltitlecount++;
});

document.getElementById('clear-button').addEventListener('click', function() {
    
    const authorFields = document.querySelectorAll('.author-fields');
    authorFields.forEach((field, index) => {
        if (index > 0) {
            field.remove();
        }
    });
    document.getElementById('Authors_of_Original_Article').value = ''; 
});


// New author section
let newArticleAuthorCount = 1;

document.getElementById('add-new-author-button').addEventListener('click', function() {
    
    const newAuthorRow = document.createElement('div');
    newAuthorRow.classList.add('row', 'new-author-fields');

    newAuthorRow.innerHTML = `
        <div class="col">
            <label class="form-label" for="new_author_${newArticleAuthorCount}">Author/s of the (New) Article Who Cited the (Original) Research Article</label>
            <input class="form-control mb-3" type="text" id="new_author_${newArticleAuthorCount}" placeholder="Author/s of the (New) Article Who Cited the (Original) Research Article">
        </div>
        <div class="col d-flex align-items-center mt-3">
            <span class="action remove-button"><i class="fa-solid fa-trash fa-2xl" style="color: #850000;"></i></span>
        </div>
    `;

    document.getElementById('new-author-container').appendChild(newAuthorRow);

    newAuthorRow.querySelector('.remove-button').addEventListener('click', function() {
        newAuthorRow.remove();
    });

    newArticleAuthorCount++;
});

document.getElementById('clear-new-author-button').addEventListener('click', function() {
    
    const authorFields = document.querySelectorAll('.new-author-fields');
    authorFields.forEach((field, index) => {
        if (index > 0) {
            field.remove();
        }
    });
    document.getElementById('author_of_new_article').value = ''; 
});



