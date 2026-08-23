import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { urlConstants } from '../../constants/url.constants';
import { RowColumnModel } from '../../models/rowColumn.Model';
import { RowColumnEndpointService } from './rowColumn.endpoint.service';

describe('RowColumnEndpointService', () => {
  let service: RowColumnEndpointService;
  let httpMock: HttpTestingController;

  const row: RowColumnModel = { id: 1, cards: [], rowPosition: 1, rowWidth: 1000 };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(RowColumnEndpointService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('sends a GET request to the row endpoint and returns the response', () => {
    let result: RowColumnModel[] | null | undefined;

    service.get().subscribe((res) => (result = res));

    const req = httpMock.expectOne(urlConstants.rowColumn.get);
    expect(req.request.method).toBe('GET');
    req.flush([row]);

    expect(result).toEqual([row]);
  });

  it('sends a POST request with the rows to batch save, and returns the saved rows', () => {
    let result: RowColumnModel[] | undefined;

    service.batchSave([row]).subscribe((res) => (result = res));

    const req = httpMock.expectOne(urlConstants.rowColumn.batchSave);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual([row]);
    req.flush([row]);

    expect(result).toEqual([row]);
  });
});
