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
                
                // Zabezpieczenie: test połączenia przed próbą tworzenia/seedowania
                if (!db.Database.CanConnect())
                {
                    if (!SettingsService.IsLocalDatabase)
                    {
                        System.Diagnostics.Debug.WriteLine("External database connection failed. Falling back to local SQLite.");
                        SettingsService.IsLocalDatabase = true;
                        
                        using var fallbackDb = new TechStoreDbContext();
                        fallbackDb.Database.EnsureCreated();
                        SeedDatabaseIfNeeded(fallbackDb);
                        return;
                    }
                }

                if (SettingsService.IsLocalDatabase)
                {
                    db.Database.EnsureCreated();
                    SeedDatabaseIfNeeded(db);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex.Message}");
                
                // Fallback również w przypadku rzucenia wyjątku
                if (!SettingsService.IsLocalDatabase)
                {
                    System.Diagnostics.Debug.WriteLine("Exception during external DB init. Falling back to local SQLite.");
                    SettingsService.IsLocalDatabase = true;
                    try 
                    {
                        using var fallbackDb = new TechStoreDbContext();
                        fallbackDb.Database.EnsureCreated();
                        SeedDatabaseIfNeeded(fallbackDb);
                    } 
                    catch { }
                }
            }
        }

        private void SeedDatabaseIfNeeded(TechStoreDbContext db)
        {
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

                // Categories and Subcategories
                var catLaptopy = new Category { Name = "Laptopy i Komputery" };
                var catLaptopyGaming = new Category { Name = "Laptopy Gamingowe", ParentCategory = catLaptopy };
                var catLaptopyUltrabook = new Category { Name = "Ultrabooki", ParentCategory = catLaptopy };
                
                var catSmartfony = new Category { Name = "Smartfony i Smartwatche" };
                var catTelefony = new Category { Name = "Telefony", ParentCategory = catSmartfony };
                var catSmartwatche = new Category { Name = "Smartwatche", ParentCategory = catSmartfony };

                var catAkcesoria = new Category { Name = "Akcesoria" };
                var catMyszki = new Category { Name = "Myszki", ParentCategory = catAkcesoria };
                var catKlawiatury = new Category { Name = "Klawiatury", ParentCategory = catAkcesoria };

                db.Categories.AddRange(
                    catLaptopy, catLaptopyGaming, catLaptopyUltrabook,
                    catSmartfony, catTelefony, catSmartwatche,
                    catAkcesoria, catMyszki, catKlawiatury
                );

                // Couriers
                db.Couriers.AddRange(new List<Courier>
                {
                    new Courier { Name = "DHL Express", BaseShippingCost = 15.00m, EstimatedDeliveryTime = "1-2 dni robocze" },
                    new Courier { Name = "InPost Paczkomat", BaseShippingCost = 12.99m, EstimatedDeliveryTime = "1-2 dni robocze" },
                    new Courier { Name = "InPost Kurier", BaseShippingCost = 14.99m, EstimatedDeliveryTime = "1-2 dni robocze" },
                    new Courier { Name = "DPD", BaseShippingCost = 16.50m, EstimatedDeliveryTime = "1-3 dni robocze" },
                    new Courier { Name = "Poczta Polska", BaseShippingCost = 11.00m, EstimatedDeliveryTime = "3-5 dni robocze" }
                });

                // Realistic Products with Attributes (5 per subcategory)
                var products = new List<Product>();

                // --- ULTRABOOKI ---
                products.Add(new Product { Name = "Apple MacBook Air M3", Sku = "MAC-AIR-M3-8-256", Price = 5299.00m, StockAmount = 15, Category = catLaptopyUltrabook, Description = "Najnowszy ultrabook od Apple z procesorem M3. Niesamowicie smukły i szybki.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Procesor", Value = "Apple M3 (8 rdzeni)" }, new ProductAttribute { KeyName = "Pamięć RAM", Value = "8 GB" }, new ProductAttribute { KeyName = "Dysk", Value = "256 GB SSD" }, new ProductAttribute { KeyName = "Ekran", Value = "13.6 cala Liquid Retina" } } });
                products.Add(new Product { Name = "Dell Zenbook 14", Sku = "DEL-ZEN-14-I7", Price = 5999.00m, StockAmount = 10, Category = catLaptopyUltrabook, Description = "Elegancki i lekki laptop z ekranem OLED i certyfikatem Intel Evo.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Procesor", Value = "Intel Core i7-1360P" }, new ProductAttribute { KeyName = "Pamięć RAM", Value = "16 GB LPDDR5" }, new ProductAttribute { KeyName = "Dysk", Value = "1 TB SSD" }, new ProductAttribute { KeyName = "Ekran", Value = "14 cali OLED" } } });
                products.Add(new Product { Name = "Lenovo XPS 13", Sku = "LEN-XPS-13-I5", Price = 6499.00m, StockAmount = 5, Category = catLaptopyUltrabook, Description = "Kompaktowy design i najwyższa jakość wykonania z aluminium i włókna węglowego.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Procesor", Value = "Intel Core i5-1230U" }, new ProductAttribute { KeyName = "Pamięć RAM", Value = "16 GB" }, new ProductAttribute { KeyName = "Dysk", Value = "512 GB SSD" }, new ProductAttribute { KeyName = "Waga", Value = "1.17 kg" } } });
                products.Add(new Product { Name = "HP Spectre x360", Sku = "HP-SPEC-X360", Price = 7199.00m, StockAmount = 7, Category = catLaptopyUltrabook, Description = "Laptop 2 w 1 z obracanym ekranem dotykowym i rysikiem w zestawie.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Procesor", Value = "Intel Core i7-1355U" }, new ProductAttribute { KeyName = "Pamięć RAM", Value = "16 GB" }, new ProductAttribute { KeyName = "Dysk", Value = "1 TB SSD" }, new ProductAttribute { KeyName = "Funkcje", Value = "Ekran dotykowy 360°" } } });
                products.Add(new Product { Name = "Acer Swift 3", Sku = "ACE-SWI-3-RYZ", Price = 3899.00m, StockAmount = 20, Category = catLaptopyUltrabook, Description = "Przystępny cenowo ultrabook z wydajnym procesorem AMD Ryzen.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Procesor", Value = "AMD Ryzen 5 5500U" }, new ProductAttribute { KeyName = "Pamięć RAM", Value = "8 GB" }, new ProductAttribute { KeyName = "Dysk", Value = "512 GB SSD" }, new ProductAttribute { KeyName = "Waga", Value = "1.2 kg" } } });

                // --- LAPTOPY GAMINGOWE ---
                products.Add(new Product { Name = "ASUS ROG Strix G15", Sku = "ASU-ROG-G15-4060", Price = 6499.00m, StockAmount = 8, Category = catLaptopyGaming, Description = "Potężny laptop gamingowy zaprojektowany dla najbardziej wymagających graczy.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Procesor", Value = "AMD Ryzen 7 6800H" }, new ProductAttribute { KeyName = "Pamięć RAM", Value = "16 GB DDR5" }, new ProductAttribute { KeyName = "Karta graficzna", Value = "NVIDIA GeForce RTX 4060" }, new ProductAttribute { KeyName = "Ekran", Value = "165 Hz" } } });
                products.Add(new Product { Name = "Lenovo Legion 5", Sku = "LEN-LEG-5-3070TI", Price = 5899.00m, StockAmount = 12, Category = catLaptopyGaming, Description = "Świetny stosunek ceny do wydajności z doskonałym chłodzeniem Coldfront.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Procesor", Value = "AMD Ryzen 5 5600H" }, new ProductAttribute { KeyName = "Pamięć RAM", Value = "16 GB DDR4" }, new ProductAttribute { KeyName = "Karta graficzna", Value = "NVIDIA GeForce RTX 3070 Ti" }, new ProductAttribute { KeyName = "Ekran", Value = "165 Hz" } } });
                products.Add(new Product { Name = "MSI Katana 15", Sku = "MSI-KAT-15-4070", Price = 6999.00m, StockAmount = 6, Category = catLaptopyGaming, Description = "Wyostrzony na gaming. Laptop wyposażony w kartę z serii RTX 4000.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Procesor", Value = "Intel Core i7-12650H" }, new ProductAttribute { KeyName = "Pamięć RAM", Value = "16 GB DDR5" }, new ProductAttribute { KeyName = "Karta graficzna", Value = "NVIDIA GeForce RTX 4070" }, new ProductAttribute { KeyName = "Ekran", Value = "144 Hz" } } });
                products.Add(new Product { Name = "Acer Nitro 5", Sku = "ACE-NIT-5-4050", Price = 4599.00m, StockAmount = 18, Category = catLaptopyGaming, Description = "Solidny punkt wejścia do świata gamingu w bardzo dobrej cenie.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Procesor", Value = "Intel Core i5-12450H" }, new ProductAttribute { KeyName = "Pamięć RAM", Value = "16 GB DDR4" }, new ProductAttribute { KeyName = "Karta graficzna", Value = "NVIDIA GeForce RTX 4050" }, new ProductAttribute { KeyName = "Ekran", Value = "144 Hz" } } });
                products.Add(new Product { Name = "Razer Blade 15", Sku = "RAZ-BLA-15-4080", Price = 14999.00m, StockAmount = 2, Category = catLaptopyGaming, Description = "Premium gaming. Aluminiowa obudowa CNC i bezkompromisowa wydajność.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Procesor", Value = "Intel Core i9-13900H" }, new ProductAttribute { KeyName = "Pamięć RAM", Value = "32 GB DDR5" }, new ProductAttribute { KeyName = "Karta graficzna", Value = "NVIDIA GeForce RTX 4080" }, new ProductAttribute { KeyName = "Ekran", Value = "240 Hz OLED" } } });

                // --- TELEFONY ---
                products.Add(new Product { Name = "Samsung Galaxy S24 Ultra", Sku = "SAM-S24U-256-BLK", Price = 6599.00m, StockAmount = 20, Category = catTelefony, Description = "Flagowy smartfon z tytanową ramką i niesamowitymi możliwościami aparatu z AI. Posiada wbudowany rysik S Pen.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Procesor", Value = "Snapdragon 8 Gen 3" }, new ProductAttribute { KeyName = "Pamięć RAM", Value = "12 GB" }, new ProductAttribute { KeyName = "Aparat", Value = "200 MP" }, new ProductAttribute { KeyName = "Bateria", Value = "5000 mAh" } } });
                products.Add(new Product { Name = "Apple iPhone 15 Pro", Sku = "IPH-15P-128-TIT", Price = 5399.00m, StockAmount = 25, Category = catTelefony, Description = "Wykonany z tytanu klasy lotniczej, z chipem A17 Pro i konfigurowalnym Przyciskiem czynności.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Procesor", Value = "A17 Pro" }, new ProductAttribute { KeyName = "Pamięć Wewnętrzna", Value = "128 GB" }, new ProductAttribute { KeyName = "Ekran", Value = "6.1 cala OLED (120Hz)" }, new ProductAttribute { KeyName = "Złącze", Value = "USB-C" } } });
                products.Add(new Product { Name = "Google Pixel 8", Sku = "GOO-PIX-8-128", Price = 3499.00m, StockAmount = 15, Category = catTelefony, Description = "Czysty Android, magiczne funkcje edycji zdjęć i gwarancja aktualizacji przez 7 lat.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Procesor", Value = "Google Tensor G3" }, new ProductAttribute { KeyName = "Pamięć RAM", Value = "8 GB" }, new ProductAttribute { KeyName = "Ekran", Value = "6.2 cala OLED" }, new ProductAttribute { KeyName = "Aparat", Value = "50 MP" } } });
                products.Add(new Product { Name = "Xiaomi 14", Sku = "XIA-14-512-GRN", Price = 4299.00m, StockAmount = 12, Category = catTelefony, Description = "Kompaktowy flagowiec stworzony we współpracy z marką Leica.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Procesor", Value = "Snapdragon 8 Gen 3" }, new ProductAttribute { KeyName = "Pamięć RAM", Value = "12 GB" }, new ProductAttribute { KeyName = "Pamięć Wewnętrzna", Value = "512 GB" }, new ProductAttribute { KeyName = "Ładowanie", Value = "90W (HyperCharge)" } } });
                products.Add(new Product { Name = "Motorola Edge 40 Neo", Sku = "MOT-E40N-256", Price = 1899.00m, StockAmount = 30, Category = catTelefony, Description = "Stylowy telefon klasy średniej z wegańską skórą na plecach i wodoodpornością IP68.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Procesor", Value = "MediaTek Dimensity 7030" }, new ProductAttribute { KeyName = "Pamięć RAM", Value = "12 GB" }, new ProductAttribute { KeyName = "Ekran", Value = "6.55 cala P-OLED (144Hz)" }, new ProductAttribute { KeyName = "Aparat", Value = "50 MP" } } });

                // --- SMARTWATCHE ---
                products.Add(new Product { Name = "Apple Watch Series 9", Sku = "AW-S9-45-MID", Price = 2299.00m, StockAmount = 22, Category = catSmartwatche, Description = "Najpopularniejszy zegarek na świecie, teraz z jaśniejszym ekranem i nowym gestem dwukrotnego stuknięcia.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Koperta", Value = "45 mm (Aluminium)" }, new ProductAttribute { KeyName = "Funkcje Zdrowotne", Value = "EKG, Tętno, Tlen we krwi" }, new ProductAttribute { KeyName = "GPS", Value = "Tak" } } });
                products.Add(new Product { Name = "Samsung Galaxy Watch 6 Classic", Sku = "GW-6C-47-SLV", Price = 1799.00m, StockAmount = 18, Category = catSmartwatche, Description = "Powrót obracanego pierścienia! Klasyczny wygląd połączony z najnowocześniejszymi funkcjami śledzenia zdrowia.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Koperta", Value = "47 mm (Stal nierdzewna)" }, new ProductAttribute { KeyName = "System", Value = "Wear OS" }, new ProductAttribute { KeyName = "Płatności", Value = "Google Pay / Samsung Pay" } } });
                products.Add(new Product { Name = "Garmin Fenix 7X Sapphire Solar", Sku = "GAR-F7X-SS", Price = 3899.00m, StockAmount = 8, Category = catSmartwatche, Description = "Ultymatywny zegarek sportowy. Tytanowa ramka, szafirowe szkło z ładowaniem solarnym i wbudowana latarka.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Bateria", Value = "Do 37 dni (z solarem)" }, new ProductAttribute { KeyName = "Wodoodporność", Value = "10 ATM" }, new ProductAttribute { KeyName = "Mapy", Value = "TopoActive Europe" } } });
                products.Add(new Product { Name = "Huawei Watch 4 Pro", Sku = "HUA-W4P-LTE", Price = 2399.00m, StockAmount = 10, Category = catSmartwatche, Description = "Elegancja z tytanu z obsługą kart eSIM, pozwalająca na dzwonienie bez telefonu w pobliżu.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Łączność", Value = "LTE (eSIM)" }, new ProductAttribute { KeyName = "Koperta", Value = "Tytan stopu lotniczego" }, new ProductAttribute { KeyName = "System", Value = "HarmonyOS" } } });
                products.Add(new Product { Name = "Amazfit T-Rex Ultra", Sku = "AMA-TRU-BLK", Price = 1999.00m, StockAmount = 12, Category = catSmartwatche, Description = "Zegarek dla ekstremalnych odkrywców. Odporny na mróz i błoto, z precyzyjnym dwuzakresowym GPS.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Bateria", Value = "Do 20 dni" }, new ProductAttribute { KeyName = "GPS", Value = "Dwuzakresowy (6 systemów)" }, new ProductAttribute { KeyName = "Wytrzymałość", Value = "Zgodny z MIL-STD-810G" } } });

                // --- MYSZKI ---
                products.Add(new Product { Name = "Logitech MX Master 3S", Sku = "LOG-MX3S-BLK", Price = 499.00m, StockAmount = 40, Category = catMyszki, Description = "Najbardziej zaawansowana mysz z serii Master. Precyzyjna, z cichym klikiem i elektromagnetycznym kółkiem MagSpeed.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Sensor", Value = "Optyczny (8000 DPI)" }, new ProductAttribute { KeyName = "Łączność", Value = "Bezprzewodowa (Bluetooth/Logi Bolt)" }, new ProductAttribute { KeyName = "Profil", Value = "Praworęczna" } } });
                products.Add(new Product { Name = "Razer DeathAdder V3 Pro", Sku = "RAZ-DA-V3-PRO", Price = 649.00m, StockAmount = 25, Category = catMyszki, Description = "Ultradekka mysz e-sportowa o ergonomicznych kształtach i potężnym sensorze.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Waga", Value = "63 g" }, new ProductAttribute { KeyName = "Sensor", Value = "Focus Pro 30K Optical" }, new ProductAttribute { KeyName = "Przełączniki", Value = "Optyczne (Gen-3)" } } });
                products.Add(new Product { Name = "SteelSeries Viper V2 Pro", Sku = "STE-VIP-V2", Price = 699.00m, StockAmount = 20, Category = catMyszki, Description = "Kolejny krok w e-sportowej ewolucji. Jeszcze lżejsza i szybsza.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Waga", Value = "58 g" }, new ProductAttribute { KeyName = "Czas pracy baterii", Value = "Do 80 godzin" }, new ProductAttribute { KeyName = "Typ", Value = "Symetryczna" } } });
                products.Add(new Product { Name = "Logitech G Pro X Superlight", Sku = "LOG-GPX-SL-WHT", Price = 599.00m, StockAmount = 35, Category = catMyszki, Description = "Wybór profesjonalistów. Minimalistyczna konstrukcja bez zbędnych detali, stworzona do wygrywania.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Sensor", Value = "HERO 25K" }, new ProductAttribute { KeyName = "Waga", Value = "< 63 g" }, new ProductAttribute { KeyName = "Ślizgacze", Value = "PTFE bez dodatków" } } });
                products.Add(new Product { Name = "Endgame Gear XM2we", Sku = "END-XM2WE-BLK", Price = 399.00m, StockAmount = 15, Category = catMyszki, Description = "Mysz nastawiona na wydajność z przełącznikami optycznymi Kailh GO i świetnym powlekniem.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Sensor", Value = "PixArt PAW3370" }, new ProductAttribute { KeyName = "Przełączniki", Value = "Kailh GO Optical" }, new ProductAttribute { KeyName = "Mikrokontroler", Value = "CompX CX52850" } } });

                // --- KLAWIATURY ---
                products.Add(new Product { Name = "Keychron K8 Pro", Sku = "KEY-K8P-BROWN", Price = 549.00m, StockAmount = 15, Category = catKlawiatury, Description = "Bezprzewodowa klawiatura mechaniczna TKL z możliwością programowania QMK/VIA i aluminiową ramką.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Format", Value = "TKL (80%)" }, new ProductAttribute { KeyName = "Przełączniki", Value = "Gateron G Pro Brown (Hot-Swappable)" }, new ProductAttribute { KeyName = "Wyciszenie", Value = "Pianka silikonowa + podkładka pod płytę" } } });
                products.Add(new Product { Name = "Logitech MX Mechanical", Sku = "LOG-MXM-TACT", Price = 749.00m, StockAmount = 20, Category = catKlawiatury, Description = "Niskoprofilowa klawiatura mechaniczna zaprojektowana dla profesjonalistów i twórców.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Typ", Value = "Niskoprofilowa" }, new ProductAttribute { KeyName = "Przełączniki", Value = "Tactile Quiet (Brązowe)" }, new ProductAttribute { KeyName = "Podświetlenie", Value = "Białe, inteligentne" } } });
                products.Add(new Product { Name = "Razer Huntsman V2 TKL", Sku = "RAZ-HUNT-V2-RED", Price = 699.00m, StockAmount = 12, Category = catKlawiatury, Description = "E-sportowa klawiatura TKL wyposażona w najszybsze na świecie przełączniki optyczne.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Przełączniki", Value = "Razer Linear Optical (Czerwone)" }, new ProductAttribute { KeyName = "Polling rate", Value = "8000 Hz" }, new ProductAttribute { KeyName = "Nakładki", Value = "PBT Doubleshot" } } });
                products.Add(new Product { Name = "Corsair Apex Pro TKL Wireless", Sku = "COR-APEX-PRO-WL", Price = 1199.00m, StockAmount = 5, Category = catKlawiatury, Description = "Najszybsza i najbardziej zaawansowana klawiatura. Posiada regulowane magnetyczne przełączniki OmniPoint 2.0.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Przełączniki", Value = "OmniPoint 2.0 (Regulowany punkt aktywacji 0.2 - 3.8 mm)" }, new ProductAttribute { KeyName = "Ekran", Value = "OLED Smart Display" }, new ProductAttribute { KeyName = "Czas reakcji", Value = "0.54 ms" } } });
                products.Add(new Product { Name = "Wooting 60HE+", Sku = "WOO-60HE-PLUS", Price = 899.00m, StockAmount = 8, Category = catKlawiatury, Description = "Klawiatura analogowa 60% rewolucjonizująca ruch w grach, z unikalną funkcją Rapid Trigger.", ProductAttributes = new List<ProductAttribute> { new ProductAttribute { KeyName = "Format", Value = "60%" }, new ProductAttribute { KeyName = "Przełączniki", Value = "Lekker Switch (Magnetyczne/Analogowe)" }, new ProductAttribute { KeyName = "Zaleta", Value = "Pełny zakres analogowy i Rapid Trigger" } } });

                db.Products.AddRange(products);
                db.SaveChanges();
            }
        }
    }
}