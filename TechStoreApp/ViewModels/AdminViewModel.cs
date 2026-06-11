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

        public ICommand SaveProductCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand ToggleAdminCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand ResetPasswordCommand { get; }
        public ICommand DeleteOrderCommand { get; }

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
            LoadData();
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
