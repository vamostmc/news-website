import { Injectable } from '@angular/core';
import { Tintuc } from '../../models/tintuc';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class TinTucService {
  
  private apiUrl = 'https://localhost:7233/api/TinTuc';
  private ManageTinUrl = 'https://localhost:7233/api/TinTuc/TintucWithDanhmuc';
  private DeleteUrl = 'https://localhost:7233/api/TinTuc/Delete';
  private PostUrl = 'https://localhost:7233/api/TinTuc/ThemTinTuc';
  private UpdateUrl = 'https://localhost:7233/api/TinTuc/Edit';
  private UpdateStatus = 'https://localhost:7233/api/TinTuc/UpdateStatus';
  

  constructor(private http: HttpClient) {}

  getTintuc(): Observable<Tintuc[]> {
    return this.http.get<Tintuc[]>(this.ManageTinUrl);
    
  }

  getTintucById(id: number): Observable<Tintuc> {
    const GetByIdUrl = `${this.apiUrl}/${id}`;
    return this.http.get<Tintuc>(GetByIdUrl);
  }

  deleteTintuc(id: number): Observable<Tintuc[]> {
    const deleteURL = `${this.DeleteUrl}/${id}`;
    return this.http.delete<Tintuc[]>(deleteURL);
  }

  addTintuc(newTintuc : FormData): Observable<Tintuc> {
      return this.http.post<Tintuc>(this.PostUrl, newTintuc);
  }

  updateTintuc(TintucEdit: any, id: number): Observable<Tintuc> {
    const EditUrl = `${this.UpdateUrl}/${id}`;
    return this.http.put<Tintuc>(EditUrl, TintucEdit);
  }

  updateStatus(id: number, trangThai: boolean): Observable<Tintuc[]> {
    const StatusUrl = `${this.UpdateStatus}/${id}`;
    console.log(`Trạng thái là: ${trangThai}`);
    return this.http.put<Tintuc[]>(StatusUrl,trangThai);
  }
}
