-- Schema thử nghiệm riêng BTL_GIAYTHE_TEST.
-- Kết nối bằng user BTL_GIAYTHE_TEST sau khi chạy 01_TAO_TEST_SCHEMA.sql.
-- Tạo 12 bảng dữ liệu SQL_PROJECT và nạp dữ liệu mẫu (không chứa phần mở rộng legacy).
-- Chỉ chạy một lần trên schema BTL_GIAYTHE_TEST mới.

CREATE TABLE Brands (
    BrandID    NUMBER(3)    CONSTRAINT pk_brands PRIMARY KEY,
    BrandName  VARCHAR2(50) NOT NULL CONSTRAINT uq_brands_name UNIQUE,
    Country    VARCHAR2(50) NOT NULL
);

-- 2. Danh mục giày
CREATE TABLE Categories (
    CategoryID   NUMBER(3)     CONSTRAINT pk_categories PRIMARY KEY,
    CategoryName VARCHAR2(50)  NOT NULL CONSTRAINT uq_categories_name UNIQUE,
    Description  VARCHAR2(200)
);

-- 3. Nhà cung cấp
CREATE TABLE Suppliers (
    SupplierID   NUMBER(3)     CONSTRAINT pk_suppliers PRIMARY KEY,
    SupplierName VARCHAR2(100) NOT NULL,
    Phone        VARCHAR2(15),
    Email        VARCHAR2(100),
    Address      VARCHAR2(200)
);

-- 4. Sản phẩm (mỗi dòng = 1 MẪU giày, 130 mẫu)
CREATE TABLE Products (
    ProductID   NUMBER(5)     CONSTRAINT pk_products PRIMARY KEY,
    ProductName VARCHAR2(100) NOT NULL CONSTRAINT uq_products_name UNIQUE,
    BrandID     NUMBER(3)     NOT NULL,
    CategoryID  NUMBER(3)     NOT NULL,
    SupplierID  NUMBER(3)     NOT NULL,
    Gender      VARCHAR2(10)  NOT NULL,
    Price       NUMBER(12)    NOT NULL,
    Description VARCHAR2(300),
    ImageURL    VARCHAR2(500),                       -- bổ sung link ảnh sau
    Status      VARCHAR2(10)  DEFAULT 'Active' NOT NULL,
    CONSTRAINT fk_products_brand    FOREIGN KEY (BrandID)    REFERENCES Brands(BrandID),
    CONSTRAINT fk_products_category FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    CONSTRAINT fk_products_supplier FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID),
    CONSTRAINT ck_products_gender   CHECK (Gender IN ('Nam','Nu','Unisex')),
    CONSTRAINT ck_products_price    CHECK (Price > 0),
    CONSTRAINT ck_products_status   CHECK (Status IN ('Active','Inactive'))
);

-- 5. Màu sắc
CREATE TABLE Colors (
    ColorID   NUMBER(3)    CONSTRAINT pk_colors PRIMARY KEY,
    ColorName VARCHAR2(30) NOT NULL CONSTRAINT uq_colors_name UNIQUE
);

-- 6. Kích cỡ
CREATE TABLE Sizes (
    SizeID    NUMBER(3) CONSTRAINT pk_sizes PRIMARY KEY,
    SizeValue NUMBER(3,1) NOT NULL CONSTRAINT uq_sizes_value UNIQUE,
    CONSTRAINT ck_sizes_value CHECK (SizeValue BETWEEN 30 AND 50)
);

-- 7. Biến thể sản phẩm (màu + size + tồn kho)
CREATE TABLE ProductVariants (
    VariantID     NUMBER(6) CONSTRAINT pk_variants PRIMARY KEY,
    ProductID     NUMBER(5) NOT NULL,
    ColorID       NUMBER(3) NOT NULL,
    SizeID        NUMBER(3) NOT NULL,
    StockQuantity NUMBER(5) DEFAULT 0 NOT NULL,
    CONSTRAINT fk_variants_product FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    CONSTRAINT fk_variants_color   FOREIGN KEY (ColorID)   REFERENCES Colors(ColorID),
    CONSTRAINT fk_variants_size    FOREIGN KEY (SizeID)    REFERENCES Sizes(SizeID),
    CONSTRAINT uq_variants_combo   UNIQUE (ProductID, ColorID, SizeID),
    CONSTRAINT ck_variants_stock   CHECK (StockQuantity >= 0)
);

-- 8. Khách hàng
CREATE TABLE Customers (
    CustomerID   NUMBER(5)     CONSTRAINT pk_customers PRIMARY KEY,
    FullName     VARCHAR2(100) NOT NULL,
    Phone        VARCHAR2(15)  NOT NULL CONSTRAINT uq_customers_phone UNIQUE,
    Email        VARCHAR2(100) CONSTRAINT uq_customers_email UNIQUE,
    Address      VARCHAR2(200),
    RegisterDate DATE DEFAULT SYSDATE NOT NULL
);

-- 9. Nhân viên
CREATE TABLE Employees (
    EmployeeID NUMBER(4)     CONSTRAINT pk_employees PRIMARY KEY,
    FullName   VARCHAR2(100) NOT NULL,
    Position   VARCHAR2(50)  NOT NULL,
    Phone      VARCHAR2(15)  NOT NULL CONSTRAINT uq_employees_phone UNIQUE,
    HireDate   DATE          NOT NULL,
    Salary     NUMBER(12)    NOT NULL,
    CONSTRAINT ck_employees_salary CHECK (Salary > 0)
);

-- 10. Đơn hàng
CREATE TABLE Orders (
    OrderID     NUMBER(6)    CONSTRAINT pk_orders PRIMARY KEY,
    CustomerID  NUMBER(5)    NOT NULL,
    EmployeeID  NUMBER(4),                              -- NULL = đơn đặt online
    OrderDate   DATE DEFAULT SYSDATE NOT NULL,
    Status      VARCHAR2(15) DEFAULT 'Pending' NOT NULL,
    TotalAmount NUMBER(14)   DEFAULT 0 NOT NULL,
    CONSTRAINT fk_orders_customer FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
    CONSTRAINT fk_orders_employee FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID),
    CONSTRAINT ck_orders_status   CHECK (Status IN ('Pending','Shipping','Completed','Cancelled')),
    CONSTRAINT ck_orders_total    CHECK (TotalAmount >= 0)
);

