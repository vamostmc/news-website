import { Component, OnInit } from '@angular/core';
import { AfterViewInit } from '@angular/core'; 


declare var tinymce: any;
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})

export class AppComponent implements OnInit  {
  title = 'my-angular-app';
  
  constructor() {}
  ngAfterViewInit() {
    tinymce.init({
      selector: '#editor',
      plugins: 'autolink lists link image charmap print preview anchor',
      toolbar: 'undo redo | formatselect | bold italic backcolor | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent',
    });
  }

  ngOnInit(): void {
    
  }
}

