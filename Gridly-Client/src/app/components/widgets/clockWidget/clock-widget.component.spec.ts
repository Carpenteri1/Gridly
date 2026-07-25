import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CardModel } from '../../../models/card.Model';
import { ClockService } from '../../../services/clock_services/clock.service';
import { ClockWidgetComponent } from './clock-widget.component';

describe('ClockWidgetComponent', () => {
  let fixture: ComponentFixture<ClockWidgetComponent>;

  const clockServiceMock = {
    resolveClockData: jest.fn(() =>
      Promise.resolve({ timeZone: 'Europe/Stockholm', utcOffsetSeconds: 7200, dstActive: true })),
  };

  const digitalCard: CardModel = {
    id: 1,
    indexPosition: 1,
    name: 'Clock',
    url: '',
    settings: {
      width: 250, height: 250, imageHidden: false, titleHidden: false,
      timeZone: 'Europe/Stockholm', displayFormat: 'digital',
    },
  };

  beforeEach(async () => {
    jest.clearAllMocks();
    jest.useFakeTimers();

    await TestBed.configureTestingModule({
      imports: [ClockWidgetComponent],
      providers: [{ provide: ClockService, useValue: clockServiceMock }],
    }).compileComponents();

    fixture = TestBed.createComponent(ClockWidgetComponent);
    fixture.componentRef.setInput('card', digitalCard);
  });

  afterEach(() => {
    jest.useRealTimers();
  });

  it('fetches the offset once on init and renders the digital time', async () => {
    fixture.detectChanges();
    await Promise.resolve();
    await Promise.resolve();
    fixture.detectChanges();

    expect(clockServiceMock.resolveClockData).toHaveBeenCalledTimes(1);
    expect(clockServiceMock.resolveClockData).toHaveBeenCalledWith('Europe/Stockholm');

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('.clock-digital-time')).not.toBeNull();
    expect(element.querySelector('.clock-timezone-label')?.textContent).toContain('Europe/Stockholm');
  });

  it('ticks locally every second without calling the service again', async () => {
    fixture.detectChanges();
    await Promise.resolve();
    await Promise.resolve();

    jest.advanceTimersByTime(5000);

    expect(clockServiceMock.resolveClockData).toHaveBeenCalledTimes(1);
  });

  it('renders the analog clock face when displayFormat is analog', async () => {
    const analogCard: CardModel = {
      ...digitalCard,
      settings: { ...digitalCard.settings!, displayFormat: 'analog' },
    };
    fixture.componentRef.setInput('card', analogCard);
    fixture.detectChanges();
    await Promise.resolve();
    await Promise.resolve();
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('svg.clock-face')).not.toBeNull();
    expect(element.querySelector('.clock-digital-time')).toBeNull();
  });

  it('clears the timers on destroy', async () => {
    fixture.detectChanges();
    await Promise.resolve();
    await Promise.resolve();

    const clearSpy = jest.spyOn(global, 'clearInterval');
    fixture.destroy();

    expect(clearSpy).toHaveBeenCalledTimes(2);
  });
});
