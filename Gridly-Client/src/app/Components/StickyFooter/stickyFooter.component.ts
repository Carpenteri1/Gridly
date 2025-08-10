import { Component } from "@angular/core";
import {ComponentService} from "../../Services/component.service";
import {TextStringsUtil} from "../../Constants/text.strings.util";
import {MatButton} from "@angular/material/button";
import {MatTooltip} from "@angular/material/tooltip";

@Component({
  selector: 'stickyFooter-component',
  templateUrl: './stickyFooter.component.html',
  styleUrl: './stickyFooter.component.css',
  standalone: true,
  imports: [
    MatTooltip,
    MatButton
  ]
})

export class StickyFooterComponent {
  constructor(public componentService: ComponentService) {}

  protected readonly TextStringsUtil = TextStringsUtil;
}
