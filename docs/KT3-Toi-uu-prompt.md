# Nhật ký tối ưu prompt - KT3

## Mục tiêu

So sánh ba phiên bản prompt gợi ý dịch vụ trong **cùng điều kiện chạy**, sau đó giữ V3 làm prompt chính thức. Các phiên bản V1, V2 và V3 được cấu hình ngoài code tại `SalonManagement/Prompts/SalonPrompts.json`; giao diện `/AI/Recommendation` cho phép chọn đúng phiên bản để thử nghiệm.

## Điều kiện thử nghiệm cố định

| Thành phần | Giá trị cần giữ nguyên cho cả ba vòng |
| --- | --- |
| Trang chạy | `/AI/Recommendation` - không dùng Chat vì Chat có lịch sử hội thoại. |
| Provider / model | Gemini / `gemini-3.1-flash-lite`. |
| Temperature | `0.3` - cấu hình hiện tại trong `AIService`. |
| Khách hàng | Nguyễn Thanh Hiếu, đã có lịch sử `Cắt tóc nam` và `Nhuộm tóc`. Không chụp email hoặc số điện thoại. |
| Tình trạng tóc | `Khô, xơ hoặc hư tổn`. |
| Mong muốn | `Phục hồi tóc`. |
| Ngân sách tối đa | `700000` VNĐ. |
| Chăm sóc tại nhà | `Có thể chăm sóc cơ bản`. |
| Nhu cầu nhập | `Tóc khô, xơ sau nhuộm; muốn phục hồi nhưng không muốn chăm sóc quá phức tạp.` |
| Danh mục và lịch sử | Không thêm, sửa hoặc xóa dữ liệu salon giữa ba lần chạy. |

Chỉ dùng ảnh có dòng `Nguồn kết quả: Gemini - model gemini-3.1-flash-lite` (hoặc model Gemini đang cấu hình) và **không** có nhãn `Local Fallback`. Nếu API trả 503, chạy lại sau ít phút; ảnh fallback chỉ dùng cho tiêu chí xử lý lỗi AI.

## Ba phiên bản prompt

| Vòng | System prompt | User prompt | Mục đích và kết quả mong đợi |
| --- | --- | --- | --- |
| V1 - cơ bản | `Bạn là trợ lý salon tóc. Trả lời bằng tiếng Việt, lịch sự.` | Chỉ có tên khách và nhu cầu: `Khách hàng: {{customerName}} ... Hãy gợi ý dịch vụ phù hợp.` | Cho thấy hạn chế: thiếu danh mục, lịch sử, ngân sách và định dạng đầu ra. |
| V2 - thêm dữ liệu salon | Trợ lý tiếng Việt ngắn gọn; chỉ tư vấn, không tự tạo/sửa dữ liệu nghiệp vụ. | Thêm hồ sơ tư vấn, danh mục dịch vụ đang hoạt động và lịch sử khách. | Kết quả bám danh mục/lịch sử hơn, nhưng có thể còn dài hoặc thiếu cấu trúc. |
| V3 - chính thức | Prompt an toàn hiện hành: chỉ dùng dữ liệu hệ thống, không tạo dịch vụ/giá/thời lượng/chính sách, chống yêu cầu đổi quy tắc. | V2 + tối đa 3 dịch vụ, lý do gắn tình trạng tóc/mong muốn/ngân sách và lưu ý thợ kiểm tra trực tiếp. | Kết quả ngắn, có cấu trúc, ưu tiên dịch vụ phục hồi trong ngân sách - ví dụ `Phục hồi keratin 650.000đ` nếu dữ liệu salon không đổi. |

Nội dung đầy đủ của từng prompt là nguồn cấu hình trong `SalonManagement/Prompts/SalonPrompts.json`; code chỉ chọn template theo phiên bản, không nhúng prompt vào Controller hoặc View.

## Tiêu chí so sánh

| Tiêu chí | Cách đánh giá |
| --- | --- |
| Bám dữ liệu salon | Không nêu dịch vụ ngoài CSDL; không tự tạo giá/thời lượng. |
| Bám hồ sơ khách | Nhắc đúng tóc khô sau nhuộm, nhu cầu phục hồi và ngân sách 700.000đ. |
| Phù hợp ngân sách | Không ưu tiên nhuộm/uốn vượt ngân sách khi mục tiêu là phục hồi. |
| Rõ ràng | Tối đa 3 lựa chọn, lý do ngắn và lưu ý cho thợ. |
| An toàn | Không cam kết kết quả và nhắc kiểm tra tóc trực tiếp. |

## Cách tạo ảnh minh chứng

1. Khởi động lại ứng dụng sau khi cập nhật mã nguồn, đăng nhập rồi mở **Trợ lý AI → AI tư vấn tóc thông minh**.
2. Nhập đúng dữ liệu trong bảng điều kiện cố định, chọn **V1 - Nhu cầu cơ bản**, bấm **Phân tích & gợi ý**. Chụp toàn bộ form, nhãn Gemini/model và kết quả; lưu là `04-Prompt-vong-1-Gemini.png`.
3. Giữ nguyên toàn bộ dữ liệu, chỉ đổi sang **V2 - Bổ sung dữ liệu salon**; chụp `05-Prompt-vong-2-Gemini.png`.
4. Giữ nguyên toàn bộ dữ liệu, chỉ đổi sang **V3 - Prompt chính thức**; chụp `06-Prompt-vong-3-Gemini.png`.
5. Đặt ba ảnh vào `docs/minh-chung-kt3/`, cập nhật bảng dưới đây bằng kết quả thực tế và commit lên GitHub.

## Bảng kết quả thực tế

| Vòng | Tên ảnh cần có | Kết quả / nhận xét sau khi chạy | Trạng thái |
| --- | --- | --- | --- |
| V1 | `04-Prompt-vong-1-Gemini.png` | Chụp lại với Gemini, cùng dữ liệu cố định. | Chờ chụp lại |
| V2 | `05-Prompt-vong-2-Gemini.png` | Chụp lại với Gemini, cùng dữ liệu cố định. | Chờ chụp lại |
| V3 | `06-Prompt-vong-3-Gemini.png` | Chụp lại với Gemini, cùng dữ liệu cố định. | Chờ chụp lại |

Các ảnh `04-Prompt-vong-1-local-fallback.png`, `05-Prompt-vong-2.png` và `06-Prompt-vong-3.png` trước đây chỉ là ảnh thử nghiệm ban đầu; chúng không được dùng làm minh chứng cuối cùng cho tiêu chí 4 vì không chạy trong cùng điều kiện.
