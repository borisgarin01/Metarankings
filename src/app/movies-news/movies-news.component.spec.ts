import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MoviesNewsComponent } from './movies-news.component';

describe('MoviesNewsComponent', () => {
  let component: MoviesNewsComponent;
  let fixture: ComponentFixture<MoviesNewsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MoviesNewsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(MoviesNewsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
