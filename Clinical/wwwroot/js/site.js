// site.js - simple client-side demo auth (localStorage)
// NOTE: Replace with server-side auth in production.

(function ($) {
    'use strict';

    const storageKey = 'cttms_demo_user';

    // on DOM ready
    $(function () {
        // animate hero blocks
        $('.hero-cta, .device-mockup, .card').addClass('fade-up');
        setTimeout(() => { $('.fade-up').addClass('in'); }, 100);

        // restore user nav state
        refreshUserNav();

        // login form submit (demo)
        $('#loginForm').on('submit', function (e) {
            e.preventDefault();
            // basic validation
            const email = $('#loginEmail').val().trim();
            const pwd = $('#loginPassword').val().trim();

            if (!email || !pwd) {
                $('#loginForm').addClass('was-validated');
                return;
            }

            // For demo: accept any credentials; set a "user" object
            const name = email.split('@')[0].replace('.', ' ').replace('-', ' ');
            const user = {
                name: capitalize(name),
                email: email,
                role: (email.toLowerCase() === 'admin@gmail.com') ? 'Admin' : 'User',
                loggedInAt: new Date().toISOString()
            };

            // Save to localStorage as 'session'
            localStorage.setItem(storageKey, JSON.stringify(user));

            // update nav UI
            $('#loginModal').modal('hide');
            setTimeout(() => {
                refreshUserNav();
                // optional: toast or welcome
                showToast(`Welcome back, ${user.name}!`);
            }, 300);
        });

        // logout btn
        $('#logoutBtn').on('click', function (e) {
            e.preventDefault();
            localStorage.removeItem(storageKey);
            refreshUserNav();
            showToast('Logged out');
        });

        // If login button is present in content, open modal
        $('#openLoginBtn').on('click', function () {
            $('#loginModal').modal('show');
        });
    });

    function refreshUserNav() {
        const stored = localStorage.getItem(storageKey);
        if (stored) {
            try {
                const u = JSON.parse(stored);
                $('#navWelcome').text(`Welcome, ${u.name}`);
                $('#navbarUserMenu').removeClass('d-none');
                $('#loginNavItem').addClass('d-none');
            } catch (e) {
                console.error('Invalid user in storage');
                localStorage.removeItem(storageKey);
            }
        } else {
            $('#navbarUserMenu').addClass('d-none');
            $('#loginNavItem').removeClass('d-none');
        }
    }

    function showToast(message) {
        // quick temporary toast using Bootstrap toast markup
        const toastId = 'cttmsToast';
        let toastEl = $(`#${toastId}`);
        if (toastEl.length === 0) {
            const toastHtml = `
            <div id="${toastId}" class="toast align-items-center text-bg-dark border-0 position-fixed bottom-0 end-0 m-3" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">${message}</div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>`;
            $('body').append(toastHtml);
            toastEl = $(`#${toastId}`);
        } else {
            toastEl.find('.toast-body').text(message);
        }

        const bsToast = new bootstrap.Toast(toastEl[0], { delay: 3000 });
        bsToast.show();
    }

    function capitalize(s) {
        return s.split(' ').map(p => p.charAt(0).toUpperCase() + p.slice(1)).join(' ');
    }

})(jQuery);
