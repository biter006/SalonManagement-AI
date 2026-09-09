# User story — Đề tài 39: Hệ thống quản lý đặt lịch salon tóc có tích hợp AI

## 1. Đăng nhập và phân quyền

**Chức năng:** Đăng nhập và phân quyền quản lý, lễ tân, thợ

**User story:**  
Là một quản lý salon, tôi muốn tạo tài khoản và gán vai trò cho từng nhân viên để mỗi người chỉ truy cập các chức năng phù hợp với công việc của mình.

**Tiêu chí chấp nhận:**

- Có thể tạo tài khoản cho quản lý, lễ tân và thợ với tên đăng nhập riêng.
- Người dùng phải đăng nhập hợp lệ trước khi truy cập hệ thống.
- Lễ tân không được thay đổi lịch làm việc của thợ nếu không có quyền được cấp.

## 2. Quản lý khách hàng và lịch sử dịch vụ

**Chức năng:** Quản lý khách hàng và lịch sử dịch vụ

**User story:**  
Là một lễ tân, tôi muốn thêm, cập nhật và xem lịch sử dịch vụ của khách để đặt lịch và chăm sóc khách chính xác hơn.

**Tiêu chí chấp nhận:**

- Có thể lưu họ tên, số điện thoại và ghi chú phục vụ của khách hàng.
- Không cho lưu khách hàng nếu thiếu họ tên hoặc số điện thoại.
- Có thể xem các dịch vụ, ngày thực hiện và thợ đã phục vụ trong lịch sử của khách.

## 3. Quản lý thợ và lịch làm việc

**Chức năng:** Quản lý thợ và lịch làm việc

**User story:**  
Là một quản lý salon, tôi muốn quản lý thông tin thợ và ca làm việc để hệ thống chỉ nhận lịch hẹn vào thời gian thợ có mặt.

**Tiêu chí chấp nhận:**

- Có thể thêm tên, chuyên môn và trạng thái làm việc của từng thợ.
- Có thể thiết lập ca làm việc theo ngày cho từng thợ.
- Hệ thống không cho đặt lịch cho thợ ngoài ca làm việc hoặc trong thời gian đã có lịch hẹn.

## 4. Quản lý dịch vụ

**Chức năng:** Quản lý dịch vụ, giá và thời lượng

**User story:**  
Là một quản lý salon, tôi muốn quản lý danh mục dịch vụ để lễ tân báo giá và sắp lịch đúng thời lượng cho khách.

**Tiêu chí chấp nhận:**

- Có thể nhập tên dịch vụ, giá, thời lượng và trạng thái đang cung cấp.
- Không cho lưu dịch vụ thiếu tên hoặc có giá, thời lượng nhỏ hơn hoặc bằng 0.
- Dịch vụ ngừng cung cấp không được chọn khi đặt lịch mới.

## 5. Đặt lịch, đổi lịch và hủy lịch

**Chức năng:** Quản lý lịch hẹn

**User story:**  
Là một lễ tân, tôi muốn đặt lịch, đổi lịch và hủy lịch hẹn cho khách để điều phối dịch vụ thuận tiện và tránh trùng lịch thợ.

**Tiêu chí chấp nhận:**

- Khi đặt lịch, có thể chọn khách, dịch vụ, thợ, thời gian và ghi chú.
- Hệ thống chỉ cho lưu lịch nếu thời lượng dịch vụ nằm trọn trong ca làm việc và không trùng lịch của thợ.
- Khi hủy lịch, trạng thái lịch được cập nhật và khung giờ của thợ được mở lại để có thể đặt lịch khác.

## 6. Lập hóa đơn và thanh toán

**Chức năng:** Lập hóa đơn và thanh toán

**User story:**  
Là một lễ tân, tôi muốn lập hóa đơn từ các dịch vụ khách đã sử dụng và ghi nhận thanh toán để doanh thu được tính chính xác.

**Tiêu chí chấp nhận:**

