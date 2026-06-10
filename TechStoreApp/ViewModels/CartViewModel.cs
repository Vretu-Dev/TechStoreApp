using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TechStoreApp.Models;
using TechStoreApp.Services;

namespace TechStoreApp.ViewModels
{
    public class CartViewModel : BaseViewModel
    {
        public ObservableCollection<CartItem> Items => CartService.Items;

        public decimal TotalAmount => CartService.TotalAmount;

        private string _orderMessage = string.Empty;
        public string OrderMessage
        {
            get => _orderMessage;
            set => SetProperty(ref _orderMessage, value);
        }

        public ICommand RemoveItemCommand { get; }
        public ICommand PlaceOrderCommand { get; }

        public CartViewModel()
        {
            RemoveItemCommand = new RelayCommand(i => DoRemoveItem(i as CartItem));
            PlaceOrderCommand = new RelayCommand(_ => DoPlaceOrder());
            CartService.StaticPropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CartService.TotalAmount))
                {
                    OnPropertyChanged(nameof(TotalAmount));
                }
            };
        }

        private void DoRemoveItem(CartItem? item)
        {
            if (item != null)
            {
                CartService.RemoveItem(item);
            }
        }

        private void DoPlaceOrder()
        {
            if (AuthService.CurrentUser == null)
            {
                OrderMessage = "Musisz się zalogować, aby złożyć zamówienie.";
                return;
            }

            if (!Items.Any())
            {
                OrderMessage = "Twój koszyk jest pusty.";
                return;
            }

            try
            {
                using var db = new TechStoreDbContext();
                var order = new Order
                {
                    CustomerId = AuthService.CurrentUser.CustomerId,
                    OrderDate = DateTime.Now,
                    Status = "Nowe",
                    TotalAmount = TotalAmount
                };

                foreach (var item in Items)
                {
                    order.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = item.Product.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price
                    });
                }

                db.Orders.Add(order);
                db.SaveChanges();

                CartService.Clear();
                OrderMessage = "Zamówienie zostało złożone pomyślnie!";
            }
            catch (Exception ex)
            {
                OrderMessage = "Błąd podczas składania zamówienia: " + ex.Message;
            }
        }
    }
}
