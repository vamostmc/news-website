import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManagePostDetailComponent } from './manage-post-detail.component';

describe('ManagePostDetailComponent', () => {
  let component: ManagePostDetailComponent;
  let fixture: ComponentFixture<ManagePostDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ManagePostDetailComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ManagePostDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
