import {ComponentModel} from "../Models/Component.Model";
import {EditComponentModel} from "../Models/editComponent.Model";

export function MapEditComponentData(component: ComponentModel, selected: number): EditComponentModel {
    return {
        editComponent: component,
        selectedDropDownIconValue: selected
    } as EditComponentModel;
}