-- 11. Chi tiết đơn hàng
CREATE TABLE OrderDetails (
    OrderDetailID NUMBER(7)  CONSTRAINT pk_orderdetails PRIMARY KEY,
    OrderID       NUMBER(6)  NOT NULL,
    VariantID     NUMBER(6)  NOT NULL,
    Quantity      NUMBER(3)  NOT NULL,
    UnitPrice     NUMBER(12) NOT NULL,
    CONSTRAINT fk_details_order   FOREIGN KEY (OrderID)   REFERENCES Orders(OrderID),
    CONSTRAINT fk_details_variant FOREIGN KEY (VariantID) REFERENCES ProductVariants(VariantID),
    CONSTRAINT uq_details_combo   UNIQUE (OrderID, VariantID),
    CONSTRAINT ck_details_qty     CHECK (Quantity > 0),
    CONSTRAINT ck_details_price   CHECK (UnitPrice > 0)
);

-- 12. Thanh toán (mỗi đơn tối đa 1 thanh toán)
CREATE TABLE Payments (
    PaymentID     NUMBER(6)    CONSTRAINT pk_payments PRIMARY KEY,
    OrderID       NUMBER(6)    NOT NULL CONSTRAINT uq_payments_order UNIQUE,
    PaymentMethod VARCHAR2(20) NOT NULL,
    Amount        NUMBER(14)   NOT NULL,
    PaymentDate   DATE,
    Status        VARCHAR2(10) DEFAULT 'Unpaid' NOT NULL,
    CONSTRAINT fk_payments_order  FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    CONSTRAINT ck_payments_method CHECK (PaymentMethod IN ('Cash','Bank Transfer','Card','E-Wallet')),
    CONSTRAINT ck_payments_amount CHECK (Amount >= 0),
    CONSTRAINT ck_payments_status CHECK (Status IN ('Paid','Unpaid'))
);

-- Kiểm tra: phải thấy đúng 12 bảng
SELECT table_name FROM user_tables
WHERE table_name IN ('BRANDS','CATEGORIES','SUPPLIERS','PRODUCTS','COLORS','SIZES',
                     'PRODUCTVARIANTS','CUSTOMERS','EMPLOYEES','ORDERS','ORDERDETAILS','PAYMENTS')
ORDER BY table_name;



-- =====================================================================
-- FILE 2: NHẬP DỮ LIỆU MẪU (chạy SAU file 01) - Run Script (F5)
-- Thứ tự: Brands -> Categories -> Suppliers -> Colors -> Sizes -> Products
--         -> ProductVariants -> Customers -> Employees -> Orders
--         -> OrderDetails -> cập nhật TotalAmount -> Payments
-- ImageURL để NULL, bạn bổ sung sau bằng lệnh UPDATE (xem file 03).
-- =====================================================================

-- 1. Thương hiệu (10)
INSERT INTO Brands (BrandID, BrandName, Country) VALUES (1,  'Nike',         'Mỹ');
INSERT INTO Brands (BrandID, BrandName, Country) VALUES (2,  'Adidas',       'Đức');
INSERT INTO Brands (BrandID, BrandName, Country) VALUES (3,  'Puma',         'Đức');
INSERT INTO Brands (BrandID, BrandName, Country) VALUES (4,  'New Balance',  'Mỹ');
INSERT INTO Brands (BrandID, BrandName, Country) VALUES (5,  'ASICS',        'Nhật Bản');
INSERT INTO Brands (BrandID, BrandName, Country) VALUES (6,  'Skechers',     'Mỹ');
INSERT INTO Brands (BrandID, BrandName, Country) VALUES (7,  'Mizuno',       'Nhật Bản');
INSERT INTO Brands (BrandID, BrandName, Country) VALUES (8,  'Li-Ning',      'Trung Quốc');
INSERT INTO Brands (BrandID, BrandName, Country) VALUES (9,  'ANTA',         'Trung Quốc');
INSERT INTO Brands (BrandID, BrandName, Country) VALUES (10, 'Under Armour', 'Mỹ');

-- 2. Danh mục (8)
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (1, 'Chạy bộ',        'Giày chạy bộ đường dài, đường nhựa, thi đấu');
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (2, 'Bóng đá',        'Giày đá bóng sân cỏ tự nhiên, sân nhân tạo');
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (3, 'Bóng rổ',        'Giày bóng rổ trong nhà và ngoài trời');
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (4, 'Lifestyle',      'Giày sneaker thời trang hằng ngày');
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (5, 'Đi bộ',          'Giày đi bộ, êm chân, dùng hằng ngày');
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (6, 'Tập luyện',      'Giày tập gym và luyện tập thể lực');
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (7, 'Tennis',         'Giày tennis, sân cứng');
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (8, 'Bóng chuyền',    'Giày bóng chuyền trong nhà');

-- 3. Nhà cung cấp (5) - dữ liệu giả lập
INSERT INTO Suppliers (SupplierID, SupplierName, Phone, Email, Address) VALUES (1, 'Công ty Phân phối Thể thao Việt Á',  '0281112221', 'vieta@example.com',   'Quận 1, TP. Hồ Chí Minh');
INSERT INTO Suppliers (SupplierID, SupplierName, Phone, Email, Address) VALUES (2, 'Công ty Giày Thể thao Đông Dương',  '0281112222', 'dongduong@example.com','Quận 3, TP. Hồ Chí Minh');
INSERT INTO Suppliers (SupplierID, SupplierName, Phone, Email, Address) VALUES (3, 'Công ty TNHH Sport Hub',            '0241112223', 'sporthub@example.com', 'Cầu Giấy, Hà Nội');
INSERT INTO Suppliers (SupplierID, SupplierName, Phone, Email, Address) VALUES (4, 'Công ty TNHH Á Châu Footwear',      '0241112224', 'achau@example.com',    'Đống Đa, Hà Nội');
INSERT INTO Suppliers (SupplierID, SupplierName, Phone, Email, Address) VALUES (5, 'Công ty Nhập khẩu Hoa Nam',         '0236111225', 'hoanam@example.com',   'Hải Châu, Đà Nẵng');

