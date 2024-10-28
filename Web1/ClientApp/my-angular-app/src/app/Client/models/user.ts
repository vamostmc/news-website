export interface User {
    dateUser: string;       // Ngày đăng ký của người dùng
    address: string;         // Địa chỉ
    creationDate: string;   // Ngày tạo tài khoản
    fullName: string;       // Tên đầy đủ
    binhLuans?: any[];       // Danh sách các bình luận của người dùng (có thể là một mảng các đối tượng)
    id: string;             // ID của người dùng
    userName: string;       // Tên người dùng (username)
    isActive?: boolean;
    userRole?: string;
    userRoleList?: string[];
}