import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from '../app/Client/login/login.component';
import { RegisterComponent } from './Client/register/register-customer/register.component';

import { AdminComponent } from './admin/admin.component';
import { authGuard, authResetToken } from './guards/auth.guard';
import { SuccessComponent } from './admin/success/success.component';
import { ConfirmEmailComponent } from './Client/register/confirm-email/confirm-email.component';
import { ForgotPasswordComponent } from './Client/password/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './Client/password/reset-password/reset-password.component';
import { HomeComponent } from './home/home.component';
import { ErrorPageComponent } from './error-page/error-page.component';

const routes: Routes = [
  { path : '', redirectTo: 'home', pathMatch: 'full' },
  { path : 'home', component: HomeComponent},
  { path : 'error-page', component: ErrorPageComponent },
  { path : 'login', component: LoginComponent },
  { path : 'register', component: RegisterComponent},
  { path : 'success', component: SuccessComponent},
  { path : 'confirmEmail', component: ConfirmEmailComponent},
  { path: 'forgotPassword',  component: ForgotPasswordComponent},
  { path: 'resetPassword/:id/:resetToken',canActivate: [authResetToken], component: ResetPasswordComponent},
  { path : 'admin' , canActivate: [authGuard], loadChildren: () => import('./admin/admin.module').then((m) => m.AdminModule)},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
