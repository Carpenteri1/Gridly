import { Injectable } from "@angular/core";

@Injectable({providedIn: 'root'})
export class GridService {
    public inEditMode!: boolean;
    
    constructor() {
        this.inEditMode = false;
    }
}