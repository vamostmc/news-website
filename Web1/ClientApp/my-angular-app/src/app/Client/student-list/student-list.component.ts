
import { Component, OnInit } from '@angular/core';
import { StudentService } from '../service-client/student-service/student.service';
import { Student } from '../models/student'; // Import class Student
import { AuthenService } from '../service-client/authen-service/authen.service';


@Component({
  selector: 'app-student-list',
  templateUrl: './student-list.component.html',
  styleUrls: ['./student-list.component.css']
})
export class StudentListComponent implements OnInit {
  students: Student[] = [];
  
  public editorData: string = '<p>Nhập nội dung của bạn vào đây!</p>';
  constructor(private studentService: StudentService, private authen: AuthenService) { }

  ngOnInit(): void {
    // console.log('HEloo');
    console.log(`Role user là: ${this.authen.GetRoleUser()}`);

    this.studentService.getStudents().subscribe(data => {
      this.students = data;
    });
  }

 
}
