import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TabBsComponent } from './tab-bs.component';

describe('TabBsComponent', () => {
  let component: TabBsComponent;
  let fixture: ComponentFixture<TabBsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TabBsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TabBsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
