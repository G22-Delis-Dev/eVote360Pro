// ========================================
// eVote360 Pro - Global JavaScript
// ========================================

document.addEventListener('DOMContentLoaded', () => {

    // ---- Alert auto-dismiss ----
    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(alert => {
        // Auto close after 6 seconds
        const timer = setTimeout(() => {
            dismissAlert(alert);
        }, 6000);

        const closeBtn = alert.querySelector('.alert-close');
        if (closeBtn) {
            closeBtn.addEventListener('click', () => {
                clearTimeout(timer);
                dismissAlert(alert);
            });
        }
    });

    function dismissAlert(el) {
        el.style.opacity = '0';
        el.style.transform = 'translateY(-8px)';
        el.style.transition = 'opacity 0.25s ease, transform 0.25s ease';
        setTimeout(() => el.remove(), 250);
    }

    // ---- Auto-uppercase for siglas inputs ----
    const uppercaseInputs = document.querySelectorAll('.input-uppercase');
    uppercaseInputs.forEach(input => {
        input.addEventListener('input', function () {
            this.value = this.value.toUpperCase();
        });
    });

    // ---- Sidebar mobile toggle ----
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('sidebar');
    const sidebarOverlay = document.getElementById('sidebarOverlay');

    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener('click', () => {
            openSidebar();
        });
    }

    if (sidebarOverlay) {
        sidebarOverlay.addEventListener('click', () => {
            closeSidebar();
        });
    }

    function openSidebar() {
        if (!sidebar) return;
        sidebar.classList.add('open');
        if (sidebarOverlay) sidebarOverlay.classList.add('active');
        document.body.style.overflow = 'hidden';
    }

    function closeSidebar() {
        if (!sidebar) return;
        sidebar.classList.remove('open');
        if (sidebarOverlay) sidebarOverlay.classList.remove('active');
        document.body.style.overflow = '';
    }

    // Close sidebar on Escape key
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape' && sidebar && sidebar.classList.contains('open')) {
            closeSidebar();
        }
    });
});

// ========================================
// Password visibility toggle
// ========================================

window.togglePasswordVisibility = function (inputId, btn) {
    const input = document.getElementById(inputId);
    if (!input) return;

    const isPassword = input.type === 'password';
    input.type = isPassword ? 'text' : 'password';

    const eyeOpen = btn.querySelector('.icon-eye-open');
    const eyeClosed = btn.querySelector('.icon-eye-closed');

    if (eyeOpen && eyeClosed) {
        eyeOpen.style.display = isPassword ? 'none' : 'block';
        eyeClosed.style.display = isPassword ? 'block' : 'none';
    }
};

// ========================================
// Confirm Modal
// ========================================

window.showConfirmModal = function (title, message, formAction, btnClass) {
    const modal = document.getElementById('confirmModal');
    const titleEl = document.getElementById('confirmModalTitle');
    const messageEl = document.getElementById('confirmModalMessage');
    const form = document.getElementById('confirmModalForm');
    const confirmBtn = document.getElementById('confirmModalBtn');

    if (modal && titleEl && messageEl && form && confirmBtn) {
        titleEl.textContent = title;
        messageEl.textContent = message;
        form.action = formAction;

        // Reset button styling
        confirmBtn.className = 'btn';
        if (btnClass && btnClass.includes('green')) {
            confirmBtn.classList.add('btn-success');
        } else {
            confirmBtn.classList.add('btn-danger');
        }

        modal.classList.add('active');
    }
};

window.closeConfirmModal = function () {
    const modal = document.getElementById('confirmModal');
    if (modal) {
        modal.classList.remove('active');
    }
};

// Close modal on backdrop click
document.addEventListener('click', (e) => {
    const modal = document.getElementById('confirmModal');
    if (modal && e.target === modal) {
        closeConfirmModal();
    }
});

// Close modal on Escape
document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') {
        closeConfirmModal();
    }
});
