import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LatestGamesReviewsComponent } from './latest-games-reviews.component';

describe('LatestGamesReviewsComponent', () => {
  let component: LatestGamesReviewsComponent;
  let fixture: ComponentFixture<LatestGamesReviewsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LatestGamesReviewsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(LatestGamesReviewsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
