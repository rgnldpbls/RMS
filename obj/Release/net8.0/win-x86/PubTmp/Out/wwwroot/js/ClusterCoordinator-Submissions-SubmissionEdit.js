//authors
let authorCount = 1;
    
        document.getElementById('add-button').addEventListener('click', function() {

            const newAuthorRow = document.createElement('div');
            newAuthorRow.classList.add('row', 'author-fields');
    
            newAuthorRow.innerHTML = `
                <div class="col-lg-4 col-sm-12">
                    <label class="form-label" for="author_${authorCount}">Author</label>
                    <input class="form-control mb-3" type="text" id="author_${authorCount}" placeholder="Author">
                </div>
                <div class="col-lg-4 col-sm-12">
                    <label class="form-label" for="nature_of_involvement_${authorCount}">Nature of Involvement</label>
                    <select name="nature_of_involvement_${authorCount}" id="nature_of_involvement_${authorCount}" class="form-select mb-3">
                        <option value="">Lead</option>
                        <option value="">Co-Lead</option>
                        <option value="">Member</option>
                    </select>
                </div>
                <div class="col-lg-4 col-sm-12 d-flex align-items-center mt-3">
                    <span class="action remove-button"><i class="fa-solid fa-trash fa-2xl" style="color: #850000;"></i></span>
                </div>
            `;
    
            document.getElementById('authors-container').appendChild(newAuthorRow);
    
            newAuthorRow.querySelector('.remove-button').addEventListener('click', function() {
                newAuthorRow.remove();
            });
    
            authorCount++;
        });
    
        document.getElementById('clear-button').addEventListener('click', function() {
            const authorFields = document.querySelectorAll('.author-fields');
            authorFields.forEach((field, index) => {
                if (index > 0) {
                    field.remove();
                }
            });
            document.getElementById('author_0').value = ''; 
            document.getElementById('nature_of_involvement_0').value = ''; 
        });

//keywords
let keywordCount = 1;

    document.getElementById('add-keyword').addEventListener('click', function() {
        const newKeywordRow = document.createElement('div');
        newKeywordRow.classList.add('row', 'keyword-fields');

        newKeywordRow.innerHTML = `
            <div class="col-lg-4 col-sm-12">
                <label class="form-label" for="keyword_${keywordCount}">Keyword/s</label>
                <input class="form-control mb-3" type="text" id="keyword_${keywordCount}" placeholder="keyword">
            </div>
            <div class="col-lg-4 col-sm-12 d-flex align-items-center mt-3">
                <span class="action remove-button"><i class="fa-solid fa-trash fa-2xl" style="color: #850000;"></i></span>
            </div>
        `;

        document.getElementById('keywords-container').appendChild(newKeywordRow);

        newKeywordRow.querySelector('.remove-button').addEventListener('click', function() {
            newKeywordRow.remove();
        });

        keywordCount++;
    });

    document.getElementById('clear-keywords').addEventListener('click', function() {
        
        const keywordFields = document.querySelectorAll('.keyword-fields');
        keywordFields.forEach((field, index) => {
            if (index > 0) {
                field.remove();
            }
        });
        document.getElementById('keyword_0').value = ''; 
    });

//funding
function toggleFields() {
    const fundingType = document.getElementById("type_of_funding").value;
    const fundingAgencyField = document.getElementById("funding_agency_field");
    const amountOfFundingField = document.getElementById("amount_of_funding_field");

    if (fundingType === "na" || fundingType === "self-funded") {
        fundingAgencyField.style.display = "none";
        amountOfFundingField.style.display = "none";
    } else if (fundingType === "internal") {
        fundingAgencyField.style.display = "none";
        amountOfFundingField.style.display = "block";
    } else if (fundingType === "external") {
        fundingAgencyField.style.display = "block";
        amountOfFundingField.style.display = "block";
    }
}

