import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { CardModel } from '../../models/card.Model';
import { RowColumnEndpointService } from '../endpoint_services/rowColumn.endpoint.service';
import { GridService } from './grid.service';

describe('GridService', () => {
  let service: GridService;

  const endpointMock = {
    get: jest.fn(),
    batchSave: jest.fn(),
  };

  const createCard = (id: number, width = 250): CardModel => ({
    id,
    indexPosition: 1,
    rowPosition: 1,
    name: `Card ${id}`,
    url: `https://card-${id}.example`,
    settings: { width, height: 250, imageHidden: false, titleHidden: false },
  });

  beforeEach(() => {
    jest.clearAllMocks();
    endpointMock.get.mockReturnValue(of([]));
    endpointMock.batchSave.mockReturnValue(of([]));

    TestBed.configureTestingModule({
      providers: [
        GridService,
        { provide: RowColumnEndpointService, useValue: endpointMock },
      ],
    });

    service = TestBed.inject(GridService);
  });

  it('creates a new row when adding a card and no rows exist', () => {
    const card = createCard(0);

    service.addCardToFirstAvailableRow(card);

    expect(service.currentRowColumns()).toEqual([
      {
        id: 0,
        rowPosition: 1,
        rowWidth: 0,
        cards: [{
          ...card,
          id: 0,
          indexPosition: 1,
          rowPosition: 1,
          rowColumnId: 0,
        }],
      },
    ]);
  });

  it('treats an empty backend response as no rows', () => {
    endpointMock.get.mockReturnValue(of(null));

    service.refresh();

    expect(service.currentRowColumns()).toEqual([]);
  });

  it('keeps adding cards to the first row with space before creating a new row', () => {
    service.setAvailableRowWidth(600);

    service.addCardToFirstAvailableRow(createCard(0));
    service.addCardToFirstAvailableRow(createCard(0));
    service.addCardToFirstAvailableRow(createCard(0));

    expect(service.currentRowColumns().map(row => row.cards.length)).toEqual([2, 1]);
    expect(service.currentRowColumns()[0].cards.map(card => card.indexPosition)).toEqual([1, 2]);
    expect(service.currentRowColumns()[1].cards[0].indexPosition).toBe(1);
  });

  it('skips full rows and adds to the first later row with space', () => {
    const firstRowCards = [createCard(1), createCard(2)];
    const secondRowCard = createCard(3);
    service.setRowsForView([
      { id: 1, rowPosition: 1, cards: firstRowCards },
      { id: 2, rowPosition: 2, cards: [secondRowCard] },
    ]);
    service.setAvailableRowWidth(600);

    service.addCardToFirstAvailableRow(createCard(0));

    expect(service.currentRowColumns().map(row => row.cards.length)).toEqual([2, 2]);
    expect(service.currentRowColumns()[1].cards[1].indexPosition).toBe(2);
  });

  it('does not reflow backend rows when measuring available row width', () => {
    const firstRowCards = [createCard(1), createCard(2)];
    const secondRowCard = createCard(3);
    service.setRowsForView([
      { id: 1, rowPosition: 1, cards: firstRowCards },
      { id: 2, rowPosition: 2, cards: [secondRowCard] },
    ]);

    service.setAvailableRowWidth(2000);

    expect(service.currentRowColumns().map(row => row.cards.map(card => card.id))).toEqual([[1, 2], [3]]);
  });

  it('removes cards from the view and renumbers remaining rows and cards', () => {
    const firstCard = createCard(1);
    const removedCard = createCard(2);
    const lastCard = createCard(3);
    service.setRowsForView([
      { id: 1, rowPosition: 1, cards: [firstCard, removedCard] },
      { id: 2, rowPosition: 2, cards: [lastCard] },
    ]);

    service.removeCardFromView(removedCard);

    expect(service.currentRowColumns()).toEqual([
      {
        id: 1,
        rowPosition: 1,
        rowWidth: 0,
        cards: [{ ...firstCard, indexPosition: 1, rowPosition: 1, rowColumnId: 1 }],
      },
      {
        id: 2,
        rowPosition: 2,
        rowWidth: 0,
        cards: [{ ...lastCard, indexPosition: 1, rowPosition: 2, rowColumnId: 2 }],
      },
    ]);
  });

  it('saves normalized rows through the row batchSave endpoint', async () => {
    const card = createCard(1);
    service.setRowsForView([{ id: 1, rowPosition: 1, cards: [card] }]);

    await service.batchSave(service.currentRowColumns());

    expect(endpointMock.batchSave).toHaveBeenCalledWith([
      {
        id: 1,
        rowPosition: 1,
        rowWidth: 0,
        cards: [{ ...card, indexPosition: 1, rowPosition: 1, rowColumnId: 1 }],
      },
    ]);
  });
});
