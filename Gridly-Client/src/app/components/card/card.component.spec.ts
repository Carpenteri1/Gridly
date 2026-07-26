import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { CardModel } from '../../models/card.Model';
import { CardRulesService } from '../../services/card_services/card-rules.service';
import { GridService } from '../../services/grid_services/grid.service';
import { ProviderKeysService } from '../../services/provider_key_services/provider-keys.service';
import { ProviderKeyStatusModel } from '../../models/providerKeyStatus.Model';
import { ProviderKeyStatus } from '../../enums/provider-key-status.enum';
import { CardTypes } from '../../enums/card.types.enum';
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

  const providerKeyStatus = signal<ProviderKeyStatusModel | null>(null);
  const providerKeysServiceMock = {
    currentStatus: providerKeyStatus.asReadonly(),
    refreshStatus: jest.fn(),
    save: jest.fn(),
    onKeySaved: jest.fn(),
  };

  const createComponent = (card: CardModel) => {
    fixture = TestBed.createComponent(CardComponent);
    createCardComponent = fixture.componentInstance;
    fixture.componentRef.setInput('card', card);
    fixture.detectChanges();
  };

  beforeEach(async () => {
    jest.clearAllMocks();
    providerKeyStatus.set(null);

    await TestBed.configureTestingModule({
      imports: [CardComponent],
      providers: [
        { provide: CardRulesService, useValue: cardRulesServiceMock },
        { provide: GridService, useValue: gridServiceMock },
        { provide: ProviderKeysService, useValue: providerKeysServiceMock },
      ],
    }).compileComponents();

    createComponent(currentCard);
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

  describe('provider key button', () => {
    const weatherCard: CardModel = { ...currentCard, type: CardTypes.Weather };

    const keyButton = () =>
      (fixture.nativeElement as HTMLElement).querySelector('.bi-key');

    it('is shown on a weather card when no key is stored', () => {
      providerKeyStatus.set({ exists: false, status: ProviderKeyStatus.Unknown });
      createComponent(weatherCard);

      expect(createCardComponent.showProviderKeyButton()).toBe(true);
      expect(keyButton()).not.toBeNull();
    });

    it('is shown on a weather card when the stored key is invalid', () => {
      providerKeyStatus.set({ exists: true, status: ProviderKeyStatus.Invalid });
      createComponent(weatherCard);

      expect(createCardComponent.showProviderKeyButton()).toBe(true);
      expect(keyButton()).not.toBeNull();
    });

    it('is hidden on a weather card once the stored key is valid', () => {
      providerKeyStatus.set({ exists: true, status: ProviderKeyStatus.Valid });
      createComponent(weatherCard);

      expect(createCardComponent.showProviderKeyButton()).toBe(false);
      expect(keyButton()).toBeNull();
    });

    it('is hidden on cards that are not weather cards', () => {
      providerKeyStatus.set({ exists: false, status: ProviderKeyStatus.Unknown });
      createComponent({ ...currentCard, type: CardTypes.Empty });

      expect(createCardComponent.showProviderKeyButton()).toBe(false);
      expect(keyButton()).toBeNull();
    });
  });

});
