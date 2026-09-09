# Kết quả kiểm chứng SQL bằng Self-Consistency — Đề tài 39

## Bài toán và lược đồ áp dụng

Tìm 10 dịch vụ được sử dụng nhiều nhất tại salon trong khoảng `:from_date` đến `:to_date`, chỉ tính hóa đơn có trạng thái `paid`.

Lược đồ dùng cho truy vấn:

- `services(id, code, name, category_id, price, duration_minutes, status)`
- `invoices(id, invoice_date, status, total_amount)`
- `invoice_items(id, invoice_id, service_id, quantity, unit_price)`

Các truy vấn dưới đây dùng cú pháp MySQL 8. `invoice_date` có thể có cả giờ; vì vậy khoảng thời gian dùng cận trái đóng và cận phải mở: từ đầu ngày `:from_date` đến trước ngày kế tiếp của `:to_date`.

## 1. Phương án 1

Gộp trực tiếp dữ liệu hóa đơn và chi tiết hóa đơn, sau đó nhóm theo dịch vụ.

```sql
SELECT
    s.id AS service_id,
    s.code,
    s.name,
    SUM(ii.quantity) AS total_quantity,
    SUM(ii.quantity * ii.unit_price) AS total_revenue
FROM invoices AS i
JOIN invoice_items AS ii ON ii.invoice_id = i.id
JOIN services AS s ON s.id = ii.service_id
WHERE i.status = 'paid'
  AND i.invoice_date >= :from_date
  AND i.invoice_date < DATE_ADD(:to_date, INTERVAL 1 DAY)
GROUP BY s.id, s.code, s.name
ORDER BY total_quantity DESC, total_revenue DESC
LIMIT 10;
```

Đánh giá: ngắn gọn, trực tiếp và trả đúng các cột yêu cầu. Tuy nhiên, nhóm và tính doanh thu diễn ra trong cùng truy vấn chính nên khi cần mở rộng điều kiện sẽ kém rõ ràng hơn phương án dùng CTE.

## 2. Phương án 2

Lọc hóa đơn hợp lệ trước, rồi tổng hợp chi tiết theo dịch vụ bằng CTE.

```sql
WITH paid_invoice_items AS (
    SELECT
        ii.service_id,
        ii.quantity,
        ii.unit_price
    FROM invoices AS i
    JOIN invoice_items AS ii ON ii.invoice_id = i.id
    WHERE i.status = 'paid'
      AND i.invoice_date >= :from_date
      AND i.invoice_date < DATE_ADD(:to_date, INTERVAL 1 DAY)
),
service_totals AS (
    SELECT
        service_id,
        SUM(quantity) AS total_quantity,
        SUM(quantity * unit_price) AS total_revenue
    FROM paid_invoice_items
    GROUP BY service_id
)
SELECT
    s.id AS service_id,
    s.code,
    s.name,
    st.total_quantity,
    st.total_revenue
FROM service_totals AS st
JOIN services AS s ON s.id = st.service_id
ORDER BY st.total_quantity DESC, st.total_revenue DESC
LIMIT 10;
```

Đánh giá: tách rõ ba bước lọc, tổng hợp và gắn thông tin dịch vụ; dễ kiểm tra và mở rộng. Kết quả tương đương phương án 1.

## 3. Phương án 3

Tổng hợp bằng bảng dẫn xuất rồi nối với danh mục dịch vụ.

```sql
SELECT
    s.id AS service_id,
    s.code,
    s.name,
    x.total_quantity,
    x.total_revenue
FROM services AS s
JOIN (
    SELECT
        ii.service_id,
        SUM(ii.quantity) AS total_quantity,
        SUM(ii.quantity * ii.unit_price) AS total_revenue
    FROM invoice_items AS ii
    JOIN invoices AS i ON i.id = ii.invoice_id
    WHERE i.status = 'paid'
      AND i.invoice_date >= :from_date
      AND i.invoice_date < DATE_ADD(:to_date, INTERVAL 1 DAY)
    GROUP BY ii.service_id
) AS x ON x.service_id = s.id
ORDER BY x.total_quantity DESC, x.total_revenue DESC
LIMIT 10;
```

Đánh giá: cũng trả kết quả đúng và cô lập phần tổng hợp. Tuy nhiên, tên bảng dẫn xuất ít diễn đạt ý nghĩa hơn CTE, nên khó đọc hơn khi truy vấn phát triển thêm.

## 4. Bảng kiểm tra edge case

| Edge case | Phương án 1 | Phương án 2 | Phương án 3 | Kết luận kiểm tra |
|---|---|---|---|---|
| Không có hóa đơn trong khoảng thời gian | Trả về 0 dòng. | Trả về 0 dòng. | Trả về 0 dòng. | Đúng; không dùng `LEFT JOIN` nên không xuất hiện dịch vụ có doanh thu 0. |
| Có hóa đơn bị hủy (`cancelled`) | Bị loại bởi điều kiện `i.status = 'paid'`. | Bị loại trước khi tổng hợp trong CTE đầu. | Bị loại trong bảng dẫn xuất. | Đúng; hóa đơn bị hủy không ảnh hưởng số lượng hoặc doanh thu. |
| Dịch vụ đã ngừng kinh doanh (`services.status = 'inactive'`) nhưng từng có hóa đơn đã thanh toán | Vẫn được tính. | Vẫn được tính. | Vẫn được tính. | Đúng cho báo cáo lịch sử; không lọc trạng thái dịch vụ hiện tại. |
| Hóa đơn lập lúc 23:30 của ngày `:to_date` | Được tính nhờ cận phải là trước ngày kế tiếp. | Được tính. | Được tính. | Đúng với cột ngày giờ, tránh bỏ sót cuối ngày. |
| Hai dịch vụ có cùng số lượt dùng | Sắp xếp tiếp theo doanh thu giảm dần. | Sắp xếp tiếp theo doanh thu giảm dần. | Sắp xếp tiếp theo doanh thu giảm dần. | Đúng theo yêu cầu. |

## 5. Truy vấn cuối cùng

Chọn **Phương án 2** vì tách rõ dữ liệu hóa đơn hợp lệ khỏi bước tổng hợp và dễ kiểm chứng nhất, trong khi vẫn có hiệu năng và kết quả tương đương hai phương án còn lại.

```sql
WITH paid_invoice_items AS (
    SELECT
        ii.service_id,
        ii.quantity,
        ii.unit_price
    FROM invoices AS i
    JOIN invoice_items AS ii ON ii.invoice_id = i.id
    WHERE i.status = 'paid'
      AND i.invoice_date >= :from_date
      AND i.invoice_date < DATE_ADD(:to_date, INTERVAL 1 DAY)
),
service_totals AS (
    SELECT
        service_id,
        SUM(quantity) AS total_quantity,
        SUM(quantity * unit_price) AS total_revenue
    FROM paid_invoice_items
    GROUP BY service_id
)
SELECT
    s.id AS service_id,
    s.code,
    s.name,
    st.total_quantity,
    st.total_revenue
FROM service_totals AS st
JOIN services AS s ON s.id = st.service_id
ORDER BY st.total_quantity DESC, st.total_revenue DESC
LIMIT 10;
```
