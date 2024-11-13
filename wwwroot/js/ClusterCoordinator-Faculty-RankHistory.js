function toggleEdit(button) {
    const row = button.closest('tr');
    const cells = row.querySelectorAll('.editable');

    const isEditing = button.querySelector('i').classList.contains('fa-pen-to-square');

    if (isEditing) {
        cells.forEach((cell, index) => {
            const cellText = cell.textContent.trim();

            switch (index) {
                case 0:
                    cell.innerHTML = `
                                <select class="form-select">
                                    <option value="Instructor 1">Instructor 1</option>
                            <option value="Instructor 2">Instructor 2</option>
                            <option value="Instructor 3">Instructor 3</option>
                            <option value="Assistant Professor 1">Assistant Professor 1</option>
                            <option value="Assistant Professor 2">Assistant Professor 2</option>
                            <option value="Assistant Professor 3">Assistant Professor 3</option>
                            <option value="Assistant Professor 4">Assistant Professor 4</option>
                            <option value="Associate Professor 1">Associate Professor 1</option>
                            <option value="Associate Professor 2">Associate Professor 2</option>
                            <option value="Associate Professor 3">Associate Professor 3</option>
                            <option value="Associate Professor 4">Associate Professor 4</option>
                            <option value="Associate Professor 5">Associate Professor 5</option>
                            <option value="Professor 1">Professor 1</option>
                            <option value="Professor 2">Professor 2</option>
                            <option value="Professor 3">Professor 3</option>
                            <option value="Professor 4">Professor 4</option>
                            <option value="Professor 5">Professor 5</option>
                            <option value="Professor 6">Professor 6</option>
                            <option value="College Professor">College Professor</option>
                            <option value="University Professor">University Professor</option>
                                </select>`;
                    break;
                case 1:
                    cell.innerHTML = `
                                <select class="form-select">
                                    <option value="CAF">College of Accountancy and Finance (CAF)</option>
                                    <option value="CADBE">College of Architecture, Design and the Built Environment (CADBE)</option>
                                    <option value="CAL">College of Arts and Letters (CAL)</option>
                                    <option value="CBA">College of Business Administration (CBA)</option>
                                    <option value="COC">College of Communication (COC)</option>
                                    <option value="CCIS">College of Computer and Information Sciences (CCIS)</option>
                                    <option value="COED">College of Education (COED)</option>
                                    <option value="CE">College of Engineering (CE)</option>
                                    <option value="CHK">College of Human Kinetics (CHK)</option>
                                    <option value="CL">College of Law (CL)</option>
                                    <option value="CPSPA">College of Political Science and Public Administration (CPSPA)</option>
                                    <option value="CSSD">College of Social Sciences and Development (CSSD)</option>
                                    <option value="CS">College of Science (CS)</option>
                                    <option value="CTHTM">College of Tourism, Hospitality and Transportation Management (CTHTM)</option>
                                    <option value="ITECH"> Institute of Technology (ITECH)</option>
                                </select>`;
                    break;
                case 2:
                    cell.innerHTML = `<input type="number" class="form-control" value="${cellText}">`;
                    break;
                case 3:
                    cell.innerHTML = `<input type="date" class="form-control" value="${cellText}">`;
                    break;
                case 4:
                    cell.innerHTML = `<input type="date" class="form-control" value="${cellText}">`;
                    break;
                default:
                    break;
            }
        });


        button.innerHTML = '<i class="fa-solid fa-floppy-disk fa-2xl" style="color: #850000;"></i>';
    } else {
        cells.forEach((cell, index) => {
            const input = cell.querySelector('input, select');
            const inputValue = input.value;
            cell.textContent = inputValue;
        });


        button.innerHTML = '<i class="fa-solid fa-pen-to-square fa-2xl" style="color: #850000;"></i>';


        const saveModal = new bootstrap.Modal(document.getElementById('saveModal'));
        saveModal.show();
    }
}

