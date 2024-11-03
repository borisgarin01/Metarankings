import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MoviesDropDownMainPageComponent } from './movies-drop-down-main-page.component';

describe('MoviesDropDownMainPageComponent', () => {
  let component: MoviesDropDownMainPageComponent;
  let fixture: ComponentFixture<MoviesDropDownMainPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MoviesDropDownMainPageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(MoviesDropDownMainPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
