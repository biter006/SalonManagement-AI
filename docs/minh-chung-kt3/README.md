# Ảnh minh chứng KT3

| Tệp | Minh chứng | Tiêu chí KT3 |
| --- | --- | --- |
| `01-Gemini-tu-van-dich-vu.png` | Kết quả tư vấn dịch vụ AI hiển thị trong ứng dụng. | 1, 5, 6, 10 |
| `02-Local-Fallback.png` | Giao diện gắn nhãn `Local Fallback` khi provider ngoài không khả dụng. | 2, 7 |
| `03-Gemini-503-retry-200.png` | Log gọi Gemini thật: HTTP 503, retry và sau đó HTTP 200. | 2, 7 |
| `04-Prompt-vong-1-local-fallback.png` | Ảnh thử fallback ban đầu, chỉ dùng cho xử lý lỗi; không dùng để so sánh prompt. | 7 |
| `05-Prompt-vong-2.png`, `06-Prompt-vong-3.png` | Ảnh thăm dò prompt cũ; không cùng điều kiện thử nghiệm. | Tham khảo nội bộ |
| `07-Test-17-17.png` | Kết quả `dotnet test`: 17 thành công, 0 thất bại. | 8 |

Ảnh hồ sơ khách hàng chứa số điện thoại/email chỉ nằm trong gói nộp cục bộ để tránh công khai thông tin liên hệ. Trước khi đưa ảnh đó lên GitHub, cần che dữ liệu liên hệ.

Để hoàn thành tiêu chí 4, thay bằng ba ảnh Gemini thật `04-Prompt-vong-1-Gemini.png`, `05-Prompt-vong-2-Gemini.png`, `06-Prompt-vong-3-Gemini.png` theo đúng điều kiện trong [KT3-Toi-uu-prompt.md](../KT3-Toi-uu-prompt.md).
