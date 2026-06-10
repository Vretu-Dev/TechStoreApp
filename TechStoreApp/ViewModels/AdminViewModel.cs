using System;
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
                        EditProductPrice = value.Price;
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
        
        private decimal _editProductPrice;
        public decimal EditProductPrice { get => _editProductPrice; set => SetProperty(ref _editProductPrice, value); }
        
        private int _editProductStock;
        public int EditProductStock { get => _editProductStock; set => SetProperty(ref _editProductStock, value); }

        private Category? _selectedEditCategory;
        public Category? SelectedEditCategory { get => _selectedEditCategory; set => SetProperty(ref _selectedEditCategory, value); }

        public ICommand SaveProductCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand ToggleAdminCommand { get; }

        public AdminViewModel()
        {
            SaveProductCommand = new RelayCommand(_ => DoSaveProduct());
            AddProductCommand = new RelayCommand(_ => DoAddProduct());
            DeleteProductCommand = new RelayCommand(_ => DoDeleteProduct());
            ToggleAdminCommand = new RelayCommand(u => DoToggleAdmin(u as Customer));
            LoadData();
        }

        private void LoadData()
        {
            using var db = new TechStoreDbContext();
            Products = new ObservableCollection<Product>(db.Products.Include(p => p.Category).ToList());
            Users = new ObservableCollection<Customer>(db.Customers.ToList());
            Categories = new ObservableCollection<Category>(db.Categories.ToList());
        }

        private void DoSaveProduct()
        {
            if (SelectedProduct == null || SelectedEditCategory == null) return;

            using var db = new TechStoreDbContext();
            var product = db.Products.Find(SelectedProduct.ProductId);
            if (product != null)
            {
                product.Name = EditProductName;
                product.Sku = EditProductSku;
                product.Price = EditProductPrice;
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

            using var db = new TechStoreDbContext();
            var product = new Product
            {
                Name = EditProductName,
                Sku = EditProductSku,
                Price = EditProductPrice,
                StockAmount = EditProductStock,
                CategoryId = SelectedEditCategory.CategoryId
            };
            db.Products.Add(product);
            db.SaveChanges();
            LoadData();
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
