import { NgModule } from '@angular/core';
import { BrowserModule, provideClientHydration } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { HttpClientModule } from '@angular/common/http';
import { LoginComponent } from '../app/Client/login/login.component';
import { NavbarComponent } from './Client/navbar/navbar.component';
import { ReactiveFormsModule } from '@angular/forms';
import { RegisterComponent } from './Client/register/register.component';
import { ManageComponent } from './Client/manage/manage.component';
import { FormsModule } from '@angular/forms';
import { AdminComponent } from './admin/admin.component';
import { AdminModule } from './admin/admin.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { CKEditorModule } from 'ckeditor4-angular';
import { EditorModule } from '@tinymce/tinymce-angular';


@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    NavbarComponent,
    RegisterComponent,
    ManageComponent,
    AdminComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    AdminModule,
    NgxPaginationModule,
    CKEditorModule, 
    EditorModule
  ],
  providers: [
    provideClientHydration(),
    
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
