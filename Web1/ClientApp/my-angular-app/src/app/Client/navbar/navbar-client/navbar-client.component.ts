import { Component, OnInit } from '@angular/core';
import { AuthenService } from '../../service-client/authen-service/authen.service';
import { TinTucService } from '../../service-client/tintuc-service/tin-tuc.service';
import { DanhmucService } from '../../service-client/danhmuc-service/danhmuc.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-navbar-client',
  templateUrl: './navbar-client.component.html',
  styleUrl: './navbar-client.component.css'
})
export class NavbarClientComponent implements OnInit {
  today: Date = new Date();
  dayOfWeek: string = '';
  formattedDate: string = '';
  fullName: string | null = null;

  constructor(
    private authen: AuthenService,
    private tintucService: TinTucService,
    private danhmucservice: DanhmucService,
    private route: ActivatedRoute,
  ) {

  }

  ngOnInit(): void {
    this.getTodayInfo();
    this.fullName = localStorage.getItem('fullName');
  }
  getTodayInfo() {
    const daysOfWeek = ['Chủ Nhật', 'Thứ Hai', 'Thứ Ba', 'Thứ Tư', 'Thứ Năm', 'Thứ Sáu', 'Thứ Bảy'];
    this.dayOfWeek = daysOfWeek[this.today.getDay()];

    const date = this.today.getDate();
    const month = this.today.getMonth() + 1;
    const year = this.today.getFullYear();
    
    this.formattedDate = `${date}/${month}/${year}`;
  }
}
