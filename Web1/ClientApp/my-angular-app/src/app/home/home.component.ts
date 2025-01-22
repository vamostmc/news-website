import { Component, OnInit } from '@angular/core';
import { AuthenService } from '../Client/service-client/authen-service/authen.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  weatherData: any;
  constructor( private authen: AuthenService) {

  }

  ngOnInit(): void {
    
  }

  fetchWeather(): void {
    this.authen.getWeatherForecast().subscribe(
      (data) => {
        this.weatherData = data;
        console.log('Weather data:', data); // Hiển thị dữ liệu trong console
      }, 
    );
  }
}
