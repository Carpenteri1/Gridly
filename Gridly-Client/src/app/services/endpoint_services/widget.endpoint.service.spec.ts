import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { urlConstants } from '../../constants/url.constants';
import { Widget } from '../../interfaces/widget.Interface';
import { WidgetEndpointService } from './widget.endpoint.service';

describe('WidgetEndpointService', () => {
  let service: WidgetEndpointService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(WidgetEndpointService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('sends a GET request to the widget endpoint and returns the response', () => {
    const widgets: Widget[] = [{ id: 1, widgetType: 'Empty', label: 'Empty', description: '', icon: '' }];
    let result: Widget[] | undefined;

    service.get().subscribe((res) => (result = res));

    const req = httpMock.expectOne(urlConstants.widget.get);
    expect(req.request.method).toBe('GET');
    req.flush(widgets);

    expect(result).toEqual(widgets);
  });
});
