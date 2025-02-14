import { Component, Input, OnInit } from '@angular/core';
import { TinTucService } from '../service-client/tintuc-service/tin-tuc.service';
import { ActivatedRoute } from '@angular/router';
import { Tintuc } from '../models/tintuc';
import { Comment } from '../models/comment';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

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

  constructor(private tintucService: TinTucService,
              private route: ActivatedRoute,
              private fb: FormBuilder
  ) {}

  getNewId() {
    this.newsId = Number(this.route.snapshot.paramMap.get('id'));
  }

  toggleReply(commentId: number) {
    this.checkToken = !this.checkToken;
    this.activeReplyId = commentId;
    console.log(this.checkToken)
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
      parentId: [''],
      trangThai: [false, Validators.required]
    });
  }

  addReply(commentId: number) {
    console.log(this.currentComment);
    console.log(commentId);
  }

  updateParent(tintucNew: Tintuc[]) {
    this.tintucTop = tintucNew;
    console.log("Tin tức từ cha: ", this.tintucTop);
  }

  

  ngOnInit(): void {
    this.initForm();
    this.getNewId();
    this.tintucTop = this.tintucService.getData();
    console.log("TinTucTop là:",this.tintucTop);
    this.tintucService.getTintucById(this.newsId).subscribe(
      (data) => {
        this.tintucs = data;
        this.comments = this.tintucs.binhLuan;
        console.log(this.tintucs);
        console.log(this.comments);
      }
    )
    console.log("Tin tức từ cha: ", this.tintucTop);
  }
}
