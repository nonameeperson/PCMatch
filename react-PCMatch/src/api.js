export async function getComponents(type, minPrice, maxPrice, search) {
  const url = new URL("/api/build/components", window.location.origin);

  if (type) url.searchParams.append("type", type);
  if (minPrice) url.searchParams.append("minPrice", minPrice);
  if (maxPrice) url.searchParams.append("maxPrice", maxPrice);
  if (search) url.searchParams.append("search", search);

  const res = await fetch(url);
  if (!res.ok) throw new Error("Failed to fetch components");
  return res.json();
}

export async function getRecommendation(purpose, budget) {
  const url = new URL("/api/build/recommend", window.location.origin);
  url.searchParams.append("purpose", purpose);
  url.searchParams.append("budget", budget);

  const res = await fetch(url);
  if (!res.ok) {
    const errorText = await res.text();
    throw new Error(errorText);
  }
  return res.json();
}

export async function validateBuild(cpuId, motherboardId, gpuId, psuId) {
  const url = new URL("/api/build/validate", window.location.origin);

  url.searchParams.append("cpuId", cpuId);
  url.searchParams.append("motherboardId", motherboardId);
  if (gpuId) url.searchParams.append("gpuId", gpuId);
  if (psuId) url.searchParams.append("psuId", psuId);

  const res = await fetch(url);
  return res.json();
}

export async function getFps(cpuId, gpuId) {
  const url = new URL("/api/build/forecast", window.location.origin);

  url.searchParams.append("cpuId", cpuId);
  url.searchParams.append("gpuId", gpuId);

  const res = await fetch(url);
  return res.json();
}
