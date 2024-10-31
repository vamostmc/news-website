import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchFilterPostComponent } from './search-filter-post.component';

describe('SearchFilterPostComponent', () => {
  let component: SearchFilterPostComponent;
  let fixture: ComponentFixture<SearchFilterPostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SearchFilterPostComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SearchFilterPostComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
