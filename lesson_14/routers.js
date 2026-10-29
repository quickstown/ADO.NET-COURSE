let activeRequest = null;

async function loadStudents() {
    await loadTable('/students/all', 'studentsTable', [
        'studentId', 'firstName', 'lastName', 'age'
    ]);
}

async function loadGroups() {
    await loadTable('/groups/all', 'groupsTable', [
        'groupId', 'groupName'
    ]);
}

async function loadTable(url, tableId, fields) {
    // Отменяем предыдущий запрос при повторном нажатии или смене таблицы.
    if (activeRequest) activeRequest.abort();
    const controller = new AbortController();
    activeRequest = controller;

    ClearPage();
    const table = document.getElementById(tableId);
    const tbody = table.querySelector('tbody');
    const status = document.getElementById('loadStatus');
    tbody.replaceChildren();
    status.textContent = 'Загрузка…';

    try {
        const response = await fetch(url, { signal: controller.signal });
        if (!response.ok) {
            throw new Error(`Ошибка HTTP: ${response.status}`);
        }

        const items = await response.json();
        if (!Array.isArray(items) || items.some(item =>
            item === null || typeof item !== 'object' || Array.isArray(item)
        )) {
            throw new Error('Сервер вернул данные в неверном формате.');
        }

        // Старый ответ не должен изменять интерфейс после нового нажатия.
        if (activeRequest !== controller) return;

        const fragment = document.createDocumentFragment();
        for (const item of items) {
            const row = document.createElement('tr');
            for (const field of fields) {
                const cell = document.createElement('td');
                cell.textContent = item[field] ?? '';
                row.appendChild(cell);
            }
            fragment.appendChild(row);
        }

        tbody.replaceChildren(fragment);
        table.style.display = 'table';
        status.textContent = items.length === 0 ? 'Записей пока нет.' : '';
    } catch (error) {
        if (controller.signal.aborted || activeRequest !== controller) return;
        status.textContent = 'Не удалось загрузить данные. Попробуйте ещё раз.';
        console.error('Ошибка загрузки:', error);
    } finally {
        if (activeRequest === controller) activeRequest = null;
    }
}

function ClearPage() {
    document.getElementById('studentsTable').style.display = 'none';
    document.getElementById('groupsTable').style.display = 'none';
}
