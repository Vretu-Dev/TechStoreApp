using Microsoft.UI.Xaml;
using System;
using System.IO;

namespace TechStoreApp.Services
{
    public static class SettingsService
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
            "TechStoreApp", "theme.txt");

        public static ElementTheme Theme
        {
            get
            {
                try
                {
                    if (File.Exists(SettingsPath))
                    {
                        var savedTheme = File.ReadAllText(SettingsPath);
                        if (Enum.TryParse<ElementTheme>(savedTheme, out var theme))
                        {
                            return theme;
                        }
                    }
                }
                catch { }
                return ElementTheme.Default;
            }
            set
            {
                try
                {
                    var dir = Path.GetDirectoryName(SettingsPath);
                    if (!string.IsNullOrEmpty(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    File.WriteAllText(SettingsPath, value.ToString());
                    ApplyTheme(value);
                }
                catch { }
            }
        }

        public static void ApplyTheme(ElementTheme theme)
        {
            if (App.Current is App app && app.GetMainWindow() is Window window)
            {
                window.DispatcherQueue.TryEnqueue(() =>
                {
                    if (window.Content is FrameworkElement rootElement)
                    {
                        rootElement.RequestedTheme = theme;
                    }
                });
            }
        }

        public static void Initialize()
        {
            ApplyTheme(Theme);
        }
    }
}
