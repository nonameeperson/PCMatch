import "./style.css";
import { getComponents, validateBuild, getRecommendation } from "./api.js";

const ITEMS_PER_PAGE = 5;

let currentPage = 1;
let currentComponents = [];
let currentBuild = {
  Cpu: null,
  Motherboard: null,
  Gpu: null,
  Psu: null,
  Ram: null,
  Storage: null,
  Case: null,
};

function formatPrice(price) {
  return `${price} $`;
}

function renderList() {
  const container = document.querySelector("#searchResults");
  const start = (currentPage - 1) * ITEMS_PER_PAGE;
  const end = start + ITEMS_PER_PAGE;
  const pageItems = currentComponents.slice(start, end);

  if (pageItems.length === 0) {
    container.innerHTML =
      "<p style='text-align:center; color:#888'>Нічого не знайдено.</p>";
    document.getElementById("paginationControls").style.display = "none";
    return;
  }

  container.innerHTML = pageItems
    .map(
      (item) => `
        <div class="component-item">
            <div>
                <strong style="font-size: 1.1em; color: white;">${
                  item.name
                }</strong>
                
                <div style="color: #ccc; font-size: 0.9em; margin: 4px 0;">
                    ${item.description || "Характеристики відсутні"}
                </div>
                
                <div style="color: #aaa; font-size: 0.8em;">
                    Тип: ${item.componentType}
                </div>
                <div class="price-tag">${formatPrice(item.price)}</div>
            </div>
            <button class="btn-add" 
                onclick="window.addToBuild('${item.id}', '${
        item.componentType
      }', '${item.name.replace(/'/g, "\\'")}', ${item.price})">
                + Додати
            </button>
        </div>
    `
    )
    .join("");

  document.getElementById("paginationControls").style.display = "block";
  document.getElementById(
    "pageIndicator"
  ).innerText = `Сторінка ${currentPage}`;
  document.getElementById("prevPage").disabled = currentPage === 1;
  document.getElementById("nextPage").disabled =
    end >= currentComponents.length;
}

function renderBuild() {
  const list = document.getElementById("currentBuildList");
  let total = 0;
  let html = "";

  for (const [type, item] of Object.entries(currentBuild)) {
    if (item) {
      total += item.price;
      html += `
                <li>
                    <span><strong style="color:#aaa">${type}:</strong> ${
        item.name
      }</span>
                    <span>
                        ${formatPrice(item.price)}
                        <span class="remove-btn" onclick="window.removeFromBuild('${type}')">✖</span>
                    </span>
                </li>`;
    } else {
      html += `<li style="color: #555;">${type}: —</li>`;
    }
  }

  list.innerHTML = html;
  document.getElementById("totalBuildPrice").innerText = formatPrice(total);
  document.getElementById("validationResult").innerHTML = "";
}

window.addToBuild = (id, type, name, price) => {
  let key = Object.keys(currentBuild).find(
    (k) => k.toLowerCase() === type.toLowerCase()
  );
  if (key) {
    currentBuild[key] = { id, name, price };
    renderBuild();
  } else {
    alert(`Невідомий тип компонента: ${type}`);
  }
};

window.removeFromBuild = (type) => {
  currentBuild[type] = null;
  renderBuild();
};

document.querySelector("#searchButton").addEventListener("click", async () => {
  const type = document.querySelector("#componentType").value;
  const name = document.querySelector("#searchName").value;
  const min = document.querySelector("#minPrice").value;
  const max = document.querySelector("#maxPrice").value;

  const container = document.querySelector("#searchResults");
  container.innerHTML = "<p style='text-align:center'>Завантаження...</p>";

  try {
    currentComponents = await getComponents(type, min, max, name);
    currentPage = 1;
    renderList();
  } catch (e) {
    container.innerHTML = `<p style="color: #ff5252;">Помилка: ${e.message}</p>`;
  }
});

document.querySelector("#prevPage").addEventListener("click", () => {
  if (currentPage > 1) {
    currentPage--;
    renderList();
  }
});
document.querySelector("#nextPage").addEventListener("click", () => {
  if (currentPage * ITEMS_PER_PAGE < currentComponents.length) {
    currentPage++;
    renderList();
  }
});

document
  .querySelector("#validateButton")
  .addEventListener("click", async () => {
    const resDiv = document.getElementById("validationResult");
    resDiv.innerHTML = "Перевірка...";

    if (!currentBuild.Cpu || !currentBuild.Motherboard) {
      resDiv.innerHTML =
        "<span style='color:orange'>⚠️ Потрібні CPU та Motherboard.</span>";
      return;
    }

    try {
      const result = await validateBuild(
        currentBuild.Cpu.id,
        currentBuild.Motherboard.id,
        currentBuild.Gpu?.id,
        currentBuild.Psu?.id
      );

      if (result.isValid) {
        resDiv.innerHTML = `<div style="color: #4CAF50; font-weight: bold; background: #1b3a1b; padding: 10px; border-radius: 6px;">${result.messages[0]}</div>`;
      } else {
        resDiv.innerHTML = `<div style="color: #ff5252; background: #3a1b1b; padding: 10px; border-radius: 6px;">Проблеми:<ul>${result.messages
          .map((m) => `<li>${m}</li>`)
          .join("")}</ul></div>`;
      }
    } catch (e) {
      resDiv.innerHTML = "Помилка сервера.";
    }
  });

document
  .querySelector("#recommendButton")
  .addEventListener("click", async () => {
    const budget = document.querySelector("#budgetInput").value;
    const purpose = document.querySelector("#purposeInput").value;
    const container = document.querySelector("#recommendResult");

    if (!budget) {
      alert("Введіть бюджет!");
      return;
    }

    container.innerHTML = "Підбір...";

    try {
      const data = await getRecommendation(purpose, budget);

      let html = `<ul class="build-list">`;
      for (const [key, part] of Object.entries(data.build)) {
        if (part)
          html += `<li><strong style="color:#aaa">${key}:</strong> ${
            part.name
          } <span style="color:#4CAF50">${formatPrice(part.price)}</span></li>`;
      }
      html += `</ul>`;
      container.innerHTML = html;
    } catch (e) {
      container.innerHTML = `<span style="color:#ff5252">${e.message}</span>`;
    }
  });

renderBuild();