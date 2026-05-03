import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { CardModel } from '../models/card.Model';
import { ComponentEndpointService } from './endpoints/component.endpoint.service';
import { ComponentService } from './component.service';

describe('ComponentService', () => {
  let service: ComponentService;

  const componentA: CardModel = {
    id: 1,
    indexPosition: 1,
    name: 'Alpha',
    url: 'https://alpha.example',
    iconData: { name: 'dashboard', type: 'svg', base64Data: 'abc', materialIcon: 'dashboard' },
    componentSettings: { width: 250, height: 250, imageHidden: false, titleHidden: false },
  };
  const componentB: CardModel = {
    id: 2,
    indexPosition: 2,
    name: 'Beta',
    url: 'https://beta.example',
    iconUrl: 'https://cdn.example/icon.png',
    componentSettings: { width: 250, height: 250, imageHidden: false, titleHidden: false },
  };

  const endpointMock = {
    add: jest.fn(),
    batchEdit: jest.fn(),
    delete: jest.fn(),
    edit: jest.fn(),
    get: jest.fn(),
    getById: jest.fn(),
  };

  beforeEach(() => {
    jest.clearAllMocks();
    endpointMock.get.mockReturnValue(of([componentA, componentB]));
    endpointMock.getById.mockReturnValue(of(componentA));
    endpointMock.add.mockReturnValue(of(componentB));
    endpointMock.edit.mockReturnValue(of(componentB));
    endpointMock.delete.mockReturnValue(of(componentA));

    TestBed.configureTestingModule({
      providers: [
        ComponentService,
        { provide: ComponentEndpointService, useValue: endpointMock },
      ],
    });

    service = TestBed.inject(ComponentService);
  });

  it('loads components once on construction', () => {
    expect(service.currentComponents()).toEqual([componentA, componentB]);
    expect(endpointMock.get).toHaveBeenCalledTimes(2);
  });

  it('refreshes the component stream on demand', () => {
    service.refresh();

    expect(endpointMock.get).toHaveBeenCalledTimes(3);
  });

  it('delegates add, edit, delete, and getById to the endpoint service', async () => {
    await service.add(componentB);
    await service.edit(componentB);
    await expect(service.getById(1)).resolves.toEqual(componentA);
    await service.delete(1);

    expect(endpointMock.add).toHaveBeenCalledWith(componentB);
    expect(endpointMock.edit).toHaveBeenCalledWith({
      editComponent: componentB,
      selectedDropDownIconValue: 2,
    });
    expect(endpointMock.getById).toHaveBeenCalledWith(1);
    expect(endpointMock.delete).toHaveBeenCalledWith(1);
    expect(endpointMock.get).toHaveBeenCalledTimes(5);
  });
});
