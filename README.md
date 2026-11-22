# Quản lý bán giày thể thao

Ứng dụng Windows Forms (.NET 8) dùng Oracle làm nơi lưu trữ dữ liệu và ODP.NET Managed để kết nối. Bộ chạy hiện tại được cấu hình cho PDB `ORCLPDB`, schema mẫu `BTL_GIAYTHE_TEST` và hai tài khoản đăng nhập phân quyền `QL_GIAYTHE` / `NV_GIAYTHE`.

## Trạng thái hiện tại

- Schema test có 12 bảng và dữ liệu mẫu; lần chạy vừa xác nhận tạo được 130 sản phẩm, 1.300 biến thể, 30 đơn, 46 chi tiết đơn và 28 thanh toán. Dùng script `Database/TestData/04_KIEM_TRA_DU_LIEU_MAU.sql` để xem số dòng đang có.
- Visual Studio build được ứng dụng với 0 cảnh báo và 0 lỗi.
- Nếu PDB vẫn ở `RESTRICTED=YES`, hai tài khoản ứng dụng phải được cấp `RESTRICTED SESSION` trực tiếp trong PDB. Quyền này chỉ cần khi database còn Restricted; chi tiết ở phần [Oracle đang Restricted](#oracle-dang-restricted).
- Lỗi đồng bộ user `HR` trong PDB chưa được xử lý; không xóa user `HR` nào. Dữ liệu ứng dụng bán giày nằm trong schema `BTL_GIAYTHE_TEST`, tách khỏi các schema `HR`.

## Tài khoản và cấu hình

| Dùng cho | Tài khoản | Mật khẩu ban đầu |
|---|---|---|
| Đăng nhập quản lý | `QL_GIAYTHE` | `QuanLy@2026` |
| Đăng nhập nhân viên | `NV_GIAYTHE` | `NhanVien@2026` |
| Chủ sở hữu bảng test, chỉ dùng để chạy script SQL | `BTL_GIAYTHE_TEST` | `BTLTest@2026` |

Mật khẩu chỉ đúng nếu được tạo theo các script của dự án và chưa bị đổi. Không đăng nhập ứng dụng bằng `SYS` hoặc `BTL_GIAYTHE_TEST`; app xác thực role Oracle của `QL_GIAYTHE` / `NV_GIAYTHE`.

File local [App.config](App.config) hiện đặt:

- `OracleSchemaOwner=BTL_GIAYTHE_TEST`
- `Data Source=localhost:1521/ORCLPDB`
- User và mật khẩu được nhập tại màn hình đăng nhập, không lưu trong file cấu hình.

Nếu máy khác dùng service/host/port khác, sửa `Data Source` trong `App.config`. File này được Git bỏ qua; dùng [App.config.example](App.config.example) làm mẫu và tạo `App.config` cục bộ.

## Chạy nhanh ứng dụng đã cài database

1. Đảm bảo Oracle listener và PDB `ORCLPDB` đang chạy `READ WRITE`.
2. Mở `QuanLyBanGiay.csproj` bằng Visual Studio 2022. Cần workload **.NET desktop development** và .NET 8 SDK.
3. Mở `App.config`, xác nhận schema là `BTL_GIAYTHE_TEST` và service là service PDB trong SQL Developer.
4. Nếu PDB đang Restricted, thực hiện phần [Oracle đang Restricted](#oracle-dang-restricted) trước khi đăng nhập app.
5. Chọn cấu hình **Debug / Any CPU**, dùng **Build > Build Solution**, sau đó chạy **F5** hoặc **Ctrl+F5**.
6. Đăng nhập `QL_GIAYTHE` hoặc `NV_GIAYTHE` và mật khẩu tương ứng ở bảng trên.

Nút **Kiểm tra kết nối Oracle** cũng yêu cầu nhập username và password trước khi bấm. Nếu các ô trống, thông báo yêu cầu nhập tài khoản là hoạt động kiểm tra đầu vào bình thường.

## Cài database test từ đầu

Các bước này dựng schema cô lập để demo; không cần import DMP bàn giao. Chỉ chạy trên Oracle database/PDB mà bạn được phép dùng để test. Nếu user, role hoặc bảng đã tồn tại, kiểm tra trước và không chạy lại script tạo.

### 1. Chuẩn bị PDB và role ứng dụng

Trong SQL Developer, tạo connection DBA tới service PDB, user `SYS`, **Role = SYSDBA**. Xác nhận container:

```sql
SELECT USER, SYS_CONTEXT('USERENV','CON_NAME') AS CON_NAME,
       SYS_CONTEXT('USERENV','SERVICE_NAME') AS SERVICE_NAME
FROM DUAL;
```

Nếu đang ở `CDB$ROOT`, hãy chuyển connection/container sang PDB `ORCLPDB` trước khi tạo user cục bộ. Trên connection DBA/PDB, kiểm tra các role và app user:

```sql
SELECT role FROM dba_roles
WHERE role IN ('RL_GIAYTHE_QUANLY', 'RL_GIAYTHE_NHANVIEN');

SELECT username, account_status FROM dba_users
WHERE username IN ('QL_GIAYTHE', 'NV_GIAYTHE');
```

Chỉ khi cả hai role và hai user đều chưa tồn tại, chạy `BAN_GIAO_DATABASE/02_PHAN_QUYEN/01_TAO_ROLE_USER.sql` bằng **F5** một lần. Script tạo hai role `RL_GIAYTHE_QUANLY`, `RL_GIAYTHE_NHANVIEN` và hai user đăng nhập ứng dụng. Nếu đã có bất kỳ đối tượng nào, không chạy lại toàn bộ file tạo; nhờ DBA kiểm tra và tạo phần còn thiếu.

### 2. Tạo chủ sở hữu schema test

Trên connection DBA đang ở PDB, mở `Database/TestData/01_TAO_TEST_SCHEMA.sql` và nhấn **F5** một lần. File tạo `BTL_GIAYTHE_TEST` với quyền tạo session/bảng và quota trên tablespace `USERS`.

Tạo connection riêng trong SQL Developer:

- Username: `BTL_GIAYTHE_TEST`
- Password: `BTLTest@2026` (nếu chưa đổi)
- Host/port/service: giống connection DBA tới `ORCLPDB`

Bấm **Test**, rồi **Connect**. Nếu PDB đang Restricted, trước khi kết nối user này, dùng SYSDBA cấp quyền theo mục [Oracle đang Restricted](#oracle-dang-restricted). User này chỉ sở hữu bảng và chạy setup; không dùng để đăng nhập app.

### 3. Tạo 12 bảng, nạp dữ liệu và cấp object privileges

Giữ connection `BTL_GIAYTHE_TEST`, lần lượt mở và chạy bằng **F5**:

1. `Database/TestData/02_TAO_BANG_VA_DU_LIEU_MAU.sql` — tạo 12 bảng, ràng buộc và dữ liệu mẫu. Chỉ chạy một lần trên schema trống.
2. `Database/TestData/03_CAP_QUYEN_TEST.sql` — cấp quyền trên các bảng cho role quản lý/nhân viên. Chạy sau khi 12 bảng được tạo thành công.
3. `Database/TestData/04_KIEM_TRA_DU_LIEU_MAU.sql` — kiểm tra số dòng và xem mẫu sản phẩm/đơn.
4. `Database/TestData/05_TRUY_VAN_NGHIEP_VU.sql` — bộ truy vấn đọc, join, nhóm, thống kê và truy vấn lồng phục vụ demo/báo cáo.

Không chạy `BAN_GIAO_DATABASE/02_PHAN_QUYEN/02_CAP_QUYEN.sql` trên schema test; file đó chỉ dành cho schema `BTL_GIAYTHETHAO` sau khi import DMP.

## Oracle đang Restricted

Nếu `ORCLPDB` mở `READ WRITE` nhưng `RESTRICTED=YES`, user thông thường sẽ gặp `ORA-01035`. Trên connection DBA dùng `SYS`, **Role = SYSDBA**, và đang ở service/PDB `ORCLPDB`, cấp Restricted Session cho user cần đăng nhập. Xác nhận `USER=SYS` và `CON_NAME=ORCLPDB` trước khi chạy lệnh.

Khi vừa tạo owner schema test, nếu cần đăng nhập `BTL_GIAYTHE_TEST` để chạy scripts 02–05, DBA cấp:

```sql
GRANT RESTRICTED SESSION TO BTL_GIAYTHE_TEST;
```

Trước khi chạy ứng dụng, DBA cấp cho hai user app:

```sql
GRANT RESTRICTED SESSION TO QL_GIAYTHE;
GRANT RESTRICTED SESSION TO NV_GIAYTHE;
```

Không chạy các lệnh `GRANT RESTRICTED SESSION` bằng connection `BTL_GIAYTHE_TEST`: owner schema không có quyền cấp system privilege và sẽ gặp `ORA-01031`. Nếu sau này PDB hết Restricted và đã được DBA xác nhận ổn định, có thể thu hồi các quyền tạm bằng `REVOKE RESTRICTED SESSION FROM ...`.

Trong môi trường hiện tại, Oracle ghi nhận lỗi đồng bộ user `HR` giữa CDB root và PDB; PDB có schema HR mẫu, root cũng có user HR với dữ liệu riêng. Giữ nguyên cả hai schema trừ khi đã sao lưu và chủ động chọn dữ liệu cần bỏ. Restricted Session là cách cho phép tài khoản app/test kết nối để demo, không phải cách sửa lỗi đồng bộ.

## Kịch bản demo

### Demo quản lý

1. Đăng nhập `QL_GIAYTHE`; xác nhận dashboard đọc doanh thu, đơn đang xử lý, số biến thể hết hàng và danh sách đơn mới từ Oracle.
2. Mở **Sản phẩm**, tìm theo tên/thương hiệu, thêm mẫu hoặc biến thể, sửa thông tin và ngừng kinh doanh sản phẩm.
3. Mở **Khách hàng & nhân viên**, thêm và sửa hồ sơ. Dùng số điện thoại/email chưa trùng để không vi phạm unique constraints.
4. Mở **Kho**, chọn biến thể và tăng tồn. Schema hiện không lưu phiếu nhập, giá vốn hoặc lịch sử dịch chuyển kho.
5. Mở **POS**, chọn khách/nhân viên và biến thể còn hàng, thanh toán. Kiểm tra đơn `Completed`, payment `Paid` và số tồn giảm.
6. Mở **Báo cáo**, chọn khoảng ngày, xem doanh thu, top bán chạy, tồn bằng 0, đơn theo trạng thái và xuất CSV.

### Demo nhân viên và vòng đời đơn online

1. Đăng xuất, đăng nhập `NV_GIAYTHE`; thử xem sản phẩm/dashboard và lập đơn cho số điện thoại khách đã tồn tại.
2. Đơn mới ở trạng thái `Pending`, payment `Unpaid`; tồn kho chưa bị trừ/giữ.
3. Đăng nhập lại quản lý, chuyển đơn sang `Shipping`, rồi `Completed`. Khi hoàn tất, app kiểm tra tồn, trừ kho và đánh dấu payment `Paid` trong transaction.
4. Có thể thử hủy đơn chưa hoàn tất. Trạng thái `Completed`/`Cancelled` là trạng thái cuối; thao tác cập nhật lỗi sẽ rollback transaction.

## Chức năng đã hoàn thành

- Đăng nhập Oracle bằng tài khoản thật, đọc `SESSION_ROLES` để phân quyền giao diện và kiểm tra quyền thao tác ở tầng nghiệp vụ.
- Dashboard, danh sách/tìm kiếm sản phẩm, cập nhật mẫu/biến thể, quản lý khách hàng/nhân viên.
- POS tạo đơn, chi tiết và payment, cập nhật tồn kho và in hóa đơn.
- Tiếp nhận đơn online, lọc/tìm kiếm, chuyển trạng thái và thanh toán khi hoàn tất.
- Quản lý tồn theo biến thể, báo cáo doanh thu/top sản phẩm/tồn hết hàng/trạng thái đơn, xuất CSV.
- Dữ liệu nghiệp vụ đọc/ghi trực tiếp bằng Oracle SQL. Các tham số nhập từ giao diện dùng bind parameters; các luồng ghi chính dùng transaction và rollback khi lỗi.

### Giới hạn đã biết của schema bàn giao

Schema bàn giao không có cột giữ tồn, lịch sử trạng thái, người phân công, phiếu nhập, giá vốn, lịch sử tồn, liên kết user Oracle với `EmployeeID` hoặc sequence. Vì vậy:

- Đơn mới không giữ tồn và không ghi audit trail trạng thái.
- Màn hình không lưu ghi chú/phân công/người xử lý; lịch sử và phân công đơn là phần placeholder, không dùng trong demo.
- Form tiếp nhận đơn yêu cầu số điện thoại khách đã có; tên/địa chỉ nhập ở form không được lưu vào schema hiện tại.
- Điều chỉnh tồn chỉ cập nhật `ProductVariants.StockQuantity`; không tạo phiếu nhập. Các ô nhà cung cấp/giá vốn/nhân viên trên form kho không được lưu.
- `Products.Price` ở cấp mẫu, không phải giá riêng cho từng màu/cỡ; `VariantID` được dùng làm mã biến thể hiển thị.
- ID được tạo bởi ứng dụng từ `MAX(ID)+1` với khóa bảng/transaction trong luồng ứng dụng. Đây là cách phù hợp cho project demo đơn máy; triển khai nhiều client ghi đồng thời nên chuyển sang sequence/identity.

## Ứng dụng liên hệ Oracle ở đâu?

| Phần Oracle trong báo cáo/môn học | Ánh xạ trong project | Cách trình bày/demo |
|---|---|---|
| Kết nối CDB/PDB, service name | `App.config`, ODP.NET trong `UI/LoginForm.cs` | Chỉ service `ORCLPDB` vào PDB; app không kết nối CDB root để thao tác dữ liệu bán hàng. |
| Schema, bảng và dữ liệu | `BTL_GIAYTHE_TEST` + `Database/TestData/02...sql`; bản thật ở DMP `BTL_GIAYTHETHAO` | Xem 12 bảng, PK/FK/check/unique constraints và dữ liệu qua SQL Developer. |
| User, role và object privileges | `01_TAO_ROLE_USER.sql`, `03_CAP_QUYEN_TEST.sql`, `02_CAP_QUYEN.sql` | Quản lý/nhân viên nhận quyền khác nhau qua `RL_GIAYTHE_QUANLY` và `RL_GIAYTHE_NHANVIEN`. |
| Xác thực và phân quyền ứng dụng | `AuthRepository` trong `UI/LoginForm.cs`, `MainForm.Require` | App đọc Oracle user và role đang bật trong `SESSION_ROLES`; POS/kho/báo cáo chỉ cho quản lý. |
| SQL nghiệp vụ | Các repository/service trong `UI/*.cs`, `05_TRUY_VAN_NGHIEP_VU.sql` | `SELECT`, `JOIN`, `GROUP BY`, hàm tổng hợp, lọc và bind parameters. |
| Transaction và toàn vẹn dữ liệu | `UI/PosForm.cs`, `UI/OnlineOrdersForm.cs`, `UI/ProductsForm.cs`, `UI/CustomerEmployeeForm.cs` | Demo một giao dịch ghi nhiều bảng, trừ tồn, `COMMIT` khi thành công, `ROLLBACK` khi lỗi. |
| Quản trị Oracle ngoài ứng dụng | `BAN_GIAO_DATABASE/` | Import DMP, cấp quyền, kiểm tra role/dữ liệu bằng SQL Developer/impdp. Đây là quy trình DBA, không phải chức năng WinForms. |
| Instance, tablespace/datafile, segment/extent, backup/recovery | Tài liệu/scripts quản trị riêng của nhóm, không nằm trong app bán hàng | Trình bày ở phần Oracle DBA của báo cáo; không mô tả như chức năng phần mềm bán giày. |

`OracleSchemaOwner` đặt `CURRENT_SCHEMA` để truy vấn dùng tên bảng ngắn; nó không tự cấp quyền. User đăng nhập vẫn cần object privileges qua role. `SYS` chỉ dùng cài đặt/quản trị, không dùng làm tài khoản app.

## Bộ DMP bàn giao

`BAN_GIAO_DATABASE/01_DATABASE/BTL_GIAYTHETHAO.dmp` là export schema, không phải backup toàn bộ CDB và không tự tạo user/role ứng dụng. Nếu cần chạy trên schema bàn giao:

1. Đọc `BAN_GIAO_DATABASE/04_HUONG_DAN/HUONG_DAN_IMPORT.txt` và xác minh database đích.
2. DBA tạo Oracle `DIRECTORY` trỏ tới thư mục trên máy chủ Oracle; chạy `impdp` từ command line với service/schema đích.
3. Sau import thành công, chạy `02_CAP_QUYEN.sql`, rồi hai file kiểm tra trong `03_KIEM_TRA`.
4. Đổi `OracleSchemaOwner` trong `App.config` thành `BTL_GIAYTHETHAO` và giữ service PDB đúng.

Không import DMP vào database nguồn hoặc ghi đè schema có dữ liệu khi chưa có backup và kế hoạch. Luồng test và luồng DMP là hai lựa chọn riêng.

## Cấu trúc thư mục

```text
QuanLyBanGiay/
├── README.md                         # setup, chạy, demo, Oracle mapping
├── App.config.example                # cấu hình mẫu; tạo App.config cục bộ
├── Program.cs / QuanLyBanGiay.csproj # điểm khởi chạy và cấu hình .NET 8
├── UI/                               # WinForms, service/query Oracle, resources từng Form
├── Database/TestData/                # scripts test chạy theo thứ tự 01–05
└── BAN_GIAO_DATABASE/                # DMP, export log, DBA scripts và hướng dẫn import
```

`.vs/`, `bin/`, `obj/`, `*.user` và `App.config` là file cục bộ/cache/build được Git bỏ qua; không chạy chúng như script database và không cần đóng gói vào source bàn giao. Các file `.resx` giống nhau một phần là resource mặc định riêng cho từng Form WinForms; giữ lại để Visual Studio Designer hoạt động bình thường.
