import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { User } from '../../models/user';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { API_ENDPOINTS } from '../../../constants/api-endpoints.ts';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {

  getTotalCustomer(): Observable<number> {
    return this.http.get<number>(API_ENDPOINTS.customer.total);
  }
  
  getAllCustomer(): Observable<User[]> {
    return this.http.get<User[]>(API_ENDPOINTS.customer.getAll);
  }
  
  GetCustomerById(id: string): Observable<User> {
    const url = API_ENDPOINTS.customer.getByUserId(id);
    return this.http.get<User>(url);
  }
  
  AddCustomer(NewData: any): Observable<User> {
    return this.http.post<User>(API_ENDPOINTS.customer.add, NewData);
  }
  
  DeleteCustomer(id: string): Observable<User> {
    const url = API_ENDPOINTS.customer.delete(id);
    return this.http.delete<User>(url);
  }
  
  UpdateCustomer(id: string, NewData: any): Observable<User> {
    const url = API_ENDPOINTS.customer.update(id);
    return this.http.put<User>(url, NewData);
  }
  
  UpdateStatusCustomer(id: string, status: boolean): Observable<User[]> {
    const url = API_ENDPOINTS.customer.updateStatus(id);
    return this.http.put<User[]>(url, status);
  }
  
  SetFormData(FormUser: any): FormData {
    const formData = new FormData();
    Object.keys(FormUser).forEach(key => {
      formData.append(key, FormUser[key]);
    });
    console.log("Form Data là:");
    console.log(formData);
    return formData;
  }

  SetFormDataAdd(postForm: FormGroup): FormData {
    let formData = new FormData();
    // Gửi dữ liệu từ form group sang form data
    formData.append("userName", postForm.get("userName")?.value); // Tên trường trùng khớp
    formData.append("dateUser", postForm.get("dateUser")?.value); // Định dạng ngày
    formData.append("address", postForm.get("address")?.value); // Tên trường trùng khớp
    formData.append("fullName", postForm.get("fullName")?.value); // Tên trường trùng khớp
    formData.append("email", postForm.get("email")?.value);
    formData.append("userRole", postForm.get("userRole")?.value); // Chuyển mảng thành chuỗi JSON
    formData.append("password", postForm.get("password")?.value); // Tên trường trùng khớp
    formData.append("confirmPassword", postForm.get("confirmPassword")?.value); // Tên trường ConfirmPassword
    return formData;
  }

  SetFormDataEdit(postForm: FormGroup): FormData {
    let formData = new FormData();
    formData.append("id", postForm.get("id")?.value);
    formData.append("userName", postForm.get("userName")?.value); // Tên trường trùng khớp
    formData.append("email", postForm.get("email")?.value);
    formData.append("dateUser", postForm.get("dateUser")?.value); // Định dạng ngày
    formData.append("address", postForm.get("address")?.value); // Tên trường trùng khớp
    formData.append("fullName", postForm.get("fullName")?.value); // Tên trường trùng khớp
    formData.append("userRoleList", postForm.get("userRoleList")?.value); // Chuyển mảng thành chuỗi JSON
    formData.append("isActive", postForm.get("isActive")?.value);
    return formData;
  }

  SendDataPost(form: FormData) {
    this.AddCustomer(form).subscribe({
      next: (response) => {
        console.log('Thêm thành công', response);
        this.route.navigate(['/success']);
      },
      error: (error) => {
        console.error('Lỗi khi thêm danh mục', error);
      },
    });
  }

  constructor(private http: HttpClient, private route: Router) { }
}
