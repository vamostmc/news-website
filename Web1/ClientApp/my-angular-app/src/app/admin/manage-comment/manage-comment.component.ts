import { Component, OnInit } from '@angular/core';
import { CommentService } from '../../Client/service-client/comment-service/comment.service';
import { Comment } from '../../Client/models/comment';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { error } from 'console';
import { Router } from '@angular/router';
import { DashboardService } from '../../Client/service-client/dashboard-service/dashboard.service';

@Component({
  selector: 'app-manage-comment',
  templateUrl: './manage-comment.component.html',
  styleUrl: './manage-comment.component.css'
})
export class ManageCommentComponent implements OnInit {
  comments: Comment[] = [];
  commentView: Comment[] = [];
  selectedComment: Comment | null = null;

  showAddComment: boolean = false;
  showEditComment: boolean = false;

  postForm !: FormGroup;
  editForm !: FormGroup;

  constructor(private commentservice: CommentService,
    private fb: FormBuilder,
    private route: Router,
    private dashboardservice: DashboardService) {}

    p: number = 1; // Biến để lưu số trang hiện tại
    itemsPerPage: number = 5;
    
    onPageChange(page: number) {
      this.p = page;
    }


  initForm(): void {
    this.postForm = this.fb.group({
      binhluanId: [0], // Giá trị mặc định
      tintucId: ['', Validators.required],   
      userId: ['', Validators.required],
      ngayGioBinhLuan: [new Date(), Validators.required],
      noiDung: ['', Validators.required],
      userName: [''],
      tieuDeTinTuc: [''],
      trangThai: [false, Validators.required]
    });

    this.editForm = this.fb.group({
      binhluanId: [0], // Giá trị mặc định
      tintucId: [''],   
      userId: [''],
      ngayGioBinhLuan: [new Date(), Validators.required],
      noiDung: ['', Validators.required],
      userName: [''],
      tieuDeTinTuc: [''],
      trangThai: [false, Validators.required]
    });
  }
  

  //Hiển thị Form Add Comment
  selectedAdd() {
    this.showAddComment = true;
  }

  //Nhấn vào phần sửa chức năng edit
  selectedEdit(id: number) {
    this.showEditComment = true;
    this.commentservice.getIdComment(id).subscribe(
      (data) => {
        this.selectedComment = data;
        console.log(this.selectedComment);
        if(this.selectedComment) {
          this.editForm.patchValue({
            binhluanId: this.selectedComment.binhluanId,
            userName: this.selectedComment.userName,
            tintucId: this.selectedComment.tintucId,
            userId:this.selectedComment.userId,
            tieuDeTinTuc: this.selectedComment.tieuDeTinTuc,
            ngayGioBinhLuan: this.selectedComment.ngayGioBinhLuan,
            noiDung: this.selectedComment.noiDung,
            trangThai: this.selectedComment.trangThai
          })
        }
      }
    )
  }

  //Ẩn Form
  hideForm() {
    this.showAddComment = false;
    this.showEditComment = false
    this.postForm.reset();
  }

  //Thực hiện add comment
  AddComment() {
    if(this.postForm.valid) {
      console.log(this.postForm.value);
      const data = this.commentservice.SendFormPost(this.postForm);
      this.commentservice.PostComment(data).subscribe(
        (data) => {
          console.log("Thành công");
          this.dashboardservice.NotifyAddBinhLuan(this.postForm.get('userId')?.value, this.postForm.get('tintucId')?.value);
          this.route.navigate(['/success']);
        },
        
      )
    }
    else {
      console.log("Lỗi Form");
    }
  }

  //Thực hiện chức năng sửa comment
  EditComment() {
    const data = this.commentservice.SendFormEdit(this.editForm);
    this.commentservice.PutComment(this.editForm.get('binhluanId')?.value, data).subscribe(
      (data) => {
        console.log("Thành công");
        this.dashboardservice.NotifyUpdateBinhLuan(this.editForm.get('userId')?.value, this.editForm.get('tintucId')?.value);
        this.route.navigate(['/success']);
      }
    )
  }

  

  //Thực hiện xóa comment
  async Delete(id : number) {
    let DataRemove = this.comments.find((data) => data.binhluanId == id) || null;
    let UserId: string;
    let TinTucId: number;
    if (DataRemove) {
        UserId = DataRemove.userId;
        TinTucId = DataRemove.tintucId;        
    } else {
        UserId = '';
        TinTucId = 0; // Nếu không tìm thấy, gán giá trị mặc định
    }
    
    //Đợi cập nhật dữ liệu thông tin xóa ra bảng thông báo
    await this.dashboardservice.NotifyRemoveBinhLuan(UserId,TinTucId);

    this.commentservice.DeleteComment(id).subscribe(
      (data) => {
        console.log("Thành công");
        this.route.navigate(['/success']);
      }
    )
  }


  //Cập nhật trạng thái bình luận
  UpdateStatus(id: number, event: Event) {
    const status = event.target as HTMLInputElement;
    this.commentservice.StatusComment(id, status.checked).subscribe(
      (data) => {
        this.comments = data;
        this.commentView = this.comments;
      }
    )
  }


  

  ngOnInit(): void {
    this.commentservice.getAllComment().subscribe(
      (data) => { 
        this.comments = data; 
        this.commentView = this.comments;
        console.log(this.comments); }
    );

    this.initForm();
  }
}
