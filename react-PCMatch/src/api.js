// Получение компонентов
export async function getComponents(type, minPrice, maxPrice) {
    const url = new URL('/api/build/components', window.location.origin);

    if (type) url.searchParams.append('type', type);
    if (minPrice) url.searchParams.append('minPrice', minPrice);
    if (maxPrice) url.searchParams.append('maxPrice', maxPrice);

    const res = await fetch(url);
    return res.json();
}

// Проверка совместимости сборки
export async function validateBuild(cpuId, motherboardId, gpuId, psuId) {
    const url = new URL('/api/build/validate', window.location.origin);

    url.searchParams.append('cpuId', cpuId);
    url.searchParams.append('motherboardId', motherboardId);

    if (gpuId) url.searchParams.append('gpuId', gpuId);
    if (psuId) url.searchParams.append('psuId', psuId);

    const res = await fetch(url);
    return res.json();
}

// Прогноз FPS
export async function getFps(cpuId, gpuId) {
    const url = new URL('/api/build/forecast', window.location.origin);

    url.searchParams.append('cpuId', cpuId);
    url.searchParams.append('gpuId', gpuId);

    const res = await fetch(url);
    return res.json();
}