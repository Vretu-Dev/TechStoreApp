using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using TechStoreApp.Models;
using TechStoreApp.Services;

namespace TechStoreApp.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        private ObservableCollection<Product> _products = new();
        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        private ObservableCollection<Customer> _users = new();
        public ObservableCollection<Customer> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        private ObservableCollection<Category> _categories = new();
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        private Product? _selectedProduct;
        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (SetProperty(ref _selectedProduct, value))
                {
                    if (value != null)
                    {
                        EditProductName = value.Name;
                        EditProductSku = value.Sku;
                        EditProductPrice = (double)value.Price;
                        EditProductStock = value.StockAmount;
                        SelectedEditCategory = Categories.FirstOrDefault(c => c.CategoryId == value.CategoryId);
                    }
                }
            }
        }

        private string _editProductName = string.Empty;
        public string EditProductName { get => _editProductName; set => SetProperty(ref _editProductName, value); }
        
        private string _editProductSku = string.Empty;
        public string EditProductSku { get => _editProductSku; set => SetProperty(ref _editProductSku, value); }
        
        private double _editProductPrice;
        public double EditProductPrice { get => _editProductPrice; set => SetProperty(ref _editProductPrice, value); }
        
        private int _editProductStock;
        public int EditProductStock { get => _editProductStock; set => SetProperty(ref _editProductStock, value); }

        private string _errorMessage = string.Empty;
        public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

        private string _productSearchText = string.Empty;
        public string ProductSearchText
        {
            get => _productSearchText;
            set
            {
                if (SetProperty(ref _productSearchText, value))
                {
                    ApplySort();
                }
            }
        }

        private string _userSearchText = string.Empty;
        public string UserSearchText
        {
            get => _userSearchText;
            set
            {
                if (SetProperty(ref _userSearchText, value))
                {
                    ApplyUserFilter();
                }
            }
        }

        private string _selectedSort = "Nazwa";
        public string SelectedSort
        {
            get => _selectedSort;
            set
            {
                if (SetProperty(ref _selectedSort, value))
                {
                    ApplySort();
                }
            }
        }

        public ObservableCollection<string> SortOptions { get; } = new() { "Nazwa", "SKU", "Cena (rosnąco)", "Cena (malejąco)", "Stan" };

        private Category? _selectedEditCategory;
        public Category? SelectedEditCategory { get => _selectedEditCategory; set => SetProperty(ref _selectedEditCategory, value); }

        private Category? _selectedAdminCategory;
        public Category? SelectedAdminCategory
        {
            get => _selectedAdminCategory;
            set
            {
                if (SetProperty(ref _selectedAdminCategory, value))
                {
                    if (value != null)
                    {
                        EditCategoryName = value.Name;
                        SelectedParentCategory = Categories.FirstOrDefault(c => c.CategoryId == value.ParentCategoryId);
                    }
                }
            }
        }

        private string _editCategoryName = string.Empty;
        public string EditCategoryName { get => _editCategoryName; set => SetProperty(ref _editCategoryName, value); }

        private Category? _selectedParentCategory;
        public Category? SelectedParentCategory { get => _selectedParentCategory; set => SetProperty(ref _selectedParentCategory, value); }

        public ICommand SaveCategoryCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand ClearParentCategoryCommand { get; }

        public ICommand SaveProductCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand ToggleAdminCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand ResetPasswordCommand { get; }
        public ICommand DeleteOrderCommand { get; }
        public ICommand ChangeOrderStatusCommand { get; }

        // --- WIZARD PROPERTIES ---
        private bool _isWizardOpen;
        public bool IsWizardOpen { get => _isWizardOpen; set => SetProperty(ref _isWizardOpen, value); }

        private string _wizardName = string.Empty;
        public string WizardName { get => _wizardName; set => SetProperty(ref _wizardName, value); }

        private string _wizardSku = string.Empty;
        public string WizardSku { get => _wizardSku; set => SetProperty(ref _wizardSku, value); }

        private double _wizardPrice;
        public double WizardPrice { get => _wizardPrice; set => SetProperty(ref _wizardPrice, value); }

        private int _wizardStock;
        public int WizardStock { get => _wizardStock; set => SetProperty(ref _wizardStock, value); }

        private string _wizardDescription = string.Empty;
        public string WizardDescription { get => _wizardDescription; set => SetProperty(ref _wizardDescription, value); }

        private Category? _wizardCategory;
        public Category? WizardCategory { get => _wizardCategory; set => SetProperty(ref _wizardCategory, value); }

        private ObservableCollection<ProductAttribute> _wizardAttributes = new();
        public ObservableCollection<ProductAttribute> WizardAttributes { get => _wizardAttributes; set => SetProperty(ref _wizardAttributes, value); }

        public ICommand OpenWizardCommand { get; }
        public ICommand CloseWizardCommand { get; }
        public ICommand SaveWizardCommand { get; }
        public ICommand AddWizardAttributeCommand { get; }
        public ICommand RemoveWizardAttributeCommand { get; }
        // -------------------------

        public ObservableCollection<string> OrderStatuses { get; } = new() 
        { 
            "Nowe", "W trakcie realizacji", "Wysłane", "Zrealizowane", "Anulowane" 
        };

        private ObservableCollection<Order> _userOrders = new();
        public ObservableCollection<Order> UserOrders { get => _userOrders; set => SetProperty(ref _userOrders, value); }

        private Customer? _selectedUser;
        public Customer? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (SetProperty(ref _selectedUser, value))
                {
                    LoadUserOrders();
                }
            }
        }

        public AdminViewModel()
        {
            SaveProductCommand = new RelayCommand(_ => DoSaveProduct());
            AddProductCommand = new RelayCommand(_ => DoAddProduct());
            DeleteProductCommand = new RelayCommand(_ => DoDeleteProduct());
            ToggleAdminCommand = new RelayCommand(u => DoToggleAdmin(u as Customer));
            DeleteUserCommand = new RelayCommand(u => DoDeleteUser(u as Customer));
            ResetPasswordCommand = new RelayCommand(u => DoResetPassword(u as Customer));
            DeleteOrderCommand = new RelayCommand(o => DoDeleteOrder(o as Order));
            ChangeOrderStatusCommand = new RelayCommand(o => DoChangeOrderStatus(o as Order));
            SaveCategoryCommand = new RelayCommand(_ => DoSaveCategory());
            AddCategoryCommand = new RelayCommand(_ => DoAddCategory());
            DeleteCategoryCommand = new RelayCommand(_ => DoDeleteCategory());
            ClearParentCategoryCommand = new RelayCommand(_ => SelectedParentCategory = null);

            OpenWizardCommand = new RelayCommand(_ => DoOpenWizard());
            CloseWizardCommand = new RelayCommand(_ => IsWizardOpen = false);
            SaveWizardCommand = new RelayCommand(_ => DoSaveWizardProduct());
            AddWizardAttributeCommand = new RelayCommand(_ => WizardAttributes.Add(new ProductAttribute { KeyName = "Nowa cecha", Value = "Wartość" }));
            RemoveWizardAttributeCommand = new RelayCommand(a => { if (a is ProductAttribute attr) WizardAttributes.Remove(attr); });

            LoadData();
        }

        private void DoOpenWizard()
        {
            WizardName = string.Empty;
            WizardSku = string.Empty;
            WizardPrice = 0;
            WizardStock = 0;
            WizardDescription = string.Empty;
            WizardCategory = Categories.FirstOrDefault();
            WizardAttributes.Clear();
            IsWizardOpen = true;
            ErrorMessage = string.Empty;
        }

        private void DoSaveWizardProduct()
        {
            if (WizardCategory == null) return;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(WizardName) || string.IsNullOrWhiteSpace(WizardSku))
            {
                ErrorMessage = "Nazwa i SKU są wymagane.";
                return;
            }

            using var db = new TechStoreDbContext();

            if (db.Products.Any(p => p.Sku == WizardSku))
            {
                ErrorMessage = "Produkt z tym SKU już istnieje!";
                return;
            }

            var product = new Product
            {
                Name = WizardName,
                Sku = WizardSku,
                Price = (decimal)WizardPrice,
                StockAmount = WizardStock,
                CategoryId = WizardCategory.CategoryId,
                Description = WizardDescription,
                ProductAttributes = WizardAttributes.ToList()
            };

            db.Products.Add(product);
            db.SaveChanges();
            
            LoadData();
            IsWizardOpen = false;
        }

        private void DoSaveCategory()
        {
            if (SelectedAdminCategory == null) return;
            if (string.IsNullOrWhiteSpace(EditCategoryName)) return;

            using var db = new TechStoreDbContext();
            var cat = db.Categories.Find(SelectedAdminCategory.CategoryId);
            if (cat != null)
            {
                cat.Name = EditCategoryName;
                cat.ParentCategoryId = SelectedParentCategory?.CategoryId;

                // Prevent circular reference
                if (cat.ParentCategoryId == cat.CategoryId)
                {
                    cat.ParentCategoryId = null;
                }

                db.SaveChanges();
                LoadData();
            }
        }

        private void DoAddCategory()
        {
            if (string.IsNullOrWhiteSpace(EditCategoryName)) return;

            using var db = new TechStoreDbContext();
            var cat = new Category
            {
                Name = EditCategoryName,
                ParentCategoryId = SelectedParentCategory?.CategoryId
            };
            db.Categories.Add(cat);
            db.SaveChanges();
            LoadData();

            EditCategoryName = string.Empty;
        }

        private void DoDeleteCategory()
        {
            if (SelectedAdminCategory == null) return;

            using var db = new TechStoreDbContext();
            var cat = db.Categories.Find(SelectedAdminCategory.CategoryId);
            if (cat != null)
            {
                db.Categories.Remove(cat);
                db.SaveChanges();
                LoadData();
            }
        }

        private void DoChangeOrderStatus(Order? order)
        {
            if (order == null) return;

            using var db = new TechStoreDbContext();
            var dbOrder = db.Orders.Find(order.OrderId);
            if (dbOrder != null)
            {
                dbOrder.Status = order.Status;
                db.SaveChanges();
            }
        }

        private void LoadData()
        {
            using var db = new TechStoreDbContext();
            var products = db.Products.Include(p => p.Category).ToList();
            var users = db.Customers.ToList();
            Categories = new ObservableCollection<Category>(db.Categories.ToList());
            
            _allProducts = products;
            _allUsers = users;
            ApplySort();
            ApplyUserFilter();
        }

        private void ApplyUserFilter()
        {
            var filtered = string.IsNullOrWhiteSpace(UserSearchText) 
                ? _allUsers 
                : _allUsers.Where(u => 
                    (u.Email?.Contains(UserSearchText, StringComparison.OrdinalIgnoreCase) ?? false) || 
                    (u.FirstName?.Contains(UserSearchText, StringComparison.OrdinalIgnoreCase) ?? false) || 
                    (u.LastName?.Contains(UserSearchText, StringComparison.OrdinalIgnoreCase) ?? false));
            
            Users = new ObservableCollection<Customer>(filtered);
        }

        private void LoadUserOrders()
        {
            if (SelectedUser == null)
            {
                UserOrders.Clear();
                return;
            }

            using var db = new TechStoreDbContext();
            var orders = db.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.Shipments)
                .Where(o => o.CustomerId == SelectedUser.CustomerId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            
            UserOrders = new ObservableCollection<Order>(orders);
        }

        private void DoDeleteUser(Customer? user)
        {
            if (user == null || user.CustomerId == AuthService.CurrentUser?.CustomerId) return;

            using var db = new TechStoreDbContext();
            var dbUser = db.Customers.Find(user.CustomerId);
            if (dbUser != null)
            {
                db.Customers.Remove(dbUser);
                db.SaveChanges();
                LoadData();
            }
        }

        private void DoResetPassword(Customer? user)
        {
            if (user == null) return;

            using var db = new TechStoreDbContext();
            var dbUser = db.Customers.Find(user.CustomerId);
            if (dbUser != null)
            {
                dbUser.PasswordHash = AuthService.HashPassword("1234");
                db.SaveChanges();
                ErrorMessage = $"Hasło użytkownika {user.Email} zostało zresetowane na '1234'";
            }
        }

        private void DoDeleteOrder(Order? order)
        {
            if (order == null) return;

            using var db = new TechStoreDbContext();
            var dbOrder = db.Orders.Find(order.OrderId);
            if (dbOrder != null)
            {
                db.Orders.Remove(dbOrder);
                db.SaveChanges();
                LoadUserOrders();
            }
        }

        private List<Product> _allProducts = new();
        private List<Customer> _allUsers = new();

        private void ApplySort()
        {
            var filtered = string.IsNullOrWhiteSpace(ProductSearchText)
                ? _allProducts
                : _allProducts.Where(p => 
                    (p.Name?.Contains(ProductSearchText, StringComparison.OrdinalIgnoreCase) ?? false) || 
                    (p.Sku?.Contains(ProductSearchText, StringComparison.OrdinalIgnoreCase) ?? false));

            IEnumerable<Product> sorted = SelectedSort switch
            {
                "SKU" => filtered.OrderBy(p => p.Sku),
                "Cena (rosnąco)" => filtered.OrderBy(p => p.Price),
                "Cena (malejąco)" => filtered.OrderByDescending(p => p.Price),
                "Stan" => filtered.OrderByDescending(p => p.StockAmount),
                _ => filtered.OrderBy(p => p.Name)
            };
            Products = new ObservableCollection<Product>(sorted);
        }

        private void DoSaveProduct()
        {
            if (SelectedProduct == null || SelectedEditCategory == null) return;
            ErrorMessage = string.Empty;

            using var db = new TechStoreDbContext();
            
            // Check if SKU is taken by another product
            if (db.Products.Any(p => p.Sku == EditProductSku && p.ProductId != SelectedProduct.ProductId))
            {
                ErrorMessage = "Produkt z tym SKU już istnieje!";
                return;
            }

            var product = db.Products.Find(SelectedProduct.ProductId);
            if (product != null)
            {
                product.Name = EditProductName;
                product.Sku = EditProductSku;
                product.Price = (decimal)EditProductPrice;
                product.StockAmount = EditProductStock;
                product.CategoryId = SelectedEditCategory.CategoryId;
                db.SaveChanges();
                
                var savedId = product.ProductId;
                LoadData();
                SelectedProduct = Products.FirstOrDefault(p => p.ProductId == savedId);
            }
        }

        private void DoAddProduct()
        {
            if (SelectedEditCategory == null) return;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(EditProductName) || string.IsNullOrWhiteSpace(EditProductSku))
            {
                ErrorMessage = "Nazwa i SKU są wymagane.";
                return;
            }

            using var db = new TechStoreDbContext();

            // Check if SKU exists
            if (db.Products.Any(p => p.Sku == EditProductSku))
            {
                ErrorMessage = "Produkt z tym SKU już istnieje!";
                return;
            }

            var product = new Product
            {
                Name = EditProductName,
                Sku = EditProductSku,
                Price = (decimal)EditProductPrice,
                StockAmount = EditProductStock,
                CategoryId = SelectedEditCategory.CategoryId
            };
            db.Products.Add(product);
            db.SaveChanges();
            LoadData();
            
            // Clear fields after add
            EditProductName = string.Empty;
            EditProductSku = string.Empty;
            EditProductPrice = 0;
            EditProductStock = 0;
        }

        private void DoDeleteProduct()
        {
            if (SelectedProduct == null) return;

            using var db = new TechStoreDbContext();
            var product = db.Products.Find(SelectedProduct.ProductId);
            if (product != null)
            {
                db.Products.Remove(product);
                db.SaveChanges();
                LoadData();
            }
        }

        private void DoToggleAdmin(Customer? user)
        {
            if (user == null) return;

            using var db = new TechStoreDbContext();
            var dbUser = db.Customers.Find(user.CustomerId);
            if (dbUser != null)
            {
                dbUser.IsAdmin = !dbUser.IsAdmin;
                db.SaveChanges();
                LoadData();
            }
        }
    }
}
