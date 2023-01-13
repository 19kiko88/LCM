import { TestBed } from '@angular/core/testing';

import { BsEighteenService } from './bs-eighteen.service';

describe('BsEighteenService', () => {
  let service: BsEighteenService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BsEighteenService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
