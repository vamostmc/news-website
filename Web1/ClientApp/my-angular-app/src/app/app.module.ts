import { NgModule } from '@angular/core';
import { BrowserModule, provideClientHydration } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HTTP_INTERCEPTORS, HttpClientModule, provideHttpClient, withFetch } from '@angular/common/http';
import { LoginComponent } from '../app/Client/login/login.component';
import { ReactiveFormsModule } from '@angular/forms';
import { RegisterComponent } from './Client/register/register-customer/register.component';
import { FormsModule } from '@angular/forms';
import { AdminComponent } from './admin/admin.component';
import { AdminModule } from './admin/admin.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { CKEditorModule } from 'ckeditor4-angular';
import { EditorModule } from '@tinymce/tinymce-angular';
import { ConfirmEmailComponent } from './Client/register/confirm-email/confirm-email.component';
import { RouterModule } from '@angular/router';
import { ForgotPasswordComponent } from './Client/password/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './Client/password/reset-password/reset-password.component';
import { HomeComponent } from './Client/home/home.component';
import { ErrorPageComponent } from './error-page/error-page.component';
import { OAuthModule, OAuthService } from 'angular-oauth2-oidc';
import { FacebookLoginProvider, GoogleLoginProvider, SocialAuthServiceConfig, SocialLoginModule, GoogleSigninButtonModule } from '@abacritt/angularx-social-login';
import { AuthInterceptor } from './interceptor/auth.interceptor';
import { ViewDetailComponent } from './Client/home/view-detail/view-detail.component';
import { TrendingViewComponent } from './Client/home/trending-view/trending-view.component';
import { FooterComponent } from './Client/footer/footer.component';
import { NavbarClientComponent } from './Client/navbar/navbar-client/navbar-client.component';
import { NavbarAuthenComponent } from './Client/navbar/navbar-authen/navbar-authen.component';
import { ToastMessageComponent } from './Client/toast-message/toast-message.component';
import { NotificationComponent } from './Client/notification/notification.component';
import { ChatBoxComponent } from './Client/chat-box/chat-box.component';



@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    AdminComponent,
    ConfirmEmailComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent,
    HomeComponent,
    ErrorPageComponent,
    ViewDetailComponent,
    TrendingViewComponent,
    FooterComponent,
    NavbarClientComponent,
    NavbarAuthenComponent,
    ToastMessageComponent,
    NotificationComponent,
    ChatBoxComponent,

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
    EditorModule,
    RouterModule,
    OAuthModule.forRoot(),
    SocialLoginModule,
    GoogleSigninButtonModule
  ],
  providers: [
    provideClientHydration(),
    provideHttpClient(withFetch()),
    OAuthService,
    { 
      provide: 'SocialAuthServiceConfig',
      useValue: {
        autoLogin: false,
        lang: 'en',
        providers: [
          {
            id: GoogleLoginProvider.PROVIDER_ID,
            provider: new GoogleLoginProvider(
              '938095493121-gf602qo4lrm6r0bb0pgqu5aafsn1prrq.apps.googleusercontent.com'
            )
          },
          {
            id: FacebookLoginProvider.PROVIDER_ID,
            provider: new FacebookLoginProvider('1124693869096012')
          }
        ],
        onError: (err) => {
          console.error(err);
        }
      } as SocialAuthServiceConfig,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
