import { inject, Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { ComponentService } from './component.service';
import { ComponentModel } from '../Models/Component.Model';
import { IconModel } from '../Models/Icon.Model';
import { ImageExtensionsType } from '../Types/image.extensions.type.enum';

@Injectable({ providedIn: 'root' })
export class ModalService {
  readonly resetFile$ = new Subject<void>();
  #componentService = inject(ComponentService);
  readonly #supportedImageExtensions: ReadonlyArray<string> = [
    ImageExtensionsType.Svg,
    ImageExtensionsType.Png,
    ImageExtensionsType.Jpg,
    ImageExtensionsType.Jpeg,
    ImageExtensionsType.Ico,
  ];

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

  private notifyComponentToResetFileInput(): void {
    this.resetFile$.next();
  }

  noEmptyInputFields(component: ComponentModel): boolean {
    return (
      this.#componentService.CheckComponentData(component) &&
      (this.#componentService.IconDataSet(component) ||
        this.#componentService.IconUrlSet(component))
    );
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
