document.querySelectorAll('#smaller-container').forEach(container => {
    container.addEventListener('click', function() {
      
        document.querySelectorAll('#smaller-container').forEach(item => {
            item.style.backgroundColor = ''; 
        });
        
        container.style.backgroundColor = 'rgb(133, 0, 0, 0.2)';
    });
});