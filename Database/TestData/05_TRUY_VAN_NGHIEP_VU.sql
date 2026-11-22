-- Chạy trên schema BTL_GIAYTHE_TEST sau khi nạp dữ liệu mẫu.
-- Bộ truy vấn nghiệp vụ và truy vấn lồng của thành viên 2; chỉ đọc dữ liệu.

-- =============== PHẦN A: 15 TRUY VẤN NGHIỆP VỤ ===============

-- Q1. Hiển thị toàn bộ sản phẩm
-- Mục đích: xem danh mục hàng kèm thương hiệu, danh mục. Mong đợi: 130 dòng.
SELECT p.ProductID, p.ProductName, b.BrandName, c.CategoryName, p.Gender, p.Price, p.Status
FROM Products p
JOIN Brands b     ON b.BrandID = p.BrandID
JOIN Categories c ON c.CategoryID = p.CategoryID
ORDER BY p.ProductID;

-- Q2. Tìm sản phẩm theo từ khóa trong tên (không phân biệt hoa thường)
-- Đổi 'air max' thành từ khóa khác. Mong đợi: Air Max 90, 270, 1.
SELECT ProductID, ProductName, Price
FROM Products
WHERE UPPER(ProductName) LIKE '%' || UPPER('air max') || '%'
ORDER BY ProductName;

-- Q3. Lọc theo thương hiệu. Mong đợi (Adidas): 18 dòng.
SELECT p.ProductID, p.ProductName, p.Price
FROM Products p
JOIN Brands b ON b.BrandID = p.BrandID
WHERE b.BrandName = 'Adidas'
ORDER BY p.Price DESC;

-- Q4. Lọc theo danh mục giày (Bóng đá)
SELECT p.ProductID, p.ProductName, b.BrandName, p.Price
FROM Products p
JOIN Brands b     ON b.BrandID = p.BrandID
JOIN Categories c ON c.CategoryID = p.CategoryID
WHERE c.CategoryName = 'Bóng đá'
ORDER BY b.BrandName, p.ProductName;

-- Q5. Lọc theo màu sắc (còn hàng trong kho)
SELECT p.ProductName, col.ColorName, s.SizeValue, v.StockQuantity
FROM ProductVariants v
JOIN Products p ON p.ProductID = v.ProductID
JOIN Colors col ON col.ColorID = v.ColorID
JOIN Sizes s    ON s.SizeID = v.SizeID
WHERE col.ColorName = 'Đen' AND v.StockQuantity > 0
ORDER BY p.ProductName, s.SizeValue;

-- Q6. Lọc theo kích cỡ (size 42 còn hàng)
SELECT p.ProductName, col.ColorName, s.SizeValue, v.StockQuantity
FROM ProductVariants v
JOIN Products p ON p.ProductID = v.ProductID
JOIN Colors col ON col.ColorID = v.ColorID
JOIN Sizes s    ON s.SizeID = v.SizeID
WHERE s.SizeValue = 42 AND v.StockQuantity > 0
ORDER BY p.ProductName;

-- Q7. Lọc theo giới tính (Nu) kèm số lượng tồn tổng của mẫu
SELECT p.ProductID, p.ProductName, b.BrandName, p.Price,
       SUM(v.StockQuantity) AS TongTon
FROM Products p
JOIN Brands b ON b.BrandID = p.BrandID
LEFT JOIN ProductVariants v ON v.ProductID = p.ProductID
WHERE p.Gender = 'Nu'
GROUP BY p.ProductID, p.ProductName, b.BrandName, p.Price
ORDER BY p.ProductID;

-- Q8. Tìm sản phẩm theo khoảng giá (2.000.000 - 3.500.000 VND)
SELECT ProductID, ProductName, Price
FROM Products
WHERE Price BETWEEN 2000000 AND 3500000
ORDER BY Price, ProductName;

