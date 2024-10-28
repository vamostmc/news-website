import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { User } from '../../models/user';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private apiURL = 'https://localhost:7233/api/Customer/TongKH';
  private GetAllUrl = 'https://localhost:7233/api/Customer/DsachKH';
  private GetUserIdUrl = 'https://localhost:7233/api/Customer/GetUserId';
  private AddUrl = 'https://localhost:7233/api/Customer/AddUser';
  private DeleteUrl = 'https://localhost:7233/api/Customer/DeleteUser';
  private UpdateUrl = 'https://localhost:7233/api/Customer/EditUser';
  private UpdateStatus = 'https://localhost:7233/api/Customer/StatusUser';



  getTotalCustomer(): Observable<number> {
    return this.http.get<number>(this.apiURL);
  }

  getAllCustomer(): Observable<User[]> {
    return this.http.get<User[]>(this.GetAllUrl);
  }

  GetCustomerById(id: string): Observable<User> {
    const url = `${this.GetUserIdUrl}/${id}`;
    return this.http.get<User>(url);
  }

  AddCustomer(NewData: any): Observable<User> {
    return this.http.post<User>(this.AddUrl,NewData);
  } 

  DeleteCustomer(id: string): Observable<User> {
    const url = `${this.DeleteUrl}/${id}`;
    return this.http.delete<User>(url);
  }

  UpdateCustomer(id: string, NewData: any): Observable<User> {
    const url = `${this.UpdateUrl}/${id}`;
    return this.http.put<User>(url, NewData);
  }

  UpdateStatusCustomer(id: string, status: boolean): Observable<User[]> {
    const url = `${this.UpdateStatus}/${id}`;
    return this.http.put<User[]>(url,status);
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
    formData.append("userRole", postForm.get("userRole")?.value); // Chuyển mảng thành chuỗi JSON
    formData.append("password", postForm.get("password")?.value); // Tên trường trùng khớp
    formData.append("confirmPassword", postForm.get("confirmPassword")?.value); // Tên trường ConfirmPassword
    return formData;
  
  }

  SetFormDataEdit(postForm: FormGroup): FormData {
    let formData = new FormData();
    formData.append("id", postForm.get("id")?.value);
    formData.append("userName", postForm.get("userName")?.value); // Tên trường trùng khớp
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
