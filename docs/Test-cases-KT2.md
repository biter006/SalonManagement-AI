# Bộ test case — Hệ thống quản lý đặt lịch Salon tóc tích hợp AI

## 1. Mục đích và quy ước

Tài liệu dùng để kiểm thử thủ công khi demo và làm minh chứng cho tiêu chí **kiểm thử chức năng quản lý và AI** của Bài kiểm tra thường xuyên 2. Mỗi test case cần được ghi kết quả `Đạt/Không đạt`, người kiểm thử và thời điểm thực hiện vào cột *Thực tế* khi nộp bài.

**Môi trường:** Windows 10/11, .NET 8, SQL Server LocalDB hoặc SQL Server, Chrome/Edge.  
**Tài khoản mẫu:** `admin@salon.local`, `receptionist@salon.local`, `stylist1@salon.local`; mật khẩu `Salon@123`.  
**Quy ước:** TC = Test case; P = Pass/Đạt; F = Fail/Không đạt.

## 2. Dữ liệu chuẩn bị

1. Chạy migration và khởi động ứng dụng theo `README.md`.
2. Đăng nhập lần lượt theo vai trò cần kiểm thử.
3. Dữ liệu mẫu có khách hàng, thợ, dịch vụ, lịch làm việc từ 08:00–18:00 và lịch hẹn mẫu.
4. Khi cần tạo lịch mới, dùng ngày từ ngày mai trở đi; chọn thợ có ca làm và khung giờ chưa có lịch hẹn.

## 3. Test case chức năng

