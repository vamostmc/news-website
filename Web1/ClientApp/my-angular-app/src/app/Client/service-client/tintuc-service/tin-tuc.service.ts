import { Injectable } from '@angular/core';
import { Tintuc } from '../../models/tintuc';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../../../constants/api-endpoints.ts';
@Injectable({
  providedIn: 'root'
})
export class TinTucService {

  constructor(private http: HttpClient) {}

  private tintucs: Tintuc[] = [];

  setData(value: any) {
    this.tintucs = value;
  }

  getData() {
    return this.tintucs;
  }

  getTintuc(): Observable<Tintuc[]> {
    return this.http.get<Tintuc[]>(API_ENDPOINTS.tinTuc.manage, { withCredentials: true });
  }

  getTintucById(id: number): Observable<Tintuc> {
    return this.http.get<Tintuc>(`${API_ENDPOINTS.tinTuc.base}/${id}`);
  }

  deleteTintuc(id: number): Observable<Tintuc[]> {
    return this.http.delete<Tintuc[]>(API_ENDPOINTS.tinTuc.delete(id));
  }

  addTintuc(newTintuc: FormData): Observable<Tintuc> {
    return this.http.post<Tintuc>(API_ENDPOINTS.tinTuc.add, newTintuc, 
                                 { withCredentials: true });
  }

  updateTintuc(TintucEdit: any, id: number): Observable<Tintuc> {
    return this.http.put<Tintuc>(API_ENDPOINTS.tinTuc.update(id), TintucEdit, 
                                { withCredentials: true });
  }

  updateStatus(id: number, trangThai: boolean): Observable<Tintuc[]> {
    return this.http.put<Tintuc[]>(API_ENDPOINTS.tinTuc.updateStatus(id),trangThai, 
                                  { withCredentials: true });
  }

  ImportExcel(FileExcel: FormData): Observable<any> {
    return this.http.post<any>(API_ENDPOINTS.tinTuc.importExcel, FileExcel, 
                              { withCredentials: true });
  }

  DownloadTempExcel(): Observable<Blob> {
    return this.http.get(API_ENDPOINTS.tinTuc.downloadExcelTemplate, { responseType: 'blob' });
  }
}
