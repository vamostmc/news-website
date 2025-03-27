import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-toast-message',
  templateUrl: './toast-message.component.html',
  styleUrl: './toast-message.component.css'
})
export class ToastMessageComponent implements OnInit {
  @Input() blockUser : boolean = false;
  constructor(private router: Router ) {}

  
  goLogin() {
    this.router.navigate(['/']);
  }

  ngOnInit(): void {
    
  }
}
