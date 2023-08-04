import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BsEighteenComponent } from './bs-eighteen.component';

describe('BsEighteenComponent', () => {
  let component: BsEighteenComponent;
  let fixture: ComponentFixture<BsEighteenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BsEighteenComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BsEighteenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
