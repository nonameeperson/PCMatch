import './style.css';
import { getComponents, getRecommendation } from './api.js';

// --- Логіка Пошуку ---
document.querySelector('#searchButton').addEventListener('click', async () => {
    const type = document.querySelector('#componentType').value;
    const container = document.querySelector("#searchResults");
    
    container.innerHTML = "Завантаження...";
    try {
        const items = await getComponents(type);
        if(items.length === 0) {
            container.innerHTML = "Нічого не знайдено.";
            return;
        }
        container.innerHTML = items.map(item => `
            <div style="background: white; padding: 10px; border-radius: 5px; border: 1px solid #ddd;">
                <strong>${item.name}</strong> <br>
                Ціна: ${item.price} грн | Тип: ${item.componentType}
            </div>
        `).join('');
    } catch (e) {
        container.innerHTML = `<span style="color:red">Помилка: ${e.message}</span>`;
    }
});

// --- Логіка Автопідбору ---
document.querySelector('#recommendButton').addEventListener('click', async () => {
    const budget = document.querySelector('#budgetInput').value;
    const purpose = document.querySelector('#purposeInput').value;
    const container = document.querySelector("#recommendResult");

    container.innerHTML = "ШІ підбирає найкращу збірку...";
    
    try {
        const data = await getRecommendation(purpose, budget);
        
        // Формуємо красивий список
        let html = `<h3>Збірка готова! (Загалом: ${data.totalPrice} грн)</h3><ul>`;
        
        // Перебираємо отримані компоненти
        for (const [key, part] of Object.entries(data.build)) {
            if(part) {
                html += `<li><b>${key}:</b> ${part.name} — ${part.price} грн</li>`;
            } else {
                html += `<li style="color:red"><b>${key}:</b> Не вистачило бюджету :(</li>`;
            }
        }
        html += `</ul><p>Залишок грошей: ${data.remainingBudget} грн</p>`;
        
        container.innerHTML = html;
    } catch (e) {
        container.innerHTML = `
            <div style="background: #ffcdd2; padding: 10px; border-radius: 5px; color: #b71c1c;">
                <b>Помилка підбору:</b> ${e.message}
            </div>`;
    }
});