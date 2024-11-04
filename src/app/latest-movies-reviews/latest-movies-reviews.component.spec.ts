import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LatestMoviesReviewsComponent } from './latest-movies-reviews.component';

describe('LatestMoviesReviewsComponent', () => {
  let component: LatestMoviesReviewsComponent;
  let fixture: ComponentFixture<LatestMoviesReviewsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LatestMoviesReviewsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(LatestMoviesReviewsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