-- Q9. Sản phẩm (biến thể) có tồn kho thấp (<= 3) cần nhập thêm
SELECT p.ProductName, col.ColorName, s.SizeValue, v.StockQuantity
FROM ProductVariants v
JOIN Products p ON p.ProductID = v.ProductID
JOIN Colors col ON col.ColorID = v.ColorID
JOIN Sizes s    ON s.SizeID = v.SizeID
WHERE v.StockQuantity <= 3
ORDER BY v.StockQuantity, p.ProductName;

-- Q10. Số mẫu giày theo thương hiệu + kiểm tra tổng 130
-- Cột Dat = 'OK' nếu đúng chỉ tiêu. Dòng cuối (TONG CONG) phải = 130.
SELECT NVL(b.BrandName, 'TONG CONG') AS Thuong_hieu,
       COUNT(p.ProductID) AS So_mau,
       CASE WHEN b.BrandName IS NULL THEN CASE WHEN COUNT(p.ProductID) = 130 THEN 'OK' ELSE 'SAI' END
            ELSE CASE WHEN COUNT(p.ProductID) = CASE b.BrandName
                 WHEN 'Nike' THEN 20 WHEN 'Adidas' THEN 18 WHEN 'Puma' THEN 13
                 WHEN 'New Balance' THEN 14 WHEN 'ASICS' THEN 14 WHEN 'Skechers' THEN 12
                 WHEN 'Mizuno' THEN 11 WHEN 'Li-Ning' THEN 12 WHEN 'ANTA' THEN 8
                 WHEN 'Under Armour' THEN 8 END THEN 'OK' ELSE 'SAI' END
       END AS Dat
FROM Brands b
LEFT JOIN Products p ON p.BrandID = b.BrandID
GROUP BY ROLLUP(b.BrandName)
ORDER BY GROUPING(b.BrandName), So_mau DESC;

-- Q11. Thông tin khách hàng + số đơn + tổng chi tiêu (không tính đơn hủy)
SELECT c.CustomerID, c.FullName, c.Phone, c.Email, c.Address,
       COUNT(o.OrderID) AS SoDon,
       NVL(SUM(o.TotalAmount), 0) AS TongChiTieu
FROM Customers c
LEFT JOIN Orders o ON o.CustomerID = c.CustomerID AND o.Status <> 'Cancelled'
GROUP BY c.CustomerID, c.FullName, c.Phone, c.Email, c.Address
ORDER BY TongChiTieu DESC;

-- Q12. Đơn hàng + khách hàng + nhân viên (LEFT JOIN vì đơn online không có nhân viên)
SELECT o.OrderID, TO_CHAR(o.OrderDate, 'DD/MM/YYYY') AS NgayDat, o.Status,
       c.FullName AS KhachHang,
       NVL(e.FullName, '(Đơn online)') AS NhanVien,
       o.TotalAmount
FROM Orders o
JOIN Customers c      ON c.CustomerID = o.CustomerID
LEFT JOIN Employees e ON e.EmployeeID = o.EmployeeID
ORDER BY o.OrderID;

-- Q13. Tổng tiền từng đơn tính từ chi tiết, so sánh với Orders.TotalAmount
-- Cột KiemTra phải toàn 'KHOP'.
SELECT o.OrderID,
       SUM(d.Quantity) AS TongSoLuong,
       SUM(d.Quantity * d.UnitPrice) AS TongTienTuChiTiet,
       o.TotalAmount,
       CASE WHEN SUM(d.Quantity * d.UnitPrice) = o.TotalAmount THEN 'KHOP' ELSE 'LECH' END AS KiemTra
FROM Orders o
JOIN OrderDetails d ON d.OrderID = o.OrderID
GROUP BY o.OrderID, o.TotalAmount
ORDER BY o.OrderID;

