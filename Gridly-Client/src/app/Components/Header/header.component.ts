import {Component, EventEmitter, Input, Output} from "@angular/core";
import {CommonModule} from "@angular/common";

@Component({
  selector: 'header-component',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  standalone: true,
  imports: [CommonModule]
})
export class HeaderComponent {
  @Input() isEditMode = false;
  @Output() toggleEdit = new EventEmitter<void>();
  @Output() addWidget = new EventEmitter<void>();
  @Output() save = new EventEmitter<void>();
  showMenu = false;

  onToggleMenu(): void {
    this.showMenu = !this.showMenu;
  }

  onToggleEdit(): void {
    this.toggleEdit.emit();
  }

  onAddWidget(): void {
    this.addWidget.emit();
  }

  onSave(): void {
    this.save.emit();
  }
}
