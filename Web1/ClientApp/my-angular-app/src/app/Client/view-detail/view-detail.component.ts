import { Component, Input, OnInit } from '@angular/core';
import { TinTucService } from '../service-client/tintuc-service/tin-tuc.service';
import { ActivatedRoute } from '@angular/router';
import { Tintuc } from '../models/tintuc';
import { Comment } from '../models/comment';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommentService } from '../service-client/comment-service/comment.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-view-detail',
  templateUrl: './view-detail.component.html',
  styleUrl: './view-detail.component.css'
})
export class ViewDetailComponent implements OnInit{
  postForm!: FormGroup;
  newsId !: number;
  tintucs !: Tintuc;
  tintucTop: Tintuc[] = [];
  checkToken: boolean = false;
  comments : Comment[] = [];
  replies: Comment[] = [];
  activeReplyId !: number;
  currentComment: string = '';
  tintucView: Tintuc[] = [];
  tintucProminent: Tintuc[] = [];
  expandedComments: Set<number> = new Set();

  constructor(private tintucService: TinTucService,
              private route: ActivatedRoute,
              private fb: FormBuilder,
              private commentService: CommentService
              
  ) {}

  getNewId() {
    this.newsId = Number(this.route.snapshot.paramMap.get('id'));
  }

  toggleReply(commentId: number) {
    this.checkToken = !this.checkToken;
    this.activeReplyId = commentId;
    console.log(this.checkToken)
  }

  ControlReply(id: number) {
    if (this.expandedComments.has(id)) {
      this.expandedComments.delete(id); // Ẩn replies nếu đã mở
    } else {
      this.expandedComments.add(id); // Hiển thị replies
    }
  }

  isExpanded(commentId: number): boolean {
    return this.expandedComments.has(commentId);
  }

  initForm(): void {
    this.postForm = this.fb.group({
      binhluanId: [0], // Giá trị mặc định
      tintucId: ['', Validators.required],   
      userId: ['', Validators.required],
      ngayGioBinhLuan: [new Date(), Validators.required],
      noiDung: ['', Validators.required],
      userName: [''],
      parentId: [''],
      trangThai: [false, Validators.required]
    });
  }

  addCommentParent(commentId: number) {
    console.log(commentId);
    if(this.postForm.value['noiDung'] != null) {
      this.postForm.patchValue({
        binhluanId: [0],
        tintucId: this.newsId,
        userId: localStorage.getItem('userId'),
        ngayGioBinhLuan: new Date().toISOString(),
        parentId: '',
        trangThai: true,
        userName: localStorage.getItem('userName'),
      })
    }
    const data = this.commentService.SendFormPost(this.postForm);
    this.commentService.PostComment(data).subscribe(
      (data) => {
        this.comments.unshift(data);
        this.postForm.reset();
        console.log(data);
      }
    )
  }

  addCommentChild(commentId: number) {
    if(this.postForm.value['noiDung'] != null) {
      this.postForm.patchValue({
        binhluanId: [0],
        tintucId: this.newsId,
        userId: localStorage.getItem('userId'),
        ngayGioBinhLuan: new Date().toISOString(),
        parentId: commentId,
        trangThai: true,
        userName: localStorage.getItem('userName'),
      })
    }
    const data = this.commentService.SendFormPost(this.postForm);
    this.commentService.PostComment(data).subscribe(
      (data) => {
        this.comments.find(t => t.binhluanId === commentId)?.replies?.push(data);
        this.postForm.reset();
        console.log(data);
      }
    )
  }

  updateParent(tintucNew: Tintuc[]) {
    this.tintucTop = tintucNew;
    console.log("Tin tức từ cha: ", this.tintucTop);
  }

  getTimeAgo(dateInput: string | Date): string {
    const commentDate = dateInput instanceof Date ? dateInput : new Date(dateInput);
    const now = new Date();
    const diffMs = now.getTime() - commentDate.getTime();
    const diffSec = Math.floor(diffMs / 1000);
    const diffMin = Math.floor(diffSec / 60);
    const diffHours = Math.floor(diffMin / 60);
    const diffDays = Math.floor(diffHours / 24);
    const diffMonths = Math.floor(diffDays / 30); 
    const diffYears = Math.floor(diffDays / 365); 
  
    if (diffSec < 60) return `${diffSec} giây trước`;
    if (diffMin < 60) return `${diffMin} phút trước`;
    if (diffHours < 24) return `${diffHours} giờ trước`;
    if (diffDays < 30) return `${diffDays} ngày trước`;
    if (diffMonths < 12) return `${diffMonths} tháng trước`;
    return `${diffYears} năm trước`;
  }

  loadData() {
    forkJoin({
      tintucById: this.tintucService.getTintucById(this.newsId),
      allTintuc: this.tintucService.getTintuc(),
      comments: this.commentService.getCommentTinTuc(this.newsId)
    }).subscribe(
      ({ tintucById, allTintuc, comments }) => {
        this.tintucs = tintucById;
        this.tintucTop = allTintuc;
        this.comments = comments;
  
        console.log("Tin tức chi tiết:", this.tintucs);
        console.log("Danh sách tất cả tin tức:", this.tintucTop);
        console.log("Danh sách bình luận:", this.comments);
      },
      (error) => {
        console.error("Lỗi khi gọi API:", error);
      }
    );
  }

  
  

  

  ngOnInit(): void {
    this.initForm();
    this.getNewId();
    console.log("TinTucTop là:",this.tintucTop);
    this.loadData();
  }
}
