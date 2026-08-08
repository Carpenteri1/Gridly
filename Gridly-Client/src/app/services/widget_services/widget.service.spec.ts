import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { WidgetEndpointService } from '../endpoint_services/widget.endpoint.service';
import { Widget } from '../../interfaces/widget.Interface';
import { WidgetService } from './widget.service';

describe('WidgetService', () => {
  let service: WidgetService;

  const widgets: Widget[] = [{ id: 1, widgetType: 'Empty', label: 'Empty', description: '', icon: '' }];

  const endpointMock = {
    get: jest.fn(() => of(widgets)),
  };

  beforeEach(() => {
    jest.clearAllMocks();
    endpointMock.get.mockReturnValue(of(widgets));

    TestBed.configureTestingModule({
      providers: [
        WidgetService,
        { provide: WidgetEndpointService, useValue: endpointMock },
      ],
    });

    service = TestBed.inject(WidgetService);
  });

  it('resolves the widgets from the endpoint service', async () => {
    const result = await service.get();

    expect(endpointMock.get).toHaveBeenCalledTimes(1);
    expect(result).toEqual(widgets);
  });
});
