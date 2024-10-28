import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { roles } from '../../models/role';

@Injectable({
  providedIn: 'root'
})
export class AuthenService {

  constructor() { }
  private userNameSource = new BehaviorSubject<string | null>(null);
  currentUserName = this.userNameSource.asObservable();

  setUserName(userName: string) {
    this.userNameSource.next(userName);
    localStorage.setItem('userName', userName);
  }

  getUserName(): string | null {
    return localStorage.getItem('userName');
  }

  isLoggedIn(): boolean {
    // Kiểm tra trạng thái đăng nhập, có thể thông qua token hoặc localStorage
    console.log(`Role khi login la: ${this.GetRoleUser()}`);
    return !!(localStorage.getItem('keyUser') && this.GetRoleUser() == "Manager");
  }
  
  //Giải mã token
  GetRoleUser(): any {
    let ListToken : [];
    const token = localStorage.getItem("keyUser");
    if(token)
    {
      //Mã hóa chuỗi token trả về từ API
      const decodeToken = jwtDecode(token);

      //Chuyển JWTPayLoad sang List dạng {key: '' , value: ''}
      const jwtList = Object.entries(decodeToken).map(([key, value]) => {
        return { key, value };});
      const roleKey = jwtList.find(item => item.key.toLowerCase().includes('role'));
      
      console.log(`${JSON.stringify(roleKey?.value)}`);
      return roleKey?.value;
    }
  }
}