-- 4. Màu sắc (8)
INSERT INTO Colors (ColorID, ColorName) VALUES (1, 'Đen');
INSERT INTO Colors (ColorID, ColorName) VALUES (2, 'Trắng');
INSERT INTO Colors (ColorID, ColorName) VALUES (3, 'Xám');
INSERT INTO Colors (ColorID, ColorName) VALUES (4, 'Xanh navy');
INSERT INTO Colors (ColorID, ColorName) VALUES (5, 'Đỏ');
INSERT INTO Colors (ColorID, ColorName) VALUES (6, 'Xanh lá');
INSERT INTO Colors (ColorID, ColorName) VALUES (7, 'Cam');
INSERT INTO Colors (ColorID, ColorName) VALUES (8, 'Be');

-- 5. Kích cỡ EU 36 -> 44 (9)
INSERT INTO Sizes (SizeID, SizeValue) VALUES (1, 36);
INSERT INTO Sizes (SizeID, SizeValue) VALUES (2, 37);
INSERT INTO Sizes (SizeID, SizeValue) VALUES (3, 38);
INSERT INTO Sizes (SizeID, SizeValue) VALUES (4, 39);
INSERT INTO Sizes (SizeID, SizeValue) VALUES (5, 40);
INSERT INTO Sizes (SizeID, SizeValue) VALUES (6, 41);
INSERT INTO Sizes (SizeID, SizeValue) VALUES (7, 42);
INSERT INTO Sizes (SizeID, SizeValue) VALUES (8, 43);
INSERT INTO Sizes (SizeID, SizeValue) VALUES (9, 44);

