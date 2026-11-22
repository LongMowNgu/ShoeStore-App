-- Kiem tra tai khoan, role, role assignment va quyen bang.
-- Chay bang tai khoan co quyen doc cac DBA_* views (vi du SYSTEM) tren DATABASE DICH.

-- User va role phai ton tai, user phai OPEN.
SELECT username, account_status, default_tablespace
FROM dba_users
WHERE username IN ('BTL_GIAYTHETHAO','QL_GIAYTHE','NV_GIAYTHE')
ORDER BY username;

SELECT role
FROM dba_roles
WHERE role IN ('RL_GIAYTHE_QUANLY','RL_GIAYTHE_NHANVIEN')
ORDER BY role;

-- Moi user ung dung duoc gan dung role.
SELECT grantee, granted_role, default_role
FROM dba_role_privs
WHERE grantee IN ('QL_GIAYTHE','NV_GIAYTHE')
ORDER BY grantee, granted_role;

-- Danh sach quyen va bang thuc te theo role.
SELECT grantee, privilege, table_name
FROM dba_tab_privs
WHERE owner='BTL_GIAYTHETHAO'
  AND grantee IN ('RL_GIAYTHE_QUANLY','RL_GIAYTHE_NHANVIEN')
ORDER BY grantee, privilege, table_name;

-- So luong quyen mong doi: quan ly 12 bang moi loai; nhan vien SELECT 10, INSERT 3.
SELECT grantee, privilege, COUNT(*) AS object_count
FROM dba_tab_privs
WHERE owner='BTL_GIAYTHETHAO'
  AND grantee IN ('RL_GIAYTHE_QUANLY','RL_GIAYTHE_NHANVIEN')
GROUP BY grantee, privilege
ORDER BY grantee, privilege;

-- Ket qua mong doi: 0 dong. Nhan vien khong co UPDATE/DELETE tren schema du an.
SELECT grantee, privilege, table_name
FROM dba_tab_privs
WHERE owner='BTL_GIAYTHETHAO'
  AND grantee='RL_GIAYTHE_NHANVIEN'
  AND privilege IN ('UPDATE','DELETE')
ORDER BY privilege, table_name;
