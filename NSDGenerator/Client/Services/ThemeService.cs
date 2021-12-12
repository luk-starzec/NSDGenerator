using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace NSDGenerator.Client.Services;

public class ThemeService : IThemeService
{
    private readonly IJSRuntime js;

    private readonly int defaultHue = 220;
    private readonly int defaultSaturation = 40;

    public ThemeService(IJSRuntime js)
    {
        this.js = js;
    }

    private async Task SetColorAsync(int hue, int saturation)
        => await js.InvokeVoidAsync("setThemeColor", hue, saturation);

    private async Task SetColorSaturationAsync(int saturation)
    {
        var hue = await GetCurrentThemeHueAsync();
        await SetColorAsync(hue, saturation);
    }

    public async Task SetColorHueAsync(int hue)
    {
        var saturation = await GetCurrentThemeSaturationAsync();
        await SetColorAsync(hue, saturation);
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
        await SetColorAsync(defaultHue, defaultSaturation);
        await SetGrayScaleAsync(false);
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
        return value is not null && bool.Parse(value);
    }
}
