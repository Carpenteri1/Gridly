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
      const newItem = this.sanitizeHtml(`<div 
        style="
        height: 300px;
        width:330px;
        flex: 1 1 330px;
        margin: 10px;
        background:darkgrey;">Flex item ${this.flexItems.length}</div>`);
        this.flexItems.push(newItem);     
  }
  removeItem(){
    if(this.flexItems.length > 1){
      this.flexItems.splice(this.flexItems.length -1,1);
    }
    if(this.flexItems.length === 1){
      this.flexItems.splice(this.flexItems.length,1);
    }
  }
}