//organizer
let organizerCount = 1;
    
        document.getElementById('add-organizer').addEventListener('click', function() {
            
            const newOrganizerRow = document.createElement('div');
            newOrganizerRow.classList.add('row', 'organizer-fields');
    
            newOrganizerRow.innerHTML = `
                <div class="col-lg-4 col-sm-12">
                    <label class="form-label" for="organizer_${organizerCount}">Organizer</label>
                    <input class="form-control mb-3" type="text" id="organizer_${organizerCount}" placeholder="Organizer">
                </div>
                <div class="col-lg-4 col-sm-12 d-flex align-items-center mt-3">
                    <span class="action remove-button"><i class="fa-solid fa-trash fa-2xl" style="color: #850000;"></i></span>
                </div>
            `;
    
            document.getElementById('organizer-container').appendChild(newOrganizerRow);
    
            
            newOrganizerRow.querySelector('.remove-button').addEventListener('click', function() {
                newOrganizerRow.remove();
            });
    
            organizerCount++;
        });
    
        document.getElementById('clear-organizer').addEventListener('click', function() {
            
            const organizerFields = document.querySelectorAll('.organizer-fields');
            organizerFields.forEach((field, index) => {
                if (index > 0) {
                    field.remove();
                }
            });
            document.getElementById('organizer_0').value = ''; 
        });



