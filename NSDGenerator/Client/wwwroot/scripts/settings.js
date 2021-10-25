function setThemeColor(hue, saturation) {
    const element = document.getElementById('app');

    element.style.setProperty('--color-30', getHsl(hue, saturation, 30));
    element.style.setProperty('--color-50', getHsl(hue, saturation, 50));
    element.style.setProperty('--color-70', getHsl(hue, saturation, 70));
    element.style.setProperty('--color-80', getHsl(hue, saturation, 80));
    element.style.setProperty('--color-90', getHsl(hue, saturation, 90));
    element.style.setProperty('--color-95', getHsl(hue, saturation, 95));
    element.style.setProperty('--color-98', getHsl(hue, saturation, 98));
    element.style.setProperty('--color-mask-85', `hsla(${hue}, ${saturation}%, 15%, 0.85)`);

    localStorage.setItem('themeHue', hue);
    localStorage.setItem('themeSaturation', saturation);
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

function setLocalValue(key, value) {
    localStorage.setItem(key, value);
}

function getLocalValue(key) {
    return localStorage.getItem(key);
}
