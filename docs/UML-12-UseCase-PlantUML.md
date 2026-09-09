# Bộ mã PlantUML cho 12 Use Case

## Bảng đặc tả tóm tắt

| UC | Tác nhân | Điều kiện trước | Kết quả |
|---|---|---|---|
| UC01 Đăng nhập, đăng xuất, phân quyền | Admin, Lễ tân, Thợ | Có tài khoản | Đúng vai trò hoặc báo lỗi |
| UC02 Tài khoản cá nhân | Admin, Lễ tân, Thợ | Đã đăng nhập | Mật khẩu được đổi |
| UC03 Người dùng và vai trò | Admin | Đã đăng nhập Admin | Tài khoản/role được cập nhật |
| UC04 Khách hàng | Admin, Lễ tân | Đã đăng nhập | Hồ sơ khách được cập nhật |
| UC05 Thợ làm tóc | Admin | Đã đăng nhập Admin | Danh sách thợ được cập nhật |
| UC06 Dịch vụ salon | Admin | Đã đăng nhập Admin | Dịch vụ được cập nhật |
| UC07 Ca làm việc | Admin | Có thợ | Ca làm hợp lệ được lưu |
| UC08 Lịch hẹn | Admin, Lễ tân, Thợ | Có khách, thợ, dịch vụ | Lịch hợp lệ/trạng thái cập nhật |
| UC09 Hóa đơn, thanh toán | Admin, Lễ tân | Lịch đã hoàn thành | Hóa đơn/thanh toán được lưu |
| UC10 Dashboard | Admin | Đã đăng nhập | Hiển thị số liệu tổng quan |
| UC11 Báo cáo | Admin | Đã đăng nhập | Hiển thị báo cáo theo thời gian |
| UC12 AI hỗ trợ | Admin, Lễ tân, Thợ | Có khách hàng | Hiển thị kết quả AI/fallback |

## UC01 Đăng nhập đăng xuất và phân quyền

### Sơ đồ tuần tự
```plantuml
@startuml
actor "Người dùng" as U
boundary "Giao diện" as V
control "ASP.NET Identity" as I
control "UserManager" as M
database "CSDL" as DB
U -> V: Nhập email, mật khẩu
V -> I: PasswordSignInAsync()
I -> M: Kiểm tra tài khoản
M -> DB: Tìm user và role
DB --> M: Thông tin user
M --> I: Kết quả xác thực
alt Thành công
 I -> I: Tạo cookie, cấp quyền
 I --> V: Thành công và vai trò
 V --> U: Vào Dashboard phù hợp
else Thất bại
 I --> V: Thông báo lỗi
 V --> U: Hiển thị lỗi
end
U -> V: Đăng xuất
V -> I: SignOutAsync()
I --> V: Xóa phiên đăng nhập
@enduml
```

### Sơ đồ hoạt động
```plantuml
@startuml
start
:Nhập email và mật khẩu;
:Kiểm tra thông tin;
if (Thông tin hợp lệ?) then (Có)
 :Xác định vai trò;
 :Tạo phiên đăng nhập;
 :Hiển thị chức năng đúng quyền;
else (Không)
 :Hiển thị thông báo lỗi;
endif
if (Người dùng đăng xuất?) then (Có)
 :Hủy phiên đăng nhập;
endif
stop
@enduml
```

## UC02 Quản lý tài khoản cá nhân và đổi mật khẩu

### Sơ đồ tuần tự
```plantuml
@startuml
actor "Người dùng" as U
boundary "Giao diện tài khoản" as V
control "Account/Identity" as I
database "CSDL" as DB
U -> V: Nhập mật khẩu cũ và mới
V -> I: ChangePasswordAsync()
I -> DB: Kiểm tra mật khẩu cũ
DB --> I: Kết quả
alt Hợp lệ
 I -> DB: Cập nhật mật khẩu mới
 I --> V: Đổi mật khẩu thành công
else Không hợp lệ
 I --> V: Thông báo lỗi
end
V --> U: Hiển thị kết quả
@enduml
```

