export class IconModel{
  fileType:string;
  name:string;
  base64Data:string;

  constructor(name: string, fileType: string, base64Data: string){
    this.name = name;
    this.fileType = fileType;
    this.base64Data = base64Data;
  }

}
