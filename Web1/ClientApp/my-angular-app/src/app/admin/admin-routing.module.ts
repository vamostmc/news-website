import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminComponent } from './admin.component';
import { authGuard } from '../guards/auth.guard';
import { NavbarAdminComponent } from './navbar-admin/navbar-admin.component';
import { DashboardAdminComponent } from './dashboard-admin/dashboard-admin.component';
import { ManagePostComponent } from './manage-post/manage-post.component';
import { SuccessComponent } from './success/success.component';
import { ManageCategoryComponent } from './manage-category/manage-category.component';
import { ManagePostDetailComponent } from './manage-post-detail/manage-post-detail.component';
import { ManageUserComponent } from './manage-user/manage-user.component';
import { ManageCommentComponent } from './manage-comment/manage-comment.component';
import { AddPostComponent } from './manage-post/add-post/add-post.component';
import { EditPostComponent } from './manage-post/edit-post/edit-post.component';
import { ManageMessageComponent } from './manage-message/manage-message.component';


const routes: Routes = [
  {path: '',canActivate: [authGuard] , component: DashboardAdminComponent},
  {path: 'ManagePost', component: ManagePostComponent},
  {path: 'Category', component: ManageCategoryComponent},
  {path: 'success', component: SuccessComponent },
  {path: 'PostDetail', component: ManagePostDetailComponent},
  {path: 'User', component: ManageUserComponent},
  {path: 'Comment', component: ManageCommentComponent},
  {path: 'ManagePost/AddPost', component: AddPostComponent},
  {path: 'ManagePost/EditPost/:id', component: EditPostComponent},
  {path: 'admin/messages', component: ManageMessageComponent},
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
