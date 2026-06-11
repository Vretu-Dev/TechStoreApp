-- Skrypt tworzący strukturę bazy danych TechStoreDB (SQL Server)

CREATE DATABASE TechStoreDB;
GO

USE TechStoreDB;
GO

-- 1. Tabela: Categories
CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    ParentCategoryID INT NULL,
    CONSTRAINT FK_Categories_Parent FOREIGN KEY (ParentCategoryID) REFERENCES Categories(CategoryID)
);
GO

-- 2. Tabela: Customers
CREATE TABLE Customers (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(100) NULL,
    LastName NVARCHAR(100) NULL,
    Email NVARCHAR(255) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    IsAdmin BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Customers_Email UNIQUE (Email)
);
GO

-- 3. Tabela: Couriers
CREATE TABLE Couriers (
    CourierId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    BaseShippingCost DECIMAL(18, 2) NOT NULL,
    EstimatedDeliveryTime NVARCHAR(100) NULL
);
GO

-- 4. Tabela: Products
CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Price DECIMAL(18, 2) NOT NULL,
    StockAmount INT NOT NULL DEFAULT 0,
    CategoryID INT NOT NULL,
    SKU NVARCHAR(50) NOT NULL,
    ImageUrl NVARCHAR(MAX) NULL,
    CONSTRAINT UQ_Products_SKU UNIQUE (SKU),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);
GO
CREATE INDEX IX_Products_CategoryID ON Products(CategoryID);
GO

-- 5. Tabela: ProductAttributes
CREATE TABLE ProductAttributes (
    AttributeID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT NOT NULL,
    KeyName NVARCHAR(100) NOT NULL,
    Value NVARCHAR(500) NOT NULL,
    CONSTRAINT FK_ProductAttributes_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE
);
GO
CREATE INDEX IX_ProductAttributes_ProductID ON ProductAttributes(ProductID);
GO

-- 6. Tabela: Orders
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT NOT NULL,
    OrderDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(18, 2) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Nowe',
    ShippingAddress NVARCHAR(500) NULL,
    ShippingCity NVARCHAR(100) NULL,
    ShippingPostalCode NVARCHAR(20) NULL,
    ShippingMethod NVARCHAR(100) NULL,
    PaymentMethod NVARCHAR(100) NULL,
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
);
GO
CREATE INDEX IX_Orders_CustomerID ON Orders(CustomerID);
GO

-- 7. Tabela: OrderDetails
CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18, 2) NOT NULL,
    CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderID) REFERENCES Orders(OrderID) ON DELETE CASCADE,
    CONSTRAINT FK_OrderDetails_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);
GO
CREATE INDEX IX_OrderDetails_OrderID ON OrderDetails(OrderID);
GO

-- 8. Tabela: Shipments
CREATE TABLE Shipments (
    ShipmentID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    TrackingNumber NVARCHAR(100) NULL,
    CourierName NVARCHAR(100) NOT NULL,
    ShippedDate DATETIME2 NULL,
    CONSTRAINT FK_Shipments_Orders FOREIGN KEY (OrderID) REFERENCES Orders(OrderID) ON DELETE CASCADE
);
GO
