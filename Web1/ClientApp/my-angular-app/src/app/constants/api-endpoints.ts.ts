import { environment } from "../../environments/environment.development"

const API_BASE = environment.apiUrl;

export const API_ENDPOINTS = {
    tinTuc: {
      base: `${API_BASE}/TinTuc`,
      manage: `${API_BASE}/TinTuc/TintucWithDanhmuc`,
      delete: (id: number) => `${API_BASE}/TinTuc/Delete/${id}`,
      add: `${API_BASE}/TinTuc/ThemTinTuc`,
      update: (id: number) => `${API_BASE}/TinTuc/Edit/${id}`,
      updateStatus: (id: number) => `${API_BASE}/TinTuc/UpdateStatus/${id}`,
      importExcel: `${API_BASE}/TinTuc/Import-Excel`,
      downloadExcelTemplate: `${API_BASE}/TinTuc/Template-Excel`
    },

    customer: {
      total: `${API_BASE}/Customer/TongKH`,
      getAll: `${API_BASE}/Customer/DsachKH`,
      getByUserId: (id: string) => `${API_BASE}/Customer/GetUserId/${id}`,
      add: `${API_BASE}/Customer/AddUser`,
      delete: (id: string) => `${API_BASE}/Customer/DeleteUser/${id}`,
      update: (id: string) => `${API_BASE}/Customer/EditUser/${id}`,
      updateStatus: (id: string) => `${API_BASE}/Customer/StatusUser/${id}`
    },

    danhMuc: {
      getAll: `${API_BASE}/DanhMuc/GetDanhmuc`,
      getById: (id: number) => `${API_BASE}/DanhMuc/GetDanhmuc/${id}`,
      add: `${API_BASE}/DanhMuc/AddDanhMuc`,
      delete: (id: number) => `${API_BASE}/DanhMuc/RemoveDanhMuc/${id}`,
      update: (id: number) => `${API_BASE}/DanhMuc/EditDanhMuc/${id}`,
      updateStatus: (id: number) => `${API_BASE}/DanhMuc/EditStatusDanhMuc/${id}`
    },

    comment: {
      getAll: `${API_BASE}/Comment/GetAllComment`,
      add: `${API_BASE}/Comment/AddCommentNew`,
      getById: (id: number) => `${API_BASE}/Comment/GetCommentById/${id}`,
      delete: (id: number) => `${API_BASE}/Comment/DeleteComment/${id}`,
      update: (id: number) => `${API_BASE}/Comment/EditCommnent/${id}`,
      updateStatus: (id: number) => `${API_BASE}/Comment/UpdateStatus/${id}`,
      getByTinTucId: (tinTucId: number) => `${API_BASE}/Comment/GetCommentByTinTucId/${tinTucId}`
    },

    authen: {
      register: `${API_BASE}/Account/Sign-Up`,
      login: `${API_BASE}/Account/Log-In-JWT`,
      loginGoogle: `${API_BASE}/Account/Login-Google`,
      checkHeader: `${API_BASE}/Account/check-header`,
      logout: `${API_BASE}/Account/LogOut`,
      refreshToken: `${API_BASE}/Account/RefreshToken`,
      checkAdmin: `${API_BASE}/Account/Check-Admin`
    },
     
    emailVerification: {
      sendVerifyMail: (Id: string) => `${API_BASE}/ConfirmEmail/send-verify-mail/${Id}`,
      checkVerifyCode: `${API_BASE}/ConfirmEmail/check-verify-code`
    },

    passwordReset: {
      sendOtpPassword: (NameorEmail: string) => `${API_BASE}/Password/SendOtpPassword/${NameorEmail}`,
      checkVerifyCodePassword: `${API_BASE}/Password/check-verify-code-password`,
      resetPassword: `${API_BASE}/Password/Reset-password`
    }
  };