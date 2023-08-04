import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManualCloseComponent } from './manual-close.component';

describe('ManualCloseComponent', () => {
  let component: ManualCloseComponent;
  let fixture: ComponentFixture<ManualCloseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManualCloseComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManualCloseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
