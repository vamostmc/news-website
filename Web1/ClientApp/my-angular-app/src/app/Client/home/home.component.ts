import { AfterViewInit, Component, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { AuthenService } from '../service-client/authen-service/authen.service';
import { Tintuc } from '../models/tintuc';
import { TinTucService } from '../service-client/tintuc-service/tin-tuc.service';
import { DanhmucService } from '../service-client/danhmuc-service/danhmuc.service';
import { Danhmuc } from '../models/danhmuc';
import { ActivatedRoute, Router } from '@angular/router';
import { error } from 'pdf-lib';
import { TrendingViewComponent } from './trending-view/trending-view.component';
import { ViewDetailComponent } from './view-detail/view-detail.component';
import { environment } from '../../../environments/environment.development';
import { catchError, finalize, of, timeout } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
   @ViewChild(TrendingViewComponent) trendingView!: TrendingViewComponent;
   @ViewChild(ViewDetailComponent) viewDetail!: ViewDetailComponent;

  weatherData: any;
  searchVisible = false;
  searchQuery = '';
  tintucView: Tintuc[] = [];
  tintucProminent: Tintuc[] = [];
  tintucTop: Tintuc[] = [];
  danhmucView: Danhmuc[] = [];
  danhs: Danhmuc[] = [];
  currentPage: number = 1;  // Trang hiện tại
  itemsPerPage: number = 4; // Số bài viết mỗi trang
  selectedCategory: number = 1; // Mặc định chọn Công nghệ
  loading: boolean = false;
  today: Date = new Date();
  dayOfWeek: string = '';
  formattedDate: string = '';
  fullName: string | null = null;
  blockUser: boolean = false;
  
  
  categories = [
    { id: 1, name: 'Công nghệ' },
    { id: 2, name: 'Thể thao' },
    { id: 3, name: 'Giải trí' },
    { id: 5, name: 'Đời sống' }
  ];
  
  
  constructor( private authen: AuthenService,
               private tintucService: TinTucService,
               private danhmucservice: DanhmucService,
               private route: ActivatedRoute,
               private router: Router,
               private http: HttpClient
  ) {}
  

  private aws_URL = environment.awsUrl;
  
  ngOnInit(): void {
    console.log("Component Home khởi chạy!");
    this.loading = true;
    setTimeout(() => {
      this.getDataNews();
    }, 800);
    
  }

  updateTinTuc() {
    if (this.viewDetail) {
      this.viewDetail.tintucTop = this.tintucTop;
    } else {
      console.warn("⚠ viewDetail chưa được khởi tạo.");
    }
  }

  getDataNews() {
    console.log("Gọi API getDataNews...");
    this.tintucService.getTintuc().pipe(
      finalize(() => {
        this.loading = false; // Tắt loading sau khi API chạy xong 
      })
    ).subscribe(
      (data) => {
        console.log("Dữ liệu API trả về:", data);
        this.tintucView = data;
        this.tintucService.setData(this.tintucView);
        this.filterNews(1);
      },
      (error) => {
        if(error.status == 403) {
          localStorage.clear();
          this.blockUser = true;
          console.log(this.blockUser)
        }
        
      }
    );
     
  }

  filterNews(IdDanhMuc: number) {
    this.selectedCategory = IdDanhMuc; // Cập nhật danh mục đang chọn
    if (!this.tintucView) return;
    this.tintucProminent = this.tintucView.filter(x => x.danhmucId === IdDanhMuc);
    this.currentPage = 1;
    console.log("Tin tức nổi bật", this.tintucProminent);
  }

    // Tính tổng số trang
  get totalPages(): number {
    return Math.ceil(this.tintucProminent.length / this.itemsPerPage);
  }

  // Lấy danh sách bài viết theo trang
  get paginatedNews(): any[] {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    return this.tintucProminent.slice(startIndex, startIndex + this.itemsPerPage);
  }

  // Chuyển đến trang tiếp theo
  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  // Quay về trang trước
  prevPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  // Chuyển đến một trang cụ thể
  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }


  getFullImageUrl(imageUrl: string): string {
    return `${this.aws_URL}/${imageUrl}`;
  }

  toggleSearch() {
    this.searchVisible = !this.searchVisible;
    console.log(this.searchVisible);
  }

  updateSearchQuery(event: any) {
    this.searchQuery = event.target.innerText;
  }
}



