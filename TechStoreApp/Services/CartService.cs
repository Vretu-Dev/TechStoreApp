using System.Collections.ObjectModel;
using System.Linq;
using TechStoreApp.Models;

namespace TechStoreApp.Services
{
    public class CartItem : ViewModels.BaseViewModel
    {
        private Product _product = null!;
        public Product Product 
        { 
            get => _product; 
            set => SetProperty(ref _product, value); 
        }

        private int _quantity;
        public int Quantity 
        { 
            get => _quantity; 
            set
            {
                if (SetProperty(ref _quantity, value))
                {
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        public decimal TotalPrice => Product.Price * Quantity;
    }

    public static class CartService
    {
        public static ObservableCollection<CartItem> Items { get; } = new();

        public static event System.ComponentModel.PropertyChangedEventHandler? StaticPropertyChanged;

        static CartService()
        {
            Items.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (CartItem item in e.NewItems)
                        item.PropertyChanged += Item_PropertyChanged;
                }
                if (e.OldItems != null)
                {
                    foreach (CartItem item in e.OldItems)
                        item.PropertyChanged -= Item_PropertyChanged;
                }
                NotifyTotalChanged();
            };
        }

        private static void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CartItem.Quantity) || e.PropertyName == nameof(CartItem.TotalPrice))
            {
                NotifyTotalChanged();
            }
        }

        private static void NotifyTotalChanged()
        {
            StaticPropertyChanged?.Invoke(null, new System.ComponentModel.PropertyChangedEventArgs(nameof(TotalAmount)));
        }

        public static void AddItem(Product product)
        {
            var existing = Items.FirstOrDefault(i => i.Product.ProductId == product.ProductId);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                Items.Add(new CartItem { Product = product, Quantity = 1 });
            }
        }

        public static void RemoveItem(CartItem item)
        {
            Items.Remove(item);
        }

        public static void Clear()
        {
            Items.Clear();
        }

        public static decimal TotalAmount => Items.Sum(i => i.TotalPrice);
    }
}
