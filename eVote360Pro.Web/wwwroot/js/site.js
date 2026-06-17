// Utilidades Globales de JS

document.addEventListener('DOMContentLoaded', () => {
    
    // Inicializar alertas con auto-cierre opcional
    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(alert => {
        const closeBtn = alert.querySelector('.btn-close-alert');
        if (closeBtn) {
            closeBtn.addEventListener('click', () => {
                alert.style.opacity = '0';
                setTimeout(() => alert.remove(), 300);
            });
        }
    });

    // Auto-uppercase para siglas
    const uppercaseInputs = document.querySelectorAll('.input-uppercase');
    uppercaseInputs.forEach(input => {
        input.addEventListener('input', function() {
            this.value = this.value.toUpperCase();
        });
    });
});

// Modal de confirmación global
window.showConfirmModal = function(title, message, formAction, btnClass = 'bg-red-600 hover:bg-red-700') {
    const modal = document.getElementById('confirmModal');
    const titleEl = document.getElementById('confirmModalTitle');
    const messageEl = document.getElementById('confirmModalMessage');
    const form = document.getElementById('confirmModalForm');
    const confirmBtn = document.getElementById('confirmModalBtn');

    if(modal && titleEl && messageEl && form && confirmBtn) {
        titleEl.textContent = title;
        messageEl.textContent = message;
        form.action = formAction;
        
        // Reset button classes and apply new
        confirmBtn.className = `px-4 py-2 text-white font-medium rounded-lg transition-colors shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2 ${btnClass}`;
        
        modal.classList.remove('hidden');
        modal.classList.add('flex');
    }
};

window.closeConfirmModal = function() {
    const modal = document.getElementById('confirmModal');
    if(modal) {
        modal.classList.add('hidden');
        modal.classList.remove('flex');
    }
};
