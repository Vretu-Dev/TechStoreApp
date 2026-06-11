using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using TechStoreApp.Models;
using TechStoreApp.Services;

namespace TechStoreApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        public Window? GetMainWindow() => _window;

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            EnsureDatabaseCreatedAndSeeded();

            _window = new MainWindow();
            _window.Activate();

            SettingsService.Initialize();
        }

        private void EnsureDatabaseCreatedAndSeeded()
        {
            try
            {
                using var db = new TechStoreDbContext();
                // EnsureCreated creates the database and all tables if they don't exist.
                // If the database exists but has no tables, it won't create them!
                // For a simple demo, we can use this approach.
                db.Database.EnsureCreated();

                // Check if we need to seed
                if (!db.Customers.Any())
                {
                    db.Customers.Add(new Customer
                    {
                        Email = "admin",
                        PasswordHash = AuthService.HashPassword("admin"),
                        FirstName = "System",
                        LastName = "Admin",
                        CreatedAt = DateTime.Now,
                        IsAdmin = true
                    });

                    var categoryNames = new[] 
                    { 
                        "Laptopy", "Smartfony", "Tablety", "Akcesoria", 
                        "Monitory", "Drukarki", "Klawiatury", "Myszy", 
                        "Słuchawki", "Głośniki" 
                    };

                    var categories = new List<Category>();
                    foreach (var name in categoryNames)
                    {
                        var cat = new Category { Name = name };
                        categories.Add(cat);
                    }
                    db.Categories.AddRange(categories);

                    db.Couriers.AddRange(new List<Courier>
                    {
                        new Courier { Name = "DHL Express", BaseShippingCost = 15.00m, EstimatedDeliveryTime = "1-2 dni robocze" },
                        new Courier { Name = "InPost Paczkomat", BaseShippingCost = 12.99m, EstimatedDeliveryTime = "1-2 dni robocze" },
                        new Courier { Name = "InPost Kurier", BaseShippingCost = 14.99m, EstimatedDeliveryTime = "1-2 dni robocze" },
                        new Courier { Name = "DPD", BaseShippingCost = 16.50m, EstimatedDeliveryTime = "1-3 dni robocze" },
                        new Courier { Name = "Poczta Polska", BaseShippingCost = 11.00m, EstimatedDeliveryTime = "3-5 dni robocze" }
                    });

                    var random = new Random();
                    foreach (var cat in categories)
                    {
                        for (int i = 1; i <= 10; i++)
                        {
                            db.Products.Add(new Product
                            {
                                Name = $"{cat.Name} Model {i}",
                                Sku = $"{cat.Name.Substring(0, 3).ToUpper()}-{cat.CategoryId}-{i:D3}",
                                Price = (decimal)(random.Next(50, 5000) + random.NextDouble()),
                                StockAmount = random.Next(1, 50),
                                Category = cat
                            });
                        }
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // In a real app, we would log this. For now, we'll just let it fail gracefully
                // or at least not crash the whole app if possible, 
                // but since the app depends on the DB, it's critical.
                System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex.Message}");
            }
        }
    }
}
