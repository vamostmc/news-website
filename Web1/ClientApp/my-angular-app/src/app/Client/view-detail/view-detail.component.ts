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
  TotalComment !: number;
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
      trangThai: [false, Validators.required],
      replyToUserId: ['']
    });
  }

  addCommentParent() {
    if(this.postForm.value['noiDung'] != null) {
      this.postForm.patchValue({
        binhluanId: [0],
        tintucId: this.newsId,
        userId: localStorage.getItem('userId'),
        ngayGioBinhLuan: new Date().toISOString(),
        parentId: '',
        trangThai: true,
        userName: localStorage.getItem('userName'),
        replyToUserId: ''
      })
    }
    const data = this.commentService.SendFormPost(this.postForm);
    this.commentService.PostComment(data).subscribe(
      (data) => {
        this.comments.unshift(data);
        this.postForm.reset();
        this.TotalComment++;
        console.log(data);
      }
    )
  }

  addCommentChild(replyUserID: string ,commentId: number) {
    if(this.postForm.value['noiDung'] != null) {
      this.postForm.patchValue({
        binhluanId: [0],
        tintucId: this.newsId,
        userId: localStorage.getItem('userId'),
        ngayGioBinhLuan: new Date().toISOString(),
        parentId: commentId,
        trangThai: true,
        userName: localStorage.getItem('userName'),
        replyToUserId: replyUserID
      })
    }
    const data = this.commentService.SendFormPost(this.postForm);
    console.log("Giá trị trong data là: ",data);
    this.commentService.PostComment(data).subscribe(
      (data) => {
        this.comments.find(t => t.binhluanId === commentId)?.replies?.push(data);
        this.TotalComment++;
        this.postForm.reset();
        
      }
    )
  }

  updateParent(tintucNew: Tintuc[]) {
    this.tintucTop = tintucNew;
    console.log("Tin tức từ cha: ", this.tintucTop);
  }

  getTimeAgo(timestamp: string): string {
    const now = new Date();  // Thời gian hiện tại
    const commentTime = new Date(timestamp);  // Thời gian bình luận
  
    const timeDiff = now.getTime() - commentTime.getTime();
  
    const seconds = Math.floor(timeDiff / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);
    const months = Math.floor(days / 30);
    const years = Math.floor(months / 12);
  
    if (seconds < 60) {
      return `${seconds} giây trước`;
    } else if (minutes < 60) {
      return `${minutes} phút trước`;
    } else if (hours < 24) {
      return `${hours} giờ trước`;
    } else if (days < 30) {
      return `${days} ngày trước`;
    } else if (months < 12) {
      return `${months} tháng trước`;
    } else {
      return `${years} năm trước`;
    }
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
        this.TotalComment = this.tintucs.soLuongComment;
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
