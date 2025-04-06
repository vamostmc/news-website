import { Component, Input, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { Tintuc } from '../../../Client/models/tintuc';
import { TinTucService } from '../../../Client/service-client/tintuc-service/tin-tuc.service'; 
import { Router } from '@angular/router';
import { Danhmuc } from '../../../Client/models/danhmuc';
import { DashboardService } from '../../../Client/service-client/dashboard-service/dashboard.service';
import { firstValueFrom, forkJoin, switchMap } from 'rxjs';
import { Location } from '@angular/common';
import { SearchFilterService } from '../../../Client/service-client/search-filter-service/search-filter.service';
import { environment } from '../../../../environments/environment.development';

@Component({
  selector: 'app-show-post',
  templateUrl: './show-post.component.html',
  styleUrl: './show-post.component.css'
})
export class ShowPostComponent implements OnInit  {
    constructor(private tintucservice: TinTucService, 
              private route: Router,
              private dashboardservice: DashboardService,
              private sf: SearchFilterService) {}
  
  //Nhận dữ liệu từ cha ManagePost
  @Input() tintucs: Tintuc[] = [];  

  danhmucs: Danhmuc[] = [];
  selectedTintuc: Tintuc | null = null;
  idSelectedSearch: number | null = null;

  showInfoTintucs = false;
  selectedTab: string = 'thong-tin';

  tintucView: Tintuc[] = [];
  loading: boolean = true;

  private aws_URL = environment.awsUrl;

  p: number = 1; // Biến để lưu số trang hiện tại
  onPageChange(page: number) {
    this.p = page;
  }

  //Lấy đường dẫn hình ảnh
  getFullImageUrl(imageUrl: string): string {
    return `${this.aws_URL}/${imageUrl}`;
  }

  
  //Chuyển sang tab khác trong form
  selectTab(tab: string) {
    this.selectedTab = tab;
  }

  // Ẩn form khi xem chi tiết
  hideForm() {
    this.showInfoTintucs = false;
    this.selectTab('thong-tin');
  
  }


  //Chọn data cần edit
  selectedEdit(id: number){
    this.route.navigate(['admin/ManagePost/EditPost', id]);
  }

  //Thao tác khi chọn tin tức chi tiết theo id
  selectedInfo(id: number) {
      this.showInfoTintucs = true;
      

    // Lấy API TinTuc/id 
    this.tintucservice.getTintucById(id).subscribe(
      (data) => {
        console.log("HAAAA");
        
        this.selectedTintuc = data; 
        console.log(this.selectedTintuc);
      }
    );
  }

  //Cập nhật lại trạng thái
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

 //Chức năng xóa
  async delete(id: number) {
    let DataRemove = this.tintucs.find((data) => data.tintucId == id) || null;
    let TieuDe: string;
    if (DataRemove) {
        TieuDe = DataRemove.tieuDe;        // Nếu tìm thấy, gán tiêu đề
    } else {
        TieuDe = "Tiêu đề không tìm thấy"; // Nếu không tìm thấy, gán giá trị mặc định
    }
    //Đưa hành động này đến bảng thông báo dashboard
    // await this.dashboardservice.NotificationRemoveTinTuc(TieuDe);
    
    //Xóa bài tin tức đó
    this.tintucs = await firstValueFrom(this.tintucservice.deleteTintuc(id));
    this.route.navigate(['/success']);
  }


  ngOnChanges(changes: SimpleChanges) {
    if (changes['tintucs'] && changes['tintucs'].currentValue) {
      this.tintucView = this.tintucs;
    }
  }


  ngOnInit(): void {
    if(this.tintucs && this.tintucs.length > 0)
      { this.tintucView = this.tintucs; }
    

    this.sf.tintucView$.subscribe(data => {
      this.tintucView = data;
      console.log('tintucViewChild cập nhật:', this.tintucView);
    });

    this.sf.tintucId$.subscribe(data => {
      this.idSelectedSearch = data;
      if(this.idSelectedSearch) {
        this.selectedInfo(this.idSelectedSearch);
      }
    })

  }
}
