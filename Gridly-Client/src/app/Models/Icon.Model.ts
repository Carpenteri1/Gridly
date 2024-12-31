export class IconModel{
  fileType:string;
  Name:string;
  base64Data:string;

  constructor(fileType: string, Name: string, base64Data: string){
    this.fileType = fileType;
    this.Name = Name;
    this.base64Data = base64Data;
  }

}
