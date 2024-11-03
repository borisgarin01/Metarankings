import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewMoviesComponent } from './new-movies.component';

describe('NewMoviesComponent', () => {
  let component: NewMoviesComponent;
  let fixture: ComponentFixture<NewMoviesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NewMoviesComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(NewMoviesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
