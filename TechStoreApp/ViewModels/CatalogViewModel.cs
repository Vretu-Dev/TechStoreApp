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

        private ObservableCollection<Category> _categories = new();
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
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

        private Category? _selectedCategory;
        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    FilterProducts();
                }
            }
        }

        private decimal _minPrice = 0;
        public decimal MinPrice
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

        private decimal _maxPrice = 10000;
        public decimal MaxPrice
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

        public CatalogViewModel()
        {
            AddToCartCommand = new RelayCommand(p => DoAddToCart(p as Product));
            LoadData();
        }

        private void LoadData()
        {
            using var db = new TechStoreDbContext();
            var cats = db.Categories.ToList();
            Categories = new ObservableCollection<Category>(cats);
            
            FilterProducts();
        }

        private void FilterProducts()
        {
            using var db = new TechStoreDbContext();
            IQueryable<Product> query = db.Products.Include(p => p.Category);

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(p => p.Name.Contains(SearchText) || p.Sku.Contains(SearchText));
            }

            if (SelectedCategory != null)
            {
                query = query.Where(p => p.CategoryId == SelectedCategory.CategoryId);
            }

            query = query.Where(p => p.Price >= MinPrice && p.Price <= MaxPrice);

            Products = new ObservableCollection<Product>(query.ToList());
        }

        private void DoAddToCart(Product? product)
        {
            if (product == null) return;
            CartService.AddItem(product);
        }
    }
}
