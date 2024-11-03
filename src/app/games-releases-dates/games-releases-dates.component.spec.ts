import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GamesReleasesDatesComponent } from './games-releases-dates.component';

describe('GamesReleasesDatesComponent', () => {
  let component: GamesReleasesDatesComponent;
  let fixture: ComponentFixture<GamesReleasesDatesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GamesReleasesDatesComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(GamesReleasesDatesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
