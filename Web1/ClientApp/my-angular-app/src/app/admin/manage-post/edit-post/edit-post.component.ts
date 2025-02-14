import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TinTucService } from '../../../Client/service-client/tintuc-service/tin-tuc.service';
import { Tintuc } from '../../../Client/models/tintuc';
import { Danhmuc } from '../../../Client/models/danhmuc';
import { DanhmucService } from '../../../Client/service-client/danhmuc-service/danhmuc.service';
import { DashboardService } from '../../../Client/service-client/dashboard-service/dashboard.service';
import { ActivatedRoute, Router } from '@angular/router';
import * as pdfjsLib from 'pdfjs-dist';
import { async } from 'rxjs';
import * as JSZip from 'jszip';





@Component({
  selector: 'app-edit-post',
  templateUrl: './edit-post.component.html',
  styleUrl: './edit-post.component.css'
})
export class EditPostComponent {
  constructor(private fb: FormBuilder,
              private tintucservice: TinTucService,
              private danhmucservice: DanhmucService,
              private dashboardservice: DashboardService,
              private route: Router,
              private activeroute : ActivatedRoute) {

  }
  

  selectedTinTuc: Tintuc | null = null;
  editorData: string = ''; // Chứa nội dung trình soạn thảo
  editorConfig: any;

  imagePreview: string | ArrayBuffer | null = null;
  selectedFile: File | null = null;
  idUrl: number | null = null;
  selectImage: string | null = null;

  currentStep = 1;  // Bắt đầu từ bước 1
  steps = [1, 2, 3, 4];  // Danh sách các bước

  postForm !: FormGroup;
  tintucs: Tintuc[] = [];
  danhmucs: Danhmuc[] = [];
  dataView: Tintuc[] = [];
  dataPost: Tintuc | null = null;

  loading: boolean = false;
  

  private Url = 'https://localhost:7233';

  get progressWidth(): number {
    return (this.currentStep - 1) / (this.steps.length - 1) * 88;
  }

  goBack() {
    
    this.route.navigate(['admin/ManagePost']); // Quay lại trang trước đó
  }

  isStep1Valid(): boolean {
    const invalidFields: string[] = [];
  
    if (!this.postForm.get('TieuDe')?.valid) invalidFields.push('Tiêu đề');
    if (!this.postForm.get('MoTaNgan')?.valid) invalidFields.push('Mô tả ngắn');
    if (!this.postForm.get('NgayDang')?.valid) invalidFields.push('Ngày đăng');
    
    if (!this.postForm.get('NgayCapNhat')?.valid) invalidFields.push('Ngày cập nhật');
    if (!this.postForm.get('LuongKhachTruyCap')?.valid) invalidFields.push('Lượng khách truy cập');
    if (!this.postForm.get('TrangThai')?.valid) invalidFields.push('Trạng thái');
    if (!this.postForm.get('DanhmucId')?.valid) invalidFields.push('Danh mục');
  
    if (invalidFields.length > 0) {
      console.log("Các trường chưa điền hoặc không hợp lệ: ", invalidFields);
      return false;
    }
  
    // Nếu tất cả các trường đều hợp lệ
    return true;
  }

  isStep2Valid(): boolean {
    const invalidFields: string[] = [];
    if (!this.postForm.get('NoiDung')?.valid) invalidFields.push('Nội dung');
   
    if (invalidFields.length > 0) {
      console.log("Các trường chưa điền hoặc không hợp lệ: ", invalidFields);
      return false;
    }
  
    // Nếu tất cả các trường đều hợp lệ
    return true;
  }

  getFullImageUrl(imageUrl: string): string {
    return `${this.Url}${imageUrl}`;
  }

  onFileChange(event: any) {
    const file = event.target.files[0];
    this.imagePreview = null;
    if (file) {
      this.selectedFile = file;
      const reader = new FileReader();
      reader.onload = (e) => {
        // Chỉ gán giá trị cho imagePreview nếu e.target.result không phải là undefined
        if (e.target && e.target.result) {
          this.imagePreview = e.target.result; // Cập nhật thuộc tính hình ảnh
        }
      };
      reader.readAsDataURL(file); // Đọc file dưới dạng URL
      // Cập nhật giá trị của HinhAnh trong postForm
      this.postForm.patchValue({
        HinhAnh: file
      });
    }
  }

  //Cuộn trang lên đầu sau khi next sang bước tiếp theo
  scrollToNext() {
    setTimeout(() => {
      window.scrollTo({
          top: 0,
          behavior: 'smooth' 
      });
    }, 120);
  }

