document.addEventListener('DOMContentLoaded', () => {
    if ("Notification" in window) {
        if (Notification.permission !== "granted" && Notification.permission !== "denied") {
            Notification.requestPermission();
        }
    }
    const notificationMenu = document.getElementById('notificationMenu');
    const dropdownTrigger = document.getElementById('notificationDropdown');

    fetch('/Notification', {
        method: 'GET',
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'Accept': 'text/html'
        }
    })
        .then(response => {
            console.log('Response status:', response.status);
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.text();
        })
        .then(html => {
            if (notificationMenu)
                notificationMenu.innerHTML = html;
            const unreadInput = document.getElementById("hiddenUnreadCount");

            if (unreadInput) {
                const unread = parseInt(unreadInput.value || "0");
                const badge = document.getElementById("unreadCount");
                if (badge) {
                    if (unread > 0) {
                        badge.textContent = unread;
                        badge.classList.remove("d-none");
                    } else {
                        badge.classList.add("d-none");
                    }
                }
            }
            if (dropdownTrigger) {
                try { new bootstrap.Dropdown(dropdownTrigger); } catch (e) {
                    console.log('Dropdown already initialized or error:', e);
                }

                dropdownTrigger.addEventListener('show.bs.dropdown', () => {
                    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
                    const token = tokenInput ? tokenInput.value : null;
                    console.log('Token for marking all as read:', token);
                    fetch('/Notification/MarkAllAsRead', {
                        method: 'POST',
                        headers: {
                            "RequestVerificationToken": token
                        }
                    })
                        .then(res => {
                            if (!res.ok) throw new Error("HTTP " + res.status);
                            document.getElementById('unreadCount')?.classList.add('d-none');

                        })
                        .catch(err => console.error("Error marking notifications read", err));
                });
            }
        })
        .catch(error => {
            console.error('Failed to load notifications:', error);
            if (notificationMenu) {
                notificationMenu.innerHTML = '<div class="text-center text-secondary py-2">Failed to load notifications</div>';
            }
        });

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notification-hub", {
            skipNegotiation: false,
            transport: signalR.HttpTransportType.WebSockets
        })
        .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.onreconnecting(error => {
        console.log("SignalR reconnecting due to error:", error);
    });

    connection.onreconnected(connectionId => {
        console.log("SignalR reconnected with ID:", connectionId);
    });

    connection.onclose(error => {
        console.log("SignalR connection closed", error);
        setTimeout(() => startConnection(), 5000);
    });

    connection.on("ReceiveNotification", function (notification) {
        console.log("New notification received:", notification);

        const badge = document.getElementById("unreadCount");
        if (badge) {
            let current = parseInt(badge.textContent?.trim() || "0", 10);
            if (isNaN(current)) current = 0;
            let count = current + 1;

            badge.textContent = count.toString();
            badge.classList.remove("d-none");

            const noNotif = document.getElementById("no-notification");
            if (noNotif) {
                noNotif.classList.add("d-none");
            }

        }

        const menu = document.getElementById("notificationMenu");
        if (menu) {
            const item = document.createElement("div");
            item.className = "dropdown-item text-white border-bottom border-secondary notification-item unread";

            const messageDiv = document.createElement("div");
            messageDiv.className = "mb-1 fw-semibold text-info notification-message";
            messageDiv.textContent = notification.message || "New notification";

            const timeDiv = document.createElement("small");
            timeDiv.className = "text-light d-block mb-2 notification-time";

            const notificationTime = notification.createdAt ? new Date(notification.createdAt) : new Date();

            const desktopTime = document.createElement("span");
            desktopTime.className = "d-none d-md-inline";
            desktopTime.textContent = notificationTime.toLocaleString();

            const mobileTime = document.createElement("span");
            mobileTime.className = "d-md-none";
            mobileTime.textContent = notificationTime.toLocaleString([], { month: 'numeric', day: 'numeric', hour: 'numeric', minute: 'numeric', hour12: true });

            item.appendChild(messageDiv);
            item.appendChild(timeDiv);
            timeDiv.appendChild(desktopTime);
            timeDiv.appendChild(mobileTime);

            if (notification.notificationType === 0) {
                const actionsDiv = document.createElement("div");
                actionsDiv.className = "notification-actions";

                const acceptForm = document.createElement("form");
                acceptForm.method = "post";
                acceptForm.action = `/ProjectMembers/AcceptInvite/${notification.projectId}`;
                acceptForm.className = "d-inline";

                const acceptBtn = document.createElement("button");
                acceptBtn.type = "submit";
                acceptBtn.className = "btn btn-sm btn-outline-success action-btn";
                acceptBtn.innerHTML = `<i class="fas fa-check d-sm-none"></i>
                                                   <span class="d-none d-sm-inline">Accept</span>`;

                acceptForm.appendChild(acceptBtn);

                const tokenInput1 = document.querySelector('input[name="__RequestVerificationToken"]')?.cloneNode(true);
                if (tokenInput1) acceptForm.appendChild(tokenInput1);

                const rejectForm = document.createElement("form");
                rejectForm.method = "post";
                rejectForm.action = `/ProjectMembers/RejectInvite/${notification.projectId}`;
                rejectForm.className = "d-inline";

                const rejectBtn = document.createElement("button");
                rejectBtn.type = "submit";
                rejectBtn.className = "btn btn-sm btn-outline-danger action-btn";
                rejectBtn.innerHTML = `<i class="fas fa-times d-sm-none"></i>
                                                   <span class="d-none d-sm-inline">Reject</span>`;

                rejectForm.appendChild(rejectBtn);

                const tokenInput2 = document.querySelector('input[name="__RequestVerificationToken"]')?.cloneNode(true);
                if (tokenInput2) rejectForm.appendChild(tokenInput2);

                actionsDiv.appendChild(acceptForm);
                actionsDiv.appendChild(rejectForm);
                item.appendChild(actionsDiv);
            }

            if (menu.firstChild) {
                menu.insertBefore(item, menu.firstChild);
            } else {
                menu.appendChild(item);
            }

            if (Notification.permission === "granted") {
                new Notification("FlowForge", {
                    body: notification.message || "New notification",
                    icon: "/icon.png"
                });
            }
        }
    });
    function startConnection() {
        if (connection.state === signalR.HubConnectionState.Connected ||
            connection.state === signalR.HubConnectionState.Connecting) {
            console.log("SignalR connection already established or in progress");
            return;
        }

        connection.start()
            .then(() => {
                console.log("SignalR connected");
                if (Notification.permission === "default") {
                    Notification.requestPermission();
                }
            })
            .catch(err => {
                console.error("SignalR connection error:", err);
                const retryDelay = Math.min(5000 * Math.pow(2, connection.retryCount || 0), 30000);
                console.log(`Retrying connection in ${retryDelay / 1000} seconds`);
                setTimeout(() => {
                    connection.retryCount = (connection.retryCount || 0) + 1;
                    startConnection();
                }, retryDelay);
            });
    }

    startConnection();
});