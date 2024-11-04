import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SidebarRightColumnComponent } from './sidebar-right-column.component';

describe('SidebarRightColumnComponent', () => {
  let component: SidebarRightColumnComponent;
  let fixture: ComponentFixture<SidebarRightColumnComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SidebarRightColumnComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SidebarRightColumnComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
