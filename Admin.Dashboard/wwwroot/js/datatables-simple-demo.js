/*
 * Library Admin Table & Filters Helper 
 * ------------------------------------
 * Adapted for Server-Side Paginated & Filtered Architecture.
 * Enhances admin lists with auto-submitting filters and UI tooltips.
 */

window.addEventListener('DOMContentLoaded', event => {

    // 1. Auto-submit filter forms when select dropdowns change (e.g., Status, Sorting, PageSize)
    // This saves admin clicks and gives an instant response feeling.
    const filterForms = document.querySelectorAll('form.lib-filters, #filterForm');

    filterForms.forEach(form => {
        const selects = form.querySelectorAll('select');
        selects.forEach(select => {
            select.addEventListener('change', () => {
                form.submit();
            });
        });
    });

    // 2. Initialize Bootstrap Tooltips across admin pages for better UX
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // 3. Fallback check: If an old static client-side table is accidentally present, 
    // handle it safely without throwing script errors.
    const datatablesSimple = document.getElementById('datatablesSimple');
    if (datatablesSimple && typeof simpleDatatables !== 'undefined') {
        new simpleDatatables.DataTable(datatablesSimple);
    }

});