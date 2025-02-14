import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NavbarAuthenComponent } from './navbar-authen.component';

describe('NavbarAuthenComponent', () => {
  let component: NavbarAuthenComponent;
  let fixture: ComponentFixture<NavbarAuthenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [NavbarAuthenComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(NavbarAuthenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
