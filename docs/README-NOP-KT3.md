# Gói nộp Bài kiểm tra thường xuyên 3 — SalonManagement AI

Thư mục nộp bài được tổ chức để người chấm có thể xem minh chứng theo thứ tự. Không có API key hoặc User Secrets trong gói này.

## Thứ tự xem khuyến nghị

1. Mở `01-Tai-lieu/KT3-Huong-dan-minh-chung-AI.md` để xem sơ đồ minh chứng 10 tiêu chí.
2. Xem `02-Anh-minh-chung/01-Gemini-503-retry-200.png`: hệ thống gọi Gemini thật, tự thử lại sau lỗi 503 và nhận HTTP 200.
3. Xem `02-Anh-minh-chung/02-Gemini-tu-van-dich-vu.png` và `08-Du-lieu-CSDL-va-AI.png`: AI được tích hợp vào luồng hồ sơ khách hàng và dùng dữ liệu salon.
4. Xem `04-Prompt-vong-1-local-fallback.png`, `05-Prompt-vong-2.png`, `06-Prompt-vong-3.png` cùng tài liệu `KT3-Toi-uu-prompt.md` để đối chiếu ba vòng tối ưu prompt.
5. Xem `07-Test-17-17.png` và `Test-cases-KT3-AI.md` để kiểm tra kết quả kiểm thử.

## Nội dung từng thư mục

- `01-Tai-lieu`: hướng dẫn minh chứng, tối ưu prompt, review code bằng AI, test cases và README dự án.
- `02-Anh-minh-chung`: ảnh chụp demo Gemini, xử lý fallback, ba vòng prompt, test 17/17 và dữ liệu CSDL. Bảy ảnh không chứa thông tin liên hệ cũng được lưu tại `docs/minh-chung-kt3` trên GitHub.
- `03-Ma-nguon-minh-chung`: các tệp mã nguồn then chốt cho AI, prompt và test; chỉ là bản sao để đối chiếu, mã nguồn đầy đủ nằm trong repository GitHub.

## Lưu ý khi trình bày

- Kết quả tư vấn AI chỉ để tham khảo; thợ kiểm tra trực tiếp trước khi chốt dịch vụ.
- Khi demo live, dùng model `gemini-3.1-flash-lite`. Nếu Gemini đang quá tải, hệ thống hiển thị nhãn `Local Fallback` và vẫn trả tư vấn cơ bản an toàn.
- Không đưa API key, thư mục `secrets.json`, `appsettings.Development.json` hoặc dữ liệu nhạy cảm vào bài nộp.
