# Kết quả chọn thiết kế CSDL bằng Self-Consistency — Đề tài 39

## Bối cảnh đã áp dụng

Hệ thống salon quản lý khách hàng, thợ, ca làm việc, dịch vụ và lịch hẹn. Thiết kế CSDL cần xử lý đúng khi tạo, đổi và hủy lịch; ngăn thợ bị đặt trùng thời gian; đồng thời hỗ trợ tra cứu lịch sử và báo cáo.

## Bảng so sánh ba phương án

| Tiêu chí | Phương án A: Chỉ lưu lịch hẹn hiện tại | Phương án B: Chỉ lưu nhật ký sự kiện lịch hẹn | Phương án C: Lưu lịch hẹn hiện tại và lịch sử biến động |
|---|---|---|---|
| Mô tả | Bảng `appointments` lưu khách, thợ, dịch vụ, thời gian bắt đầu/kết thúc và trạng thái. Ca làm việc lưu ở `stylist_work_shifts`; thời gian trống được tính từ các lịch còn hiệu lực. | Mỗi thao tác tạo, đổi, hủy lịch được ghi thành một bản ghi trong `appointment_events`; trạng thái hiện tại phải được dựng lại từ chuỗi sự kiện. | Bảng `appointments` lưu trạng thái hiện tại; bảng `appointment_events` lưu các thao tác tạo, đổi, hủy, hoàn thành cùng giá trị thời gian cũ/mới và người thao tác. |
| Đúng nghiệp vụ | Đáp ứng tạo, đổi, hủy nếu cập nhật cẩn thận, nhưng khó biết vì sao lịch có trạng thái hiện tại. | Phản ánh đầy đủ tiến trình lịch hẹn, nhưng việc dựng trạng thái hiện tại và kiểm tra trùng lịch phức tạp hơn. | Đáp ứng nhanh việc kiểm tra lịch hiện tại, đồng thời giữ được đầy đủ các thay đổi quan trọng. |
| Dễ triển khai demo | Rất dễ; ít bảng và truy vấn đơn giản. | Khó; cần quy tắc dựng trạng thái và xử lý sự kiện theo thứ tự. | Dễ ở mức phù hợp đồ án; có thêm một bảng lịch sử nhưng luồng CRUD vẫn trực tiếp trên `appointments`. |
| Dễ audit | Thấp; dữ liệu cũ bị mất khi đổi thời gian hoặc hủy lịch. | Cao; mọi thay đổi đều là sự kiện bất biến. | Cao; có thể xem lịch sử thay đổi mà không làm chậm truy vấn lịch hiện tại. |
| Hỗ trợ báo cáo | Tốt cho báo cáo lịch đang có, hạn chế khi cần phân tích tỷ lệ đổi/hủy. | Tốt cho phân tích sự kiện, nhưng truy vấn báo cáo trạng thái hiện tại dài và dễ sai. | Tốt cho cả báo cáo lịch/hóa đơn hiện tại và thống kê số lần đổi/hủy theo thời gian. |
| Rủi ro sai lệch | Trung bình; dễ mất lịch sử, và phải kiểm soát cập nhật trạng thái trong ứng dụng. | Trung bình đến cao; lỗi thứ tự hoặc thiếu sự kiện làm dựng sai trạng thái. | Thấp đến trung bình; có nguy cơ dữ liệu hiện tại và nhật ký không khớp nếu không dùng giao dịch, nhưng có thể kiểm soát rõ ràng. |

## Kết luận chọn phương án

Chọn **Phương án C: lưu lịch hẹn hiện tại kết hợp lịch sử biến động**.

### Thiết kế đề xuất

- `customers(id, full_name, phone, ...)`: thông tin khách hàng.
- `stylists(id, full_name, specialty, status, ...)`: thông tin thợ.
- `stylist_work_shifts(id, stylist_id, work_date, start_time, end_time, status)`: ca làm việc của thợ.
- `services(id, code, name, price, duration_minutes, status)`: danh mục dịch vụ.
- `appointments(id, customer_id, stylist_id, start_at, end_at, status, note, created_at, updated_at)`: lịch hẹn hiện tại; `status` gồm `booked`, `confirmed`, `completed`, `cancelled`.
- `appointment_services(id, appointment_id, service_id, quantity, unit_price)`: các dịch vụ gắn với lịch hẹn, cho phép một lịch có nhiều dịch vụ và lưu giá tại thời điểm đặt.
- `appointment_events(id, appointment_id, event_type, old_start_at, old_end_at, new_start_at, new_end_at, old_status, new_status, reason, actor_id, created_at)`: nhật ký tạo, đổi, hủy và hoàn thành lịch hẹn.

Khi đặt hoặc đổi lịch, hệ thống phải thực hiện trong một giao dịch: kiểm tra thời gian nằm trong ca làm việc, kiểm tra không giao với lịch `booked` hoặc `confirmed` của cùng thợ, cập nhật `appointments`, rồi thêm `appointment_events`. Không tạo cột `available_slots` vì đây là dữ liệu dẫn xuất, dễ sai lệch khi lịch thay đổi.

## Lý do chọn

Phương án C được nhiều tiêu chí ủng hộ nhất: truy vấn lịch của thợ và kiểm tra trùng lịch nhanh từ bảng `appointments`, trong khi bảng `appointment_events` giữ bằng chứng cho mọi lần đổi/hủy. Cách này phù hợp mức độ cơ bản của đề tài, thuận lợi khi demo CRUD và vẫn đáp ứng nhu cầu audit, chăm sóc khách cũ và báo cáo tỷ lệ hủy/đổi lịch. Rủi ro không nhất quán được giảm bằng khóa ngoại, ràng buộc trạng thái và một giao dịch cho mỗi thay đổi lịch hẹn.

## Các test case bắt buộc cho phương án được chọn

| ID | Tình huống kiểm thử | Kết quả mong đợi |
|---|---|---|
| TC-CSDL-01 | Tạo lịch hợp lệ cho thợ trong ca làm việc, không giao với lịch khác. | Thêm một dòng `appointments` có trạng thái `booked` và một sự kiện `created` trong `appointment_events`; hai bản ghi cùng `appointment_id`. |
| TC-CSDL-02 | Tạo lịch giao thời gian với một lịch `booked` hoặc `confirmed` của cùng thợ. | Giao dịch bị từ chối; không có lịch mới và không có sự kiện mới được ghi. |
| TC-CSDL-03 | Đổi lịch sang khung giờ trống trong ca làm việc. | `appointments.start_at` và `end_at` được cập nhật; `appointment_events` lưu thời gian cũ, thời gian mới và `event_type = 'rescheduled'`. |
| TC-CSDL-04 | Hủy một lịch chưa hoàn thành. | Trạng thái lịch là `cancelled`; có sự kiện `cancelled` kèm lý do; khung giờ đó không còn chặn lịch mới của thợ. |
| TC-CSDL-05 | Hai yêu cầu đặt lịch đồng thời cho cùng thợ và cùng khung giờ. | Chỉ một giao dịch thành công; giao dịch còn lại bị từ chối do kiểm tra trùng lịch trong giao dịch. |
| TC-CSDL-06 | Thống kê số lịch bị hủy trong một tháng. | Kết quả đếm từ `appointments.status = 'cancelled'` khớp với số sự kiện `cancelled` trong cùng khoảng thời gian. |
