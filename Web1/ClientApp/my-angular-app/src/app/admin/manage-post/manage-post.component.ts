import { Component, OnInit, ViewChild } from '@angular/core';
import { StudentService } from '../../Client/service-client/student-service/student.service';
import { Student } from '../../Client/models/student';
import { HttpClient } from '@angular/common/http';
import { Tintuc } from '../../Client/models/tintuc';
import { TinTucService } from '../../Client/service-client/tintuc-service/tin-tuc.service';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators  } from '@angular/forms';
import { DanhmucService } from '../../Client/service-client/danhmuc-service/danhmuc.service';
import { Danhmuc } from '../../Client/models/danhmuc';
import { FormsModule } from '@angular/forms';
import { DashboardService } from '../../Client/service-client/dashboard-service/dashboard.service';
import { firstValueFrom, forkJoin, switchMap } from 'rxjs';
import { EditPostComponent } from './edit-post/edit-post.component';
import { Location } from '@angular/common';

@Component({
  selector: 'app-manage-post',
  templateUrl: './manage-post.component.html',
  styleUrl: './manage-post.component.css'
})
export class ManagePostComponent implements OnInit {
  
  constructor(private tintucservice: TinTucService, 
              private http: HttpClient, 
              private route: Router,
              private fb: FormBuilder,
              private danhmucservice: DanhmucService,
              private dashboardservice: DashboardService,
              private location: Location) {}
  
  tintucs: Tintuc[] = [];
  danhmucs: Danhmuc[] = [];
  selectedTintuc: Tintuc | null = null;
  selectedTintucEdit: Tintuc | null = null;
  postForm!: FormGroup;
  filterForm!: FormGroup;
  
  showDeleteTintucs = false;
  showInfoTintucs = false;
  
  showFilterOptions = false;
  showSearchTintucs = false;
  
  isSearchVisible = false;    // Biến kiểm soát hiển thị trường tìm kiếm
  isFilterVisible = false;    // Biến kiểm soát hiển thị bộ lọc
  searchQuery: string = '';   // Lưu chuỗi tìm kiếm
  selectedCategory: string = '';  // Lưu giá trị của bộ lọc danh mục
  selectedTab: string = 'thong-tin';

  searchTerm: string = '';
  tintucSearch: Tintuc[] = [];
  tintucView: Tintuc[] = [];
  tintucFilter: Tintuc[] = [];

  filterCount = 0;
  FilterCategory: string = '';
  FilterStartDate: string = '';
  FilterEndDate: string = '';
  FilterStartComment: number | null = null;
  FilterEndComment: number | null = null;

  loading: boolean = true;

  @ViewChild(EditPostComponent) editPostComponent: EditPostComponent | null = null;

  private Url = 'https://localhost:7233';

  p: number = 1; // Biến để lưu số trang hiện tại
  onPageChange(page: number) {
    this.p = page;
  }

  
  //Chuyển sang tab khác trong form
  selectTab(tab: string) {
    this.selectedTab = tab;
  }

  toggleFilter() {
    console.log("KK");
    this.showFilterOptions = !this.showFilterOptions; // Chuyển đổi hiển thị
  }

  updateFilterCount() {
    let count = 0;
  
    // Lấy dữ liệu từ FormGroup
    const formValues = this.filterForm.value;
  
    // Kiểm tra các trường trong FormGroup
    if (formValues.FilterCategory) {
      count++;
    }
    if (formValues.FilterStartDate) {
      count++;
    }
    if (formValues.FilterEndDate) {
      count++;
    }
    if (formValues.FilterStartUpdateDate) {
      count++;
    }
    if (formValues.FilterEndUpdateDate) {
      count++;
    }
    if (formValues.FilterStartComment) {
      count++;
    }
    if (formValues.FilterEndComment) {
      count++;
    }
  
    // Cập nhật biến filterCount
    this.filterCount = count;
  }
  

  resetFilters() {
    console.log("OK");
    // Reset tất cả các bộ lọc
    this.filterForm.reset();
    this.filterCount = 0; // Reset bộ đếm
  }
  

