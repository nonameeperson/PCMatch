import { useState } from "react";
import SearchBar from "../components/SearchBar/SearchBar";
import { searchComponents } from "../api/searchApi";

export default function ComponentsPage() {
  const [results, setResults] = useState([]);

  const handleSearch = async (category, query) => {
    const data = await searchComponents(category, query);
    setResults(data);
  };

  return (
    <div>
      <SearchBar onSearch={handleSearch} />

      <div className="results">
        {results.map(item => (
          <div key={item.id} className="component-card">
            <h3>{item.name}</h3>
            <p>Категория: {item.category}</p>
            <p>Цена: {item.price}$</p>
          </div>
        ))}
      </div>
    </div>
  );
}