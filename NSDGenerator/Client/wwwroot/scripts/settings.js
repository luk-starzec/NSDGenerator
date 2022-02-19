function setThemeColor(hue, saturation, darkMode) {
    const element = document.getElementById('app');

    const lCorrection = darkMode ? -10 : 0;
    element.style.setProperty('--color-30', getHsl(hue, saturation, 30 + lCorrection));
    element.style.setProperty('--color-50', getHsl(hue, saturation, 50 + lCorrection));
    element.style.setProperty('--color-70', getHsl(hue, saturation, 70 + lCorrection));
    element.style.setProperty('--color-80', getHsl(hue, saturation, 80 + lCorrection));
    element.style.setProperty('--color-90', getHsl(hue, saturation, 90 + lCorrection));
    element.style.setProperty('--color-95', getHsl(hue, saturation, 95 + lCorrection));
    element.style.setProperty('--color-98', getHsl(hue, saturation, 98 + lCorrection));

    const lShadow = darkMode ? 50 : 10;
    element.style.setProperty('--color-shadow-5', getHsla(hue, saturation, lShadow, 0.05));
    element.style.setProperty('--color-shadow-10', getHsla(hue, saturation, lShadow, 0.1));
    element.style.setProperty('--color-shadow-15', getHsla(hue, saturation, lShadow, 0.15));
    element.style.setProperty('--color-shadow-20', getHsla(hue, saturation, lShadow, 0.2));

    element.style.setProperty('--color-dark-text', getHsl(hue, 0, 10));

    if (darkMode) {
        const sCorrection = darkMode ? -10 : 0;
        element.style.setProperty('--color-light-text', getHsl(hue, saturation + sCorrection, 90));
        element.style.setProperty('--color-light', getHsl(hue, saturation + sCorrection, 90));

        element.style.setProperty('--color-mask', getHsla(hue, saturation, 95, 0.95));
        element.style.setProperty('--color-mask-text', getHsl(hue, 0, 10));

        element.style.filter = 'invert(1) hue-rotate(180deg) brightness(2)';
    }
    else {
        element.style.setProperty('--color-light-text', getHsl(hue, 0, 100));
        element.style.setProperty('--color-light', getHsl(hue, 0, 100));

        element.style.setProperty('--color-mask', getHsla(hue, saturation, 15, 0.85));
        element.style.setProperty('--color-mask-text', getHsl(hue, saturation, 90));

        element.style.removeProperty('filter');
    }

    localStorage.setItem('themeHue', hue);
    localStorage.setItem('themeSaturation', saturation);
    localStorage.setItem('themeDarkMode', darkMode);
}

function getThemeHue() {
    const value = localStorage.getItem('themeHue') ?? '';
    return value !== '' ? parseInt(value, 10) : -1;
}

function getThemeSaturation() {
    const value = localStorage.getItem('themeSaturation') ?? '';
    return value !== '' ? parseInt(value, 10) : -1;
}

function setThemePreviewHue(hue) {
    const element = document.getElementById('app');
    element.style.setProperty('--theme-preview-hue', hue);
}

function getHsl(h, s, l) {
    return `hsl(${h}, ${s}%, ${l}%)`;
}

function getHsla(h, s, l, a) {
    return `hsl(${h}, ${s}%, ${l}%, ${a})`;
}

function setLocalValue(key, value) {
    localStorage.setItem(key, value);
}

function getLocalValue(key) {
    return localStorage.getItem(key);
}
