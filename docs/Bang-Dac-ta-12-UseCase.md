# 2.3.4 Đặc tả các Use Case

## UC01 Đăng nhập đăng xuất và phân quyền
| Nội dung | Mô tả |
|---|---|
| Use Case | UC01 - Đăng nhập, đăng xuất và phân quyền |
| Mục đích | Xác thực người dùng và cấp quyền truy cập theo vai trò. |
| Tác nhân | Quản trị viên, Lễ tân, Thợ làm tóc. |
| Điều kiện trước | Người dùng đã có tài khoản hợp lệ. |
| Luồng chính | 1. Nhập email, mật khẩu. 2. Hệ thống kiểm tra. 3. Xác định vai trò. 4. Tạo phiên đăng nhập. 5. Điều hướng trang phù hợp. |
| Ngoại lệ | Email/mật khẩu sai hoặc không có quyền thì thông báo lỗi. |
| Điều kiện sau | Người dùng đăng nhập đúng quyền; khi đăng xuất, phiên làm việc kết thúc. |

## UC02 Quản lý tài khoản cá nhân và đổi mật khẩu
| Nội dung | Mô tả |
|---|---|
| Use Case | UC02 - Quản lý tài khoản cá nhân và đổi mật khẩu |
| Mục đích | Cho phép người dùng đổi mật khẩu bảo mật. |
| Tác nhân | Quản trị viên, Lễ tân, Thợ làm tóc. |
| Điều kiện trước | Người dùng đã đăng nhập. |
| Luồng chính | 1. Mở trang tài khoản. 2. Nhập mật khẩu cũ và mật khẩu mới. 3. Hệ thống kiểm tra. 4. Cập nhật mật khẩu mới. |
| Ngoại lệ | Mật khẩu cũ sai hoặc mật khẩu mới không đúng quy tắc. |
| Điều kiện sau | Mật khẩu mới được lưu thành công. |

## UC03 Quản lý người dùng và vai trò
| Nội dung | Mô tả |
|---|---|
| Use Case | UC03 - Quản lý người dùng và vai trò |
| Mục đích | Quản lý tài khoản nội bộ và gán vai trò phù hợp. |
| Tác nhân | Quản trị viên. |
| Điều kiện trước | Quản trị viên đã đăng nhập. |
| Luồng chính | 1. Mở danh sách người dùng. 2. Chọn tạo tài khoản. 3. Nhập email, mật khẩu, vai trò. 4. Hệ thống tạo tài khoản và gán quyền. |
| Ngoại lệ | Email đã tồn tại, mật khẩu không hợp lệ hoặc vai trò không hợp lệ. |
| Điều kiện sau | Tài khoản mới xuất hiện trong danh sách với đúng vai trò. |

## UC04 Quản lý khách hàng
| Nội dung | Mô tả |
|---|---|
| Use Case | UC04 - Quản lý khách hàng |
| Mục đích | Lưu trữ và tra cứu thông tin khách hàng phục vụ đặt lịch. |
| Tác nhân | Quản trị viên, Lễ tân. |
| Điều kiện trước | Người dùng đã đăng nhập đúng quyền. |
| Luồng chính | 1. Mở danh sách khách. 2. Thêm, xem, sửa, xóa hoặc tìm kiếm khách. 3. Hệ thống kiểm tra dữ liệu. 4. Lưu thay đổi. |
| Ngoại lệ | Số điện thoại trùng, thiếu họ tên hoặc dữ liệu không hợp lệ. |
| Điều kiện sau | Thông tin khách hàng được cập nhật trong CSDL. |

## UC05 Quản lý thợ làm tóc
| Nội dung | Mô tả |
|---|---|
| Use Case | UC05 - Quản lý thợ làm tóc |
| Mục đích | Quản lý thông tin và trạng thái làm việc của thợ. |
| Tác nhân | Quản trị viên. |
| Điều kiện trước | Quản trị viên đã đăng nhập. |
| Luồng chính | 1. Mở danh sách thợ. 2. Thêm, xem, sửa hoặc xóa thợ. 3. Nhập chuyên môn, kinh nghiệm và trạng thái. 4. Lưu dữ liệu. |
| Ngoại lệ | Thiếu thông tin bắt buộc hoặc dữ liệu không hợp lệ. |
| Điều kiện sau | Danh sách thợ làm tóc được cập nhật. |

## UC06 Quản lý dịch vụ salon
| Nội dung | Mô tả |
|---|---|
| Use Case | UC06 - Quản lý dịch vụ salon |
| Mục đích | Quản lý tên, giá, thời lượng và trạng thái dịch vụ. |
| Tác nhân | Quản trị viên. |
| Điều kiện trước | Quản trị viên đã đăng nhập. |
| Luồng chính | 1. Mở danh sách dịch vụ. 2. Thêm, sửa, xóa hoặc đổi trạng thái dịch vụ. 3. Nhập tên, giá và thời lượng. 4. Lưu dữ liệu. |
| Ngoại lệ | Tên dịch vụ trùng; giá hoặc thời lượng không hợp lệ. |
| Điều kiện sau | Dịch vụ được cập nhật và có thể chọn khi đặt lịch. |

