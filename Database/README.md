# Oracle setup

1. Connect SQL Developer to the application's PDB/schema (for example `XEPDB1`) using a dedicated schema owner. Do not run the application as SYS or SYSTEM.
2. Run `01_Schema.sql`.
3. Run `02_SalesInventoryOrders.sql`.
4. Run `03_OrderAssignment.sql` to add the online-order assignee column and foreign key. The online-order screen requires this migration.
5. Set the `SHOESTORE_ORACLE` environment variable, for example `User Id=SHOES_APP;Password=...;Data Source=localhost:1521/XEPDB1;`.
6. Start the app. If `APP_USER` is empty, create the first manager account; create employee profiles and staff accounts before assigning online orders.

The scripts use Oracle SQL (`NUMBER`, `VARCHAR2`, `NVARCHAR2`, sequences, `DUAL`, `ROWNUM`, and Oracle bind variables). `02_SalesInventoryOrders.sql` creates order status history, which the online-order form displays as a timeline. `03_OrderAssignment.sql` is idempotent and adds assignment support.
