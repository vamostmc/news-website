import { TestBed } from '@angular/core/testing';

import { ConfirmMailService } from './confirm-mail.service';

describe('ConfirmMailService', () => {
  let service: ConfirmMailService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ConfirmMailService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
