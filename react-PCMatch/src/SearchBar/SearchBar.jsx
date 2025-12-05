import { useState } from "react";
import { categories } from "../../data/categories";

export default function SearchBar({ onSearch }) {
  const [category, setCategory] = useState("cpu");
  const [query, setQuery] = useState("");

  const handleSearch = () => {
    onSearch(category, query);
  };

  return (
    <div className="search-bar">
      <select value={category} onChange={(e) => setCategory(e.target.value)}>
        {categories.map((c) => (
          <option key={c.id} value={c.id}>{c.name}</option>
        ))}
      </select>

      <input
        type="text"
        placeholder="Поиск..."
        value={query}
        onChange={(e) => setQuery(e.target.value)}
      />

      <button onClick={handleSearch}>Найти</button>
    </div>
  );
}
