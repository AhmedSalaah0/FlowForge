document.addEventListener('DOMContentLoaded', () => {
    const profileMenu = document.getElementById('profileMenu');
    const dropdownTrigger = document.getElementById('profileDropdown');
    const nameSpan = document.getElementById('name-span');

    fetch('/Account/me', {
        method: 'GET',
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'Accept': 'text/html'
        }
    })
        .then(response => {
            console.log(`Resopnse status: ${response.status}`);
            if (!response.ok) {
                throw new Error(`Http Error status`);
            }
            return response.text();
        })
        .then(html => {
            if (profileMenu) {
                profileMenu.innerHTML = html;
                const parser = new DOMParser();
                const doc = parser.parseFromString(html, "text/html");

                const userNameElement = doc.querySelector('#user-name');
                if (userNameElement) {
                    const firstName = userNameElement.textContent.trim().split(' ')[0];
                    nameSpan.innerHTML = `<i class="fa-solid fa-person me-1"></i> ${firstName}`;
                }
            }
            if (dropdownTrigger) {
                try { new bootstrap.Dropdown(dropdownTrigger); } catch (e) {
                    console.log('Dropdown already initialized or error:', e);
                }
            }
        })
        .catch(error => {
            console.error('Failed to load user data:', error);
            if (notificationMenu) {
                notificationMenu.innerHTML = '<div class="text-center text-muted py-2">Failed to load user data</div>';
            }
        });
})