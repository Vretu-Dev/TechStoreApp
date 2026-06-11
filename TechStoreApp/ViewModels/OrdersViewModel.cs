using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TechStoreApp.Models;
using TechStoreApp.Services;

namespace TechStoreApp.ViewModels
{
    public class OrdersViewModel : BaseViewModel
    {
        private ObservableCollection<Order> _orders = new();
        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        public OrdersViewModel()
        {
            LoadOrders();
        }

        private void LoadOrders()
        {
            if (AuthService.CurrentUser == null) return;

            using var db = new TechStoreDbContext();
            var orders = db.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.Shipments)
                .Where(o => o.CustomerId == AuthService.CurrentUser.CustomerId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            Orders = new ObservableCollection<Order>(orders);
        }
    }
}