-- Q14a. Doanh thu theo tháng (không tính đơn hủy)
SELECT TO_CHAR(OrderDate, 'YYYY-MM') AS Thang,
       COUNT(*) AS SoDon, SUM(TotalAmount) AS DoanhThu
FROM Orders
WHERE Status <> 'Cancelled'
GROUP BY TO_CHAR(OrderDate, 'YYYY-MM')
ORDER BY Thang;

-- Q14b. Doanh thu theo ngày trong một khoảng thời gian
SELECT TRUNC(OrderDate) AS Ngay, COUNT(*) AS SoDon, SUM(TotalAmount) AS DoanhThu
FROM Orders
WHERE Status <> 'Cancelled'
  AND OrderDate BETWEEN DATE '2026-10-01' AND DATE '2026-10-24'
GROUP BY TRUNC(OrderDate)
ORDER BY Ngay;

-- Q14c. Tổng doanh thu cả giai đoạn
SELECT SUM(TotalAmount) AS TongDoanhThu FROM Orders WHERE Status <> 'Cancelled';

-- Q15a. Top 10 mẫu giày bán chạy nhất (theo số lượng, không tính đơn hủy)
SELECT p.ProductName, b.BrandName,
       SUM(d.Quantity) AS SoLuongBan,
       SUM(d.Quantity * d.UnitPrice) AS DoanhThu
FROM OrderDetails d
JOIN Orders o          ON o.OrderID = d.OrderID AND o.Status <> 'Cancelled'
JOIN ProductVariants v ON v.VariantID = d.VariantID
JOIN Products p        ON p.ProductID = v.ProductID
JOIN Brands b          ON b.BrandID = p.BrandID
GROUP BY p.ProductName, b.BrandName
ORDER BY SoLuongBan DESC, DoanhThu DESC
FETCH FIRST 10 ROWS ONLY;      -- cần Oracle 12c trở lên

-- Q15b. Số lượng bán và doanh thu theo thương hiệu
SELECT b.BrandName,
       NVL(SUM(d.Quantity), 0) AS SoLuongBan,
       NVL(SUM(d.Quantity * d.UnitPrice), 0) AS DoanhThu
FROM Brands b
LEFT JOIN Products p        ON p.BrandID = b.BrandID
LEFT JOIN ProductVariants v ON v.ProductID = p.ProductID
LEFT JOIN OrderDetails d    ON d.VariantID = v.VariantID
LEFT JOIN Orders o          ON o.OrderID = d.OrderID AND o.Status <> 'Cancelled'
WHERE d.OrderID IS NULL OR o.OrderID IS NOT NULL
GROUP BY b.BrandName
ORDER BY DoanhThu DESC;

-- ---- Truy vấn lồng (subquery) bổ sung cho yêu cầu báo cáo ----
-- N1. Mẫu giày có giá cao hơn giá trung bình
SELECT ProductName, Price
FROM Products
WHERE Price > (SELECT AVG(Price) FROM Products)
ORDER BY Price DESC;

-- N2. Mẫu giày chưa từng được bán
SELECT p.ProductID, p.ProductName
FROM Products p
WHERE NOT EXISTS (SELECT 1
                  FROM ProductVariants v
                  JOIN OrderDetails d ON d.VariantID = v.VariantID
                  WHERE v.ProductID = p.ProductID)
ORDER BY p.ProductID;

-- N3. Khách hàng chi tiêu cao hơn mức chi tiêu trung bình của các khách
SELECT c.FullName, SUM(o.TotalAmount) AS TongChiTieu
FROM Customers c
JOIN Orders o ON o.CustomerID = c.CustomerID AND o.Status <> 'Cancelled'
GROUP BY c.FullName
HAVING SUM(o.TotalAmount) > (SELECT AVG(t.tong)
                             FROM (SELECT SUM(TotalAmount) AS tong
                                   FROM Orders WHERE Status <> 'Cancelled'
                                   GROUP BY CustomerID) t)
ORDER BY TongChiTieu DESC;

