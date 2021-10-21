using System.Threading.Tasks;

namespace NSDGenerator.Client.Services
{
    public interface IThemeService
    {
        Task SetPreviewColorAsync(int hue);
        Task SetThemeColorAsync(int hue);
        Task SetDefaultThemeAsync();
        Task InitThemeAsync();
        Task<int> GetCurrentThemeHueAsync();
    }
}