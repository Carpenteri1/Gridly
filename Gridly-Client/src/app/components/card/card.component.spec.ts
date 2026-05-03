import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { CardModel } from '../../models/card.Model';
import { CardService } from '../../services/card.service';
import { ComponentRulesService } from '../../services/card-rules.service';
import { GridService } from '../../services/grid.service';
import { CardComponent } from './card.component';

type CardComponentTestHarness = CardComponent & {
  edit(card: CardModel): void;
  remove(id: number): void;
  hasMaterialIcon(card: CardModel): boolean;
};

describe('CardComponent', () => {
  let fixture: ComponentFixture<CardComponent>;
  let card: CardComponent;

  const currentCard: CardModel = {
    id: 7,
    indexPosition: 1,
    name: 'Weather',
    url: 'https://weather.example',
    iconData: {
      name: '',
      type: '',
      base64Data: '',
      materialIcon: 'cloud',
    },
    settings: { width: 250, height: 250, imageHidden: false, titleHidden: false },
  };

  const cardServiceMock = {
    currentcard: jest.fn(() => [currentCard]),
    currentComponents: jest.fn(() => [currentCard]),
    delete: jest.fn(),
    edit: jest.fn(),
  };

  const componentRulesServiceMock = {
    hasMaterialIcon: jest.fn(() => true),
  };

  const editMode = signal(true);
  const gridServiceMock = { editMode: editMode.asReadonly() };

  beforeEach(async () => {
    jest.clearAllMocks();

    await TestBed.configureTestingModule({
      imports: [CardComponent],
      providers: [
        { provide: CardService, useValue: cardServiceMock },
        { provide: ComponentRulesService, useValue: componentRulesServiceMock },
        { provide: GridService, useValue: gridServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CardComponent);
    card = fixture.componentInstance;
    fixture.componentRef.setInput('card', currentCard);
    fixture.detectChanges();
  });

  it('renders the current card name and material icon', () => {
    const element = fixture.nativeElement as HTMLElement;

    expect(element.querySelector('mat-icon')?.textContent).toContain('cloud');
    expect(element.textContent).toContain('Weather');
  });

  it('opens the edit and delete dialogs from the component methods', () => {
    card.openEditDialog();
    card.openDeleteDialog();

    expect(card.isEditModalOpen).toBe(true);
    expect(card.isDeleteModalOpen).toBe(true);
  });

  it('closes both dialogs when the matching modal id is emitted', () => {
    card.isEditModalOpen = true;
    card.isDeleteModalOpen = true;

    card.handleModalChange(7);

    expect(card.isEditModalOpen).toBe(false);
    expect(card.isDeleteModalOpen).toBe(false);
  });

  it('delegates edit and remove actions to the component service', () => {
    (card as CardComponentTestHarness).edit(currentCard);
    (card as CardComponentTestHarness).remove(7);

    expect(cardServiceMock.edit).toHaveBeenCalledWith(currentCard);
    expect(cardServiceMock.delete).toHaveBeenCalledWith(7);
  });
  
  it('hasMaterialIcon returns the value from the component rules service', () => {
    const result = (card as CardComponentTestHarness).hasMaterialIcon(currentCard);
    expect(componentRulesServiceMock.hasMaterialIcon).toHaveBeenCalledWith(currentCard);
    expect(result).toBe(true);
  });

});
