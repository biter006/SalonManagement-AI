# Test case — Đề tài 39: Hệ thống quản lý đặt lịch salon tóc có tích hợp AI

Chức năng: Đặt lịch hẹn

Test case ID: TC-LH-01

Mục tiêu: Tạo lịch hẹn hợp lệ trong ca làm việc của thợ

Tiền điều kiện: Có khách hàng Khách A; thợ Minh làm việc ngày 15/08/2026 từ 09:00 đến 17:00; dịch vụ Cắt tóc nữ có thời lượng 60 phút và đang được cung cấp; thợ Minh chưa có lịch từ 10:00 đến 11:00.

Bước thực hiện:

1. Đăng nhập bằng tài khoản lễ tân.
2. Chọn tạo lịch hẹn mới cho Khách A.
3. Chọn thợ Minh và dịch vụ Cắt tóc nữ.
4. Chọn thời gian bắt đầu 10:00 ngày 15/08/2026.
5. Lưu lịch hẹn.

Kết quả mong đợi:

- Lịch hẹn được tạo thành công với trạng thái đã đặt.
- Lịch có thời gian thực hiện từ 10:00 đến 11:00 ngày 15/08/2026.
- Lịch hẹn hiển thị khi tra cứu lịch của thợ Minh trong ngày 15/08/2026.

---

Chức năng: Đặt lịch hẹn

Test case ID: TC-LH-02

Mục tiêu: Không cho đặt lịch trùng thời gian của thợ

Tiền điều kiện: Thợ Minh làm việc ngày 15/08/2026 từ 09:00 đến 17:00 và đã có lịch hẹn từ 10:00 đến 11:00; dịch vụ Gội đầu có thời lượng 30 phút.

Bước thực hiện:

1. Đăng nhập bằng tài khoản lễ tân.
2. Tạo lịch hẹn mới, chọn thợ Minh và dịch vụ Gội đầu.
3. Chọn thời gian bắt đầu 10:30 ngày 15/08/2026.
4. Lưu lịch hẹn.

Kết quả mong đợi:

- Hệ thống không tạo lịch hẹn mới.
- Hiển thị thông báo thợ Minh đã có lịch trùng thời gian.
- Lịch hẹn cũ từ 10:00 đến 11:00 không bị thay đổi.

---

Chức năng: Hủy lịch hẹn

Test case ID: TC-LH-03

Mục tiêu: Hủy lịch hẹn và giải phóng khung giờ của thợ

Tiền điều kiện: Có lịch hẹn đã đặt cho Khách A với thợ Minh, dịch vụ Cắt tóc nữ, từ 10:00 đến 11:00 ngày 15/08/2026; lịch chưa được thanh toán.

Bước thực hiện:

1. Đăng nhập bằng tài khoản lễ tân.
2. Mở chi tiết lịch hẹn của Khách A.
3. Chọn Hủy lịch và xác nhận lý do khách thay đổi kế hoạch.
4. Tạo lịch hẹn mới cho khách khác với thợ Minh từ 10:00 đến 11:00 cùng ngày.

Kết quả mong đợi:

- Lịch hẹn của Khách A có trạng thái đã hủy.
- Khung giờ từ 10:00 đến 11:00 của thợ Minh được giải phóng sau khi hủy.
- Lịch hẹn mới trong khung giờ này được tạo thành công.

---

Chức năng: Lập hóa đơn và thanh toán

Test case ID: TC-HD-01

Mục tiêu: Lập hóa đơn hợp lệ từ dịch vụ đã hoàn thành

Tiền điều kiện: Khách A có lịch hẹn đã hoàn thành gồm dịch vụ Cắt tóc nữ giá 250000 đồng và Gội đầu giá 50000 đồng; chưa có hóa đơn.

Bước thực hiện:

1. Đăng nhập bằng tài khoản lễ tân.
2. Mở lịch hẹn đã hoàn thành của Khách A.
3. Chọn lập hóa đơn với hai dịch vụ có sẵn.
4. Áp dụng giảm giá 0 đồng.
5. Chọn phương thức thanh toán chuyển khoản.
6. Xác nhận thanh toán.

Kết quả mong đợi:

- Hóa đơn được tạo thành công và gắn với lịch hẹn của Khách A.
- Tổng tiền hóa đơn là 300000 đồng.
- Hóa đơn có trạng thái đã thanh toán và phương thức thanh toán chuyển khoản.

