export interface Tintuc {
    tintucId: number;                // Mã tin tức
    tieuDe: string;                  // Tiêu đề của tin tức
    hinhAnh: string;                 // Đường dẫn hình ảnh
    moTaNgan: string;                // Mô tả ngắn
    ngayDang: string;                // Ngày đăng (chuỗi)
    ngayCapNhat: string;             // Ngày cập nhật (chuỗi)
    luongKhachTruyCap: number;      // Số lượng khách truy cập
    soLuongComment: number;          // Số lượng bình luận
    danhmucId: number;               // ID danh mục
    binhLuans: any[];                // Danh sách bình luận (có thể định nghĩa thêm nếu cần)
    danhmuc: any;                    // Danh mục (có thể định nghĩa thêm nếu cần)
    tenDanhMuc?: string;
    trangThai?: boolean;
    noiDung?: string;
  }
  