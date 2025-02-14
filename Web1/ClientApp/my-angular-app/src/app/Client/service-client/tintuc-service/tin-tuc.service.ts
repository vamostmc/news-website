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
  private NewsUrl = 'https://localhost:7233/api/TinTuc';
  private DeleteUrl = 'https://localhost:7233/api/TinTuc/Delete';
  private PostUrl = 'https://localhost:7233/api/TinTuc/ThemTinTuc';
  private UpdateUrl = 'https://localhost:7233/api/TinTuc/Edit';
  private UpdateStatus = 'https://localhost:7233/api/TinTuc/UpdateStatus';
  private ExcelUrl = "https://localhost:7233/api/TinTuc/Import-Excel";
  private DownloadExcelUrl = "https://localhost:7233/api/TinTuc/Template-Excel";

  constructor(private http: HttpClient) {}

  private tintucs: Tintuc[] = [];

  setData(value: any) {
    this.tintucs = value;
  }

  getData() {
    return this.tintucs;
  }

  getTintuc(): Observable<Tintuc[]> {
    return this.http.get<Tintuc[]>(this.ManageTinUrl, { withCredentials: true });
  }

  getNewTintuc(): Observable<Tintuc[]> {
    return this.http.get<Tintuc[]>(this.NewsUrl);
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

  ImportExcel(FileExcel: FormData): Observable<any> {
    return this.http.post<any>(this.ExcelUrl,FileExcel);
  }

  DownloadTempExcel(): Observable<Blob> {
    return this.http.get(this.DownloadExcelUrl, { responseType: 'blob' });
  }
}
