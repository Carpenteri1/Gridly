import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { CardModel } from '../../models/card.Model';
import { CardEndpointService } from '../endpoint_services/card.endpoint.service';
import { CardService } from './card.service';

describe('CardService', () => {
  let service: CardService;

  const cardA: CardModel = {
    id: 1,
    indexPosition: 1,
    name: 'Alpha',
    url: 'https://alpha.example',
    iconData: { name: 'dashboard', type: 'svg', base64Data: 'abc', materialIcon: 'dashboard' },
    settings: { width: 250, height: 250, imageHidden: false, titleHidden: false },
  };
  const cardB: CardModel = {
    id: 2,
    indexPosition: 2,
    name: 'Beta',
    url: 'https://beta.example',
    iconUrl: 'https://cdn.example/icon.png',
    settings: { width: 250, height: 250, imageHidden: false, titleHidden: false },
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
    endpointMock.get.mockReturnValue(of([cardA, cardB]));
    endpointMock.getById.mockReturnValue(of(cardA));
    endpointMock.add.mockReturnValue(of(cardB));
    endpointMock.edit.mockReturnValue(of(cardB));
    endpointMock.delete.mockReturnValue(of(cardA));

    TestBed.configureTestingModule({
      providers: [
        CardService,
        { provide: CardEndpointService, useValue: endpointMock },
      ],
    });

    service = TestBed.inject(CardService);
  });

  it('loads cards once on construction', () => {
    expect(service.currentCards()).toEqual([cardA, cardB]);
    expect(endpointMock.get).toHaveBeenCalledTimes(1);
  });

  it('refreshes the card stream on demand', () => {
    service.refresh();

    expect(endpointMock.get).toHaveBeenCalledTimes(2);
  });

  it('delegates add, edit, delete, and getById to the endpoint service', async () => {
    await service.add(cardB);
    await service.edit(cardB);
    await expect(service.getById(1)).resolves.toEqual(cardA);
    await service.delete(1);

    expect(endpointMock.add).toHaveBeenCalledWith(cardB);
    expect(endpointMock.edit).toHaveBeenCalledWith({
      editCard: cardB,
      selectedDropDownIconValue: 2,
    });
    expect(endpointMock.getById).toHaveBeenCalledWith(1);
    expect(endpointMock.delete).toHaveBeenCalledWith(1);
    expect(endpointMock.get).toHaveBeenCalledTimes(4);
  });

  it('groups cards into rows when a row exceeds the max width', () => {
    const rows = service.toRows([cardA, cardB], 400);

    expect(rows.map((row) => row.map((card) => card.id))).toEqual([[1], [2]]);
    expect(rows.flat()).toEqual([
      { ...cardA, indexPosition: 1, rowPosition: 1 },
      { ...cardB, indexPosition: 2, rowPosition: 1 },
    ]);
  });

  it('updates row and row position when rows are changed', () => {
    service.setRows([[cardA], [cardB]], 1000);

    expect(service.currentCards()).toEqual([
      { ...cardA, indexPosition: 1, rowPosition: 1 },
      { ...cardB, indexPosition: 2, rowPosition: 1 },
    ]);
  });

  it('removes empty rows when cards move out of them', () => {
    service.setRows([[], [cardB, cardA]], 1000);

    expect(service.currentCards()).toEqual([
      { ...cardB, indexPosition: 1, rowPosition: 1 },
      { ...cardA, indexPosition: 1, rowPosition: 2 },
    ]);
  });
});
