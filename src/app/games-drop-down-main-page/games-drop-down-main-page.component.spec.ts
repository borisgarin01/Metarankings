import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GamesDropDownMainPageComponent } from './games-drop-down-main-page.component';

describe('GamesDropDownMainPageComponent', () => {
  let component: GamesDropDownMainPageComponent;
  let fixture: ComponentFixture<GamesDropDownMainPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GamesDropDownMainPageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(GamesDropDownMainPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