-- 6. Sản phẩm: 130 mẫu (ProductID, tên, BrandID, CategoryID, SupplierID, giới tính, giá VND)
INSERT INTO Products (ProductID, ProductName, BrandID, CategoryID, SupplierID, Gender, Price)
-- ===== NIKE (20) =====
SELECT 1,  'Nike Air Force 1 ''07', 1, 4, 1, 'Unisex', 3239000 FROM dual UNION ALL
SELECT 2,  'Nike Air Max 90', 1, 4, 1, 'Nam', 4109000 FROM dual UNION ALL
SELECT 3,  'Nike Air Max 270', 1, 4, 1, 'Nam', 4409000 FROM dual UNION ALL
SELECT 4,  'Nike Dunk Low Retro', 1, 4, 1, 'Unisex', 2929000 FROM dual UNION ALL
SELECT 5,  'Nike Pegasus 41', 1, 1, 1, 'Nam', 4109000 FROM dual UNION ALL
SELECT 6,  'Nike Pegasus 40', 1, 1, 1, 'Nu', 3829000 FROM dual UNION ALL
SELECT 7,  'Nike Vomero 17', 1, 1, 1, 'Nam', 4609000 FROM dual UNION ALL
SELECT 8,  'Nike Revolution 7', 1, 1, 1, 'Nam', 1799000 FROM dual UNION ALL
SELECT 9,  'Nike Revolution 6 Next Nature', 1, 1, 1, 'Nu', 1599000 FROM dual UNION ALL
SELECT 10, 'Nike Mercurial Vapor 15 Academy', 1, 2, 1, 'Unisex', 1999000 FROM dual UNION ALL
SELECT 11, 'Nike Mercurial Superfly 9 Elite', 1, 2, 1, 'Nam', 6689000 FROM dual UNION ALL
SELECT 12, 'Nike Phantom GX Academy', 1, 2, 1, 'Unisex', 2199000 FROM dual UNION ALL
SELECT 13, 'Nike Phantom GX 2 Elite', 1, 2, 1, 'Nam', 6689000 FROM dual UNION ALL
SELECT 14, 'Nike Tiempo Legend 10 Academy', 1, 2, 1, 'Unisex', 1999000 FROM dual UNION ALL
SELECT 15, 'Nike LeBron 21', 1, 3, 1, 'Nam', 5279000 FROM dual UNION ALL
SELECT 16, 'Nike KD 17', 1, 3, 1, 'Nam', 4699000 FROM dual UNION ALL
SELECT 17, 'Nike Sabrina 1', 1, 3, 1, 'Nu', 3519000 FROM dual UNION ALL
SELECT 18, 'Nike Ja 1', 1, 3, 1, 'Nam', 3519000 FROM dual UNION ALL
SELECT 19, 'Nike Metcon 9', 1, 6, 1, 'Nam', 3829000 FROM dual UNION ALL
SELECT 20, 'Nike Air Max 1', 1, 4, 1, 'Nu', 3829000 FROM dual UNION ALL
-- ===== ADIDAS (18) =====
SELECT 21, 'Adidas Samba OG', 2, 4, 2, 'Unisex', 2700000 FROM dual UNION ALL
SELECT 22, 'Adidas Superstar', 2, 4, 2, 'Unisex', 2500000 FROM dual UNION ALL
SELECT 23, 'Adidas Stan Smith', 2, 4, 2, 'Unisex', 2500000 FROM dual UNION ALL
SELECT 24, 'Adidas Gazelle', 2, 4, 2, 'Unisex', 2600000 FROM dual UNION ALL
SELECT 25, 'Adidas Campus 00s', 2, 4, 2, 'Unisex', 2700000 FROM dual UNION ALL
SELECT 26, 'Adidas Ultraboost 1.0', 2, 1, 2, 'Nam', 4200000 FROM dual UNION ALL
SELECT 27, 'Adidas Ultraboost Light', 2, 1, 2, 'Nu', 4500000 FROM dual UNION ALL
SELECT 28, 'Adidas Adizero Adios Pro 3', 2, 1, 2, 'Nam', 6500000 FROM dual UNION ALL
SELECT 29, 'Adidas Adizero Adios Pro 4', 2, 1, 2, 'Nam', 7000000 FROM dual UNION ALL
SELECT 30, 'Adidas Supernova Rise', 2, 1, 2, 'Nam', 3500000 FROM dual UNION ALL
SELECT 31, 'Adidas Supernova Stride', 2, 1, 2, 'Nu', 3200000 FROM dual UNION ALL
SELECT 32, 'Adidas Duramo SL', 2, 1, 2, 'Nu', 1800000 FROM dual UNION ALL
SELECT 33, 'Adidas Predator League', 2, 2, 2, 'Unisex', 2100000 FROM dual UNION ALL
SELECT 34, 'Adidas Predator Elite', 2, 2, 2, 'Nam', 5500000 FROM dual UNION ALL
SELECT 35, 'Adidas Copa Pure 2 League', 2, 2, 2, 'Unisex', 2000000 FROM dual UNION ALL
SELECT 36, 'Adidas F50 League', 2, 2, 2, 'Unisex', 2100000 FROM dual UNION ALL
SELECT 37, 'Adidas Dame 9', 2, 3, 2, 'Nam', 3500000 FROM dual UNION ALL
SELECT 38, 'Adidas Harden Vol. 8', 2, 3, 2, 'Nam', 3800000 FROM dual UNION ALL
-- ===== PUMA (13) =====
SELECT 39, 'Puma Suede Classic XXI', 3, 4, 2, 'Unisex', 2400000 FROM dual UNION ALL
SELECT 40, 'Puma Palermo', 3, 4, 2, 'Unisex', 2600000 FROM dual UNION ALL
SELECT 41, 'Puma Speedcat OG', 3, 4, 2, 'Unisex', 2800000 FROM dual UNION ALL
SELECT 42, 'Puma RS-X', 3, 4, 2, 'Unisex', 3200000 FROM dual UNION ALL
SELECT 43, 'Puma Deviate Nitro 2', 3, 1, 2, 'Nam', 4500000 FROM dual UNION ALL
SELECT 44, 'Puma Deviate Nitro 3', 3, 1, 2, 'Nu', 4800000 FROM dual UNION ALL
SELECT 45, 'Puma Velocity Nitro 3', 3, 1, 2, 'Nam', 3400000 FROM dual UNION ALL
SELECT 46, 'Puma ForeverRun Nitro', 3, 1, 2, 'Nam', 3300000 FROM dual UNION ALL
SELECT 47, 'Puma Future 7 Match', 3, 2, 2, 'Unisex', 2200000 FROM dual UNION ALL
SELECT 48, 'Puma Ultra 5 Match', 3, 2, 2, 'Unisex', 2100000 FROM dual UNION ALL
SELECT 49, 'Puma King Match', 3, 2, 2, 'Unisex', 1600000 FROM dual UNION ALL
SELECT 50, 'Puma All-Pro Nitro', 3, 3, 2, 'Nam', 3600000 FROM dual UNION ALL
SELECT 51, 'Puma Suede XL', 3, 4, 2, 'Unisex', 2600000 FROM dual UNION ALL
-- ===== NEW BALANCE (14) =====
SELECT 52, 'New Balance 574', 4, 4, 3, 'Unisex', 2800000 FROM dual UNION ALL
SELECT 53, 'New Balance 327', 4, 4, 3, 'Unisex', 2800000 FROM dual UNION ALL
SELECT 54, 'New Balance 530', 4, 4, 3, 'Unisex', 3200000 FROM dual UNION ALL
SELECT 55, 'New Balance 550', 4, 4, 3, 'Unisex', 3800000 FROM dual UNION ALL
SELECT 56, 'New Balance 9060', 4, 4, 3, 'Unisex', 4200000 FROM dual UNION ALL
SELECT 57, 'New Balance 990v6', 4, 4, 3, 'Nam', 6500000 FROM dual UNION ALL
SELECT 58, 'New Balance 2002R', 4, 4, 3, 'Unisex', 4200000 FROM dual UNION ALL
SELECT 59, 'New Balance 1906R', 4, 4, 3, 'Unisex', 4200000 FROM dual UNION ALL
SELECT 60, 'New Balance Fresh Foam X 1080v14', 4, 1, 3, 'Nam', 4700000 FROM dual UNION ALL
SELECT 61, 'New Balance Fresh Foam X 1080v13', 4, 1, 3, 'Nu', 4300000 FROM dual UNION ALL
SELECT 62, 'New Balance Fresh Foam X 880v14', 4, 1, 3, 'Nam', 3600000 FROM dual UNION ALL
SELECT 63, 'New Balance Fresh Foam X 880v13', 4, 1, 3, 'Nu', 3300000 FROM dual UNION ALL
SELECT 64, 'New Balance FuelCell Rebel v4', 4, 1, 3, 'Nam', 3800000 FROM dual UNION ALL
SELECT 65, 'New Balance FuelCell SuperComp Elite v4', 4, 1, 3, 'Nam', 6900000 FROM dual UNION ALL
-- ===== ASICS (14) =====
SELECT 66, 'ASICS GEL-KAYANO 31', 5, 1, 4, 'Nam', 4800000 FROM dual UNION ALL
SELECT 67, 'ASICS GEL-KAYANO 30', 5, 1, 4, 'Nu', 4500000 FROM dual UNION ALL
SELECT 68, 'ASICS GEL-NIMBUS 26', 5, 1, 4, 'Nam', 4500000 FROM dual UNION ALL
SELECT 69, 'ASICS GEL-NIMBUS 27', 5, 1, 4, 'Nu', 4700000 FROM dual UNION ALL
SELECT 70, 'ASICS GEL-CUMULUS 26', 5, 1, 4, 'Nam', 3500000 FROM dual UNION ALL
SELECT 71, 'ASICS GEL-CUMULUS 27', 5, 1, 4, 'Nu', 3700000 FROM dual UNION ALL
SELECT 72, 'ASICS GEL-1130', 5, 4, 4, 'Unisex', 3000000 FROM dual UNION ALL
SELECT 73, 'ASICS GEL-KAYANO 14', 5, 4, 4, 'Unisex', 4200000 FROM dual UNION ALL
SELECT 74, 'ASICS GEL-NYC', 5, 4, 4, 'Unisex', 4000000 FROM dual UNION ALL
SELECT 75, 'ASICS GT-2000 12', 5, 1, 4, 'Nam', 3200000 FROM dual UNION ALL
SELECT 76, 'ASICS Novablast 4', 5, 1, 4, 'Nam', 3900000 FROM dual UNION ALL
SELECT 77, 'ASICS Novablast 5', 5, 1, 4, 'Nu', 4000000 FROM dual UNION ALL
SELECT 78, 'ASICS Superblast 2', 5, 1, 4, 'Nam', 5200000 FROM dual UNION ALL
SELECT 79, 'ASICS Solution Speed FF 3', 5, 7, 4, 'Nam', 3800000 FROM dual UNION ALL
-- ===== SKECHERS (12) =====
SELECT 80, 'Skechers GO WALK Arch Fit 2.0', 6, 5, 3, 'Nu', 1900000 FROM dual UNION ALL
SELECT 81, 'Skechers GO WALK 7', 6, 5, 3, 'Nam', 1800000 FROM dual UNION ALL
SELECT 82, 'Skechers GO RUN Consistent', 6, 1, 3, 'Nam', 1700000 FROM dual UNION ALL
SELECT 83, 'Skechers GO RUN Ride 10', 6, 1, 3, 'Nam', 2300000 FROM dual UNION ALL
SELECT 84, 'Skechers Arch Fit D''Lux', 6, 5, 3, 'Nu', 2200000 FROM dual UNION ALL
SELECT 85, 'Skechers MAX Cushioning Elite 2.0', 6, 1, 3, 'Nam', 2500000 FROM dual UNION ALL
SELECT 86, 'Skechers D''Lites', 6, 4, 3, 'Nu', 2200000 FROM dual UNION ALL
SELECT 87, 'Skechers UNO', 6, 4, 3, 'Unisex', 2300000 FROM dual UNION ALL
SELECT 88, 'Skechers Summits', 6, 5, 3, 'Nam', 1600000 FROM dual UNION ALL
SELECT 89, 'Skechers Slip-ins Ultra Flex 3.0', 6, 5, 3, 'Nam', 2300000 FROM dual UNION ALL
SELECT 90, 'Skechers Slip-ins Summits', 6, 5, 3, 'Nu', 2400000 FROM dual UNION ALL
SELECT 91, 'Skechers GO RUN Elevate', 6, 1, 3, 'Nu', 1900000 FROM dual UNION ALL
-- ===== MIZUNO (11) =====
SELECT 92,  'Mizuno Wave Rider 28', 7, 1, 4, 'Nam', 3800000 FROM dual UNION ALL
SELECT 93,  'Mizuno Wave Rider 27', 7, 1, 4, 'Nu', 3600000 FROM dual UNION ALL
SELECT 94,  'Mizuno Wave Inspire 20', 7, 1, 4, 'Nam', 3800000 FROM dual UNION ALL
SELECT 95,  'Mizuno Wave Sky 7', 7, 1, 4, 'Nam', 4200000 FROM dual UNION ALL
SELECT 96,  'Mizuno Wave Prophecy 12', 7, 1, 4, 'Nam', 5000000 FROM dual UNION ALL
SELECT 97,  'Mizuno Wave Creation 25', 7, 1, 4, 'Nam', 4200000 FROM dual UNION ALL
SELECT 98,  'Mizuno Wave Rebellion Pro 2', 7, 1, 4, 'Nam', 6000000 FROM dual UNION ALL
SELECT 99,  'Mizuno Morelia Neo IV Pro', 7, 2, 4, 'Nam', 5500000 FROM dual UNION ALL
SELECT 100, 'Mizuno Monarcida Neo III Select', 7, 2, 4, 'Unisex', 2300000 FROM dual UNION ALL
SELECT 101, 'Mizuno Wave Lightning Neo 2', 7, 8, 4, 'Unisex', 3200000 FROM dual UNION ALL
SELECT 102, 'Mizuno Wave Momentum 3', 7, 8, 4, 'Unisex', 3500000 FROM dual UNION ALL
-- ===== LI-NING (12) =====
SELECT 103, 'Li-Ning Way of Wade 10', 8, 3, 5, 'Nam', 4500000 FROM dual UNION ALL
SELECT 104, 'Li-Ning Way of Wade 9', 8, 3, 5, 'Nam', 4000000 FROM dual UNION ALL
SELECT 105, 'Li-Ning Way of Wade 808 3 Ultra', 8, 3, 5, 'Nam', 3800000 FROM dual UNION ALL
SELECT 106, 'Li-Ning JB1', 8, 3, 5, 'Nam', 3500000 FROM dual UNION ALL
SELECT 107, 'Li-Ning Yushuai 16', 8, 3, 5, 'Nam', 2800000 FROM dual UNION ALL
SELECT 108, 'Li-Ning Yushuai 15', 8, 3, 5, 'Nam', 2600000 FROM dual UNION ALL
SELECT 109, 'Li-Ning Feidian 3 Ultra', 8, 1, 5, 'Nam', 3800000 FROM dual UNION ALL
SELECT 110, 'Li-Ning Feidian 3 Elite', 8, 1, 5, 'Nam', 5500000 FROM dual UNION ALL
SELECT 111, 'Li-Ning Red Hare 7', 8, 1, 5, 'Nam', 2200000 FROM dual UNION ALL
SELECT 112, 'Li-Ning Red Hare 8', 8, 1, 5, 'Nu', 2300000 FROM dual UNION ALL
SELECT 113, 'Li-Ning Super Light 20', 8, 1, 5, 'Nu', 2000000 FROM dual UNION ALL
SELECT 114, 'Li-Ning Sonic 11', 8, 3, 5, 'Nam', 2500000 FROM dual UNION ALL
-- ===== ANTA (8) =====
SELECT 115, 'ANTA KT9', 9, 3, 5, 'Nam', 3200000 FROM dual UNION ALL
SELECT 116, 'ANTA KT10', 9, 3, 5, 'Nam', 3500000 FROM dual UNION ALL
SELECT 117, 'ANTA KAI 1', 9, 3, 5, 'Nam', 3800000 FROM dual UNION ALL
SELECT 118, 'ANTA KAI 2', 9, 3, 5, 'Nam', 4000000 FROM dual UNION ALL
SELECT 119, 'ANTA Shock Wave 5', 9, 3, 5, 'Nam', 2500000 FROM dual UNION ALL
SELECT 120, 'ANTA GH3', 9, 3, 5, 'Nam', 2800000 FROM dual UNION ALL
SELECT 121, 'ANTA C202 5', 9, 1, 5, 'Nam', 2300000 FROM dual UNION ALL
SELECT 122, 'ANTA C202 GT', 9, 1, 5, 'Nam', 2500000 FROM dual UNION ALL
-- ===== UNDER ARMOUR (8) =====
SELECT 123, 'Under Armour Curry 11', 10, 3, 1, 'Nam', 4200000 FROM dual UNION ALL
SELECT 124, 'Under Armour Curry 12', 10, 3, 1, 'Nam', 4500000 FROM dual UNION ALL
SELECT 125, 'Under Armour Curry Flow 10', 10, 3, 1, 'Nam', 3800000 FROM dual UNION ALL
SELECT 126, 'Under Armour HOVR Phantom 3', 10, 1, 1, 'Nam', 3400000 FROM dual UNION ALL
SELECT 127, 'Under Armour HOVR Sonic 6', 10, 1, 1, 'Nam', 2900000 FROM dual UNION ALL
SELECT 128, 'Under Armour Charged Assert 10', 10, 1, 1, 'Nu', 1800000 FROM dual UNION ALL
SELECT 129, 'Under Armour Charged Bandit 7', 10, 1, 1, 'Nam', 2000000 FROM dual UNION ALL
SELECT 130, 'Under Armour TriBase Reign 5', 10, 6, 1, 'Nam', 2900000 FROM dual;