---

Chức năng: AI gợi ý dịch vụ

Test case ID: TC-AI-01

Mục tiêu: AI chỉ gợi ý dịch vụ còn trong danh mục phù hợp nhu cầu khách

Tiền điều kiện: Danh mục đang cung cấp gồm Cắt tóc nữ, Uốn lạnh và Phục hồi tóc; lịch sử của Khách A ghi nhận đã nhuộm tóc và tóc khô; nhu cầu khách là muốn thay đổi kiểu tóc, ưu tiên giảm hư tổn.

Bước thực hiện:

1. Đăng nhập bằng tài khoản lễ tân hoặc thợ.
2. Mở chức năng AI gợi ý dịch vụ cho Khách A.
3. Nhập nhu cầu và cung cấp lịch sử khách cùng danh mục dịch vụ trên.
4. Gửi yêu cầu gợi ý.

Kết quả mong đợi:

- AI trả về một hoặc nhiều dịch vụ thuộc danh mục Cắt tóc nữ, Uốn lạnh, Phục hồi tóc.
- Gợi ý có giải thích ngắn liên hệ với nhu cầu giảm hư tổn hoặc lịch sử tóc khô.
- AI không nêu dịch vụ không có trong danh mục hoặc cam kết kết quả ngoài dữ liệu.

---

Chức năng: AI sinh tin nhắn nhắc lịch

Test case ID: TC-AI-02

Mục tiêu: AI yêu cầu bổ sung dữ liệu khi thiếu thông tin lịch hẹn

Tiền điều kiện: Người dùng mở chức năng AI sinh tin nhắn nhưng chỉ có tên khách và tên dịch vụ, không có ngày giờ hẹn.

Bước thực hiện:

1. Đăng nhập bằng tài khoản lễ tân.
2. Chọn sinh tin nhắn nhắc lịch cho khách.
3. Cung cấp tên khách là Khách B và dịch vụ là Gội đầu, không cung cấp thời gian hẹn.
4. Gửi yêu cầu tạo tin nhắn.

Kết quả mong đợi:

- AI thông báo thiếu thời gian lịch hẹn hoặc yêu cầu người dùng bổ sung thông tin này.
- AI không tự đặt ngày hoặc giờ hẹn.
- Không có thao tác gửi tin nhắn tự động được thực hiện.

---

Chức năng: AI tóm tắt lịch sử làm tóc

Test case ID: TC-AI-03

Mục tiêu: AI xử lý khách chưa có lịch sử dịch vụ

Tiền điều kiện: Khách C đã có hồ sơ trong hệ thống nhưng chưa có lịch hẹn hoặc dịch vụ hoàn thành nào.

Bước thực hiện:

1. Đăng nhập bằng tài khoản thợ.
2. Mở hồ sơ Khách C.
3. Chọn AI tóm tắt lịch sử làm tóc.
4. Gửi yêu cầu tóm tắt.

Kết quả mong đợi:

- AI trả về thông báo Khách C chưa có lịch sử dịch vụ để tóm tắt.
- Nội dung không suy diễn kiểu tóc, sở thích hoặc dịch vụ chưa từng có.
- Thợ vẫn có thể nhập ghi chú phục vụ mới sau khi xem thông báo.

---

Chức năng: Xuất báo cáo doanh thu

Test case ID: TC-BC-01

Mục tiêu: Xuất báo cáo doanh thu theo tháng ra tệp CSV

Tiền điều kiện: Tháng 07/2026 có ba hóa đơn đã thanh toán với tổng doanh thu 1200000 đồng.

Bước thực hiện:

1. Đăng nhập bằng tài khoản quản lý.
2. Mở báo cáo doanh thu.
3. Chọn khoảng thời gian từ 01/07/2026 đến 31/07/2026.
4. Chọn xuất báo cáo ra CSV.
5. Mở tệp CSV vừa xuất.

Kết quả mong đợi:

- Hệ thống tạo tệp CSV để tải xuống thành công.
- Tệp có cột tối thiểu gồm ngày, mã hóa đơn và doanh thu.
- Tổng doanh thu trong tệp CSV là 1200000 đồng, khớp với dữ liệu báo cáo trên hệ thống.
