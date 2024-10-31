import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { Tintuc } from '../../models/tintuc';

@Injectable({
  providedIn: 'root'
})
export class SearchFilterService {

  private tintucViewSubject = new BehaviorSubject<Tintuc[]>([]);
  private idSubject = new BehaviorSubject<number| null>(null);

  tintucView$ = this.tintucViewSubject.asObservable();
  tintucId$ = this.idSubject.asObservable();

  //Truyền giá trị tin tức tìm được khi search
  updateTintucView(data: any) {
    this.tintucViewSubject.next(data);
    console.log(this.tintucView$);
  }

  //Truyền giá trị khi chọn vào tin tức id để hiển thị
  selectTinTucsearchId(id: number) {
    this.idSubject.next(id);
    
  }


  
  
  constructor() { }
}