- Có thể chọn một hoặc nhiều dịch vụ đã thực hiện để lập hóa đơn.
- Tổng tiền bằng tổng giá các dịch vụ sau khi áp dụng giảm giá hợp lệ.
- Có thể ghi nhận ít nhất phương thức thanh toán tiền mặt hoặc chuyển khoản và cập nhật trạng thái hóa đơn đã thanh toán.

## 7. Tra cứu lịch hẹn

**Chức năng:** Tra cứu lịch hẹn

**User story:**  
Là một lễ tân, tôi muốn lọc lịch hẹn theo ngày, thợ hoặc khách để nhanh chóng xác nhận thông tin phục vụ.

**Tiêu chí chấp nhận:**

- Có thể lọc lịch hẹn theo từng ngày hoặc khoảng ngày.
- Có thể kết hợp lọc theo thợ và khách hàng.
- Kết quả hiển thị thời gian, dịch vụ, thợ và trạng thái của lịch hẹn.

## 8. Thống kê hoạt động salon

**Chức năng:** Thống kê doanh thu, dịch vụ phổ biến và khách quay lại

**User story:**  
Là một quản lý salon, tôi muốn xem các thống kê hoạt động để đánh giá doanh thu và điều chỉnh dịch vụ phù hợp.

**Tiêu chí chấp nhận:**

- Có thể xem doanh thu theo ngày, tháng hoặc khoảng thời gian.
- Báo cáo nêu được số lượt sử dụng của từng dịch vụ để xác định dịch vụ phổ biến.
- Báo cáo xác định khách quay lại dựa trên khách có từ hai lịch hẹn hoàn thành trở lên.

## 9. AI gợi ý dịch vụ

**Chức năng:** AI gợi ý dịch vụ theo nhu cầu và lịch sử khách hàng

**User story:**  
Là một lễ tân hoặc thợ làm tóc, tôi muốn AI gợi ý dịch vụ phù hợp với nhu cầu và lịch sử của khách để tư vấn nhanh hơn.

**Tiêu chí chấp nhận:**

- AI chỉ gợi ý dịch vụ đang có trong danh mục được cung cấp.
- AI nêu ngắn gọn lý do gợi ý dựa trên nhu cầu hoặc lịch sử đã có.
- Nếu dữ liệu lịch sử hoặc danh mục dịch vụ không đủ, AI phải nói rõ giới hạn thay vì tự bịa thông tin.

## 10. AI sinh tin nhắn chăm sóc

**Chức năng:** AI sinh tin nhắn nhắc lịch hoặc chăm sóc sau dịch vụ

**User story:**  
Là một lễ tân, tôi muốn AI soạn tin nhắn nhắc lịch và chăm sóc sau dịch vụ để giao tiếp với khách nhất quán, nhanh chóng.

**Tiêu chí chấp nhận:**

- Tin nhắn nhắc lịch có tên khách, thời gian hẹn, dịch vụ và tên salon khi các dữ liệu này được cung cấp.
- Có thể xem và chỉnh sửa tin nhắn trước khi gửi.
- Khi thiếu dữ liệu bắt buộc của lịch hẹn, AI phải yêu cầu bổ sung thay vì tự tạo chi tiết.

## 11. AI tóm tắt lịch sử làm tóc

**Chức năng:** AI tóm tắt lịch sử làm tóc của khách

**User story:**  
Là một thợ làm tóc, tôi muốn xem bản tóm tắt lịch sử dịch vụ của khách để nắm nhanh sở thích và các lần làm tóc trước khi phục vụ.

**Tiêu chí chấp nhận:**

- Bản tóm tắt nêu các dịch vụ gần đây, ghi chú và sở thích nếu có trong lịch sử.
- Nội dung tóm tắt chỉ dựa trên dữ liệu lịch sử được cung cấp.
- Nếu khách chưa có lịch sử, AI phải thông báo rõ là chưa có dữ liệu để tóm tắt.
