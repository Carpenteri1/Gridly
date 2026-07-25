import { Component, inject, Input, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModel } from '../../../models/card.Model';
import { ClockService } from '../../../services/clock_services/clock.service';

const TICK_INTERVAL_MS = 1000;
const OFFSET_REFRESH_INTERVAL_MS = 30 * 60 * 1000;

@Component({
  selector: 'app-clock-widget',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './clock-widget.component.html',
  styleUrl: './clock-widget.component.css',
})
export class ClockWidgetComponent implements OnInit, OnDestroy {
  @Input({ required: true }) card!: CardModel;

  #clockService = inject(ClockService);

  protected utcOffsetSeconds = signal(0);
  protected now = signal(new Date());

  private tickHandle?: ReturnType<typeof setInterval>;
  private refreshHandle?: ReturnType<typeof setInterval>;

  protected get timeZone(): string {
    return this.card.settings?.timeZone ?? Intl.DateTimeFormat().resolvedOptions().timeZone;
  }

  protected get displayFormat(): 'digital' | 'analog' {
    return this.card.settings?.displayFormat ?? 'digital';
  }

  ngOnInit(): void {
    this.refreshOffset().then(() => this.tick());
    this.tick();
    this.tickHandle = setInterval(() => this.tick(), TICK_INTERVAL_MS);
    this.refreshHandle = setInterval(() => this.refreshOffset(), OFFSET_REFRESH_INTERVAL_MS);
  }

  ngOnDestroy(): void {
    if (this.tickHandle) clearInterval(this.tickHandle);
    if (this.refreshHandle) clearInterval(this.refreshHandle);
  }

  private async refreshOffset(): Promise<void> {
    const clock = await this.#clockService.resolveClockData(this.timeZone);
    this.utcOffsetSeconds.set(clock.utcOffsetSeconds);
  }

  private tick(): void {
    this.now.set(new Date(Date.now() + this.utcOffsetSeconds() * 1000));
  }

  protected get digitalTime(): string {
    return this.now().toISOString().substring(11, 19);
  }

  protected get hourAngle(): number {
    const d = this.now();
    return (d.getUTCHours() % 12) * 30 + d.getUTCMinutes() * 0.5;
  }

  protected get minuteAngle(): number {
    return this.now().getUTCMinutes() * 6;
  }

  protected get secondAngle(): number {
    return this.now().getUTCSeconds() * 6;
  }
}