### Sơ đồ hoạt động
```plantuml
@startuml
start
:Mở trang tài khoản;
:Nhập mật khẩu cũ và mật khẩu mới;
if (Mật khẩu cũ đúng và mật khẩu mới hợp lệ?) then (Có)
 :Cập nhật mật khẩu;
 :Thông báo thành công;
else (Không)
 :Thông báo lỗi;
endif
stop
@enduml
```

## UC03 Quản lý người dùng và vai trò

### Sơ đồ tuần tự
```plantuml
@startuml
actor Admin
boundary "Giao diện người dùng" as V
control "UsersController" as C
control "UserManager/RoleManager" as M
database CSDL as DB
Admin -> V: Nhập email, mật khẩu, vai trò
V -> C: Gửi yêu cầu tạo tài khoản
C -> M: Kiểm tra role và tạo user
M -> DB: Lưu user, role
DB --> M: Thành công
M --> C: Kết quả
C --> V: Danh sách người dùng mới
V --> Admin: Hiển thị thông báo
@enduml
```

### Sơ đồ hoạt động
```plantuml
@startuml
start
:Admin mở quản lý người dùng;
:Nhập email, mật khẩu, vai trò;
if (Vai trò và dữ liệu hợp lệ?) then (Có)
 :Tạo tài khoản;
 :Gán vai trò;
 :Thông báo thành công;
else (Không)
 :Thông báo lỗi;
endif
stop
@enduml
```

## UC04 Quản lý khách hàng

### Sơ đồ tuần tự
```plantuml
@startuml
actor "Admin/Lễ tân" as U
boundary "Giao diện khách hàng" as V
control "CustomersController" as C
database CSDL as DB
U -> V: Thêm/sửa/xóa/tìm kiếm khách
V -> C: Gửi yêu cầu
C -> DB: Kiểm tra và thao tác dữ liệu
alt Dữ liệu hợp lệ
 DB --> C: Lưu thành công
 C --> V: Danh sách cập nhật
else Trùng số điện thoại/lỗi
 DB --> C: Báo lỗi
 C --> V: Thông báo lỗi
end
V --> U: Hiển thị kết quả
@enduml
```

### Sơ đồ hoạt động
```plantuml
@startuml
start
:Chọn quản lý khách hàng;
:Chọn thêm, sửa, xóa hoặc tìm kiếm;
if (Thêm hoặc sửa?) then (Có)
 :Nhập thông tin khách;
 if (Số điện thoại hợp lệ và không trùng?) then (Có)
  :Lưu khách hàng;
 else (Không)
  :Báo lỗi;
 endif
else (Không)
 :Tra cứu hoặc xóa khách hàng;
endif
stop
@enduml
```

## UC05 Quản lý thợ làm tóc

### Sơ đồ tuần tự
```plantuml
@startuml
actor Admin
boundary "Giao diện thợ" as V
control "StylistsController" as C
database CSDL as DB
Admin -> V: Thêm/sửa/xóa thợ
V -> C: Gửi thông tin thợ
C -> DB: Kiểm tra và lưu dữ liệu
DB --> C: Kết quả
C --> V: Danh sách thợ cập nhật
V --> Admin: Hiển thị thông báo
@enduml
```

### Sơ đồ hoạt động
```plantuml
@startuml
start
:Chọn quản lý thợ;
:Nhập hoặc cập nhật thông tin;
if (Dữ liệu hợp lệ?) then (Có)
 :Lưu thông tin thợ;
 :Hiển thị danh sách thợ;
else (Không)
 :Hiển thị lỗi;
endif
stop
@enduml
```

## UC06 Quản lý dịch vụ salon

