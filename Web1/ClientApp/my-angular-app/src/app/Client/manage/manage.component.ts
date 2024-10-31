import { Component, OnInit } from '@angular/core';
import { Student } from '../models/student';
import { AuthenService } from '../service-client/authen-service/authen.service';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-manage',
  templateUrl: './manage.component.html',
  styleUrl: './manage.component.css'
})
export class ManageComponent implements OnInit {
  students: Student[] = [];
  selectedStudent: Student | null =  null;
  newStudent: Student | null = null;

  private apiUrl = 'https://localhost:7233/api/Students';

  constructor(private authen: AuthenService, private http: HttpClient) { }

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

  addStudent(): void {
    this.http.post<Student>(this.apiUrl, this.newStudent).subscribe({
      next: (response) => {
        console.log('Thêm sinh viên thành công:', response);
        this.students.push(response); // Thêm sinh viên mới vào danh sách
      },
      error: (error) => {
        console.error('Có lỗi khi thêm sinh viên:', error);
      }
    });
  }
  

  editStudent(student: Student): void {
    this.selectedStudent = { ...student }; // Sao chép thông tin sinh viên để chỉnh sửa
  }

  updateStudent(): void {
    if (this.selectedStudent) {
      const url = `${this.apiUrl}/${this.selectedStudent.maSV}`;
      this.http.put(url, this.selectedStudent).subscribe({
        next: (response) => {
          console.log('Cập nhật sinh viên thành công:', response);
          this.students = this.students.map(student => 
            student.maSV === this.selectedStudent!.maSV ? this.selectedStudent! : student
          );
          this.selectedStudent = null; // Đặt lại selectedStudent
        },
        error: (error) => {
          console.error('Có lỗi khi cập nhật sinh viên:', error);
        }
      });
    }
  }

  ngOnInit(): void {
    // console.log('HEloo');
    console.log(`Role user là: ${this.authen.GetRoleUser()}`);

   
  }
}
