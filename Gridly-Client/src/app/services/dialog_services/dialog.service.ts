import { Injectable, signal } from '@angular/core';
import { Subject } from 'rxjs';
import { IconModel } from '../../models/icon.Model';
import { SettingsModel } from '../../models/settings.Model';
import { ImageExtensionsType } from '../../enums/image.extensions.type.enum';

@Injectable({ providedIn: 'root' })
export class DialogService {
  private _addProviderDialog = signal<number | null>(null);
  private _editCardDialog = signal<number | null>(null);
  private _deleteCardDialog = signal<number | null>(null);

  readonly isAddProviderDialogOpen = this._addProviderDialog.asReadonly();
  readonly isEditDialogOpen = this._editCardDialog.asReadonly();
  readonly isDeleteDialogOpen = this._deleteCardDialog.asReadonly();

  readonly resetFile$ = new Subject<void>();
  readonly #supportedImageExtensions: readonly string[] = [
    ImageExtensionsType.Svg,
    ImageExtensionsType.Png,
    ImageExtensionsType.Jpg,
    ImageExtensionsType.Jpeg,
    ImageExtensionsType.Ico,
  ];

  closeAddProviderKeyDialog = () => this._addProviderDialog.update(() => null);
  closeEditDialog = () => this._editCardDialog.update(() => null);
  closeDeleteDialog = () => this._deleteCardDialog.update(() => null);

  openProviderKeyDialog = (cardId: number) => this._addProviderDialog.update(() => cardId);
  openEditDialog = (cardId: number) => this._editCardDialog.update(() => cardId);
  openDeleteDialog = (cardId: number) => this._deleteCardDialog.update(() => cardId);

  async onFileUpload(event: Event): Promise<IconModel | undefined> {
    const fileInput = event.target as HTMLInputElement | null;
    const file = fileInput?.files?.item(0);

    if (!this.isSupportedImage(file!))
      return undefined;

    const iconData = new IconModel();
    const extension = this.getFileExtension(file!.name);

    iconData.name = this.getFileName(file!.name);
    iconData.type = extension;
    iconData.materialIcon = '';
    iconData.base64Data = await this.readFileAsBase64(file!);

    return iconData;
  }

  resetImageData(): void {
    this.notifyComponentToResetFileInput();
  }

  settings(): SettingsModel {
    return {
      width: 250,
      height: 250,
      imageHidden: false,
      titleHidden: false,
    };
  }

  setIcon(icon : string): IconModel {
    return {
      id: undefined,
      type: '',
      name: '',
      base64Data: '',
      materialIcon: icon,
    };
  }

  private notifyComponentToResetFileInput(): void {
    this.resetFile$.next();
  }

  private isSupportedImage(file: File | null): boolean {
    if (!file) return false;
    const normalizedType = file.type.toLowerCase();
    const extension = this.getFileExtension(file.name);

    return (
      normalizedType.startsWith('image/') ||
      this.#supportedImageExtensions.includes(extension)
    );
  }

  private getFileName(fileName: string): string {
    const lastDotIndex = fileName.lastIndexOf('.');
    return lastDotIndex > 0 ? fileName.slice(0, lastDotIndex) : fileName;
  }

  private getFileExtension(fileName: string): string {
    const lastDotIndex = fileName.lastIndexOf('.');
    return lastDotIndex >= 0 ? fileName.slice(lastDotIndex + 1).toLowerCase() : '';
  }

  private readFileAsBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();

      reader.onload = () => {
        const result = typeof reader.result === 'string' ? reader.result : '';
        resolve(result.split(',')[1] ?? '');
      };
      reader.onerror = () => reject(reader.error ?? new Error('Unable to read file.'));

      reader.readAsDataURL(file);
    });
  }
}
