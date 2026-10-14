/*!
 * Start Bootstrap - Library Admin v7.0.5 (Modified)
 * Copyright 2013-2022 Start Bootstrap
 * Licensed under MIT 
 */

window.addEventListener('DOMContentLoaded', event => {

    // 1. Toggle the side navigation & persist state across page refreshes
    const sidebarToggle = document.body.querySelector('#sidebarToggle');
    if (sidebarToggle) {
        // Restore sidebar state from localStorage on page load
        if (localStorage.getItem('sb|sidebar-toggle') === 'true') {
            document.body.classList.add('sb-sidenav-toggled');
        }

        sidebarToggle.addEventListener('click', event => {
            event.preventDefault();
            document.body.classList.toggle('sb-sidenav-toggled');
            // Save state to localStorage
            localStorage.setItem('sb|sidebar-toggle', document.body.classList.contains('sb-sidenav-toggled'));
        });
    }

    // 2. Global form confirmation handler (for Delete, Pay Fine, etc.)
    document.addEventListener('submit', function (e) {
        const message = e.target.getAttribute('data-confirm');
        if (message && !window.confirm(message)) {
            e.preventDefault();
        }
    });

    // 3. Auto-dismiss Bootstrap alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(alert => {
        setTimeout(() => {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            if (bsAlert) {
                bsAlert.close();
            }
        }, 5000);
    });

});