-- Tự sinh mô tả ngắn từ danh mục + tên mẫu
UPDATE Products p
SET Description = (SELECT 'Giày ' || LOWER(c.CategoryName) || ' - ' || p.ProductName
                   FROM Categories c WHERE c.CategoryID = p.CategoryID);

-- 7. Biến thể: mỗi mẫu có 2 màu x 5 size = 10 biến thể (tổng 1300)
--    Nam: size 40-44 | Nu: size 36-40 | Unisex: size 38-42
--    Tồn kho tính theo công thức (0..24) để có cả hàng tồn thấp, dữ liệu cố định, không ngẫu nhiên
INSERT INTO ProductVariants (VariantID, ProductID, ColorID, SizeID, StockQuantity)
SELECT ROW_NUMBER() OVER (ORDER BY p.ProductID, c.ColorID, s.SizeID),
       p.ProductID, c.ColorID, s.SizeID,
       MOD(p.ProductID * 7 + s.SizeID * 3 + c.ColorID * 5, 25)
FROM Products p
JOIN Colors c ON c.ColorID IN (MOD(p.ProductID, 8) + 1, MOD(p.ProductID + 3, 8) + 1)
JOIN Sizes  s ON (p.Gender = 'Nam'    AND s.SizeValue BETWEEN 40 AND 44)
              OR (p.Gender = 'Nu'     AND s.SizeValue BETWEEN 36 AND 40)
              OR (p.Gender = 'Unisex' AND s.SizeValue BETWEEN 38 AND 42);

