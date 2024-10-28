import { Component, OnInit } from '@angular/core';
import { DanhmucService } from '../../Client/service-client/danhmuc-service/danhmuc.service';
import { HttpClient } from '@angular/common/http';
import { Danhmuc } from '../../Client/models/danhmuc';
import { Tintuc } from '../../Client/models/tintuc';
import { DashboardService } from '../../Client/service-client/dashboard-service/dashboard.service';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-manage-category',
  templateUrl: './manage-category.component.html',
  styleUrl: './manage-category.component.css'
})
export class ManageCategoryComponent implements OnInit {

  constructor(private http: HttpClient, 
              private danhmucService: DanhmucService, 
              private dashboardService: DashboardService,
              private route : Router,
              private fb: FormBuilder) {}

  danhmucs: Danhmuc[] = [];
  danhmucView: Danhmuc[] = [];
  selectedDanhmuc: Danhmuc | null = null;
  
  showAddDanhmuc = false;
  showEditDanhmuc = false;
  selectedTab: string = 'thong-tin';
  postForm!: FormGroup;
  

  //Khởi tạo Form Group để add và edit data
  initialization() {
    this.postForm = this.fb.group({
      danhmucId: [0],
      tenDanhMuc: ['', Validators.required],
      trangThai: [false, Validators.required],
      soLuongTinTuc: [0]
    });
  }

  //Chuyển sang tab khác trong form
  selectTab(tab: string) {
    this.selectedTab = tab;
  }

  //Ẩn form đi khi nhấn ra bên ngoài
  hideForm() {
    this.showAddDanhmuc = false;
    this.showEditDanhmuc = false;
    this.postForm.reset();
  }

  //Thực hiện chức năng xóa
  deleteDanhMuc(id: number) {
    this.danhmucService.DeleteDanhMuc(id).subscribe(
      (data) => {
        console.log("Xóa thành công");
        this.route.navigate(['/success']);
      }
    )
  }

  //Click vào thêm danh mục để hiện form
  selectedAdd() {
    this.showAddDanhmuc = true;
  }

  //Chọn danh mục Id cần edit
  selectedEdit(id: number) {
    this.showEditDanhmuc = true;
    this.danhmucService.GetDanhMucById(id).subscribe(
      (data) => {
        this.selectedDanhmuc = data;
        console.log(this.selectedDanhmuc);
        if(this.selectedDanhmuc) {
          this.postForm.patchValue({
            danhmucId: this.selectedDanhmuc.danhmucId,
            tenDanhMuc: this.selectedDanhmuc.tenDanhMuc,
            trangThai: this.selectedDanhmuc.trangThai
          });
        }
      }
    )
  }

  //Thực hiện chức năng thêm mới
  addDanhMuc() {
    if(this.postForm.valid) {
      this.danhmucService.SendDataPost(this.postForm.getRawValue());
    }
    else {
      console.log("kk");
    }

  }

  //Thực hiện chức năng sửa
  editDanhMuc() {
    if(this.postForm.valid) {
      this.danhmucService.SendDataUpdate(this.postForm.get('danhmucId')?.value, this.postForm);
    }
    else {
      console.log("Form edit lỗi");
    }
  }

  UpdateStatusDM(id: number, event: Event) {
    const checkbox = event.target as HTMLInputElement;
    this.danhmucService.UpdateStatus(id,checkbox.checked
    ).subscribe(
      (data) => { 
        this.danhmucView = data;
        console.log(this.danhmucView);
      }
    )
  }

  ngOnInit(): void {
    this.danhmucService.GetDanhmuc().subscribe(
      (data) => {
        this.danhmucs = data;
        this.danhmucView = this.danhmucs;
        console.log(this.danhmucs)
      }
    )

    this.initialization();
  }
  
}
