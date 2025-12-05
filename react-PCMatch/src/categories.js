let selectedCategory = null;

document.querySelectorAll('.category-btn').forEach(btn => {
    btn.addEventListener('click', () => {
        selectedCategory = btn.dataset.category;
        console.log("Selected category:", selectedCategory);
    });
});

export function getSelectedCategory() {
    return selectedCategory;
}