### Sơ đồ tuần tự
```plantuml
@startuml
actor Admin
boundary "Giao diện dịch vụ" as V
control "ServicesController" as C
database CSDL as DB
Admin -> V: Thêm/sửa/xóa dịch vụ
V -> C: Gửi tên, giá, thời lượng
C -> DB: Kiểm tra tên dịch vụ
alt Không trùng và hợp lệ
 C -> DB: Lưu dịch vụ
 DB --> C: Thành công
else Không hợp lệ
 DB --> C: Báo lỗi
end
C --> V: Hiển thị kết quả
@enduml
```

### Sơ đồ hoạt động
```plantuml
@startuml
start
:Nhập tên, giá, thời lượng dịch vụ;
if (Tên không trùng và dữ liệu hợp lệ?) then (Có)
 :Lưu dịch vụ;
 :Cập nhật danh sách;
else (Không)
 :Thông báo lỗi;
endif
stop
@enduml
```

## UC07 Quản lý ca làm việc

### Sơ đồ tuần tự
```plantuml
@startuml
actor Admin
boundary "Giao diện ca làm" as V
control "StylistSchedulesController" as C
database CSDL as DB
Admin -> V: Chọn thợ, ngày, giờ bắt đầu/kết thúc
V -> C: Gửi ca làm việc
C -> DB: Kiểm tra thợ và khung giờ
alt Ca hợp lệ
 C -> DB: Lưu ca làm việc
 DB --> C: Thành công
else Không hợp lệ/trùng
 DB --> C: Báo lỗi
end
C --> V: Hiển thị kết quả
@enduml
```

### Sơ đồ hoạt động
```plantuml
@startuml
start
:Chọn thợ và ngày làm việc;
:Nhập giờ bắt đầu, kết thúc;
if (Giờ kết thúc lớn hơn giờ bắt đầu?) then (Có)
 :Lưu ca làm việc;
 :Thông báo thành công;
else (Không)
 :Thông báo lỗi;
endif
stop
@enduml
```

## UC08 Quản lý lịch hẹn

### Sơ đồ tuần tự
```plantuml
@startuml
actor "Admin/Lễ tân" as U
boundary "Giao diện lịch hẹn" as V
control "AppointmentsController" as C
control "AppointmentService" as S
database CSDL as DB
U -> V: Chọn khách, thợ, dịch vụ, ngày giờ
V -> C: Tạo/sửa lịch hẹn
C -> S: CreateAsync/RescheduleAsync
S -> DB: Kiểm tra thợ, ca làm, lịch trùng
alt Hợp lệ
 S -> DB: Lưu lịch và giờ kết thúc
 DB --> S: Thành công
 S --> C: Mã lịch hẹn
 C --> V: Xác nhận đặt lịch
else Không hợp lệ
 DB --> S: Lỗi trùng lịch/ngoài ca/quá khứ
 S --> C: Thông báo lỗi
 C --> V: Hiển thị lỗi
end
@enduml
```

### Sơ đồ hoạt động
```plantuml
@startuml
start
:Chọn khách, thợ, dịch vụ, ngày giờ;
if (Ngày hẹn hợp lệ?) then (Có)
 if (Thợ hoạt động và có ca làm?) then (Có)
  if (Không trùng lịch?) then (Có)
   :Tính giờ kết thúc;
   :Lưu lịch hẹn;
  else (Không)
   :Thông báo trùng lịch;
  endif
 else (Không)
  :Thông báo ngoài ca hoặc thợ ngừng hoạt động;
 endif
else (Không)
 :Thông báo không đặt lịch quá khứ;
endif
stop
@enduml
```

## UC09 Quản lý hóa đơn và thanh toán

### Sơ đồ tuần tự
```plantuml
@startuml
actor "Admin/Lễ tân" as U
boundary "Giao diện hóa đơn" as V
control "InvoicesController" as C
control "InvoiceService" as S
database CSDL as DB
U -> V: Chọn lịch hoàn thành, phương thức thanh toán
V -> C: Yêu cầu lập hóa đơn
C -> S: CreateAsync()
S -> DB: Kiểm tra lịch hoàn thành/chưa có hóa đơn
alt Hợp lệ
 S -> DB: Lưu hóa đơn
 S --> C: Hóa đơn mới
 C --> V: Hiển thị hóa đơn
else Không hợp lệ
 S --> C: Thông báo lỗi
 C --> V: Hiển thị lỗi
end
@enduml
```

