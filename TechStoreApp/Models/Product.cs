using System;
using System.Collections.Generic;

namespace TechStoreApp.Models;

public partial class Product : ViewModels.BaseViewModel
{
    private int _productId;
    public int ProductId { get => _productId; set => SetProperty(ref _productId, value); }

    private string _sku = null!;
    public string Sku { get => _sku; set => SetProperty(ref _sku, value); }

    private string _name = null!;
    public string Name { get => _name; set => SetProperty(ref _name, value); }

    private decimal _price;
    public decimal Price { get => _price; set => SetProperty(ref _price, value); }

    private int _stockAmount;
    public int StockAmount { get => _stockAmount; set => SetProperty(ref _stockAmount, value); }

    private int _categoryId;
    public int CategoryId { get => _categoryId; set => SetProperty(ref _categoryId, value); }

    private string? _description;
    public string? Description { get => _description; set => SetProperty(ref _description, value); }

    private string? _imageUrl;
    public string? ImageUrl { get => _imageUrl; set => SetProperty(ref _imageUrl, value); }

    private Category _category = null!;
    public virtual Category Category { get => _category; set => SetProperty(ref _category, value); }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ProductAttribute> ProductAttributes { get; set; } = new List<ProductAttribute>();
}