-- 8. Khách hàng (12)
INSERT INTO Customers (CustomerID, FullName, Phone, Email, Address, RegisterDate) VALUES (1,  'Nguyễn Văn An',    '0901000001', 'an.nguyen@example.com',   'Cầu Giấy, Hà Nội',        DATE '2026-01-10');
INSERT INTO Customers (CustomerID, FullName, Phone, Email, Address, RegisterDate) VALUES (2,  'Trần Thị Bình',    '0901000002', 'binh.tran@example.com',   'Quận 1, TP. Hồ Chí Minh', DATE '2026-01-22');
INSERT INTO Customers (CustomerID, FullName, Phone, Email, Address, RegisterDate) VALUES (3,  'Lê Hoàng Cường',   '0901000003', 'cuong.le@example.com',    'Hải Châu, Đà Nẵng',       DATE '2026-02-05');
INSERT INTO Customers (CustomerID, FullName, Phone, Email, Address, RegisterDate) VALUES (4,  'Phạm Minh Dũng',   '0901000004', 'dung.pham@example.com',   'Đống Đa, Hà Nội',         DATE '2026-02-18');
INSERT INTO Customers (CustomerID, FullName, Phone, Email, Address, RegisterDate) VALUES (5,  'Võ Thị Em',       '0901000005', 'em.vo@example.com',       'Quận 7, TP. Hồ Chí Minh', DATE '2026-03-03');
INSERT INTO Customers (CustomerID, FullName, Phone, Email, Address, RegisterDate) VALUES (6,  'Đặng Quốc Phong',  '0901000006', 'phong.dang@example.com',  'Ninh Kiều, Cần Thơ',      DATE '2026-03-15');
INSERT INTO Customers (CustomerID, FullName, Phone, Email, Address, RegisterDate) VALUES (7,  'Bùi Thu Giang',    '0901000007', 'giang.bui@example.com',   'Thanh Xuân, Hà Nội',      DATE '2026-04-01');
INSERT INTO Customers (CustomerID, FullName, Phone, Email, Address, RegisterDate) VALUES (8,  'Hoàng Gia Huy',    '0901000008', 'huy.hoang@example.com',   'Sơn Trà, Đà Nẵng',        DATE '2026-04-20');
INSERT INTO Customers (CustomerID, FullName, Phone, Email, Address, RegisterDate) VALUES (9,  'Ngô Thanh Hà',     '0901000009', 'ha.ngo@example.com',      'Quận 3, TP. Hồ Chí Minh', DATE '2026-05-08');
INSERT INTO Customers (CustomerID, FullName, Phone, Email, Address, RegisterDate) VALUES (10, 'Đỗ Anh Khoa',      '0901000010', 'khoa.do@example.com',     'Hoàn Kiếm, Hà Nội',       DATE '2026-05-27');
INSERT INTO Customers (CustomerID, FullName, Phone, Email, Address, RegisterDate) VALUES (11, 'Lý Mỹ Linh',       '0901000011', 'linh.ly@example.com',     'Biên Hòa, Đồng Nai',      DATE '2026-06-12');
INSERT INTO Customers (CustomerID, FullName, Phone, Email, Address, RegisterDate) VALUES (12, 'Vũ Đức Mạnh',      '0901000012', 'manh.vu@example.com',     'Hồng Bàng, Hải Phòng',    DATE '2026-06-30');

