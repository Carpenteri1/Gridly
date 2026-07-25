import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { CardModel } from '../../models/card.Model';
import { CardRulesService } from '../../services/card_services/card-rules.service';
import { GridService } from '../../services/grid_services/grid.service';
import { CardComponent } from './card.component';

type CardComponentFixture = CardComponent & {
  edit(card: CardModel): void;
  remove(card: CardModel): void;
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
    iconData: { name: '', type: '', base64Data: '', materialIcon: 'cloud' },
    settings: { width: 250, height: 250, imageHidden: false, titleHidden: false },
  };

  const cardRulesServiceMock = {
    hasMaterialIcon: jest.fn(() => true),
  };

  const editMode = signal(true);
  const gridServiceMock = {
    inEditMode: editMode.asReadonly(),
    updateCardInView: jest.fn(),
    removeCardFromView: jest.fn(),
  };

  beforeEach(async () => {
    jest.clearAllMocks();

    await TestBed.configureTestingModule({
      imports: [CardComponent],
      providers: [
        { provide: CardRulesService, useValue: cardRulesServiceMock },
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
  /*TODO add test later
  it('opens the edit and delete dialogs from the card methods', () => {
    createCardComponent.openEditDialog();
    createCardComponent.openDeleteDialog();
    createCardComponent.openAddProviderKeyDialog()

    expect(createCardComponent.isEditDialogOpen).toBe(true);
    expect(createCardComponent.isDeleteDialogOpen).toBe(true);
    expect(createCardComponent.isProviderDialogOpen).toBe(true);
  });


  it('closes both dialogs when the matching dialog id is emitted', () => {
    createCardComponent.isEditDialogOpen = true;
    createCardComponent.isDeleteDialogOpen = true;

    createCardComponent.handleDialogChange(7);

    expect(createCardComponent.isEditDialogOpen).toBe(false);
    expect(createCardComponent.isDeleteDialogOpen).toBe(false);
  });*/

  it('delegates edit and remove actions to the grid service', () => {
    (createCardComponent as CardComponentFixture).edit(currentCard);
    (createCardComponent as CardComponentFixture).remove(currentCard);

    expect(gridServiceMock.updateCardInView).toHaveBeenCalledWith(currentCard, currentCard);
    expect(gridServiceMock.removeCardFromView).toHaveBeenCalledWith(currentCard);
  });

  it('hasMaterialIcon returns the value from the card rules service', () => {
    const result = (createCardComponent as CardComponentFixture).hasMaterialIcon(currentCard);
    expect(cardRulesServiceMock.hasMaterialIcon).toHaveBeenCalledWith(currentCard);
    expect(result).toBe(true);
  });

});
