import { Component, OnInit } from '@angular/core';
import { AuthenService } from '../../Client/service-client/authen-service/authen.service';

@Component({
  selector: 'app-navbar-admin',
  templateUrl: './navbar-admin.component.html',
  styleUrl: './navbar-admin.component.css'
})
export class NavbarAdminComponent implements OnInit {
    constructor(private authen: AuthenService) {}
    userNameAdmin : string | null = null;


    
    ngOnInit(): void {
      this.userNameAdmin =  this.authen.getUserName();
    }
}
