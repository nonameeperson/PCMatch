export async function getComponents(type) {
    // URL тепер відносний, проксі (див. крок 1) перенаправить його куди треба
    const url = new URL('/api/build/components', window.location.origin);
    if (type) url.searchParams.append('type', type);

    const res = await fetch(url);
    if (!res.ok) throw new Error('Failed to fetch components');
    return res.json();
}

// Нова функція для автопідбору
export async function getRecommendation(purpose, budget) {
    const url = new URL('/api/build/recommend', window.location.origin);
    url.searchParams.append('purpose', purpose);
    url.searchParams.append('budget', budget);

    const res = await fetch(url);
    if (!res.ok) {
        // Читаємо текст помилки з бекенда (наприклад "Бюджет замалий")
        const errorText = await res.text();
        throw new Error(errorText);
    }
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