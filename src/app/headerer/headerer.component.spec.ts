import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HeadererComponent } from './headerer.component';

describe('HeadererComponent', () => {
  let component: HeadererComponent;
  let fixture: ComponentFixture<HeadererComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HeadererComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(HeadererComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
