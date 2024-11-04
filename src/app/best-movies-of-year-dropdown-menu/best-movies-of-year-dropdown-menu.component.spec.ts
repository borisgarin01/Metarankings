import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BestMoviesOfYearDropdownMenuComponent } from './best-movies-of-year-dropdown-menu.component';

describe('BestMoviesOfYearDropdownMenuComponent', () => {
  let component: BestMoviesOfYearDropdownMenuComponent;
  let fixture: ComponentFixture<BestMoviesOfYearDropdownMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BestMoviesOfYearDropdownMenuComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(BestMoviesOfYearDropdownMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
