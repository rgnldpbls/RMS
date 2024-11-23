function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.getElementById('main');
    sidebar.classList.toggle('open');
    mainContent.classList.toggle('sidebar-open');
}