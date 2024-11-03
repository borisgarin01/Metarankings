import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SoonAtMoviesComponent } from './soon-at-movies.component';

describe('SoonAtMoviesComponent', () => {
  let component: SoonAtMoviesComponent;
  let fixture: ComponentFixture<SoonAtMoviesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SoonAtMoviesComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SoonAtMoviesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
