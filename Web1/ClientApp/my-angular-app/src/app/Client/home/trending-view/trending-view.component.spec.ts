import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrendingViewComponent } from './trending-view.component';

describe('TrendingViewComponent', () => {
  let component: TrendingViewComponent;
  let fixture: ComponentFixture<TrendingViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TrendingViewComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(TrendingViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
