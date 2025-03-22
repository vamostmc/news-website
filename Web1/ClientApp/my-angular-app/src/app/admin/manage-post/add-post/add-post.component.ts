import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TinTucService } from '../../../Client/service-client/tintuc-service/tin-tuc.service';
import { Tintuc } from '../../../Client/models/tintuc';
import { Danhmuc } from '../../../Client/models/danhmuc';
import { DanhmucService } from '../../../Client/service-client/danhmuc-service/danhmuc.service';
import { DashboardService } from '../../../Client/service-client/dashboard-service/dashboard.service';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import * as JSZip from 'jszip';
import { EditorComponent } from '@tinymce/tinymce-angular';

@Component({
  selector: 'app-add-post',
  templateUrl: './add-post.component.html',
  styleUrl: './add-post.component.css'
})
export class AddPostComponent implements OnInit {
  
  constructor(private fb: FormBuilder,
              private tintucservice: TinTucService,
              private danhmucservice: DanhmucService,
              private dashboardservice: DashboardService,
              private route: Router,
              private location: Location) {

  }

  editorData: string = ''; // Chứa nội dung trình soạn thảo
  editorConfig: any;

  imagePreview: string | ArrayBuffer | null = null;
  selectedFile: File | null = null;

  currentStep = 1;  // Bắt đầu từ bước 1
  steps = [1, 2, 3, 4];  // Danh sách các bước

  postForm !: FormGroup;
  tintucs: Tintuc[] = [];
  danhmucs: Danhmuc[] = [];
  dataView: Tintuc[] = [];
  dataPost: Tintuc | null = null;
  showNotify: boolean = false;
  typeNotify: boolean = true;
  messNotify: string[] = [];
  checkStep1: boolean = false;
  

  loading: boolean = false;
  showPostData: boolean = false;
  fileExcelName: string | null = null;
  selectedFileExcel: File | null = null;
  isDialogOpen = false;
  isUploading = false;
  uploadProgress = 0;
  

  private Url = 'https://localhost:7233';

  // Hàm xử lý thanh tiến trình mỗi lần click sẽ tăng 1 đoạn width tương ứng 4 tiến trình
  get progressWidth(): number {
    return (this.currentStep - 1) / (this.steps.length - 1) * 88;
  }

  //Trở lại trang show Tin Tuc
  goBack() {
    this.route.navigate(['admin/ManagePost']); // Quay lại trang trước đó
  }

