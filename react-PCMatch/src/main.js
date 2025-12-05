import './style.css';
import viteLogo from '/logo.svg';
import { getComponents } from './api.js';

document.querySelector('#app').innerHTML = `
  <div>
      <img src="${viteLogo}" class="logo" alt="Vite logo" />
      <h1>Hello, and wellcome member!</h1>
      <p class="read-the-docs">
        Це ваш кабінет, натисніть "Пошук..." щоб почати збірку
      </p>
  </div>

  <div class="search-container">
    <input type="text" id="searchInput" placeholder="Поиск...">

    <select id="categorySelect1">
      <option value="">— Компонент —</option>
      <option value="CPU">Процессоры</option>
      <option value="GPU">Видеокарты</option>
      <option value="RAM">Оперативная память</option>
      <option value="Motherboard">Материнские платы</option>
      <option value="PSU">Блоки питания</option>
      <option value="Storage">SSD</option>
    </select>

    <select id="categorySelect2">
      <option value="">— Цель —</option>
      <option value="Games">Для ігор</option>
      <option value="Office">Для офісу</option>
      <option value="Editing">Для монтажу</option>
    </select>

    <button id="searchButton">Найти</button>
  </div>

  <div id="results"></div>
`;

document.querySelector('#searchButton').addEventListener('click', async () => {
    const searchText = document.querySelector('#searchInput').value;
    const componentType = document.querySelector('#categorySelect1').value;
    const purpose = document.querySelector('#categorySelect2').value;

    console.log("Searching:", searchText, componentType, purpose);

    const resultsDiv = document.querySelector("#results");
    resultsDiv.innerHTML = "Загрузка...";

    // Запрос в BuildController
    const components = await getComponents(componentType);

    console.log("Components from API:", components);

    resultsDiv.innerHTML = components.length === 0
        ? "<p>Ничего не найдено</p>"
        : components.map(c => `
            <div class="component-card">
                <h3>${c.name}</h3>
                <p>Тип: ${c.componentType}</p>
                <p>Цена: ${c.price} грн</p>
            </div>
          `).join('');
});