-- 9. Nhân viên (8)
INSERT INTO Employees (EmployeeID, FullName, Position, Phone, HireDate, Salary) VALUES (1, 'Nguyễn Thị Hương',  'Quản lý cửa hàng',   '0912000001', DATE '2024-03-01', 18000000);
INSERT INTO Employees (EmployeeID, FullName, Position, Phone, HireDate, Salary) VALUES (2, 'Trần Quang Vinh',   'Nhân viên bán hàng', '0912000002', DATE '2024-06-15', 9000000);
INSERT INTO Employees (EmployeeID, FullName, Position, Phone, HireDate, Salary) VALUES (3, 'Lê Thị Mai',        'Nhân viên bán hàng', '0912000003', DATE '2024-09-01', 9000000);
INSERT INTO Employees (EmployeeID, FullName, Position, Phone, HireDate, Salary) VALUES (4, 'Phạm Văn Nam',      'Nhân viên bán hàng', '0912000004', DATE '2025-01-10', 8500000);
INSERT INTO Employees (EmployeeID, FullName, Position, Phone, HireDate, Salary) VALUES (5, 'Hoàng Thị Oanh',    'Thu ngân',           '0912000005', DATE '2025-02-20', 8000000);
INSERT INTO Employees (EmployeeID, FullName, Position, Phone, HireDate, Salary) VALUES (6, 'Đinh Xuân Sơn',     'Thu ngân',           '0912000006', DATE '2025-05-05', 8000000);
INSERT INTO Employees (EmployeeID, FullName, Position, Phone, HireDate, Salary) VALUES (7, 'Bùi Ngọc Tú',       'Nhân viên kho',      '0912000007', DATE '2025-07-01', 7500000);
INSERT INTO Employees (EmployeeID, FullName, Position, Phone, HireDate, Salary) VALUES (8, 'Cao Thị Uyên',      'Nhân viên kho',      '0912000008', DATE '2025-09-15', 7500000);

-- 10. Đơn hàng (30) - EmployeeID NULL = đơn online; TotalAmount tạm = 0, sẽ tính lại bên dưới
INSERT INTO Orders (OrderID, CustomerID, EmployeeID, OrderDate, Status, TotalAmount)
SELECT 1,  3,  2,    DATE '2026-07-03', 'Completed', 0 FROM dual UNION ALL
SELECT 2,  1,  3,    DATE '2026-07-10', 'Completed', 0 FROM dual UNION ALL
SELECT 3,  5,  2,    DATE '2026-07-18', 'Completed', 0 FROM dual UNION ALL
SELECT 4,  2,  4,    DATE '2026-07-25', 'Completed', 0 FROM dual UNION ALL
SELECT 5,  8,  NULL, DATE '2026-08-02', 'Completed', 0 FROM dual UNION ALL
SELECT 6,  4,  3,    DATE '2026-08-05', 'Completed', 0 FROM dual UNION ALL
SELECT 7,  7,  5,    DATE '2026-08-09', 'Completed', 0 FROM dual UNION ALL
SELECT 8,  10, 2,    DATE '2026-08-14', 'Cancelled', 0 FROM dual UNION ALL
SELECT 9,  6,  4,    DATE '2026-08-20', 'Completed', 0 FROM dual UNION ALL
SELECT 10, 12, 6,    DATE '2026-08-27', 'Completed', 0 FROM dual UNION ALL
SELECT 11, 9,  3,    DATE '2026-09-01', 'Completed', 0 FROM dual UNION ALL
SELECT 12, 11, 5,    DATE '2026-09-04', 'Completed', 0 FROM dual UNION ALL
SELECT 13, 1,  NULL, DATE '2026-09-08', 'Completed', 0 FROM dual UNION ALL
SELECT 14, 3,  2,    DATE '2026-09-12', 'Completed', 0 FROM dual UNION ALL
SELECT 15, 5,  4,    DATE '2026-09-15', 'Completed', 0 FROM dual UNION ALL
SELECT 16, 2,  6,    DATE '2026-09-18', 'Completed', 0 FROM dual UNION ALL
SELECT 17, 7,  3,    DATE '2026-09-21', 'Completed', 0 FROM dual UNION ALL
SELECT 18, 4,  5,    DATE '2026-09-24', 'Completed', 0 FROM dual UNION ALL
SELECT 19, 6,  2,    DATE '2026-09-27', 'Cancelled', 0 FROM dual UNION ALL
SELECT 20, 9,  4,    DATE '2026-09-30', 'Completed', 0 FROM dual UNION ALL
SELECT 21, 12, NULL, DATE '2026-10-02', 'Completed', 0 FROM dual UNION ALL
SELECT 22, 8,  6,    DATE '2026-10-05', 'Completed', 0 FROM dual UNION ALL
SELECT 23, 10, 3,    DATE '2026-10-08', 'Completed', 0 FROM dual UNION ALL
SELECT 24, 1,  5,    DATE '2026-10-11', 'Completed', 0 FROM dual UNION ALL
SELECT 25, 11, 2,    DATE '2026-10-14', 'Completed', 0 FROM dual UNION ALL
SELECT 26, 3,  4,    DATE '2026-10-17', 'Shipping',  0 FROM dual UNION ALL
SELECT 27, 5,  6,    DATE '2026-10-19', 'Shipping',  0 FROM dual UNION ALL
SELECT 28, 2,  3,    DATE '2026-10-21', 'Pending',   0 FROM dual UNION ALL
SELECT 29, 7,  5,    DATE '2026-10-22', 'Pending',   0 FROM dual UNION ALL
SELECT 30, 4,  2,    DATE '2026-10-23', 'Pending',   0 FROM dual;