| Mã | Chức năng | Tiền điều kiện | Bước thực hiện / Dữ liệu vào | Kết quả mong đợi | Thực tế |
|---|---|---|---|---|---|
| TC-AUTH-01 | Đăng nhập hợp lệ | Có tài khoản Admin | Nhập `admin@salon.local` / `Salon@123`, bấm Đăng nhập | Đến Dashboard, hiển thị tên tài khoản | |
| TC-AUTH-02 | Sai mật khẩu | Có tài khoản Admin | Nhập đúng email, mật khẩu `SaiMatKhau1!` | Không đăng nhập; hiện thông báo lỗi, không lộ chi tiết hệ thống | |
| TC-AUTH-03 | Bỏ trống dữ liệu đăng nhập | Không | Bỏ trống email hoặc mật khẩu rồi gửi biểu mẫu | Báo bắt buộc nhập tại trường thiếu | |
| TC-AUTH-04 | Bảo vệ trang cần đăng nhập | Đã đăng xuất | Mở trực tiếp `/Appointments` | Chuyển đến trang đăng nhập | |
| TC-AUTH-05 | Phân quyền trang người dùng | Đăng nhập Lễ tân/Thợ | Mở trực tiếp `/Users` | Bị từ chối truy cập (403) hoặc không thấy menu Quản lý người dùng | |
| TC-USR-01 | Xem danh sách người dùng | Đăng nhập Admin | Mở menu **Quản lý người dùng** | Hiển thị email và vai trò, không lỗi 500/DataReader | |
| TC-USR-02 | Tạo tài khoản hợp lệ | Đăng nhập Admin | Tạo `demo.test@salon.local`, mật khẩu đủ 8 ký tự, vai trò Lễ tân | Thông báo tạo thành công; tài khoản xuất hiện trong danh sách với vai trò Lễ tân | |
| TC-USR-03 | Chặn vai trò không hợp lệ | Đăng nhập Admin | Gửi biểu mẫu tạo tài khoản với vai trò không có trong danh sách | Không tạo tài khoản; hiện “Vai trò không hợp lệ” | |
| TC-CUS-01 | Thêm khách hàng | Đăng nhập Admin/Lễ tân | Thêm khách có tên, điện thoại chưa tồn tại | Lưu thành công; khách xuất hiện trong danh sách | |
| TC-CUS-02 | Trùng số điện thoại | Đã có khách số `0900000001` | Thêm khách khác cùng số điện thoại | Không lưu; báo lỗi số điện thoại đã tồn tại/ràng buộc dữ liệu | |
| TC-CUS-03 | Tìm khách | Có dữ liệu khách | Nhập một phần tên hoặc số điện thoại vào ô tìm kiếm | Chỉ hiển thị khách phù hợp từ khóa | |
| TC-CUS-04 | Sửa thông tin khách | Có khách | Mở Sửa, thay đổi ghi chú, Lưu | Trang chi tiết/danh sách hiển thị ghi chú mới | |
| TC-SER-01 | Thêm dịch vụ | Đăng nhập Admin | Thêm “Cắt test”, giá `150000`, thời lượng `45` | Dịch vụ được lưu và hiện giá/thời lượng đúng | |
| TC-SER-02 | Chặn dịch vụ trùng tên | Đã có “Cắt test” | Tạo lại dịch vụ cùng tên | Không lưu; thông báo tên dịch vụ đã tồn tại | |
| TC-STY-01 | Tạo thợ và ca làm | Đăng nhập Admin | Thêm thợ đang hoạt động; tạo ca 08:00–18:00 cho ngày mai | Thợ và ca làm hiển thị đúng trong danh sách | |
| TC-APT-01 | Đặt lịch hợp lệ | Có khách, thợ hoạt động, ca làm, dịch vụ | Đặt ngày mai lúc 09:00 với dịch vụ 60 phút | Lịch được tạo; giờ kết thúc tự tính 10:00 | |
| TC-APT-02 | Chặn lịch quá khứ | Có dữ liệu đặt lịch | Chọn ngày hôm qua | Không lưu; thông báo không đặt lịch trong quá khứ | |
| TC-APT-03 | Chặn ngoài ca làm | Thợ có ca 08:00–18:00 | Đặt lịch lúc 17:30, dịch vụ 60 phút | Không lưu; thông báo lịch vượt ngoài ca làm | |
| TC-APT-04 | Chặn trùng lịch thợ | Thợ đã có lịch 09:00–10:00 | Đặt lịch cùng thợ lúc 09:30 | Không lưu; thông báo lịch bị trùng | |
| TC-APT-05 | Lọc lịch hẹn | Có nhiều lịch hẹn | Lọc theo ngày, thợ, khách hoặc trạng thái | Danh sách chỉ hiển thị bản ghi phù hợp tiêu chí | |
| TC-APT-06 | Hủy lịch | Có lịch Chờ xác nhận/Xác nhận | Nhập lý do và bấm Hủy | Trạng thái chuyển Đã hủy, lý do được lưu | |
| TC-APT-07 | Hoàn thành lịch | Có lịch Xác nhận | Bấm Hoàn thành, nhập ghi chú của thợ | Trạng thái Hoàn thành; tạo lịch sử dịch vụ cho khách | |
| TC-INV-01 | Lập hóa đơn lịch hoàn thành | Có lịch Hoàn thành, chưa có hóa đơn | Lập hóa đơn, chọn tiền mặt/chuyển khoản/thẻ | Hóa đơn tạo thành công; tổng tiền bằng giá dịch vụ | |
| TC-INV-02 | Chặn lập hóa đơn lịch chưa hoàn thành | Có lịch Xác nhận | Thử lập hóa đơn | Không tạo hóa đơn; thông báo chỉ áp dụng cho lịch hoàn thành | |
| TC-INV-03 | Chống tạo hóa đơn trùng | Lịch đã có hóa đơn | Thử lập lần hai | Không tạo hóa đơn thứ hai cho cùng lịch hẹn | |
| TC-RPT-01 | Dashboard/báo cáo | Có dữ liệu mẫu | Mở Dashboard và Báo cáo | Có số liệu tổng quan, doanh thu, dịch vụ phổ biến, khách quay lại; không lỗi | |
| TC-AI-01 | AI gợi ý dịch vụ | Có khách, danh sách dịch vụ | Vào AI Gợi ý, nhập nhu cầu tóc | Hiện gợi ý dễ đọc; chỉ tham chiếu dịch vụ đang có | |
| TC-AI-02 | AI sinh tin nhắn | Có khách/lịch hẹn | Chọn Nhắc lịch hoặc Chăm sóc sau dịch vụ, bấm tạo | Hiện nội dung tin nhắn và lưu lịch sử sinh tin | |
| TC-AI-03 | AI tóm tắt lịch sử | Khách có lịch sử dịch vụ | Mở AI Tóm tắt lịch sử | Có tóm tắt ngắn để thợ tham khảo | |
| TC-AI-04 | Không có API key | `AI:Provider=LocalFallback` hoặc không cấu hình key | Thực hiện một chức năng AI | Không crash; trả fallback/Thông báo thân thiện, không lộ API key | |
| TC-ERR-01 | ID không tồn tại | Đăng nhập phù hợp | Mở `/Appointments/Details/999999` | Trả trang Không tìm thấy (404), không lỗi 500 | |

## 4. Kiểm thử tự động hiện có

Chạy lệnh sau tại thư mục gốc dự án:

```powershell
dotnet test SalonManagement.Tests/SalonManagement.Tests.csproj
```

Các test tự động bao phủ tính giờ kết thúc của lịch hẹn, chặn lịch trùng/ngoài ca/quá khứ, tạo hóa đơn, phản hồi AI hợp lệ/rỗng/lỗi và hiển thị danh sách người dùng kèm vai trò. Test `UsersControllerTests.Index_LoadsUsersAndDisplaysTheirRoles` được thêm sau lỗi DataReader để kiểm tra trang Người dùng trả về danh sách và role bình thường.

## 5. Kết quả kiểm thử cần ghi khi demo

Sau khi chạy từng trường hợp, ghi `P` nếu đúng kết quả mong đợi; nếu `F`, ghi thêm ảnh lỗi, bước tái hiện và cách đã xử lý. Tối thiểu nên chụp minh chứng cho TC-USR-01, TC-APT-04, TC-INV-01, TC-AI-01 và kết quả lệnh `dotnet test`.
