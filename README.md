# ShoeStore WinForms (Oracle)

Ứng dụng WinForms quản lý cửa hàng giày kết nối Oracle, gồm sản phẩm/biến thể, bán hàng tại quầy, nhập kho, đơn online, báo cáo và tài khoản.

## Cài đặt CSDL

Trong SQL Developer, kết nối bằng schema ứng dụng trên PDB (không dùng SYS/SYSTEM), sau đó chạy theo thứ tự:

1. `Database/01_Schema.sql`
2. `Database/02_SalesInventoryOrders.sql`
3. `Database/03_OrderAssignment.sql`

Script thứ ba thêm khóa ngoại và cột nhân viên được giao xử lý đơn online. Có thể chạy lại script này an toàn.

## Kết nối và chạy

Đặt biến môi trường `SHOESTORE_ORACLE`, ví dụ:

```text
User Id=SHOES_APP;Password=your_password;Data Source=localhost:1521/XEPDB1;
```

Mở `ShoeStore.WinForms.csproj` bằng Visual Studio 2022 có workload .NET desktop development, restore NuGet và chạy. Máy demo cần kết nối được Oracle listener/PDB. Nếu `APP_USER` chưa có tài khoản, tạo quản lý đầu tiên từ màn hình đăng nhập; quản lý có thể tạo tài khoản nhân viên.

## Luồng demo đơn online

- Tìm theo mã đơn, tên khách hoặc số điện thoại; lọc theo trạng thái.
- Chọn đơn để xem lịch sử cập nhật, người thao tác và ghi chú.
- Phân công đơn cho tài khoản nhân viên thu ngân/kho đang hoạt động.
- Cập nhật trạng thái theo luồng nghiệp vụ; xác nhận giữ tồn, hủy/thất bại hoàn tồn.

Ứng dụng dùng ODP.NET Managed Driver, bind parameters, transaction và PBKDF2-SHA256 kèm salt cho mật khẩu. Tài khoản Oracle không được nhúng trong mã nguồn.
