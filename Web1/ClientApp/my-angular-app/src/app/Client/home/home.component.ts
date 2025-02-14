import { AfterViewInit, Component, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { AuthenService } from '../service-client/authen-service/authen.service';
import { Tintuc } from '../models/tintuc';
import { TinTucService } from '../service-client/tintuc-service/tin-tuc.service';
import { DanhmucService } from '../service-client/danhmuc-service/danhmuc.service';
import { Danhmuc } from '../models/danhmuc';
import { ActivatedRoute } from '@angular/router';
import { error } from 'pdf-lib';
import { TrendingViewComponent } from './trending-view/trending-view.component';
import { ViewDetailComponent } from '../view-detail/view-detail.component';


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
               
  ) {}
  

  private Url = 'https://localhost:7233';

  ngOnInit(): void {
    console.log("Component Home khởi chạy!");
    this.loading = true;
    // this.getDataNews();
    setTimeout(() => {
      this.getDataNews();
      this.loading = false;
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
    this.tintucService.getTintuc().subscribe(
      (data) => {
        console.log("Dữ liệu API trả về:", data);
        this.tintucView = data;
        this.tintucService.setData(this.tintucView);
        this.filterNews(1);
      },
      (error) => {
        console.error("Lỗi khi gọi API:", error);
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
    return `${this.Url}${imageUrl}`;
  }

  toggleSearch() {
    this.searchVisible = !this.searchVisible;
    console.log(this.searchVisible);
  }

  updateSearchQuery(event: any) {
    this.searchQuery = event.target.innerText;
  }

  fetchWeather(): void {
    this.authen.getWeatherForecast().subscribe(
      (data) => {
        this.weatherData = data;
        console.log('Weather data:', data); // Hiển thị dữ liệu trong console
      }, 
      (error) => {
        console.error("Lỗi khi gọi API:", error);
      }
    );
  }
}
