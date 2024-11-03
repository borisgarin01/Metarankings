import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BestInThisMonthSlideshowComponent } from './best-in-this-month-slideshow.component';

describe('BestInThisMonthSlideshowComponent', () => {
  let component: BestInThisMonthSlideshowComponent;
  let fixture: ComponentFixture<BestInThisMonthSlideshowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BestInThisMonthSlideshowComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(BestInThisMonthSlideshowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
