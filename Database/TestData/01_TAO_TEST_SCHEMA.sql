-- Chạy bằng SYSTEM/SYS trên đúng service Oracle dùng để kiểm thử.
-- Tạo schema cô lập; không xóa hay thay đổi schema BTL_GIAYTHETHAO.
-- Chỉ chạy một lần. Nếu user đã tồn tại, kiểm tra trước khi tạo lại.
CREATE USER BTL_GIAYTHE_TEST IDENTIFIED BY "BTLTest@2026"
  DEFAULT TABLESPACE USERS
  TEMPORARY TABLESPACE TEMP
  QUOTA 100M ON USERS;

GRANT CREATE SESSION, CREATE TABLE TO BTL_GIAYTHE_TEST;
