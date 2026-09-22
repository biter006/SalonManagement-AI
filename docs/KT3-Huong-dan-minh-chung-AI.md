# Hướng dẫn minh chứng KT3

## Cấu hình API Key an toàn

Không đưa API Key vào GitHub, `.env.example` hoặc `appsettings.json`. Tại thư mục dự án, chạy:

```powershell
dotnet user-secrets set "AI:Provider" "OpenAI" --project SalonManagement
dotnet user-secrets set "AI:ApiKey" "API_KEY_CUA_BAN" --project SalonManagement
dotnet user-secrets set "AI:Model" "TEN_MODEL_BAN_CO_QUYEN_DUNG" --project SalonManagement
```

### Gemini API Free Tier

Project hỗ trợ Gemini cùng với OpenAI và Local Fallback. Tạo Gemini API Key tại Google AI Studio, sau đó cấu hình:

```powershell
dotnet user-secrets set "AI:Provider" "Gemini" --project SalonManagement
dotnet user-secrets set "AI:ApiKey" "GEMINI_API_KEY_CUA_BAN" --project SalonManagement
dotnet user-secrets set "AI:Model" "gemini-3.1-flash-lite" --project SalonManagement
```

Không đưa Gemini API Key vào GitHub. Free Tier có giới hạn lượt gọi; dùng Local Fallback khi hết quota hoặc khi demo không có mạng.

Sau khi chụp ảnh kết quả AI thật, có thể chuyển lại demo offline:

```powershell
dotnet user-secrets set "AI:Provider" "LocalFallback" --project SalonManagement
```

## Ảnh cần chụp

1. Cấu hình secret (che toàn bộ API Key).
2. Trang khách hàng có nút AI gợi ý, AI tóm tắt, AI tin nhắn.
3. Kết quả gợi ý dịch vụ bằng AI Provider thật.
4. Kết quả sinh tin nhắn và phần cảnh báo nhân viên kiểm tra trước khi gửi.
5. Kết quả tóm tắt lịch sử khách hàng.
6. Local Fallback hoặc lỗi AI được xử lý an toàn.
7. Kết quả chạy `dotnet test`.
8. Bảng 3 vòng tối ưu prompt và nhật ký AI review code.

## Luồng demo AI

1. Đăng nhập Admin hoặc Lễ tân.
2. Mở chi tiết một khách đã có lịch sử dịch vụ.
3. Chọn **AI gợi ý**, nhập nhu cầu, tạo gợi ý.
4. Quay lại khách, chọn **AI tóm tắt** để thợ xem nhanh lịch sử.
5. Chọn **AI tin nhắn**, chọn lịch hẹn và tạo nội dung.
6. Nhấn mạnh nhân viên có quyền kiểm tra/chỉnh sửa; AI không tự gửi tin nhắn và không tự thay đổi dữ liệu nghiệp vụ.