//dept and college select
$(document).ready(function() {
    var departments = {
        CAF: [
            { value: 'BSA', text: 'Bachelor of Science in Accountancy' },
            { value: 'BSMA', text: 'Bachelor of Science in Management Accounting' },
            { value: 'BSBAFM', text: 'Bachelor of Science in Business Administration Major in Financial Management' }
        ],
        CADBE: [
            { value: 'BS-ARCH', text: 'Bachelor of Science in Architecture' },
            { value: 'BSID', text: 'Bachelor of Science in Interior Design' },
            { value: 'BSEP', text: 'Bachelor of Science in Environmental Planning' }
        ],
        CAL: [
            { value: 'ABELS', text: 'Bachelor of Arts in English Language Studies' },
            { value: 'ABF', text: 'Bachelor of Arts in Filipinology' },
            { value: 'ABLCS', text: 'Bachelor of Arts in Literary and Cultural Studies' },
            { value: 'AB-PHILO', text: 'Bachelor of Arts in Philosophy' },
            { value: 'BPEA', text: 'Bachelor of Performing Arts major in Theater Arts' }
        ],
        CBA: [
            { value: 'DBA', text: 'Doctor in Business Administration' },
            { value: 'MBA', text: 'Master in Business Administration' },
            { value: 'BSBAHRM', text: 'Bachelor of Science in Business Administration major in Human Resource Management' },
            { value: 'BSBA-MM', text: 'Bachelor of Science in Business Administration major in Marketing Management' },
            { value: 'BSENTREP', text: 'Bachelor of Science in Entrepreneurship' },
            { value: 'BSOA', text: 'Bachelor of Science in Office Administration' },

        ],
        COC: [
            { value: 'BADPR', text: 'Bachelor in Advertising and Public Relations' },
            { value: 'BA_Broadcasting', text: 'Bachelor of Arts in Broadcasting' },
            { value: 'BAJ', text: 'Bachelor of Arts in Journalism' }
        ],
        CCIS: [
            { value: 'BSCS', text: 'Bachelor of Science in Computer Science' },
            { value: 'BSIT', text: 'Bachelor of Science in Information Technology' }
        ],
        COED: [
            { value: 'PhDEM', text: 'Doctor of Philsophy in Education Management' },
            { value: 'MAEM', text: 'Master of Arts in Education Management' },
            { value: 'MBE', text: 'Master in Business Education' },
            { value: 'MLIS', text: 'Master in Library and Information Science' },
            { value: 'MAELT', text: 'Master of Arts in English Language Teaching' },
            { value: 'MAEd-ME', text: 'Master of Arts in Education major in Mathematics Education' },
            { value: 'MAPES', text: 'Master of Arts in Physical Education and Sports' },
            { value: 'MAED-TCA', text: 'Master of Arts in Education major in Teaching in the Challenged Areas' },
            { value: 'PBDE', text: 'Post-Baccalaureate Diploma in Education' },
            { value: 'BTLEd', text: 'Bachelor of Technology and Livelihood Education' },
            { value: 'BLIS', text: 'Bachelor of Library and Information Science' },
            { value: 'BSEd', text: 'Bachelor of Secondary Education' },
            { value: 'BEEd', text: 'Bachelor of Elementary Education' },
            { value: 'BECEd', text: 'Bachelor of Early Childhood Education' },
        ],
        CE: [
            { value: 'BSCE', text: 'Bachelor of Science in Civil Engineering' },
            { value: 'BSCpE', text: 'Bachelor of Science in Computer Engineering' },
            { value: 'BSEE', text: 'Bachelor of Science in Electrical Engineering' },
            { value: 'BSECE', text: 'Bachelor of Science in Electronics Engineering' },
            { value: 'BSIE', text: 'Bachelor of Science in Industrial Engineering' },
            { value: 'BSME', text: 'Bachelor of Science in Mechanical Engineering' },
            { value: 'BSRE', text: 'Bachelor of Science in Railway Engineering' },

        ],
        CHK: [
            { value: 'BPE', text: 'Bachelor of Physical Education' },
            { value: 'BSESS', text: 'Bachelor of Science in Exercises and Sports' }
        ],
        CL: [
            { value: 'JD', text: 'Juris Doctor' }
        ],
        CPSPA: [
            { value: 'DPA', text: 'Doctor in Public Administration' },
            { value: 'MPA', text: 'Master in Public Administration' },
            { value: 'BPA', text: 'Bachelor of Public Administration' },
            { value: 'BAIS', text: 'Bachelor of Arts in International Studies' },
            { value: 'BAPE', text: 'Bachelor of Arts in Political Economy' },
            { value: 'BAPS', text: 'Bachelor of Arts in Political Science' },
        ],
        CSSD: [
            { value: 'BAH', text: 'Bachelor of Arts in History' },
            { value: 'BAS', text: 'Bachelor of Arts in Sociology' },
            { value: 'BSC', text: 'Bachelor of Science in Cooperatives' },
            { value: 'BSE', text: 'Bachelor of Science in Economics' },
            { value: 'BSPSY', text: 'Bachelor of Science in Psychology' },
        ],
        CS: [
            { value: 'BSFT', text: 'Bachelor of Science Food Technology' },
            { value: 'BSAPMATH', text: 'Bachelor of Science in Applied Mathematics' },
            { value: 'BSBIO', text: 'Bachelor of Science in Biology' },
            { value: 'BSCHEM', text: 'Bachelor of Science in Chemistry' },
            { value: 'BSMATH', text: 'Bachelor of Science in Mathematics' },
            { value: 'BSND', text: 'Bachelor of Science in Nutrition and Dietetics' },
            { value: 'BSPHY', text: 'Bachelor of Science in Physics' },
            { value: 'BSSTAT', text: 'Bachelor of Science in Statistics' }
        ],
        CTHTM: [
            { value: 'BSHM', text: 'Bachelor of Science in Hospitality Management' },
            { value: 'BSTM', text: 'Bachelor of Science in Tourism Management' },
            { value: 'BSTRM', text: 'Bachelor of Science in Transportation Management' }
        ],
        ITECH: [
            { value: 'DCvET', text: 'Diploma in Civil Engineering Technology' },
            { value: 'DCET', text: 'Diploma in Computer Engineering Technology' },
            { value: 'DEET', text: 'Diploma in Electrical Engineering Technology' },
            { value: 'DECET', text: 'Diploma in Electronics Engineering Technology' },
            { value: 'DICT', text: 'Diploma in Information Communication Technology' },
            { value: 'DMET', text: 'Diploma in Mechanical Engineering Technology' },
            { value: 'DOMT', text: 'Diploma in Office Management Technology' },
            { value: 'DRET', text: 'Diploma in Railway Engineering Technology' }
        ]
    };

    $('#college').change(function() {
        var college = $(this).val();
        var $department = $('#department');
        
        $department.empty();
        
        if (college && departments[college]) {
            departments[college].forEach(function(department) {
                $department.append('<option value="' + department.value + '">' + department.text + '</option>');
            });
        }
    });
});