# Nhật ký tối ưu prompt – KT3

## Mục tiêu

Tối ưu chức năng AI gợi ý dịch vụ để kết quả bám danh mục salon, ngắn gọn và hữu ích cho nhân viên tư vấn.

| Vòng | Prompt và dữ liệu thử | Kết quả quan sát | Vấn đề | Cải tiến được áp dụng |
|---|---|---|---|---|
| 1 | “Hãy gợi ý dịch vụ cho khách muốn thay đổi kiểu tóc.” | Kết quả chung chung, có thể gợi ý dịch vụ không nằm trong salon. | Không có ràng buộc danh mục và không có lịch sử khách. | Thêm danh sách dịch vụ đang hoạt động và lịch sử dịch vụ. |
| 2 | Thêm tên khách, nhu cầu, lịch sử và danh sách dịch vụ. | Kết quả phù hợp hơn nhưng dài, khó đọc nhanh; có thể nhiều hơn 3 lựa chọn. | Chưa quy định định dạng và số lượng gợi ý. | Bắt buộc tối đa 3 dịch vụ, chia rõ dịch vụ – lý do – lưu ý. |
| 3 | Prompt hiện hành trong `Prompts/SalonPrompts.json`: chỉ dịch vụ có sẵn, tối đa 3 gợi ý, 3 mục đầu ra, không tự tạo giá/chính sách. | Kết quả ngắn gọn, đúng danh mục và có lưu ý tư vấn. | Phù hợp dữ liệu demo. | Chọn làm prompt chính thức. |

## Kết luận

Prompt vòng 3 được áp dụng cho hệ thống. Nhóm cần chụp ảnh kết quả của cả ba vòng với cùng một khách hàng để làm minh chứng khi nộp bài.

## Dữ liệu demo nên dùng

- Chọn khách hàng đã có ít nhất một lịch sử dịch vụ.
- Nhu cầu: “Muốn kiểu tóc gọn, dễ chăm sóc, phù hợp đi học.”
- So sánh kết quả trước và sau khi thêm ràng buộc danh sách dịch vụ, số lượng và định dạng đầu ra.
