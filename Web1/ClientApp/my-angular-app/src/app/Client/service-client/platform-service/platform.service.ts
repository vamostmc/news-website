import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class PlatformService {
  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  //Kiểm tra xem đang chạy trên trình duyệt hay không
  isBrowser(): boolean {
    return isPlatformBrowser(this.platformId);
  }
}
