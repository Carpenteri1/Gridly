import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BaseDialogComponent } from '../../../directives/base-dialog.directive';
import { DialogDirective } from '../../../directives/dialog.directive';
import { ApiKeyService } from '../../../services/api_key_services/api-key.service';
import { ThirdPartyProvider } from '../../../types/third-party-provider.enum';

@Component({
  selector: 'app-api-key-dialog',
  standalone: true,
  imports: [FormsModule, DialogDirective],
  templateUrl: './api-key-dialog.component.html',
  styleUrls: ['../../../css/shared.dialog.css'],
})
export class ApiKeyDialogComponent extends BaseDialogComponent {
  @Input() open = false;
  @Input() id = 0;
  @Input() invalidKey = false;
  @Output() openChange = new EventEmitter<number>();
  @Output() keySaved = new EventEmitter<void>();

  #apiKeyService = inject(ApiKeyService);

  rawKey = '';
  saving = false;
  errorMessage = '';

  onSubmit(): void {
    if (!this.rawKey.trim()) return;

    this.saving = true;
    this.errorMessage = '';

    this.#apiKeyService.save(this.rawKey, ThirdPartyProvider.VisualCrossing).subscribe({
      next: () => {
        this.saving = false;
        this.rawKey = '';
        this.#apiKeyService.onKeySaved(ThirdPartyProvider.VisualCrossing);
        this.close();
        this.keySaved.emit();
      },
      error: () => {
        this.saving = false;
        this.errorMessage = this.TextStringsUtil.DialogApiKeySaveFailedMessage;
      },
    });
  }
}
