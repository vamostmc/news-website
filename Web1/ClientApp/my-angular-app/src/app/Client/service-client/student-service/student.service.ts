
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Student } from '../../models/student'; // Import class Student

@Injectable({
  providedIn: 'root'
})
export class StudentService {
  private apiUrl = 'https://localhost:7233/api/Students';

  students: Student[] = [];

  constructor(private http: HttpClient) {}

  getStudents(): Observable<Student[]> {
    return this.http.get<Student[]>(this.apiUrl);
  }

  deleteStudent(maSV: number) {
    const url = `${this.apiUrl}/${maSV}`;
    this.http.delete(url).subscribe({
      next: (response) => {
        console.log('Xóa sinh viên thành công:', response);
        // Cập nhật lại danh sách sinh viên sau khi xóa
        this.students = this.students.filter(student => student.maSV !== maSV);
      },
      error: (error) => {
        console.error('Có lỗi khi xóa sinh viên:', error);
        // Có thể hiển thị thông báo lỗi cho người dùng
      }
    });
  }


}
