-- Chạy bằng BTL_GIAYTHE_TEST để kiểm tra bảng và dữ liệu mẫu.
SELECT 'BRANDS' AS TABLE_NAME, COUNT(*) AS ROW_COUNT FROM BRANDS UNION ALL
SELECT 'CATEGORIES', COUNT(*) FROM CATEGORIES UNION ALL
SELECT 'COLORS', COUNT(*) FROM COLORS UNION ALL
SELECT 'CUSTOMERS', COUNT(*) FROM CUSTOMERS UNION ALL
SELECT 'EMPLOYEES', COUNT(*) FROM EMPLOYEES UNION ALL
SELECT 'ORDERDETAILS', COUNT(*) FROM ORDERDETAILS UNION ALL
SELECT 'ORDERS', COUNT(*) FROM ORDERS UNION ALL
SELECT 'PAYMENTS', COUNT(*) FROM PAYMENTS UNION ALL
SELECT 'PRODUCTS', COUNT(*) FROM PRODUCTS UNION ALL
SELECT 'PRODUCTVARIANTS', COUNT(*) FROM PRODUCTVARIANTS UNION ALL
SELECT 'SIZES', COUNT(*) FROM SIZES UNION ALL
SELECT 'SUPPLIERS', COUNT(*) FROM SUPPLIERS UNION ALL
SELECT 'ORDERASSIGNMENTS', COUNT(*) FROM ORDERASSIGNMENTS UNION ALL
SELECT 'ORDERHISTORY', COUNT(*) FROM ORDERHISTORY
ORDER BY 1;

-- Danh mục mẫu để xem trực quan sản phẩm, màu, cỡ và tồn kho.
SELECT p.ProductID, p.ProductName, b.BrandName, ca.CategoryName, p.Price,
       v.VariantID, cl.ColorName, sz.SizeValue, v.StockQuantity
FROM Products p
JOIN Brands b ON b.BrandID=p.BrandID
JOIN Categories ca ON ca.CategoryID=p.CategoryID
JOIN ProductVariants v ON v.ProductID=p.ProductID
JOIN Colors cl ON cl.ColorID=v.ColorID
JOIN Sizes sz ON sz.SizeID=v.SizeID
ORDER BY p.ProductID, sz.SizeValue, cl.ColorName
FETCH FIRST 25 ROWS ONLY;

-- Đơn mẫu và chi tiết thanh toán để kiểm tra màn hình đơn hàng/báo cáo.
SELECT o.OrderID, c.FullName AS CustomerName, o.OrderDate, o.Status,
       o.TotalAmount, pay.PaymentMethod, pay.Status AS PaymentStatus,
       COUNT(d.OrderDetailID) AS LineCount
FROM Orders o
JOIN Customers c ON c.CustomerID=o.CustomerID
LEFT JOIN Payments pay ON pay.OrderID=o.OrderID
LEFT JOIN OrderDetails d ON d.OrderID=o.OrderID
GROUP BY o.OrderID,c.FullName,o.OrderDate,o.Status,o.TotalAmount,pay.PaymentMethod,pay.Status
ORDER BY o.OrderID
FETCH FIRST 20 ROWS ONLY;
