import { Component, OnInit, HostListener } from '@angular/core';
import { ChangeDetectorRef } from '@angular/core';
import { AuthenService } from '../service-client/authen-service/authen.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements OnInit {
    constructor(private changeDetectorRef: ChangeDetectorRef,
                private authService: AuthenService,
                private router: Router) {}

    isDropdownOpen = false;
    userRole = ''; // Giả sử bạn đã lưu thông tin role của user
  
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
        // Xóa token và thông tin người dùng khỏi localStorage hoặc sessionStorage
        localStorage.removeItem('keyUser');
        localStorage.removeItem('userName');
        this.username = ''; // Xóa thông tin username
        window.location.reload();
        this.router.navigate(['/']); // Chuyển hướng về trang login
    }

    ngOnInit(): void {
      console.log(`Click : ${this.isDropdownOpen}`);
      this.authService.currentUserName.subscribe(userName => {
        this.username = userName;
        this.username = localStorage.getItem("userName");
        this.userRole = this.authService.GetRoleUser();
      });
      
      
      
    }
}
