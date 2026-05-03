import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { CardModel } from '../../models/card.Model';
import { CardService } from '../../services/card.service';
import { ComponentRulesService } from '../../services/card-rules.service';
import { GridService } from '../../services/grid.service';
import { CardComponent } from './card.component';

type CardComponentFixture = CardComponent & {
  edit(card: CardModel): void;
  remove(id: number): void;
  hasMaterialIcon(card: CardModel): boolean;
};

describe('CardComponent', () => {
  let fixture: ComponentFixture<CardComponent>;
  let createCardComponent: CardComponent;

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
    delete: jest.fn(),
    edit: jest.fn(),
  };

  const cardComponentRulesServiceMock = {
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
        { provide: ComponentRulesService, useValue: cardComponentRulesServiceMock },
        { provide: GridService, useValue: gridServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CardComponent);
    createCardComponent = fixture.componentInstance;
    fixture.componentRef.setInput('card', currentCard);
    fixture.detectChanges();
  });

  it('renders the current card name and material icon', () => {
    const element = fixture.nativeElement as HTMLElement;

    expect(element.querySelector('mat-icon')?.textContent).toContain('cloud');
    expect(element.textContent).toContain('Weather');
  });

  it('opens the edit and delete dialogs from the component methods', () => {
    createCardComponent.openEditDialog();
    createCardComponent.openDeleteDialog();

    expect(createCardComponent.isEditDialogOpen).toBe(true);
    expect(createCardComponent.isDeleteDialogOpen).toBe(true);
  });

  it('closes both dialogs when the matching modal id is emitted', () => {
    createCardComponent.isEditDialogOpen = true;
    createCardComponent.isDeleteDialogOpen = true;

    createCardComponent.handleDialogChange(7);

    expect(createCardComponent.isEditDialogOpen).toBe(false);
    expect(createCardComponent.isDeleteDialogOpen).toBe(false);
  });

  it('delegates edit and remove actions to the component service', () => {
    (createCardComponent as CardComponentFixture).edit(currentCard);
    (createCardComponent as CardComponentFixture).remove(7);

    expect(cardServiceMock.edit).toHaveBeenCalledWith(currentCard);
    expect(cardServiceMock.delete).toHaveBeenCalledWith(7);
  });
  
  it('hasMaterialIcon returns the value from the component rules service', () => {
    const result = (createCardComponent as CardComponentFixture).hasMaterialIcon(currentCard);
    expect(cardComponentRulesServiceMock.hasMaterialIcon).toHaveBeenCalledWith(currentCard);
    expect(result).toBe(true);
  });

});
