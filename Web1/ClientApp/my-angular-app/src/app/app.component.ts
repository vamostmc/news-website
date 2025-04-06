import { Component, OnInit } from '@angular/core';
import { AfterViewInit } from '@angular/core'; 
import { SignalRService } from './Client/service-client/signalR-service/signal-r.service';


declare var tinymce: any;
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})

export class AppComponent implements OnInit  {
  title = 'my-angular-app';
  
  constructor() {}
  

  ngOnInit(): void {
    
  }
}

