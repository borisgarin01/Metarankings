import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GamesNewsComponent } from './games-news.component';

describe('GamesNewsComponent', () => {
  let component: GamesNewsComponent;
  let fixture: ComponentFixture<GamesNewsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GamesNewsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(GamesNewsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
