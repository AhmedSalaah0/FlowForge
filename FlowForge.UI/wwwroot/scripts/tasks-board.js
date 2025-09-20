document.addEventListener("DOMContentLoaded", function () {
    function attachEditable(h5) {
        h5.addEventListener("click", function () {
            const sectionId = this.dataset.sectionId;
            const projectId = this.dataset.projectId;
            const text = this.innerText;

            const input = document.createElement("input");
            input.type = "text";
            input.value = text;
            input.className = "form-control form-control-sm";
            input.style.width = "auto";

            this.replaceWith(input);
            input.focus();

            const save = () => {
                const newName = input.value.trim();
                if (newName && newName !== text) {
                    fetch(`/Sections/EditSectionName`, {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
                        },
                        body: JSON.stringify({
                            SectionId: sectionId,
                            ProjectId: projectId,
                            SectionName: newName
                        })
                    })
                        .then(res => res.json())
                        .then(data => {
                            if (data.success) {
                                input.replaceWith(createH5(newName, sectionId, projectId));
                            } else {
                                alert("Error updating section name");
                                input.replaceWith(createH5(text, sectionId, projectId));
                            }
                        });
                } else {
                    input.replaceWith(createH5(text, sectionId, projectId));
                }
            };

            input.addEventListener("blur", save);
            input.addEventListener("keydown", e => {
                if (e.key === "Enter") {
                    save();
                }
            });
        });
    }

    function createH5(text, sectionId, projectId) {
        const h5 = document.createElement("h5");
        h5.className = "text-info m-0 editable-section";
        h5.dataset.sectionId = sectionId;
        h5.dataset.projectId = projectId;
        h5.innerText = text;
        attachEditable(h5);
        return h5;
    }

    document.querySelectorAll(".editable-section").forEach(attachEditable);

    var dropdownElementList = [].slice.call(document.querySelectorAll('[data-bs-toggle="dropdown"]'));
    var dropdownList = dropdownElementList.map(function (dropdownToggleEl) {
        return new bootstrap.Dropdown(dropdownToggleEl);
    });

    document.querySelectorAll('.status-option').forEach(button => {
        button.addEventListener('click', function () {
            const taskId = this.dataset.taskId;
            const projectId = this.dataset.projectId;
            const newStatus = this.dataset.status;

            const taskCard = document.querySelector(`.task-card[data-task-id="${taskId}"]`);
            if (!taskCard) {
                console.warn('task-card not found for', taskId);
                return;
            }

            const checkbox = taskCard.querySelector(".form-check-input");
            const text = taskCard.querySelector(".fw-semibold");
            const dropdownBtn = document.getElementById(`selectedStatus_${taskId}`) || taskCard.querySelector("[id^='selectedStatus']");
            const spans = dropdownBtn ? dropdownBtn.querySelectorAll("span") : [];
            const dotSpan = spans[0];
            const textSpan = spans[1];

            document.querySelectorAll(`.status-option[data-task-id="${taskId}"]`).forEach(opt => opt.classList.remove('active'));
            this.classList.add('active');

            fetch('/Tasks/taskState', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                },
                body: JSON.stringify({ taskId, projectId, status: newStatus })
            })
                .then(response => {
                    if (response.ok) {
                        if (newStatus === "Completed") {
                            text.classList.add("text-decoration-line-through");
                            text.style.color = "#979494d1";
                            dotSpan.style.backgroundColor = "#198754";
                            textSpan.textContent = "Completed";
                            checkbox.checked = true;
                        } else if (newStatus === "Pending") {
                            dotSpan.style.backgroundColor = "#ffc107";
                            textSpan.textContent = "Pending";
                            text.classList.remove("text-decoration-line-through");
                            text.classList.remove("text-secondary")
                            text.style.color = "#ffffff";
                            checkbox.checked = false;
                        } else {
                            textSpan.textContent = "In Progress";
                            dotSpan.style.backgroundColor = "#0d6efd";
                            text.classList.remove("text-decoration-line-through");
                            text.classList.remove("text-secondary")
                            text.style.color = "#ffffff";
                            checkbox.checked = false;
                        }
                    } else {
                        alert("Failed to update task status.");
                    }
                });
        });
    });

    document.body.addEventListener("change", function (e) {
        if (e.target.classList.contains("form-check-input")) {
            const taskId = e.target.dataset.taskId;
            const projectId = e.target.dataset.projectId;
            const status = e.target.dataset.status == "Completed" ? "Pending" : "Completed";
            const taskCard = document.querySelector(`.task-card[data-task-id="${taskId}"]`);
            const checkbox = taskCard.querySelector(".form-check-input");
            const text = taskCard.querySelector(".fw-semibold");
            const dropdownBtn = document.getElementById(`selectedStatus_${taskId}`) || taskCard.querySelector("[id^='selectedStatus']");
            const spans = dropdownBtn ? dropdownBtn.querySelectorAll("span") : [];
            const dotSpan = spans[0];
            const textSpan = spans[1];
            fetch("/Tasks/TaskState", {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                },
                body: JSON.stringify({
                    taskId,
                    projectId,
                    status
                })
            })
                .then(response => {
                    if (response.ok) {
                        if (status === "Completed") {
                            text.classList.add("text-decoration-line-through");
                            text.style.color = "#979494d1";
                            dotSpan.style.backgroundColor = "#198754";
                            textSpan.textContent = "Completed";
                            checkbox.checked = true;
                        }
                        else {
                            dotSpan.style.backgroundColor = "#ffc107";
                            textSpan.textContent = "Pending";
                            text.classList.remove("text-decoration-line-through");
                            text.classList.remove("text-secondary")
                            text.style.color = "#ffffff";
                            checkbox.checked = false;
                        }
                        e.target.dataset.status = status;
                    }
                })
        }
    })
    document.body.addEventListener('click', function (e) {
        if (e.target.classList.contains('assignee-option')) {
            const taskId = e.target.dataset.taskId;
            const projectId = e.target.dataset.projectId;
            const assigneeId = e.target.dataset.assigneeId;
            const assigneeSpan = document.querySelector(`#Assignee-span-${taskId}`);

            e.target.closest('ul').querySelectorAll('.assignee-option').forEach(opt => opt.classList.remove('active'));
            e.target.classList.add('active');

            fetch('/Tasks/Assign', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                },
                body: JSON.stringify({
                    taskId,
                    projectId,
                    memberId: assigneeId
                })
            })
                .then(response => {
                    if (response.ok) {
                        assigneeSpan.textContent = e.target.textContent.trim();
                    } else {
                        alert("Failed to assign task.");
                    }
                });
        }
    });


    document.querySelectorAll(".task-list").forEach(list => {
        new Sortable(list, {
            group: "tasks",
            animation: 100,
            delay: 0,
            delayOnTouchStart: false,
            forceFallback: false,
            fallbackTolerance: 0,
            dragoverBubble: false,
            removeCloneOnHide: true,
            preventOnFilter: false,

            onStart: function (evt) {
                evt.item.style.opacity = "0.6";
                evt.item.style.transform = "rotate(5deg)";
            },

            onEnd: async function (evt) {
                evt.item.style.opacity = "1";
                evt.item.style.transform = "none";

                const taskId = evt.item.dataset.taskId;
                const oldSectionId = evt.from.dataset.sectionId;
                const newSectionId = evt.to.dataset.sectionId;
                const projectId = evt.to.dataset.projectId;

                const prevTask = evt.item.previousElementSibling;
                const nextTask = evt.item.nextElementSibling;

                const prevTaskId = prevTask ? prevTask.dataset.taskId : null;
                const nextTaskId = nextTask ? nextTask.dataset.taskId : null;

                try {
                    const response = await fetch("/Tasks/ReorderTasks", {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
                        },
                        body: JSON.stringify({
                            SectionId: newSectionId,
                            ProjectId: projectId,
                            TaskId: taskId,
                            PrevTaskId: prevTaskId,
                            NextTaskId: nextTaskId
                        })
                    });

                    if (!response.ok) throw new Error("Failed to reorder tasks.");
                    console.log(`✅ Reorder saved: Task ${taskId}, prev=${prevTaskId}, next=${nextTaskId}`);
                } catch (err) {
                    alert(err.message);
                }
            }
        });
    });
    document.querySelectorAll('.task-card .dropdown').forEach(dropdown => {
        const toggle = dropdown.querySelector('[data-bs-toggle="dropdown"]');
        const menu = dropdown.querySelector('.dropdown-menu');
        if (!toggle || !menu) return;

        let placeholder = null;

        dropdown.addEventListener('show.bs.dropdown', () => {
            placeholder = document.createComment('dropdown-placeholder');
            menu.parentNode.insertBefore(placeholder, menu);

            document.body.appendChild(menu);
            menu.style.position = 'absolute';
            menu.style.zIndex = '99999';
            menu.style.minWidth = Math.max(toggle.offsetWidth, menu.offsetWidth) + 'px';
        });

        dropdown.addEventListener('shown.bs.dropdown', () => {
            const r = toggle.getBoundingClientRect();
            const menuRect = menu.getBoundingClientRect();

            let left = r.left + window.scrollX;
            let top = r.bottom + window.scrollY;

            if (left + menuRect.width > window.innerWidth) {
                left = Math.max(window.innerWidth - menuRect.width - 8, 8);
            }
            if (top + menuRect.height > window.scrollY + window.innerHeight) {
                top = r.top + window.scrollY - menuRect.height;
            }

            menu.style.left = left + 'px';
            menu.style.top = top + 'px';
        });

        dropdown.addEventListener('hide.bs.dropdown', () => {
            if (placeholder) {
                placeholder.parentNode.insertBefore(menu, placeholder);
                placeholder.remove();
                placeholder = null;
                menu.style.position = '';
                menu.style.left = '';
                menu.style.top = '';
                menu.style.zIndex = '';
                menu.style.minWidth = '';
            }
        });
    });
});