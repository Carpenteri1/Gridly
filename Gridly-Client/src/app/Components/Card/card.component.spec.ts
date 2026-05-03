import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { CardModel } from '../../Models/Card.Model';
import { ComponentService } from '../../Services/component.service';
import { ComponentRulesService } from '../../Services/component-rules.service';
import { GridService } from '../../Services/grid.service';
import { CardComponent } from './card.component';

type CardComponentTestHarness = CardComponent & {
  edit(card: CardModel): void;
  remove(id: number): void;
  hasMaterialIcon(card: CardModel): boolean;
};

describe('CardComponent', () => {
  let fixture: ComponentFixture<CardComponent>;
  let cardComponent: CardComponent;

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
    componentSettings: { width: 250, height: 250, imageHidden: false, titleHidden: false },
  };

  const componentServiceMock = {
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
        { provide: ComponentService, useValue: componentServiceMock },
        { provide: ComponentRulesService, useValue: componentRulesServiceMock },
        { provide: GridService, useValue: gridServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CardComponent);
    cardComponent = fixture.componentInstance;
    fixture.componentRef.setInput('card', currentCard);
    fixture.detectChanges();
  });

  it('renders the current card name and material icon', () => {
    const element = fixture.nativeElement as HTMLElement;

    expect(element.querySelector('mat-icon')?.textContent).toContain('cloud');
    expect(element.textContent).toContain('Weather');
  });

  it('opens the edit and delete dialogs from the component methods', () => {
    cardComponent.openEditDialog();
    cardComponent.openDeleteDialog();

    expect(cardComponent.isEditModalOpen).toBe(true);
    expect(cardComponent.isDeleteModalOpen).toBe(true);
  });

  it('closes both dialogs when the matching modal id is emitted', () => {
    cardComponent.isEditModalOpen = true;
    cardComponent.isDeleteModalOpen = true;

    cardComponent.handleModalChange(7);

    expect(cardComponent.isEditModalOpen).toBe(false);
    expect(cardComponent.isDeleteModalOpen).toBe(false);
  });

  it('delegates edit and remove actions to the component service', () => {
    (cardComponent as CardComponentTestHarness).edit(currentCard);
    (cardComponent as CardComponentTestHarness).remove(7);

    expect(componentServiceMock.edit).toHaveBeenCalledWith(currentCard);
    expect(componentServiceMock.delete).toHaveBeenCalledWith(7);
  });
  
  it('hasMaterialIcon returns the value from the component rules service', () => {
    const result = (cardComponent as CardComponentTestHarness).hasMaterialIcon(currentCard);
    expect(componentRulesServiceMock.hasMaterialIcon).toHaveBeenCalledWith(currentCard);
    expect(result).toBe(true);
  });

});
