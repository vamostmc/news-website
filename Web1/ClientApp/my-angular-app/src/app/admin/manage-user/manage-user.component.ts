import { Component, OnInit } from '@angular/core';
import { CustomerService } from '../../Client/service-client/customer-service/customer.service';
import { User } from '../../Client/models/user';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { JsonPipe } from '@angular/common';

@Component({
  selector: 'app-manage-user',
  templateUrl: './manage-user.component.html',
  styleUrl: './manage-user.component.css'
})
export class ManageUserComponent implements OnInit {

  users: User[] = [];
  userView: User[] = [];
  selectedUser: User | null = null;

  postForm !: FormGroup;
  editForm !: FormGroup;
  showAddUser: boolean = false;
  showEditUser: boolean = false;
  provinces: any[] = [];
  

  constructor(private customerservice: CustomerService,
              private fb: FormBuilder,
              private route: Router
  ) {}

  p: number = 1; // Biến để lưu số trang hiện tại
  itemsPerPage: number = 5;
  
  onPageChange(page: number) {
    this.p = page;
  }

  initialization() {
    this.postForm = this.fb.group({
      id: [0],
      userName: ['', Validators.required],       
      dateUser: [new Date(), Validators.required], 
      address: ['', Validators.required],        
      fullName: ['', Validators.required],      
      isActive: [true],                        
      userRole: ['', Validators.required],      
      email: ['', Validators.required],
      password: ['', Validators.required],      
      confirmPassword: ['', Validators.required]  
    });

    this.editForm = this.fb.group({
      id: [0],
      userName: ['', Validators.required],      
      dateUser: [new Date(), Validators.required],
      address: ['', Validators.required],        
      fullName: ['', Validators.required],
      email: ['', Validators.required],      
      isActive: [true, Validators.required],                        
      userRoleList: [[], Validators.required],      
    });

    
  }

  hideForm() {
    this.showAddUser = false;
    this.showEditUser = false;
    this.postForm.reset();
  }

  // Thêm user mới
  addUser() {
    if(this.postForm.valid)
    {
      console.log(this.postForm.value);
      const formData = this.customerservice.SetFormDataAdd(this.postForm);
      this.customerservice.AddCustomer(formData).subscribe(
        (data) => {
          console.log("Thành công");
          this.route.navigate(['/success']);
        }
      )
    }
    else {
      console.log("Lỗi form");
    }
  }

  selectedEditUser(id: string) {
    this.showEditUser = true;
    this.customerservice.GetCustomerById(id).subscribe(
      (data) => {
        this.selectedUser = data;
        if(this.selectedUser) {
          this.editForm.patchValue({
            id: this.selectedUser.id,
            userName: this.selectedUser.userName,
            fullName: this.selectedUser.fullName,
            address: this.selectedUser.address,
            email: this.selectedUser.email,
            dateUser: this.selectedUser.dateUser.split('T')[0],
            userRoleList: this.selectedUser.userRoleList,
            isActive: this.selectedUser.isActive
          });
        }
        console.log(this.postForm);
      }
    )
  }

  //Cập nhật thông tin người dùng
  updateUser() {
    const formData = this.customerservice.SetFormDataEdit(this.editForm);
    formData.forEach((value, key) => {
      console.log(`${key}: ${value}`);
  });
    this.customerservice.UpdateCustomer(this.editForm.get('id')?.value, formData).subscribe(
      (data) => {
        console.log("Thành công");
        this.route.navigate(['/success']);
      }
    )
  }

  updateStatus(id: string, event: Event) {
    const status = event.target as HTMLInputElement;
    this.customerservice.UpdateStatusCustomer(id, status.checked).subscribe(
      (data) => {
        this.userView = data;
        console.log(this.userView);
      }
    );
  }


  // Xóa user 
  deleteUser(id: string) {
    console.log(id);
    this.customerservice.DeleteCustomer(id).subscribe(
      (data) => {
        console.log("Thành công");
        this.route.navigate(['/success']);
      }
    )
  }
  

  //Lấy danh sách vai trò của người dùng
  getRoleDisplayName(roles: string[]): string {
    return roles.map(role => {
      switch (role) {
        case 'Customer':
          return 'Khách hàng';
        case 'Manager':
          return 'Quản lý';
        default:
          return role; 
      }
    }).join(', '); // Kết hợp các tên vai trò thành một chuỗi
  }

  //Hiện Add Form User
  selectedAddForm() {
    console.log("OK");
    this.showAddUser = true;
  }

  
  onProvinceChange(event: any) {
    const selectedProvince = event.target.value;
    console.log('Tỉnh thành được chọn hoặc nhập:', selectedProvince);
    // Tiếp tục xử lý tỉnh thành ở đây
  }

  ngOnInit(): void {
    this.customerservice.getAllCustomer().subscribe(
      (data) => {
        this.users = data;
        this.userView = this.users;
        
      }
    );

    this.initialization();


  }

}
