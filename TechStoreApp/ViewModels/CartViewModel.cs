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

        private string _shippingAddress = string.Empty;
        public string ShippingAddress
        {
            get => _shippingAddress;
            set => SetProperty(ref _shippingAddress, value);
        }

        private string _shippingCity = string.Empty;
        public string ShippingCity
        {
            get => _shippingCity;
            set => SetProperty(ref _shippingCity, value);
        }

        private string _shippingPostalCode = string.Empty;
        public string ShippingPostalCode
        {
            get => _shippingPostalCode;
            set => SetProperty(ref _shippingPostalCode, value);
        }

        private ObservableCollection<Courier> _couriers = new();
        public ObservableCollection<Courier> Couriers
        {
            get => _couriers;
            set => SetProperty(ref _couriers, value);
        }

        private Courier? _selectedCourier;
        public Courier? SelectedCourier
        {
            get => _selectedCourier;
            set
            {
                if (SetProperty(ref _selectedCourier, value))
                {
                    OnPropertyChanged(nameof(TotalWithShipping));
                }
            }
        }

        public decimal TotalWithShipping => TotalAmount + (SelectedCourier?.BaseShippingCost ?? 0);

        private ObservableCollection<string> _paymentMethods = new() { "Karta płatnicza", "Przelew bankowy", "BLIK", "Za pobraniem" };
        public ObservableCollection<string> PaymentMethods => _paymentMethods;

        private string? _selectedPaymentMethod;
        public string? SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set => SetProperty(ref _selectedPaymentMethod, value);
        }

        public ICommand RemoveItemCommand { get; }
        public ICommand PlaceOrderCommand { get; }

        public CartViewModel()
        {
            RemoveItemCommand = new RelayCommand(i => DoRemoveItem(i as CartItem));
            PlaceOrderCommand = new RelayCommand(_ => DoPlaceOrder());
            LoadCouriers();
            CartService.StaticPropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CartService.TotalAmount))
                {
                    OnPropertyChanged(nameof(TotalAmount));
                    OnPropertyChanged(nameof(TotalWithShipping));
                }
            };
        }

        private void LoadCouriers()
        {
            try
            {
                using var db = new TechStoreDbContext();
                var couriers = db.Couriers.ToList();
                Couriers = new ObservableCollection<Courier>(couriers);
                if (Couriers.Any())
                {
                    SelectedCourier = Couriers.First();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load couriers: {ex.Message}");
            }
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

            if (string.IsNullOrWhiteSpace(ShippingAddress) || 
                string.IsNullOrWhiteSpace(ShippingCity) || 
                string.IsNullOrWhiteSpace(ShippingPostalCode))
            {
                OrderMessage = "Proszę uzupełnić dane adresowe.";
                return;
            }

            if (SelectedCourier == null)
            {
                OrderMessage = "Proszę wybrać formę dostawy.";
                return;
            }

            if (string.IsNullOrEmpty(SelectedPaymentMethod))
            {
                OrderMessage = "Proszę wybrać formę płatności.";
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
                    TotalAmount = TotalWithShipping,
                    ShippingAddress = ShippingAddress,
                    ShippingCity = ShippingCity,
                    ShippingPostalCode = ShippingPostalCode,
                    ShippingMethod = SelectedCourier.Name,
                    PaymentMethod = SelectedPaymentMethod
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

                // Generate random tracking number
                var random = new Random();
                string trackingNumber = $"{SelectedCourier.Name.Substring(0, 2).ToUpper()}{random.Next(100000000, 999999999)}PL";

                // Create a shipment entry as well
                db.Shipments.Add(new Shipment
                {
                    OrderId = order.OrderId,
                    CourierName = SelectedCourier.Name,
                    TrackingNumber = trackingNumber,
                    ShippedDate = DateTime.Now.AddHours(2) // Simulate shipping a bit later
                });
                db.SaveChanges();

                CartService.Clear();
                OrderMessage = "Zamówienie zostało złożone pomyślnie!";
                
                // Reset form
                ShippingAddress = string.Empty;
                ShippingCity = string.Empty;
                ShippingPostalCode = string.Empty;
            }
            catch (Exception ex)
            {
                OrderMessage = "Błąd podczas składania zamówienia: " + ex.Message;
            }
        }
    }
}
