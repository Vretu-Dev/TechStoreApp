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

        private static readonly string DbModePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
            "TechStoreApp", "db_mode.txt");

        private static readonly string DbConnPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
            "TechStoreApp", "db_conn.txt");

        public static bool IsLocalDatabase
        {
            get
            {
                try
                {
                    if (File.Exists(DbModePath))
                    {
                        var val = File.ReadAllText(DbModePath);
                        if (bool.TryParse(val, out var isLocal)) return isLocal;
                    }
                }
                catch { }
                return true;
            }
            set
            {
                try
                {
                    var dir = Path.GetDirectoryName(DbModePath);
                    if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                    File.WriteAllText(DbModePath, value.ToString());
                }
                catch { }
            }
        }

        public static string ExternalConnectionString
        {
            get
            {
                try
                {
                    if (File.Exists(DbConnPath))
                    {
                        return File.ReadAllText(DbConnPath);
                    }
                }
                catch { }
                return string.Empty;
            }
            set
            {
                try
                {
                    var dir = Path.GetDirectoryName(DbConnPath);
                    if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                    File.WriteAllText(DbConnPath, value ?? string.Empty);
                }
                catch { }
            }
        }

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
