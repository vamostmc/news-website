import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { Tintuc } from '../../../Client/models/tintuc';
import { TinTucService } from '../../../Client/service-client/tintuc-service/tin-tuc.service';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators  } from '@angular/forms';
import { DanhmucService } from '../../../Client/service-client/danhmuc-service/danhmuc.service';
import { Danhmuc } from '../../../Client/models/danhmuc';
import { FormsModule } from '@angular/forms';
import { DashboardService } from '../../../Client/service-client/dashboard-service/dashboard.service';
import { firstValueFrom, forkJoin, switchMap } from 'rxjs';
import { EditPostComponent } from '../edit-post/edit-post.component';
import { Location } from '@angular/common';
import { ShowPostComponent } from '../show-post/show-post.component';
import { SearchFilterService } from '../../../Client/service-client/search-filter-service/search-filter.service';


@Component({
  selector: 'app-search-filter-post',
  templateUrl: './search-filter-post.component.html',
  styleUrl: './search-filter-post.component.css'
})
export class SearchFilterPostComponent {
  constructor(private tintucservice: TinTucService, 
              private http: HttpClient, 
              private route: Router,
              private fb: FormBuilder,
              private sf: SearchFilterService,
              private location: Location) {}
  
  //Nhận dữ liệu tin tức từ cha
  @Input() tintucs:Tintuc[] = [];
  @Input() danhmucs: Danhmuc[] = [];

  selectedTintuc: Tintuc | null = null;
  filterForm!: FormGroup;
  
  showFilterOptions = false;
  showSearchTintucs = false;
  
  isSearchVisible = false;    // Biến kiểm soát hiển thị trường tìm kiếm
  isFilterVisible = false;    // Biến kiểm soát hiển thị bộ lọc
  searchQuery: string = '';   // Lưu chuỗi tìm kiếm
  selectedCategory: string = '';  // Lưu giá trị của bộ lọc danh mục
  
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

  private Url = 'https://localhost:7233';
  
  //Lấy đường dẫn để hiển thị hình ảnh
  getFullImageUrl(imageUrl: string): string {
    return `${this.Url}${imageUrl}`;
  }

  //Sự kiện với button bộ lọc
  toggleFilter() {
    this.showFilterOptions = !this.showFilterOptions; // Chuyển đổi hiển thị
  }

  //Hiển thị số trường đã chọn trong bộ lọc
  updateFilterCount() {
    this.filterCount = Object.values(this.filterForm.value).filter(value => value).length;
  }
  
  //Reset lại filter
  resetFilters() {
    this.filterForm.reset();
    this.sf.updateTintucView(this.tintucs);
    this.filterCount = 0; // Reset bộ đếm
  }
  

  initializeForm() {
    this.filterForm = this.fb.group({
      FilterCategory: [''],
      FilterStartDate: [null],
      FilterEndDate: [null],
      FilterStartUpdateDate: [null],
      FilterEndUpdateDate: [null],     
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
      this.sf.updateTintucView(this.tintucView);
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
      this.sf.updateTintucView(this.tintucView);
      console.log(this.tintucFilter);
      this.showFilterOptions = false;
    }
  }

  
  //Tìm từ khóa khi nhập vào input
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
          this.sf.updateTintucView(this.tintucView);

        } else {
          this.tintucView = this.tintucView; // Nếu không có từ khóa, trả về mảng rỗng
          this.sf.updateTintucView(this.tintucView);
        }
      } 
      else {
            // Trường không có giá trị
        if (searchTerm && searchTerm.trim()) {
          const searchTermNoAccent = this.removeVietnameseAccent(searchTerm.toLowerCase());
          
          this.tintucView = this.tintucs.filter(tintuc =>
            this.removeVietnameseAccent(tintuc.tieuDe.toLowerCase()).includes(searchTermNoAccent)
          );
          this.sf.updateTintucView(this.tintucView);

        } else {
          this.tintucView = this.tintucView // Nếu không có từ khóa, trả về mảng rỗng
          this.sf.updateTintucView(this.tintucView);
        }
      }
    }
    


  
  //Đưa các chữ dạng VietNam về dạng thường 
  removeVietnameseAccent(str: string): string {
    const accents = 'àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũûừứựửữỳýỵỷỹ';
    const withoutAccents = 'aaaaaaaaaaAAAAAAAAAeeeeeeeeeeiiiiiooooooooooooOOOOOOOOOOouuuuuuuuuuuuuuuyyyyy';
    
    return str.split('').map(char => {
      const index = accents.indexOf(char);
      return index !== -1 ? withoutAccents[index] : char;
    }).join('').toLowerCase();
  }

  //Ẩn bộ lọc và kết quả tìm kiếm
  hideForm() {
    this.showFilterOptions = false;
    this.showSearchTintucs = false;
    
  }

  //Khi select vào tin tức id trong kết quả search
  getSearchId(id: number) {
    this.tintucSearch = [];
    this.sf.selectTinTucsearchId(id);
    
  }


  //Theo dõi bên component cha khi list danh sách tin tức truyền vào có thay đổi không 
  ngOnChanges(changes: SimpleChanges) {
    if (changes['tintucs'] && changes['tintucs'].currentValue) {
      this.tintucView = this.tintucs;
    }
  }

  ngOnInit(): void {
    console.log("New tin");
    console.log(this.tintucs);
    this.initializeForm();

  }
}
