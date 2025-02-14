import { Component, HostListener, OnInit } from '@angular/core';
import { AuthenService } from '../../service-client/authen-service/authen.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar-authen',
  templateUrl: './navbar-authen.component.html',
  styleUrl: './navbar-authen.component.css'
})
export class NavbarAuthenComponent implements OnInit {
constructor(
                private authService: AuthenService,
                private router: Router) {}

    isDropdownOpen = false;
    userRole = ''; // Giả sử bạn đã lưu thông tin role của user
    today: Date = new Date();
    dayOfWeek: string = '';
    formattedDate: string = '';
  
    // Toggle the menu when avatar is clicked
    toggleMenu(event: MouseEvent) {
      this.isDropdownOpen = !this.isDropdownOpen;
      event.stopPropagation(); // Ngăn không cho sự kiện click lan ra ngoài
    }

    @HostListener('document:click', ['$event'])
    onClickOutside(event: MouseEvent) {
    if (this.isDropdownOpen) {
      this.isDropdownOpen = false; // Đóng menu nếu nhấn ra ngoài
    }
  }

    getUserRole() {
      // Lấy thông tin role từ token hoặc từ local storage
      this.userRole = 'Customer'; // Giả sử bạn đã có thông tin role của người dùng
    }

    username : string | null = null;
    checkUser(): boolean {
      const tokenUser = localStorage.getItem("userName");
      if(tokenUser != null) {
        return true;
      }
      return false;
    }

    logOut() {
        
        this.authService.LogOut().subscribe(
          (data) => {
            console.log(data);
            localStorage.removeItem('accessToken');
            localStorage.removeItem('confirmEmail');
            localStorage.removeItem('userName');
            localStorage.removeItem('fullName');
            localStorage.removeItem('userRoles');
            this.username = ''; // Xóa thông tin username
            window.location.reload();
            this.router.navigate(['/']);
          }
        );
        
    }

    getTodayInfo() {
      const daysOfWeek = ['Chủ Nhật', 'Thứ Hai', 'Thứ Ba', 'Thứ Tư', 'Thứ Năm', 'Thứ Sáu', 'Thứ Bảy'];
      this.dayOfWeek = daysOfWeek[this.today.getDay()];
  
      const date = this.today.getDate();
      const month = this.today.getMonth() + 1;
      const year = this.today.getFullYear();
      
      this.formattedDate = `${date}/${month}/${year}`;
    }

    ngOnInit(): void {
      console.log(`Click : ${this.isDropdownOpen}`);
      this.authService.currentUserName.subscribe(userName => {
        this.username = userName;
        this.username = localStorage.getItem("fullName");
        this.userRole = this.authService.GetRoleUser();
      });
      this.getTodayInfo();
    }
}