  initializeForm() {
    this.postForm = this.fb.group({
      TintucId: [0], // ID bài viết
      TieuDe: ['', Validators.required], // Tiêu đề bài viết
      HinhAnh: ['', Validators.required], // Hình ảnh
      MoTaNgan: ['', Validators.required], // Mô tả ngắn
      NgayDang: [new Date(), Validators.required], // Ngày đăng
      NgayCapNhat: [new Date(), Validators.required], // Ngày cập nhật
      LuongKhachTruyCap: [0, Validators.required], // Lượng khách truy cập
      TrangThai: [false], // Số lượng bình luận
      DanhmucId: ['', Validators.required] // Danh mục
    });

    this.filterForm = this.fb.group({
      FilterCategory: [''],
      FilterStartDate: [null],
      FilterEndDate: [null],
      FilterStartUpdateDate: [null], // Ngày bắt đầu cập nhật
      FilterEndUpdateDate: [null],     // Ngày kết thúc cập nhật
      FilterStartComment: [null],
      FilterEndComment: [null]
    });
  }

  applyFilters() {
    const filters = this.filterForm.value;

    //Trường hợp đã gõ từ khóa tìm kiếm thì chỉ lọc trong những phần tìm kiếm
    if (this.searchTerm && this.searchTerm.trim() !== '') {
      this.tintucFilter = this.tintucView.filter(tin => {
        return (!filters.FilterCategory || tin.danhmucId === +filters.FilterCategory) &&
          (!filters.FilterStartDate || new Date(tin.ngayDang) >= new Date(filters.FilterStartDate)) &&
          (!filters.FilterEndDate || new Date(tin.ngayDang) <= new Date(filters.FilterEndDate)) &&
          (!filters.FilterStartUpdateDate || new Date(tin.ngayCapNhat) >= new Date(filters.FilterStartUpdateDate)) &&
          (!filters.FilterEndUpdateDate || new Date(tin.ngayCapNhat) <= new Date(filters.FilterEndUpdateDate)) &&
          (filters.FilterStartComment === null || (tin.soLuongComment ?? 0) >= filters.FilterStartComment) &&
          (filters.FilterEndComment === null || (tin.soLuongComment ?? 0) <= filters.FilterEndComment);
      });
      this.tintucView = this.tintucFilter;
      console.log(this.tintucFilter);
      this.showFilterOptions = false;
    }

    //Nếu chưa gõ từ khóa tìm kiếm thì lọc tất cả dữ liệu Tin tức
    else {
      this.tintucFilter = this.tintucs.filter(tin => {
        return (!filters.FilterCategory || tin.danhmucId === +filters.FilterCategory) &&
          (!filters.FilterStartDate || new Date(tin.ngayDang) >= new Date(filters.FilterStartDate)) &&
          (!filters.FilterEndDate || new Date(tin.ngayDang) <= new Date(filters.FilterEndDate)) &&
          (!filters.FilterStartUpdateDate || new Date(tin.ngayCapNhat) >= new Date(filters.FilterStartUpdateDate)) &&
          (!filters.FilterEndUpdateDate || new Date(tin.ngayCapNhat) <= new Date(filters.FilterEndUpdateDate)) &&
          (filters.FilterStartComment === null || (tin.soLuongComment ?? 0) >= filters.FilterStartComment) &&
          (filters.FilterEndComment === null || (tin.soLuongComment ?? 0) <= filters.FilterEndComment);
      });
      this.tintucView = this.tintucFilter;
      console.log(this.tintucFilter);
      this.showFilterOptions = false;
    }
  }

  

  onSearchChange() {
    this.showSearchTintucs = true;
    if (this.searchTerm && this.searchTerm.trim()) {
      const searchTermNoAccent = this.removeVietnameseAccent(this.searchTerm.toLowerCase());
      
      this.tintucSearch = this.tintucs.filter(tintuc =>
        this.removeVietnameseAccent(tintuc.tieuDe.toLowerCase()).includes(searchTermNoAccent)
      );
    } else {
      this.tintucSearch = []; // Nếu không có từ khóa, trả về mảng rỗng
    }
  }
  
