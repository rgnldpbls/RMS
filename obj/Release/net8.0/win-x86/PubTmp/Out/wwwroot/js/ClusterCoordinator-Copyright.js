// Researcher/Inventors section
let researcherCount = 1;

document.getElementById('add-researcher-button').addEventListener('click', function() {
    
    const newResearcherRow = document.createElement('div');
    newResearcherRow.classList.add('row', 'researcher-fields');

    newResearcherRow.innerHTML = `
        <div class="col-lg-6 col-sm-12">
            <label class="form-label" for="researcher_${researcherCount}">Researcher/Inventor</label>
            <input class="form-control mb-3" type="text" id="researcher_${researcherCount}" placeholder="Researcher/Inventor">
        </div>
        <div class="col-lg-4 col-sm-12 d-flex align-items-center mt-3">
            <span class="action remove-button"><i class="fa-solid fa-trash fa-2xl" style="color: #850000;"></i></span>
        </div>
    `;

    document.getElementById('researcher-container').appendChild(newResearcherRow);

    newResearcherRow.querySelector('.remove-button').addEventListener('click', function() {
        newResearcherRow.remove();
    });

    researcherCount++;
});

document.getElementById('clear-researcher-button').addEventListener('click', function() {
    
    const researcherFields = document.querySelectorAll('.researcher-fields');
    researcherFields.forEach((field, index) => {
        if (index > 0) {
            field.remove();
        }
    });
    document.getElementById('researcher_inventors').value = ''; 
});
