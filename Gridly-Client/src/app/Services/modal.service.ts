import { Injectable } from "@angular/core";
import { MatDialog, MatDialogRef } from "@angular/material/dialog";
import {FormType} from "../Types/form.types.enum";
import {ModalsComponent} from "../Components/Modals/ComponentModals/modals.component";
import {TextStringsUtil} from "../Constants/text.strings.util";
import {IModalsModel} from "../Models/IModalsModel";
import {IComponentModel} from "../Models/IComponent.Model";

@Injectable({providedIn: 'root'})

export class ModalService{
  constructor(private dialog: MatDialog){}

  GetModalType(type:FormType){
    switch (type) {
      case FormType.Add:
        console.log(type);
        this.openModal(
          this.SetModalData(
            TextStringsUtil.ModalAddTitle,
            TextStringsUtil.ModalAddAcceptBtnTitle,
            TextStringsUtil.ModalAddCloseBtnTitle,
            '',
            type,
            {
              Id: 0,
              Name: '',
              Url: '',
              IconData: {
                base64Data: '',
                name: '',
                fileType: ''
              },
              IsActive: false,
              IsResizable: false,
              IsDraggable: false
            } as IComponentModel
          ));
        break;
      case FormType.Edit:
        //TODO maybe do something
        break;
      case FormType.Drag:
        //TODO maybe do something
        break;
      case FormType.Resize:
        //TODO maybe do something
        break;
    }
  }

  private openModal(data: IModalsModel): MatDialogRef<ModalsComponent> {
    const ref = this.dialog.open(ModalsComponent, {
      width: '600px',
      enterAnimationDuration: '500ms',
      exitAnimationDuration: '500ms',
    });
    ref.componentInstance.viewModel = data;
    return ref;
  }

  private SetModalData(
    title: string,
    acceptBtnTitle: string,
    closeBtnTitle: string,
    description: string,
    type: FormType,
    component: IComponentModel): IModalsModel {
    return {
      Title: title,
      AcceptBtnTitle: acceptBtnTitle,
      CloseBtnTitle: closeBtnTitle,
      Description: description,
      Type: type,
      Component: component
    } as IModalsModel;
  }
}