  nextStep() {
    if (this.currentStep < this.steps.length) {
      if (this.currentStep === 1) {
        // Kiểm tra nếu bước 1 đã hoàn thành (hợp lệ) thì chuyển bước
        if (this.isStep1Valid()) {
          // console.log(this.postForm.value);
          this.currentStep++;

          //Đưa trang lên trên đầu sau khi next sang bước tiếp theo
          this.scrollToNext(); 
          return;
        } else {
          // Hiển thị thông báo nếu form không hợp lệ
          alert('Vui lòng hoàn thành tất cả các thông tin trong bước 1');
          return;
        }
      }

      if(this.currentStep === 2) {
        // Kiểm tra nếu bước 2 đã hoàn thành (hợp lệ) thì chuyển bước
        if (this.isStep2Valid()) {
          // console.log(this.postForm.value);
          this.currentStep++;

          //Đưa trang lên trên đầu sau khi next sang bước tiếp theo
          this.scrollToNext(); 
          return;
        } else {
          // Hiển thị thông báo nếu form không hợp lệ
          alert('Vui lòng hoàn thành tất cả các thông tin trong bước 2');
          return;
        }
      }

      if(this.currentStep === 3) {
        this.dataPost = this.postForm.value;
        console.log(this.dataPost);
        this.edit();
        this.currentStep++;

        this.loading = true;
        setTimeout(() => {
          this.loading = false;
          this.route.navigate(['/admin/ManagePost']);
        }, 1800);
        
        console.log('Cập nhật thành công');
        return;
      }
      else {
        return;
      }
    }
  }

  previousStep() {
    if (this.currentStep > 1) {
      this.currentStep--;
      this.scrollToNext();
    }
  }

  initializeForm() {
    this.postForm = this.fb.group({
      TintucId: [0], // ID bài viết
      TieuDe: ['', Validators.required], // Tiêu đề bài viết
      HinhAnh: ['', Validators.required], // Hình ảnh
      MoTaNgan: ['', Validators.required], // Mô tả ngắn
      NgayDang: [null, Validators.required], // Ngày đăng
      NgayCapNhat: [null, Validators.required], // Ngày cập nhật
      LuongKhachTruyCap: [0, Validators.required], // Lượng khách truy cập
      TrangThai: [false], // Số lượng bình luận
      DanhmucId: ['', Validators.required], // Danh mục
      NoiDung: ['', Validators.required]
    });
  }

  GetData(): void {
    // Lấy giá trị của FormControl 'NoiDung'
    const noiDungValue = this.postForm.get('NoiDung')?.value;
    console.log(noiDungValue);
    // Bạn có thể thực hiện các hành động khác với giá trị này
  }


  edit() {
    const formData = new FormData();
    // Thêm các trường vào FormData
    formData.append('TintucId', this.postForm.get('TintucId')?.value);
    formData.append('TieuDe', this.postForm.get('TieuDe')?.value);
    formData.append('MoTaNgan', this.postForm.get('MoTaNgan')?.value);
    formData.append('NgayDang', this.postForm.get('NgayDang')?.value);
    formData.append('NgayCapNhat', this.postForm.get('NgayCapNhat')?.value);
    formData.append('LuongKhachTruyCap', this.postForm.get('LuongKhachTruyCap')?.value);
    formData.append('TrangThai', this.postForm.get('TrangThai')?.value);
    formData.append('NoiDung', this.postForm.get('NoiDung')?.value);
    formData.append('DanhmucId', this.postForm.get('DanhmucId')?.value);
    
    // Nếu có hình ảnh, thêm vào FormData
    if (this.selectedFile) {
      formData.append('Image', this.selectedFile);
  } else if (this.selectedTinTuc && this.selectedTinTuc.hinhAnh) {
      formData.append('Image', this.selectedTinTuc.hinhAnh); // Gửi lại hình ảnh cũ
  }

    // Lấy ID từ selectedTintuc
    if (this.selectedTinTuc) {
      const tintucId = this.selectedTinTuc.tintucId; // Lấy ID từ selectedTintuc

      this.tintucservice.updateTintuc(formData, tintucId).subscribe({
        next: (response) => {
          console.log('Cập nhật thành công', response);
          this.dashboardservice.NotificationUpdateTinTuc(this.postForm.get('TieuDe')?.value);
        },
        error: (error) => {
          console.error('Lỗi khi cập nhật tin tức', error);
        }
      });
    } else {
      console.error('Không có đối tượng nào được chọn để cập nhật.');
    }
}

  FindTinTucId() {
    this.idUrl = parseInt(this.activeroute.snapshot.paramMap.get('id')!);
    this.tintucservice.getTintucById(this.idUrl).subscribe(
      (data) => {
        this.selectedTinTuc = data;
        if (this.selectedTinTuc) {
                this.postForm.patchValue({
                  TintucId: this.selectedTinTuc.tintucId,
                  TieuDe: this.selectedTinTuc.tieuDe,
                  MoTaNgan: this.selectedTinTuc.moTaNgan,
                  NgayDang: (this.selectedTinTuc.ngayDang).split('T')[0],
                  NgayCapNhat: (this.selectedTinTuc.ngayCapNhat).split('T')[0],
                  LuongKhachTruyCap: this.selectedTinTuc.luongKhachTruyCap,
                  TrangThai: this.selectedTinTuc.trangThai,
                  DanhmucId: this.selectedTinTuc.danhmucId,
                  NoiDung: this.selectedTinTuc.noiDung
                });
                this.selectImage = this.selectedTinTuc.hinhAnh;
              }
              console.log(this.selectedTinTuc.ngayDang);
      }
    )
  }

