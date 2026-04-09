import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { ComponentService } from './component.service';
import { ComponentModel } from '../Models/Component.Model';
import { IconModel } from '../Models/Icon.Model';

@Injectable({ providedIn: 'root' })
export class ModalService {
  public resetFile$ = new Subject<void>();

  constructor(private componentService: ComponentService) {}

  onFileUpload(event: any): IconModel | undefined {
    if (event.target.files.length > 0) {
      const file = event.target.files[0];

      if (
        file.type.includes('svg') ||
        file.type.includes('png') ||
        file.type.includes('jpg') ||
        file.type.includes('jpeg') ||
        file.type.includes('ico')
      ) {
        let iconData = new IconModel();
        const reader = new FileReader();
        reader.onload = () => {
          iconData.base64Data = reader.result as string;
          iconData.base64Data = iconData.base64Data.split(',')[1];
        };
        reader.readAsDataURL(file);
        let nameSplit = file.name.split('.');
        iconData.name = nameSplit[0];
        iconData.type = nameSplit[1];
        return iconData;
      }
    }
    return undefined;
  }

  resetImageData(): void {
    this.notifyComponentToResetFileInput();
  }

  private notifyComponentToResetFileInput(): void {
    this.resetFile$.next();
  }

  noEmptyInputFields(component: ComponentModel): boolean {
    return (
      this.componentService.CheckComponentData(component) &&
      (this.componentService.IconDataSet(component) ||
        this.componentService.IconUrlSet(component))
    );
  }
}

