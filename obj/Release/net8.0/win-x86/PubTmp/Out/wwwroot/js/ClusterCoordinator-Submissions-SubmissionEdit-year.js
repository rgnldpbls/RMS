let startYear = 1900;
let endYear = new Date().getFullYear();
for (let year = endYear; year >= startYear; year--) {
    if (year === 2024) {
        document.write(`<option value="${year}" selected>${year}</option>`);
    } else {
        document.write(`<option value="${year}">${year}</option>`);
    }
}
       