  // Kiểm tra bước 1 
  isStep1Valid(): boolean {
    const invalidFields: string[] = [];
    this.messNotify = [];
    this.checkStep1 = true;
  
    if (!this.postForm.get('TieuDe')?.valid) invalidFields.push('Tiêu đề');
    if (!this.postForm.get('MoTaNgan')?.valid) invalidFields.push('Mô tả ngắn');
    if (!this.postForm.get('NgayDang')?.valid) invalidFields.push('Ngày đăng');
    if (!this.postForm.get('HinhAnh')?.valid) invalidFields.push('Hình Ảnh');
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

  // Kiểm tra bước 2
  isStep2Valid(): boolean {
    const invalidFields: string[] = [];
    this.messNotify = [];
    if (!this.postForm.get('NoiDung')?.valid) this.messNotify.push('Chưa nhập nội dung chi tiết bài viết');
   
    if (this.messNotify.length > 0) {
      console.log("Các trường chưa điền hoặc không hợp lệ: ", invalidFields);
      return false;
    }
  
    // Nếu tất cả các trường đều hợp lệ
    return true;
  }

  // Chuyển đường dẫn hình ảnh sang dạng tên local để lưu lên DB
  getFullImageUrl(imageUrl: string): string {
    return `${this.Url}${imageUrl}`;
  }

  //Từ File chuyển sang dạng đường dẫn URL
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

  // Xử lý các trường hợp next các bước tiếp theo
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
          // alert('Vui lòng hoàn thành tất cả các thông tin trong bước 1');

          this.typeNotify = false;
          this.scrollToNext();
          console.log(this.typeNotify);
          this.showNotify = true;
          setTimeout(() => {
            this.showNotify = false;
            
          },3000);
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
          this.typeNotify = false;
          this.scrollToNext();
          console.log(this.typeNotify);
          this.showNotify = true;
          setTimeout(() => {
            this.showNotify = false;
            
          },3000);
          return;
          
        }
      }

      if(this.currentStep === 3) {
        this.dataPost = this.postForm.value;
        console.log(this.dataPost);
        this.add();
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

  // Xử lý lùi lại các bước trước đó
  previousStep() {
    if (this.currentStep > 1) {
      this.currentStep--;
      this.scrollToNext();
    }
  }

  // Khởi tạo Form ban đầu
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

  // Thực hiện lệnh post data
  add() {
    console.log("OK");
    console.log(this.postForm.value);
    // if (this.postForm.valid) {
      const formData = new FormData();
      
      formData.append('TieuDe', this.postForm.get('TieuDe')?.value);
      formData.append('MoTaNgan', this.postForm.get('MoTaNgan')?.value);
      formData.append('NgayDang', this.postForm.get('NgayDang')?.value);
      formData.append('NgayCapNhat', this.postForm.get('NgayCapNhat')?.value);
      formData.append('LuongKhachTruyCap', this.postForm.get('LuongKhachTruyCap')?.value);
      formData.append('DanhmucId', this.postForm.get('DanhmucId')?.value);
      formData.append('NoiDung', this.postForm.get('NoiDung')?.value);
      formData.append('TrangThai', this.postForm.get('TrangThai')?.value);
      // Thêm file vào FormData
      if (this.selectedFile) {
        formData.append('Image', this.selectedFile);
      }

      this.tintucservice.addTintuc(formData).subscribe({
        next: (response) => {
          console.log('Thêm thành công', response);
          
          this.dashboardservice.NotificationAddTinTuc(this.postForm.get('TieuDe')?.value);
        },
        error: (error) => {
          console.error('Lỗi khi thêm tin tức', error);
        },
      });
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

  // Khởi tạo editor Tiny trình soạn thảo văn bản
  EditorInit() {
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
  }

  openFileDialog() {
    this.isDialogOpen = true;
    console.log("OK");
  }

  // Đóng hộp thoại khi click vào nền tối
  closeFileDialog() {
    this.isDialogOpen = false;
    this.resetFile();
  }


  triggerFileInput() {
    const fileInput: HTMLInputElement = document.getElementById('fileInput') as HTMLInputElement;
    fileInput.click();
  }

  resetFile() {
    this.fileExcelName = null;
  }

  // Xử lý sự kiện khi chọn file từ input
  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.fileExcelName = input.files[0].name;
      this.selectedFileExcel = input.files[0];
    }
  }

  // Xử lý sự kiện khi kéo file vào ô hình chữ nhật
  onDragOver(event: DragEvent) {
    event.preventDefault();
    const dropArea = event.target as HTMLElement;
    dropArea.classList.add('drag-over');
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    const dropArea = event.target as HTMLElement;
    dropArea.classList.remove('drag-over');
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    const dropArea = event.target as HTMLElement;
    dropArea.classList.remove('drag-over');
    const file = event.dataTransfer?.files[0];
    if (file) {
      this.fileExcelName = file.name;
    }
  }

  ImportExcel() {
    this.loading = true;
    this.messNotify = [];
    if (this.selectedFileExcel) {
      const formData = new FormData();
      formData.append('file', this.selectedFileExcel);
      this.tintucservice.ImportExcel(formData).subscribe(
        (data) => {
          console.log(data);
          if(data.success == true) {
            setTimeout(() => {
              this.loading = false;
              this.route.navigate(['/admin/ManagePost']);
            }, 1800);
          }
          else {
            this.typeNotify = false;
            this.scrollToNext();
            this.messNotify.push(data.message);
            this.showNotify = true;
            setTimeout(() => {
              this.showNotify = false;
            },3000);
            
          }
        }
      );
    } else {
      console.error('No file selected');
    }
  }

  // Giả lập quá trình tải lên file
  uploadFile(file: File) {
    this.isUploading = true;
    let progress = 0;
    const interval = setInterval(() => {
      if (progress < 100) {
        progress += 5;
        this.uploadProgress = progress;
      } else {
        clearInterval(interval);
        this.isUploading = false;
      }
    }, 200); // Giả lập tải file lên mỗi 200ms
  }

  changeFileExcel() {
    this.fileExcelName = null;
    this.uploadProgress = 0;
    const fileInput = document.getElementById('fileInput') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = ''; // Reset input file
    }
  }

  downloadTemplate() {
    this.tintucservice.DownloadTempExcel().subscribe({
      next: (blob) => {
        // Tạo URL từ Blob
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a'); 
        a.href = url;
        a.download = 'template.xlsx'; 
        document.body.appendChild(a); 
        a.click();
        document.body.removeChild(a); 
        window.URL.revokeObjectURL(url); 
      },
      error: (err) => {
        console.error('Error downloading the file', err);
      },
    });
  }






  ngOnInit(): void {

    

    this.danhmucservice.GetDanhmuc().subscribe(
      (data) => this.danhmucs = data
    );

    this.EditorInit();
    this.initializeForm();
  }
}
