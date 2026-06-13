using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using TechStoreApp.Views;
using TechStoreApp.Services;

namespace TechStoreApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            Title = "Tech Store App";

            CartService.StaticPropertyChanged += CartService_StaticPropertyChanged;
            UpdateCartBadge();
        }

        private void CartService_StaticPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CartService.TotalItemsCount))
            {
                DispatcherQueue.TryEnqueue(() => UpdateCartBadge());
            }
        }

        private void UpdateCartBadge()
        {
            int count = CartService.TotalItemsCount;
            if (count > 0)
            {
                CartBadge.Value = count;
                CartBadge.Visibility = Visibility.Visible;
                PopStoryboard.Begin();
            }
            else
            {
                CartBadge.Visibility = Visibility.Collapsed;
            }
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(CatalogPage));
            NavView.SelectedItem = NavView.MenuItems[0];
            UpdateAuthUI();
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                ContentFrame.Navigate(typeof(SettingsPage));
                return;
            }

            if (args.InvokedItemContainer != null)
            {
                var tag = args.InvokedItemContainer.Tag?.ToString();
                switch (tag)
                {
                    case "catalog":
                        ContentFrame.Navigate(typeof(CatalogPage));
                        break;
                    case "cart":
                        ContentFrame.Navigate(typeof(CartPage));
                        break;
                    case "orders":
                        if (AuthService.CurrentUser == null) { ContentFrame.Navigate(typeof(LoginPage)); }
                        else { ContentFrame.Navigate(typeof(OrdersPage)); }
                        break;
                    case "admin":
                        ContentFrame.Navigate(typeof(AdminPanelPage));
                        break;
                    case "login":
                        if (AuthService.CurrentUser == null)
                        {
                            ContentFrame.Navigate(typeof(LoginPage));
                        }
                        else
                        {
                            ContentFrame.Navigate(typeof(AccountPage));
                        }
                        break;
                    case "settings":
                        ContentFrame.Navigate(typeof(SettingsPage));
                        break;
                }
            }
        }

        public void UpdateAuthUI()
        {
            if (AuthService.CurrentUser != null)
            {
                LoginNavItem.Content = "Moje Konto (" + AuthService.CurrentUser.Email + ")";
                AdminNavItem.Visibility = AuthService.CurrentUser.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                LoginNavItem.Content = "Zaloguj się / Rejestracja";
                AdminNavItem.Visibility = Visibility.Collapsed;
            }
        }
    }
}
