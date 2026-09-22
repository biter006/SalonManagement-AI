# Test case KT3 – Chức năng AI

| Mã | Trường hợp | Dữ liệu/thiết lập | Kết quả mong đợi | Kết quả thực tế |
|---|---|---|---|---|
| AI-01 | Gợi ý dịch vụ bằng AI Provider thật | API Key, Provider và Model hợp lệ; khách có lịch sử. | Hiển thị tối đa 3 gợi ý phù hợp; chỉ có dịch vụ salon đang hoạt động. | Chờ kiểm thử |
| AI-02 | Local Fallback | `AI:Provider=LocalFallback`. | Không gọi mạng; có kết quả demo ổn định, không crash. | Chờ kiểm thử |
| AI-03 | Không cấu hình API Key | Provider OpenAI, API Key rỗng. | Chuyển sang fallback hoặc thông báo an toàn; không lộ key. | Chờ kiểm thử |
| AI-04 | API Key sai / HTTP 401 | Cấu hình key kiểm thử không hợp lệ. | Không crash; log lỗi, giao diện thông báo thân thiện/fallback. | Chờ kiểm thử |
| AI-05 | Rate limit / HTTP 429 | Dùng endpoint mô phỏng hoặc gọi quá giới hạn. | Không crash; không lưu kết quả lỗi vào CSDL. | Chờ kiểm thử |
| AI-06 | Timeout | Endpoint không phản hồi. | Sau tối đa 20 giây, hệ thống fallback/thông báo lỗi. | Chờ kiểm thử |
| AI-07 | Response rỗng/sai JSON | Mock service hoặc endpoint kiểm thử. | Không lưu kết quả rỗng; giao diện báo lỗi. | Đã có unit test |
| AI-08 | Nhu cầu quá dài | Nhập trên 1.000 ký tự. | Form từ chối dữ liệu và hiển thị lỗi tiếng Việt. | Chờ kiểm thử |
| AI-09 | Khách chưa có lịch sử | Chọn khách mới. | AI ghi rõ chưa đủ lịch sử, vẫn gợi ý theo nhu cầu. | Chờ kiểm thử |
| AI-10 | Tin nhắn nhắc lịch thiếu lịch hẹn | Loại `Reminder`, không chọn lịch hẹn. | Không tạo tin nhắn; báo phải chọn lịch hẹn. | Chờ kiểm thử |

## Unit test hiện có

- AI trả kết quả hợp lệ và lưu gợi ý.
- AI trả kết quả rỗng thì không lưu.
- AI ném exception thì không làm thay đổi CSDL.

Chạy: `dotnet test SalonManagement.Tests/SalonManagement.Tests.csproj`.
