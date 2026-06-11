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
    public class CatalogViewModel : BaseViewModel
    {
        private ObservableCollection<Product> _products = new();
        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        private ObservableCollection<Category> _parentCategories = new();
        public ObservableCollection<Category> ParentCategories
        {
            get => _parentCategories;
            set => SetProperty(ref _parentCategories, value);
        }

        private ObservableCollection<Category> _subCategories = new();
        public ObservableCollection<Category> SubCategories
        {
            get => _subCategories;
            set => SetProperty(ref _subCategories, value);
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterProducts();
                }
            }
        }

        private Product? _selectedProduct;
        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (SetProperty(ref _selectedProduct, value))
                {
                    OnPropertyChanged(nameof(IsProductSelected));
                }
            }
        }

        public bool IsProductSelected => SelectedProduct != null;

        private Category? _selectedParentCategory;
        public Category? SelectedParentCategory
        {
            get => _selectedParentCategory;
            set
            {
                if (SetProperty(ref _selectedParentCategory, value))
                {
                    UpdateSubCategories();
                    SelectedSubCategory = null;
                    FilterProducts();
                }
            }
        }

        private Category? _selectedSubCategory;
        public Category? SelectedSubCategory
        {
            get => _selectedSubCategory;
            set
            {
                if (SetProperty(ref _selectedSubCategory, value))
                {
                    FilterProducts();
                }
            }
        }

        private double? _minPrice;
        public double? MinPrice
        {
            get => _minPrice;
            set
            {
                if (SetProperty(ref _minPrice, value))
                {
                    FilterProducts();
                }
            }
        }

        private double? _maxPrice;
        public double? MaxPrice
        {
            get => _maxPrice;
            set
            {
                if (SetProperty(ref _maxPrice, value))
                {
                    FilterProducts();
                }
            }
        }

        public ICommand AddToCartCommand { get; }
        public ICommand ClearParentCategoryCommand { get; }
        public ICommand ClearSubCategoryCommand { get; }

        private List<Category> _allCategories = new();

        public CatalogViewModel()
        {
            AddToCartCommand = new RelayCommand(p => DoAddToCart(p as Product));
            ClearParentCategoryCommand = new RelayCommand(_ => SelectedParentCategory = null);
            ClearSubCategoryCommand = new RelayCommand(_ => SelectedSubCategory = null);
            LoadData();
        }

        private void LoadData()
        {
            using var db = new TechStoreDbContext();
            _allCategories = db.Categories.ToList();
            ParentCategories = new ObservableCollection<Category>(_allCategories.Where(c => c.ParentCategoryId == null));
            
            FilterProducts();
        }

        private void UpdateSubCategories()
        {
            if (SelectedParentCategory == null)
            {
                SubCategories.Clear();
            }
            else
            {
                var subs = _allCategories.Where(c => c.ParentCategoryId == SelectedParentCategory.CategoryId).ToList();
                SubCategories = new ObservableCollection<Category>(subs);
            }
        }

        private void FilterProducts()
        {
            using var db = new TechStoreDbContext();
            IQueryable<Product> query = db.Products
                .Include(p => p.Category)
                .Include(p => p.ProductAttributes);

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var lowerSearchText = SearchText.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(lowerSearchText) || p.Sku.ToLower().Contains(lowerSearchText));
            }

            if (SelectedSubCategory != null)
            {
                // Exact subcategory selected
                query = query.Where(p => p.CategoryId == SelectedSubCategory.CategoryId);
            }
            else if (SelectedParentCategory != null)
            {
                // Parent category selected, show parent and all its children
                var categoryIds = _allCategories
                    .Where(c => c.CategoryId == SelectedParentCategory.CategoryId || c.ParentCategoryId == SelectedParentCategory.CategoryId)
                    .Select(c => c.CategoryId)
                    .ToList();

                query = query.Where(p => categoryIds.Contains(p.CategoryId));
            }

            decimal min = _minPrice == null || double.IsNaN(_minPrice.Value) ? 0 : (decimal)_minPrice.Value;
            decimal max = _maxPrice == null || double.IsNaN(_maxPrice.Value) ? decimal.MaxValue : (decimal)_maxPrice.Value;
            query = query.Where(p => p.Price >= min && p.Price <= max);

            Products = new ObservableCollection<Product>(query.ToList());
        }

        private void DoAddToCart(Product? product)
        {
            if (product == null) return;
            if (product.StockAmount <= 0) return;
            CartService.AddItem(product);
        }
    }
}
