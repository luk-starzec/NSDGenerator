using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSDGenerator.Client.Services
{
    public class ThemeService : IThemeService
    {
        private readonly IJSRuntime js;
        private readonly int defaultHue = 220;

        public ThemeService(IJSRuntime js)
        {
            this.js = js;
        }

        public async Task SetThemeColorAsync(int hue)
            => await js.InvokeVoidAsync("setThemeColor", hue);

        public async Task SetPreviewColorAsync(int hue)
            => await js.InvokeVoidAsync("setThemePreviewHue", hue);

        public async Task SetDefaultThemeAsync()
        {
            await SetPreviewColorAsync(defaultHue);
            await SetThemeColorAsync(defaultHue);
        }

        public async Task<int> GetCurrentThemeHueAsync()
            => await js.InvokeAsync<int>("getThemeHue");

        public async Task InitThemeAsync()
        {
            int hue = defaultHue;

            // get hue from localstorage

            await SetPreviewColorAsync(hue);
            await SetThemeColorAsync(hue);
        }
    }
}