### Sơ đồ hoạt động
```plantuml
@startuml
start
:Chọn lịch hẹn;
if (Lịch đã hoàn thành và chưa có hóa đơn?) then (Có)
 :Tạo hóa đơn theo giá dịch vụ;
 :Chọn phương thức thanh toán;
 :Cập nhật trạng thái thanh toán;
else (Không)
 :Thông báo không thể lập hóa đơn;
endif
stop
@enduml
```

## UC10 Dashboard tổng quan

### Sơ đồ tuần tự
```plantuml
@startuml
actor Admin
boundary Dashboard as V
control "HomeController" as C
database CSDL as DB
Admin -> V: Mở Dashboard
V -> C: Yêu cầu dữ liệu tổng quan
C -> DB: Đếm khách, lịch hẹn, doanh thu
DB --> C: Số liệu tổng hợp
C --> V: DashboardViewModel
V --> Admin: Hiển thị biểu đồ/số liệu
@enduml
```

### Sơ đồ hoạt động
```plantuml
@startuml
start
:Admin mở Dashboard;
:Lấy số khách hàng, lịch hẹn, doanh thu;
:Tổng hợp số liệu;
:Hiển thị Dashboard;
stop
@enduml
```

## UC11 Báo cáo

### Sơ đồ tuần tự
```plantuml
@startuml
actor Admin
boundary "Giao diện báo cáo" as V
control "ReportsController" as C
database CSDL as DB
Admin -> V: Chọn khoảng thời gian
V -> C: Yêu cầu báo cáo
C -> DB: Truy vấn hóa đơn đã thanh toán
C -> DB: Thống kê dịch vụ và khách quay lại
DB --> C: Dữ liệu báo cáo
C --> V: ReportViewModel
V --> Admin: Hiển thị báo cáo
@enduml
```

### Sơ đồ hoạt động
```plantuml
@startuml
start
:Chọn ngày bắt đầu và ngày kết thúc;
if (Khoảng thời gian hợp lệ?) then (Có)
 :Tính doanh thu;
 :Thống kê dịch vụ phổ biến;
 :Thống kê khách quay lại;
 :Hiển thị báo cáo;
else (Không)
 :Đổi lại thứ tự ngày;
endif
stop
@enduml
```

## UC12 AI gợi ý dịch vụ sinh tin nhắn tóm tắt lịch sử

### Sơ đồ tuần tự
```plantuml
@startuml
actor "Người dùng" as U
boundary "Giao diện AI" as V
control "AIController" as C
control "AIService" as A
database CSDL as DB
U -> V: Chọn khách và chức năng AI
V -> C: Gửi yêu cầu AI
C -> DB: Lấy khách, lịch sử, dịch vụ
DB --> C: Dữ liệu cần thiết
C -> A: Recommend/Message/Summarize
alt AI phản hồi thành công
 A --> C: Nội dung AI
 C -> DB: Lưu gợi ý/tin nhắn khi cần
 C --> V: Hiển thị kết quả
else AI lỗi/chưa có API Key
 A --> C: Lỗi hoặc Local Fallback
 C --> V: Thông báo hoặc kết quả fallback
end
@enduml
```

### Sơ đồ hoạt động
```plantuml
@startuml
start
:Chọn khách hàng và chức năng AI;
:Lấy dữ liệu cần thiết;
if (Dữ liệu hợp lệ?) then (Có)
 :Gọi AI Service;
 if (AI trả kết quả?) then (Có)
  :Hiển thị kết quả AI;
 else (Không)
  :Dùng Local Fallback hoặc thông báo lỗi;
 endif
else (Không)
 :Thông báo không tìm thấy khách hàng;
endif
stop
@enduml
```
