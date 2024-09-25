import { Injectable } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';  // Import DomSanitizer and SafeHtml

@Injectable({
  providedIn: 'root'
})

export class SharedService {
  public flexItems: SafeHtml[] = [];

  constructor(private sanitizer: DomSanitizer){
    this.flexItems = [
      this.sanitizeHtml('')
    ];
  }

  sanitizeHtml(html: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(html);
  }

  addItem() {
    if(this.flexItems.length <= 4){
      const newItem = this.sanitizeHtml(`<div 
        style="
        height: 100%;
        width:330px;
        margin-left:1.5em;
        background:darkgrey;">Flex item ${this.flexItems.length}</div>`);
      this.flexItems.push(newItem); 
    }
  }
}