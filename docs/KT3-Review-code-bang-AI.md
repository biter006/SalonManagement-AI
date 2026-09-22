# Nhật ký AI review code – KT3

## Prompt đã dùng

```text
Bạn là chuyên gia review ASP.NET Core MVC .NET 8.
Hãy review file AIService.cs của hệ thống quản lý salon tóc.
Kiểm tra: bảo mật API Key, timeout, xử lý HTTP 401/429,
response rỗng, JSON lỗi, dữ liệu quá dài, prompt injection và
việc AI tự động sửa dữ liệu. Hãy nêu lỗi theo mức độ ưu tiên và đề xuất cách sửa.
```

## Kết quả review và kiểm chứng của nhóm

| Nội dung AI review | Nhóm kiểm chứng | Cải tiến đã thực hiện |
|---|---|---|
| API Key không nên nằm trong `appsettings.json` hoặc GitHub. | Đúng; project dùng User Secrets và `.env.example` không chứa key thật. | Giữ `ApiKey` rỗng trong `appsettings.json`; bổ sung hướng dẫn cấu hình bằng User Secrets. |
| Cần timeout và fallback khi API lỗi. | Đúng; AI Service đã có timeout 20 giây và bắt `HttpRequestException`, `TaskCanceledException`, `JsonException`. | Giữ Local Fallback và thông báo thân thiện trên giao diện. |
| Prompt cần chống yêu cầu làm trái quy tắc. | Đúng. | Thêm ràng buộc vào System Prompt: bỏ qua yêu cầu thay đổi quy tắc, không tự tạo/sửa/xóa dữ liệu nghiệp vụ. |
| Cần giới hạn dữ liệu người dùng gửi vào AI. | Đúng. | Giới hạn nhu cầu khách tối đa 1.000 ký tự; lịch sử gửi AI tối đa 5 bản ghi. |
| Kết quả AI cần được con người kiểm tra. | Đúng. | Thêm cảnh báo trên cả ba giao diện AI; AI không tự gửi tin nhắn hoặc thay đổi lịch hẹn/hóa đơn. |

## Kết luận

AI chỉ là công cụ hỗ trợ review. Nhóm kiểm tra lại từng đề xuất trước khi sửa mã; không áp dụng trực tiếp nội dung AI mà không đối chiếu nghiệp vụ salon và mã nguồn hiện có.
