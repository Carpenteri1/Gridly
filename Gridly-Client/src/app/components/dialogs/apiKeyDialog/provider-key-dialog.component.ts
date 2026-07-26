import {Component, EventEmitter, inject, Input, Output} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BaseDialogComponent } from '../../../directives/base-dialog.directive';
import { DialogDirective } from '../../../directives/dialog.directive';
import { ThirdPartyProvider } from '../../../enums/third-party-provider.enum';
import { ProviderKeysService } from "../../../services/provider_key_services/provider-keys.service";

@Component({
  selector: 'app-provider-key-dialog',
  standalone: true,
  imports: [FormsModule, DialogDirective],
  templateUrl: './provider-key-dialog.component.html',
  styleUrls: ['../../../css/shared.dialog.css'],
})
export class ProviderKeyDialogComponent extends BaseDialogComponent{
  @Input() open = false;
  @Input() id = 0;
  @Input() invalidKey = false;
  @Output() openChange = new EventEmitter<number>();
  @Output() keySaved = new EventEmitter<void>();

  #providerKeysService = inject(ProviderKeysService);

  rawKey!:string;
  saving = false;
  errorMessage!:string;

  onSubmit(): void {
    const key = this.rawKey?.trim();
    if (!key) return;

    this.saving = true;
    this.errorMessage = '';

    this.#providerKeysService.save(key, ThirdPartyProvider.VisualCrossing).subscribe({
      next: () => {
        this.saving = false;
        this.rawKey = '';
        this.#providerKeysService.onKeySaved(ThirdPartyProvider.VisualCrossing);
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