  importFile(editor: any): void {
    const fileInput = document.createElement('input');
    fileInput.type = 'file';
    fileInput.accept = '.pdf,.docx'; // Chỉ cho phép file PDF và Word
    fileInput.style.display = 'none';

    fileInput.onchange = async () => {
      const file = fileInput.files ? fileInput.files[0] : null;
      if (file) {
        const fileType = file.name.split('.').pop()?.toLowerCase();

        if (fileType === 'pdf') {
          this.loadPDF(file, editor);
        } else if (fileType === 'docx') {
          this.loadWord(file, editor);
        } else {
          alert('Chỉ hỗ trợ file PDF hoặc Word.');
        }
      }
    };

    
    // Kích hoạt chọn file
    fileInput.click();
  }

  // Hàm xử lý hiển thị nội dung file PDF
  async loadPDF(file: File, editor: any) {
    const reader = new FileReader();
    reader.onload = async (event: any) => {
      const pdfjsLib = await import('pdfjs-dist');
      pdfjsLib.GlobalWorkerOptions.workerSrc = 'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.16.105/pdf.worker.min.js';
      const pdf = await pdfjsLib.getDocument({ data: event.target.result }).promise;
      let content = '';
      for (let i = 1; i <= pdf.numPages; i++) {
        const page = await pdf.getPage(i);
        const textContent = await page.getTextContent();
        content += textContent.items.map((item: any) => item.str).join(' ') + '<br/>';
      }
      editor.setContent(content); // Chèn nội dung vào editor
    };
    reader.readAsArrayBuffer(file);
  }
  

  // Upload file word
  async loadWord(file: File, editor: any): Promise<void> {
    try {
      const arrayBuffer = await file.arrayBuffer();
      const zip = await JSZip.loadAsync(arrayBuffer);
  
      // Tìm thư mục hình ảnh trong file docx
      const images: any[] = [];
      zip.forEach((relativePath, file) => {
        if (file.name.startsWith('word/media/')) {
          file.async('base64').then((data: string) => {
            images.push({
              name: file.name,
              data: data
            });
          });
        }
      });
  
      // Sử dụng mammoth để trích xuất HTML với định dạng
      const mammoth = await import('mammoth');
      const { value, messages } = await mammoth.convertToHtml({ arrayBuffer });
      
      // Kiểm tra các cảnh báo từ mammoth (nếu có)
      if (messages.length > 0) {
        console.warn('Cảnh báo từ Mammoth:', messages);
      }
  
      // Chèn nội dung văn bản vào editor (HTML có định dạng)
      editor.insertContent(value);
  
      // Chèn hình ảnh vào editor
      images.forEach(image => {
        const imgElement = `<img src="data:image/jpeg;base64,${image.data}" alt="${image.name}" />`;
        editor.insertContent(imgElement);
      });
  
    } catch (error) {
      console.error('Lỗi khi xử lý tệp Word:', error);
    }
  }


  ngOnInit(): void {
    this.editorConfig = {
      height: 550, // Chiều cao editor
      menubar: false,
      plugins: 'link image code preview',
      toolbar: [
        'undo redo | styleselect | bold italic | fontselect | fontsizeselect | forecolor backcolor\n',
        'alignleft aligncenter alignright alignjustify | bullist numlist | link image | table | media | preview | importfile'
      ].join('\n'), // Sử dụng join để xuống dòng
      setup: (editor: any) => {
        editor.ui.registry.addButton('importfile', {
          text: 'Import File', // Nhãn nút
          icon: 'upload', // Biểu tượng
          onAction: () => {
            // Tạo input file để chọn file
            const input = document.createElement('input');
            input.type = 'file';
            input.accept = '.docx, .pdf'; // Hỗ trợ cả Word và PDF
            input.onchange = async (event: Event) => {
              const file = (event.target as HTMLInputElement).files?.[0];
              if (file) {
                const fileExtension = file.name.split('.').pop()?.toLowerCase();
                if (fileExtension === 'docx') {
                  await this.loadWord(file, editor); // Xử lý file Word
                } else if (fileExtension === 'pdf') {
                  await this.loadPDF(file, editor); // Xử lý file PDF
                } else {
                  console.error('File không được hỗ trợ.');
                  alert('Chỉ hỗ trợ file .docx và .pdf');
                }
              }
            };
            input.click();
          },
        });
      },
    };
    

    this.FindTinTucId();
    console.log(this.idUrl);

    this.tintucservice.getTintuc().subscribe(
      (data) => this.tintucs = data
    );

    this.danhmucservice.GetDanhmuc().subscribe(
      (data) => this.danhmucs = data
    );

    

    
    this.initializeForm();
  }
}
