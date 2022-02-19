using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace NSDGenerator.Client.Services;

public class ThemeService : IThemeService
{
    private readonly IJSRuntime js;

    private readonly int defaultHue = 220;
    private readonly int defaultSaturation = 40;
    private readonly bool defaultDarkMode = false;

    public ThemeService(IJSRuntime js)
    {
        this.js = js;
    }

    private async Task SetColorAsync(int hue, int saturation, bool darkMode)
        => await js.InvokeVoidAsync("setThemeColor", hue, saturation, darkMode);

    private async Task SetColorSaturationAsync(int saturation)
    {
        var hue = await GetCurrentThemeHueAsync();
        var darkMode = await GetCurrentDarkModeAsync();
        await SetColorAsync(hue, saturation, darkMode);
    }

    public async Task SetColorHueAsync(int hue)
    {
        var saturation = await GetCurrentThemeSaturationAsync();
        var darkMode = await GetCurrentDarkModeAsync();
        await SetColorAsync(hue, saturation, darkMode);
    }

    public async Task SetPreviewColorAsync(int hue)
        => await js.InvokeVoidAsync("setThemePreviewHue", hue);

    public async Task InitThemeAsync()
    {
        int hue = await GetCurrentThemeHueAsync();
        bool grayScale = await GetCurrentGrayScaleAsync();

        await SetPreviewColorAsync(hue);
        await SetColorHueAsync(hue);
        await SetGrayScaleAsync(grayScale);
    }

    public async Task SetDefaultThemeAsync()
    {
        await SetPreviewColorAsync(defaultHue);
        await SetColorAsync(defaultHue, defaultSaturation, defaultDarkMode);
        await SetGrayScaleAsync(false);
        await SetDarkModeAsync(defaultDarkMode);
    }

    public async Task<int> GetCurrentThemeHueAsync()
    {
        var h = await js.InvokeAsync<int>("getThemeHue");
        return h < 0 ? defaultHue : h;
    }

    private async Task<int> GetCurrentThemeSaturationAsync()
    {
        var s = await js.InvokeAsync<int>("getThemeSaturation");
        return s < 0 ? defaultSaturation : s;
    }

    public async Task SetGrayScaleAsync(bool grayScale)
    {
        await SetColorSaturationAsync(grayScale ? 0 : defaultSaturation);
        await js.InvokeVoidAsync("setLocalValue", "themeGrayScale", grayScale);
    }

    public async Task<bool> GetCurrentGrayScaleAsync()
    {
        var value = await js.InvokeAsync<string>("getLocalValue", "themeGrayScale");
        return bool.TryParse(value, out bool result) ? result : false;
    }

    public async Task SetDarkModeAsync(bool darkMode)
    {
        var hue = await GetCurrentThemeHueAsync();
        var saturation = await GetCurrentThemeSaturationAsync();
        await SetColorAsync(hue, saturation, darkMode);
    }

    public async Task<bool> GetCurrentDarkModeAsync()
    {
        var value = await js.InvokeAsync<string>("getLocalValue", "themeDarkMode");
        return bool.TryParse(value, out bool result) ? result : false;
    }
}
