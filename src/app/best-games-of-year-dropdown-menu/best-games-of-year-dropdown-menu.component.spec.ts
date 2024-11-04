import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BestGamesOfYearDropdownMenuComponent } from './best-games-of-year-dropdown-menu.component';

describe('BestGamesOfYearDropdownMenuComponent', () => {
  let component: BestGamesOfYearDropdownMenuComponent;
  let fixture: ComponentFixture<BestGamesOfYearDropdownMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BestGamesOfYearDropdownMenuComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(BestGamesOfYearDropdownMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
