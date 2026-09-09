# Nhật ký sử dụng AI khi lập trình

## P01 – Kiểm tra trùng lịch hẹn

**Prompt:** “Với lịch hẹn của cùng một thợ, hãy nêu điều kiện để phát hiện hai khoảng thời gian bị giao nhau và các trường hợp biên cần kiểm tra.”

**Phản hồi AI tóm tắt:** Hai khoảng giao nhau khi thời điểm bắt đầu của lịch mới nhỏ hơn thời điểm kết thúc của lịch cũ, đồng thời thời điểm kết thúc của lịch mới lớn hơn thời điểm bắt đầu của lịch cũ.

**Nhóm kiểm chứng và chỉnh sửa:** Áp dụng công thức tại `AppointmentService`; chỉ xét lịch `Pending` và `Confirmed`, đồng thời bổ sung kiểm tra thời điểm quá khứ, trạng thái thợ và ca làm. Test các tình huống hợp lệ/trùng/ngoài ca được đặt trong `SalonManagement.Tests`.

## P02 – Cấu trúc CRUD và validation

**Prompt:** “Hãy gợi ý cấu trúc CRUD ASP.NET Core MVC cho khách hàng và dịch vụ salon, gồm Model, Controller, ViewModel và các kiểm tra dữ liệu đầu vào.”

**Phản hồi AI tóm tắt:** Tách Model và ViewModel, dùng Data Annotation, kiểm tra `ModelState` và trả lại form có thông báo lỗi.

**Nhóm kiểm chứng và chỉnh sửa:** Giữ cách tách lớp theo MVC nhưng thêm `[Authorize]`, giới hạn quyền theo vai trò, kiểm tra dữ liệu liên quan trước khi xóa và chuẩn hóa các nhãn/thông báo bằng tiếng Việt.

## P03 – Sửa lỗi đăng nhập HTTP 400

**Prompt:** “Form đăng nhập ASP.NET Core trả HTTP 400 sau khi gửi. Cần kiểm tra anti-forgery token, tag helper và middleware theo thứ tự nào?”

**Phản hồi AI tóm tắt:** Kiểm tra form có token anti-forgery hay chưa, các tag helper có hoạt động hay không và `UseAuthentication` phải chạy trước `UseAuthorization`.

**Nhóm kiểm chứng và chỉnh sửa:** Kiểm tra form đăng nhập, bổ sung tag helper và cấu hình middleware theo đúng thứ tự. Sau đó chạy lại luồng đăng nhập với tài khoản demo.

## P04 – Độ ổn định của AIService

**Prompt:** “API AI timeout hoặc trả JSON không đúng định dạng thì hệ thống salon nên xử lý thế nào để không gián đoạn thao tác của lễ tân?”

**Phản hồi AI tóm tắt:** Đặt timeout, bắt lỗi HTTP/JSON, ghi log và dùng fallback dựa trên dữ liệu cục bộ.

**Nhóm kiểm chứng và chỉnh sửa:** Áp dụng timeout 20 giây, bắt các ngoại lệ phù hợp và chuẩn bị fallback cho gợi ý dịch vụ, tin nhắn và tóm tắt lịch sử. Không dùng phản hồi AI để tự động tạo lịch hẹn hoặc hóa đơn.