function addNewRow() {
    const table = document.querySelector('table tbody');
    const newRow = document.createElement('tr');
    newRow.classList.add('align-middle');

    // Create the new cells
    const rankCell = document.createElement('td');
    rankCell.classList.add('col', 'p-4', 'bg-transparent', 'editable');
    rankCell.innerHTML = `
                <select class="form-select">
                    <option value="Instructor 1">Instructor 1</option>
                            <option value="Instructor 2">Instructor 2</option>
                            <option value="Instructor 3">Instructor 3</option>
                            <option value="Assistant Professor 1">Assistant Professor 1</option>
                            <option value="Assistant Professor 2">Assistant Professor 2</option>
                            <option value="Assistant Professor 3">Assistant Professor 3</option>
                            <option value="Assistant Professor 4">Assistant Professor 4</option>
                            <option value="Associate Professor 1">Associate Professor 1</option>
                            <option value="Associate Professor 2">Associate Professor 2</option>
                            <option value="Associate Professor 3">Associate Professor 3</option>
                            <option value="Associate Professor 4">Associate Professor 4</option>
                            <option value="Associate Professor 5">Associate Professor 5</option>
                            <option value="Professor 1">Professor 1</option>
                            <option value="Professor 2">Professor 2</option>
                            <option value="Professor 3">Professor 3</option>
                            <option value="Professor 4">Professor 4</option>
                            <option value="Professor 5">Professor 5</option>
                            <option value="Professor 6">Professor 6</option>
                            <option value="College Professor">College Professor</option>
                            <option value="University Professor">University Professor</option>
                </select>
            `;

    const collegeCell = document.createElement('td');
    collegeCell.classList.add('col', 'p-4', 'bg-transparent', 'editable');
    collegeCell.innerHTML = `
                <select class="form-select" aria-label="College" name="college" id="college">
                            <option value="CAF">College of Accountancy and Finance (CAF)</option>
                            <option value="CADBE">College of Architecture, Design and the Built Environment (CADBE)</option>
                            <option value="CAL">College of Arts and Letters (CAL)</option>
                            <option value="CBA">College of Business Administration (CBA)</option>
                            <option value="COC">College of Communication (COC)</option>
                            <option value="CCIS">College of Computer and Information Sciences (CCIS)</option>
                            <option value="COED">College of Education (COED)</option>
                            <option value="CE">College of Engineering (CE)</option>
                            <option value="CHK">College of Human Kinetics (CHK)</option>
                            <option value="CL">College of Law (CL)</option>
                            <option value="CPSPA">College of Political Science and Public Administration (CPSPA)</option>
                            <option value="CSSD">College of Social Sciences and Development (CSSD)</option>
                            <option value="CS">College of Science (CS)</option>
                            <option value="CTHTM">College of Tourism, Hospitality and Transportation Management (CTHTM)</option>
                            <option value="ITECH"> Institute of Technology (ITECH)</option>
                        </select>
            `;

    const durationCell = document.createElement('td');
    durationCell.classList.add('col', 'p-4', 'bg-transparent', 'editable');
    durationCell.innerHTML = `<input type="number" class="form-control" value="">`;

    const startCell = document.createElement('td');
    startCell.classList.add('col', 'p-4', 'bg-transparent', 'editable');
    startCell.innerHTML = `<input type="date" class="form-control" value="">`;

    const endCell = document.createElement('td');
    endCell.classList.add('col', 'p-4', 'bg-transparent', 'editable');
    endCell.innerHTML = `<input type="date" class="form-control" value="">`;

    const actionCell = document.createElement('td');
    actionCell.classList.add('p-4', 'bg-transparent');
    actionCell.innerHTML = `
                <button id="action" class="me-2" onclick="toggleEdit(this)">
                    <i class="fa-solid fa-pen-to-square fa-2xl" style="color: #850000;"></i>
                </button>
                <button id="action" data-bs-toggle="modal" data-bs-target="#staticBackdrop">
                    <i class="fa-solid fa-trash fa-2xl" style="color: #850000;"></i>
                </button>
            `;

    // Append the cells to the new row
    newRow.appendChild(rankCell);
    newRow.appendChild(collegeCell);
    newRow.appendChild(durationCell);
    newRow.appendChild(startCell);
    newRow.appendChild(endCell);
    newRow.appendChild(actionCell);

    // Append the new row to the table
    table.appendChild(newRow);
}