  searchTintuc(searchTerm: string) {
    const formValues = this.filterForm.value;
    this.tintucSearch = [];

    const hasValue = Object.values(formValues).some(value => value !== null && value !== '');
      if (hasValue) {
        
        if (searchTerm && searchTerm.trim()) {
          const searchTermNoAccent = this.removeVietnameseAccent(searchTerm.toLowerCase());
          
          this.tintucView = this.tintucFilter.filter(tintuc =>
            this.removeVietnameseAccent(tintuc.tieuDe.toLowerCase()).includes(searchTermNoAccent)
          );

        } else {
          this.tintucView = this.tintucView; // Nếu không có từ khóa, trả về mảng rỗng
        }
      } 
      else {
            // Trường không có giá trị
        if (searchTerm && searchTerm.trim()) {
          const searchTermNoAccent = this.removeVietnameseAccent(searchTerm.toLowerCase());
          
          this.tintucView = this.tintucs.filter(tintuc =>
            this.removeVietnameseAccent(tintuc.tieuDe.toLowerCase()).includes(searchTermNoAccent)
          );

          
          
        } else {
          this.tintucView = this.tintucView // Nếu không có từ khóa, trả về mảng rỗng
        }
      }
    }
    


  

  removeVietnameseAccent(str: string): string {
    const accents = 'àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũûừứựửữỳýỵỷỹ';
    const withoutAccents = 'aaaaaaaaaaAAAAAAAAAeeeeeeeeeeiiiiiooooooooooooOOOOOOOOOOouuuuuuuuuuuuuuuyyyyy';
    
    return str.split('').map(char => {
      const index = accents.indexOf(char);
      return index !== -1 ? withoutAccents[index] : char;
    }).join('').toLowerCase();
  }

  hideForm() {
    
    
    this.showInfoTintucs = false;
    this.showFilterOptions = false;
    this.showSearchTintucs = false;
    this.selectTab('thong-tin');
    this.postForm.reset(); // Đặt lại form
  }


  //Chọn data cần edit
  selectedEdit(id: number){
    this.route.navigate(['admin/ManagePost/EditPost', id]);
  }

  SelectedAddForm() {
    this.route.navigate(['admin/ManagePost/AddPost']);
  }

  selectedInfo(id: number) {
      this.showInfoTintucs = true;
      this.tintucSearch = [];

    // Lấy API TinTuc/id 
    this.tintucservice.getTintucById(id).subscribe(
      (data) => {
        console.log("HAAAA");
        this.selectedTintuc = data; 
        console.log(this.selectedTintuc);
      }
    );
  }

  SelectStatus(id: number, event: Event) {
    this.loading = true;
    const checkbox = event.target as HTMLInputElement;
    this.tintucservice.updateStatus(id, checkbox.checked).subscribe(
      (response) => {
        this.tintucView = response;
        this.tintucs = response;
        
        setTimeout(() => {
          this.loading = false;
        }, 500);
        console.log('Cập nhật thành công', response);  
      },
      (error) => {
          console.error("Có lỗi xảy ra:", error); // In ra lỗi nếu có
      }
    ); 
  }

 


  async delete(id: number) {
    let DataRemove = this.tintucs.find((data) => data.tintucId == id) || null;
    let TieuDe: string;
    if (DataRemove) {
        TieuDe = DataRemove.tieuDe;        // Nếu tìm thấy, gán tiêu đề
    } else {
        TieuDe = "Tiêu đề không tìm thấy"; // Nếu không tìm thấy, gán giá trị mặc định
    }
    //Đưa hành động này đến bảng thông báo dashboard
    await this.dashboardservice.NotificationRemoveTinTuc(TieuDe);
    
    //Xóa bài tin tức đó
    this.tintucs = await firstValueFrom(this.tintucservice.deleteTintuc(id));
    this.route.navigate(['/success']);
}

  getFullImageUrl(imageUrl: string): string {
    return `${this.Url}${imageUrl}`;
  }

  ngOnInit(): void {
    forkJoin({
      tintucAPI: this.tintucservice.getTintuc(),
      danhmucAPI: this.danhmucservice.GetDanhmuc(),
    }).subscribe({
      next: (result) => {
        // Xử lý kết quả từ cả hai API
        this.tintucs = result.tintucAPI;
        this.tintucView = this.tintucs;  // Giả sử bạn cần cập nhật `tintucView`
        this.danhmucs = result.danhmucAPI;

        console.log('Tin tức:', this.tintucs);
        console.log('Danh mục:', this.danhmucs);

        // Chờ ít nhất 1 giây để hiển thị dữ liệu
        setTimeout(() => {
          this.loading = false;
        }, 500);
      },
      error: (error) => {
        console.error('Lỗi khi gọi API', error);
        this.loading = false;  // Tắt loading trong trường hợp lỗi
      },
    })


    this.initializeForm();

  }
}
