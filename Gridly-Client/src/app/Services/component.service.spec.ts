import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ComponentModel } from '../Models/Component.Model';
import { ComponentEndpointService } from './endpoints/component.endpoint.service';
import { ComponentService } from './component.service';

describe('ComponentService', () => {
  let service: ComponentService;

  const componentA: ComponentModel = {
    id: 1,
    indexPosition: 1,
    name: 'Alpha',
    url: 'https://alpha.example',
    iconData: { name: 'dashboard', type: 'svg', base64Data: 'abc', materialIcon: 'dashboard' },
    componentSettings: { width: 250, height: 250, imageHidden: false, titleHidden: false },
  };
  const componentB: ComponentModel = {
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

  beforeEach(async () => {
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
    await Promise.resolve();
  });

  it('loads components once on construction', () => {
    expect(endpointMock.get).toHaveBeenCalledTimes(1);
  });

  it('returns the matching component by id from the component state', () => {
    const result = service.getComponentById(1);
    expect(result).toEqual(componentA);
  });

  it('delegates add, edit, and delete to the endpoint service', async () => {
    await service.add(componentB);
    await service.edit(componentB);
    await service.delete(1);

    expect(endpointMock.add).toHaveBeenCalledWith(componentB);
    expect(endpointMock.edit).toHaveBeenCalledWith({
      editComponent: componentB,
      selectedDropDownIconValue: 2,
    });
    expect(endpointMock.delete).toHaveBeenCalledWith(1);
  });

  it('recognizes icon rendering modes correctly', () => {
    expect(service.IconDataSet(componentA)).toBe(true);
    expect(service.MaterialIconSet(componentA)).toBe(true);
    expect(service.IconUrlSet(componentB)).toBe(true);
  });

  it('validates component base data with the shared regex rules', () => {
    expect(service.CheckComponentData(componentA)).toBe(true);
    expect(service.CheckComponentData({ ...componentA, url: 'not-a-url' })).toBe(false);
    expect(service.CheckComponentData({ ...componentA, name: '' })).toBe(false);
  });
});
