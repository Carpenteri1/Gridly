import {Component, OnInit} from "@angular/core";
import {CommonModule} from "@angular/common";
import {MatFormField, MatLabel} from "@angular/material/input";
import {MatOption} from "@angular/material/core";
import {MatSelect} from "@angular/material/select";
import {MatIcon} from "@angular/material/icon";
import {TextStringsUtil} from "../../Constants/text.strings.util";
import {UrlStringsUtil} from "../../Constants/url.strings.util";
import {ModalService} from "../../Services/modal.service";
import {ModalFormType} from "../../Types/modalForm.types.enum";
import {ComponentService} from "../../Services/component.service";
import {SetModalComponentFormData} from "../../Utils/viewModel.factory";
import {VersionService} from "../../Services/version.services";

@Component({
  selector: 'header-component',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  standalone: true,
  imports: [CommonModule, MatFormField, MatOption, MatSelect, MatLabel, MatIcon]
})
export class HeaderComponent implements OnInit {
  protected readonly StringUtil = TextStringsUtil;

  constructor(
    protected versionService: VersionService,
    protected modalService: ModalService,
    protected componentService: ComponentService) {
  }

  async ngOnInit() {
    await this.versionService.SetVersion();
  }

  protected readonly UrlStringsUtil = UrlStringsUtil;
  protected readonly FormType = ModalFormType;
  protected readonly SetModalComponentFormData = SetModalComponentFormData;
}
