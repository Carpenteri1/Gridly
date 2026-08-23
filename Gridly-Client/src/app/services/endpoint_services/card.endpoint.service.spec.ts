import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { urlConstants } from '../../constants/url.constants';
import { CardModel } from '../../models/card.Model';
import { CardEndpointService } from './card.endpoint.service';

describe('CardEndpointService', () => {
  let service: CardEndpointService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(CardEndpointService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('sends a GET request to the card endpoint and returns the response', () => {
    const cards: CardModel[] = [
      { id: 1, indexPosition: 1, rowPosition: 1, name: 'Alpha', url: 'https://alpha.example' },
    ];
    let result: CardModel[] | undefined;

    service.get().subscribe((res) => (result = res));

    const req = httpMock.expectOne(urlConstants.card.get);
    expect(req.request.method).toBe('GET');
    req.flush(cards);

    expect(result).toEqual(cards);
  });
});
