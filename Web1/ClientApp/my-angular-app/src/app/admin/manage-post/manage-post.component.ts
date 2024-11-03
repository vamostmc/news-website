import { Component, OnInit } from '@angular/core';
import { Tintuc } from '../../Client/models/tintuc';
import { TinTucService } from '../../Client/service-client/tintuc-service/tin-tuc.service';
import { Router } from '@angular/router';
import { DanhmucService } from '../../Client/service-client/danhmuc-service/danhmuc.service';
import { Danhmuc } from '../../Client/models/danhmuc';
import { forkJoin } from 'rxjs';


@Component({
  selector: 'app-manage-post',
  templateUrl: './manage-post.component.html',
  styleUrl: './manage-post.component.css'
})
export class ManagePostComponent implements OnInit {
  
  constructor(private tintucservice: TinTucService, 
              private route: Router,
              private danhmucservice: DanhmucService, 
              ) {}

  tintucs: Tintuc[] = [];
  danhmucs: Danhmuc[] = [];
  loading: boolean = true;

  // Nhấn vào thêm tin tức
  SelectedAddForm() {
    this.route.navigate(['admin/ManagePost/AddPost']);
  }

  ngOnInit(): void {
    forkJoin({
      tintucAPI: this.tintucservice.getTintuc(),
      danhmucAPI: this.danhmucservice.GetDanhmuc(),
    }).subscribe({
      next: (result) => {
        // Xử lý kết quả từ cả hai API
        this.tintucs = result.tintucAPI;
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
  }
}
