using System.Threading.Tasks;

namespace NSDGenerator.Client.Services;

public interface IThemeService
{
    Task SetPreviewColorAsync(int hue);
    Task SetColorHueAsync(int hue);
    Task SetGrayScaleAsync(bool grayScale);
    Task SetDefaultThemeAsync();
    Task InitThemeAsync();
    Task<int> GetCurrentThemeHueAsync();
    Task<bool> GetCurrentGrayScaleAsync();

}
