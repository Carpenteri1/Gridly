export class VersionModel{
  name:string;
  newRelease:boolean;

  constructor(name:string, newRelease:boolean){
    this.name = name;
    this.newRelease = newRelease;
  }
}