-- 11. Chi tiết đơn hàng
--    Mỗi dòng: (OrderID, ProductID, vn = biến thể thứ mấy của mẫu đó (1-10), số lượng)
--    Đơn giá = giá hiện tại trong Products
INSERT INTO OrderDetails (OrderDetailID, OrderID, VariantID, Quantity, UnitPrice)
SELECT ROW_NUMBER() OVER (ORDER BY d.oid, d.pid), d.oid, v.VariantID, d.qty, p.Price
FROM (
    SELECT 1 oid, 5 pid, 2 vn, 1 qty FROM dual UNION ALL
    SELECT 1, 21, 3, 1 FROM dual UNION ALL
    SELECT 2, 1, 4, 1 FROM dual UNION ALL
    SELECT 3, 43, 1, 1 FROM dual UNION ALL
    SELECT 3, 76, 6, 1 FROM dual UNION ALL
    SELECT 4, 52, 5, 1 FROM dual UNION ALL
    SELECT 5, 115, 2, 1 FROM dual UNION ALL
    SELECT 5, 123, 3, 1 FROM dual UNION ALL
    SELECT 6, 80, 1, 2 FROM dual UNION ALL
    SELECT 7, 26, 7, 1 FROM dual UNION ALL
    SELECT 7, 66, 2, 1 FROM dual UNION ALL
    SELECT 7, 10, 9, 1 FROM dual UNION ALL
    SELECT 8, 39, 8, 1 FROM dual UNION ALL
    SELECT 9, 92, 1, 1 FROM dual UNION ALL
    SELECT 10, 103, 4, 1 FROM dual UNION ALL
    SELECT 10, 15, 5, 1 FROM dual UNION ALL
    SELECT 11, 2, 6, 1 FROM dual UNION ALL
    SELECT 12, 22, 1, 1 FROM dual UNION ALL
    SELECT 12, 23, 2, 1 FROM dual UNION ALL
    SELECT 13, 60, 3, 1 FROM dual UNION ALL
    SELECT 14, 109, 1, 1 FROM dual UNION ALL
    SELECT 15, 33, 4, 2 FROM dual UNION ALL
    SELECT 15, 47, 5, 1 FROM dual UNION ALL
    SELECT 16, 72, 8, 1 FROM dual UNION ALL
    SELECT 16, 73, 2, 1 FROM dual UNION ALL
    SELECT 17, 126, 1, 1 FROM dual UNION ALL
    SELECT 18, 17, 7, 1 FROM dual UNION ALL
    SELECT 18, 32, 3, 1 FROM dual UNION ALL
    SELECT 19, 107, 2, 1 FROM dual UNION ALL
    SELECT 20, 55, 6, 1 FROM dual UNION ALL
    SELECT 20, 56, 7, 1 FROM dual UNION ALL
    SELECT 21, 8, 9, 2 FROM dual UNION ALL
    SELECT 22, 94, 10, 1 FROM dual UNION ALL
    SELECT 22, 78, 1, 1 FROM dual UNION ALL
    SELECT 23, 34, 2, 1 FROM dual UNION ALL
    SELECT 24, 88, 3, 1 FROM dual UNION ALL
    SELECT 24, 81, 4, 1 FROM dual UNION ALL
    SELECT 25, 50, 5, 1 FROM dual UNION ALL
    SELECT 26, 19, 6, 1 FROM dual UNION ALL
    SELECT 26, 130, 2, 1 FROM dual UNION ALL
    SELECT 27, 3, 10, 1 FROM dual UNION ALL
    SELECT 27, 21, 8, 1 FROM dual UNION ALL
    SELECT 28, 79, 7, 1 FROM dual UNION ALL
    SELECT 29, 100, 1, 3 FROM dual UNION ALL
    SELECT 30, 124, 2, 1 FROM dual UNION ALL
    SELECT 30, 116, 4, 1 FROM dual
) d
JOIN Products p ON p.ProductID = d.pid
JOIN (SELECT VariantID, ProductID,
             ROW_NUMBER() OVER (PARTITION BY ProductID ORDER BY VariantID) rn
      FROM ProductVariants) v
  ON v.ProductID = d.pid AND v.rn = d.vn;

-- Tính chính xác tổng tiền từng đơn từ chi tiết đơn hàng
UPDATE Orders o
SET TotalAmount = (SELECT NVL(SUM(d.Quantity * d.UnitPrice), 0)
                   FROM OrderDetails d WHERE d.OrderID = o.OrderID);

-- 12. Thanh toán: mọi đơn trừ đơn đã hủy; đơn Pending = chưa thanh toán
INSERT INTO Payments (PaymentID, OrderID, PaymentMethod, Amount, PaymentDate, Status)
SELECT ROW_NUMBER() OVER (ORDER BY OrderID),
       OrderID,
       CASE MOD(OrderID, 4) WHEN 0 THEN 'Cash' WHEN 1 THEN 'Bank Transfer' WHEN 2 THEN 'Card' ELSE 'E-Wallet' END,
       TotalAmount,
       CASE WHEN Status = 'Pending' THEN NULL ELSE OrderDate END,
       CASE WHEN Status = 'Pending' THEN 'Unpaid' ELSE 'Paid' END
FROM Orders
WHERE Status <> 'Cancelled';

COMMIT;
