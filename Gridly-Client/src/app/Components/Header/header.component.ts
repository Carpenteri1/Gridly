import {Component, OnInit, signal} from "@angular/core";
import {TextStringsUtil} from "../../Constants/text.strings.util";
import {CommonModule} from "@angular/common";
import {VersionService} from "../../Services/version.services";
import {ModalService} from "../../Services/modal.service";
import {ComponentService} from "../../Services/component.service";
import {SetModalComponentFormData} from "../../Utils/viewModel.factory";
import {ModalFormType} from "../../Types/modalForm.types.enum";

@Component({
  selector: 'header-component',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  standalone: true,
  imports: [CommonModule]
})
export class HeaderComponent implements OnInit {
  constructor(
    protected versionService: VersionService,
    protected modalService: ModalService,
    protected componentService: ComponentService) {
  }

  async ngOnInit() {
    await this.versionService.SetVersion();
  }

  showMenu = signal(false);
  toggleMenu(): void {
    this.showMenu.update((showMenu) => showMenu);
  }

  addWidget(){
    if(this.componentService.InEditMode){
      this.modalService.GetModalType(SetModalComponentFormData({type: this.FormType.Add}))
    }
    if(!this.componentService.InEditMode){
      this.modalService.Cancel();
    }
  }

  protected readonly TextStringsUtil = TextStringsUtil;
  protected readonly FormType = ModalFormType;

  protected readonly SetModalComponentFormData = SetModalComponentFormData;
}
