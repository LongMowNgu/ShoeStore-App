-- Mo rong schema de luu phan cong va nhat ky don hang.
-- Chay mot lan bang user chu so huu schema (BTL_GIAYTHE_TEST hoac BTL_GIAYTHETHAO).
-- Script tao cac bang trong schema dang ket noi; khong chay bang SYS/SYSDBA.

CREATE TABLE OrderAssignments (
    OrderID     NUMBER(6)    CONSTRAINT pk_order_assignments PRIMARY KEY,
    EmployeeID  NUMBER(4)    NOT NULL,
    AssignedAt  DATE         DEFAULT SYSDATE NOT NULL,
    AssignedBy  VARCHAR2(128) NOT NULL,
    CONSTRAINT fk_assign_order FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    CONSTRAINT fk_assign_employee FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);

CREATE TABLE OrderHistory (
    HistoryID     NUMBER(10) CONSTRAINT pk_order_history PRIMARY KEY,
    OrderID       NUMBER(6) NOT NULL,
    ChangedAt     DATE DEFAULT SYSDATE NOT NULL,
    EventType     VARCHAR2(20) NOT NULL,
    OldStatus     VARCHAR2(15),
    NewStatus     VARCHAR2(15),
    OldEmployeeID NUMBER(4),
    NewEmployeeID NUMBER(4),
    Note          NVARCHAR2(500),
    ChangedBy     VARCHAR2(128) NOT NULL,
    CONSTRAINT fk_history_order FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    CONSTRAINT fk_history_old_employee FOREIGN KEY (OldEmployeeID) REFERENCES Employees(EmployeeID),
    CONSTRAINT fk_history_new_employee FOREIGN KEY (NewEmployeeID) REFERENCES Employees(EmployeeID),
    CONSTRAINT ck_history_event CHECK (EventType IN ('CREATED','STATUS','ASSIGNED','BASELINE'))
);

-- Lich su truoc thoi diem nang cap khong duoc luu trong schema cu.
-- Tao mot moc hien trang ro rang, khong gia lap cac su kien trong qua khu.
INSERT INTO OrderHistory (HistoryID, OrderID, ChangedAt, EventType, OldStatus, NewStatus, Note, ChangedBy)
SELECT ROW_NUMBER() OVER (ORDER BY OrderID), OrderID, SYSDATE, 'BASELINE', NULL, Status,
       N'Ghi nhận trạng thái hiện tại khi bật chức năng lịch sử.', 'MIGRATION'
FROM Orders;

COMMIT;

SELECT table_name FROM user_tables
WHERE table_name IN ('ORDERASSIGNMENTS','ORDERHISTORY')
ORDER BY table_name;