## UC07 Quản lý ca làm việc của thợ
| Nội dung | Mô tả |
|---|---|
| Use Case | UC07 - Quản lý ca làm việc của thợ |
| Mục đích | Thiết lập ca làm việc để phục vụ kiểm tra lịch hẹn. |
| Tác nhân | Quản trị viên. |
| Điều kiện trước | Thợ làm tóc đã tồn tại. |
| Luồng chính | 1. Chọn thợ và ngày làm việc. 2. Nhập giờ bắt đầu, giờ kết thúc. 3. Hệ thống kiểm tra khung giờ. 4. Lưu ca làm việc. |
| Ngoại lệ | Giờ kết thúc nhỏ hơn giờ bắt đầu hoặc ca làm trùng. |
| Điều kiện sau | Ca làm hợp lệ được lưu trong CSDL. |

## UC08 Quản lý lịch hẹn
| Nội dung | Mô tả |
|---|---|
| Use Case | UC08 - Quản lý lịch hẹn |
| Mục đích | Tạo, sửa, hủy, hoàn thành và tra cứu lịch hẹn. |
| Tác nhân | Quản trị viên, Lễ tân, Thợ làm tóc. |
| Điều kiện trước | Có khách hàng, thợ, dịch vụ và ca làm việc phù hợp. |
| Luồng chính | 1. Chọn khách, thợ, dịch vụ, ngày giờ. 2. Hệ thống kiểm tra lịch quá khứ, ca làm, thợ hoạt động và lịch trùng. 3. Tính giờ kết thúc. 4. Lưu lịch hẹn. |
| Ngoại lệ | Lịch quá khứ, ngoài ca, thợ ngừng hoạt động hoặc trùng lịch. |
| Điều kiện sau | Lịch hợp lệ được lưu; lịch hoàn thành tạo lịch sử dịch vụ. |

## UC09 Quản lý hóa đơn và thanh toán
| Nội dung | Mô tả |
|---|---|
| Use Case | UC09 - Quản lý hóa đơn và thanh toán |
| Mục đích | Lập hóa đơn và ghi nhận việc thanh toán. |
| Tác nhân | Quản trị viên, Lễ tân. |
| Điều kiện trước | Lịch hẹn đã hoàn thành và chưa có hóa đơn. |
| Luồng chính | 1. Chọn lịch hẹn hoàn thành. 2. Hệ thống lấy giá dịch vụ. 3. Tạo hóa đơn. 4. Chọn tiền mặt, chuyển khoản hoặc thẻ. 5. Cập nhật thanh toán. |
| Ngoại lệ | Lịch chưa hoàn thành hoặc đã có hóa đơn. |
| Điều kiện sau | Hóa đơn và trạng thái thanh toán được lưu. |

## UC10 Dashboard tổng quan
| Nội dung | Mô tả |
|---|---|
| Use Case | UC10 - Dashboard tổng quan |
| Mục đích | Cung cấp thông tin nhanh về tình hình hoạt động của salon. |
| Tác nhân | Quản trị viên. |
| Điều kiện trước | Quản trị viên đã đăng nhập. |
| Luồng chính | 1. Mở Dashboard. 2. Hệ thống tổng hợp số khách, lịch hẹn và doanh thu. 3. Hiển thị số liệu. |
| Ngoại lệ | Không có dữ liệu thì hiển thị giá trị bằng 0. |
| Điều kiện sau | Số liệu tổng quan được hiển thị. |

## UC11 Báo cáo doanh thu và thống kê
| Nội dung | Mô tả |
|---|---|
| Use Case | UC11 - Báo cáo doanh thu, dịch vụ phổ biến và khách quay lại |
| Mục đích | Hỗ trợ quản trị viên theo dõi hiệu quả hoạt động salon. |
| Tác nhân | Quản trị viên. |
| Điều kiện trước | Quản trị viên đã đăng nhập. |
| Luồng chính | 1. Chọn khoảng thời gian. 2. Hệ thống tổng hợp hóa đơn đã thanh toán. 3. Thống kê doanh thu, dịch vụ phổ biến và khách quay lại. 4. Hiển thị báo cáo. |
| Ngoại lệ | Nếu ngày kết thúc nhỏ hơn ngày bắt đầu, hệ thống điều chỉnh thứ tự ngày. |
| Điều kiện sau | Báo cáo được hiển thị theo khoảng thời gian chọn. |

## UC12 AI gợi ý dịch vụ sinh tin nhắn và tóm tắt lịch sử
| Nội dung | Mô tả |
|---|---|
| Use Case | UC12 - AI gợi ý dịch vụ, sinh tin nhắn và tóm tắt lịch sử khách hàng |
| Mục đích | Hỗ trợ tư vấn và chăm sóc khách hàng bằng AI. |
| Tác nhân | Quản trị viên, Lễ tân, Thợ làm tóc. |
| Điều kiện trước | Người dùng đã đăng nhập; khách hàng tồn tại. |
| Luồng chính | 1. Chọn khách và chức năng AI. 2. Hệ thống lấy lịch sử/dịch vụ cần thiết. 3. Gọi AI Service. 4. Hiển thị gợi ý, tin nhắn hoặc bản tóm tắt. |
| Ngoại lệ | AI lỗi, timeout, response rỗng hoặc chưa có API Key. |
| Điều kiện sau | Kết quả AI được hiển thị; người dùng kiểm tra trước khi sử dụng. |
