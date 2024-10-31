import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from '../app/Client/login/login.component';
import { RegisterComponent } from './Client/register/register.component';
import { ManageComponent } from './Client/manage/manage.component';
import { AdminComponent } from './admin/admin.component';
import { authGuard } from './guards/auth.guard';


const routes: Routes = [
  { path : '', component: LoginComponent},
  { path : 'login', component: LoginComponent },
  { path : 'register', component: RegisterComponent},
  { path : 'manage', component: ManageComponent},
  { path : 'admin' , canActivate: [authGuard], loadChildren: () => import('./admin/admin.module').then((m) => m.AdminModule)},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
