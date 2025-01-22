import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdminRoutingModule } from './admin-routing.module';
import { NavbarAdminComponent } from './navbar-admin/navbar-admin.component';
import { DashboardAdminComponent } from './dashboard-admin/dashboard-admin.component';
import { ManagePostComponent } from './manage-post/manage-post.component';
import { MenuAdminComponent } from './menu-admin/menu-admin.component';
import { ReactiveFormsModule } from '@angular/forms';
import { SuccessComponent } from './success/success.component';
import { ManageCategoryComponent } from './manage-category/manage-category.component';
import { FormsModule } from '@angular/forms';
import { NgxPaginationModule } from 'ngx-pagination';
import { ManagePostDetailComponent } from './manage-post-detail/manage-post-detail.component';
import { EditorModule } from '@tinymce/tinymce-angular';
import { ManageCommentComponent } from './manage-comment/manage-comment.component';
import { ManageUserComponent } from './manage-user/manage-user.component';
import { AddPostComponent } from './manage-post/add-post/add-post.component';
import { EditPostComponent } from './manage-post/edit-post/edit-post.component';
import { ShowPostComponent } from './manage-post/show-post/show-post.component';
import { SearchFilterPostComponent } from './manage-post/search-filter-post/search-filter-post.component';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [
    NavbarAdminComponent,
    DashboardAdminComponent,
    ManagePostComponent,
    MenuAdminComponent,
    SuccessComponent,
    ManageCategoryComponent,
    ManagePostDetailComponent,
    ManageCommentComponent,
    ManageUserComponent,
    AddPostComponent,
    EditPostComponent,
    ShowPostComponent,
    SearchFilterPostComponent,
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    ReactiveFormsModule, 
    FormsModule,
    NgxPaginationModule,
    EditorModule,
    RouterModule
  ]
})
export class AdminModule { }
