import { TestBed } from '@angular/core/testing';

import { TinTucService } from './tin-tuc.service';

describe('TinTucService', () => {
  let service: TinTucService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TinTucService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
