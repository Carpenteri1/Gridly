import {Injectable, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {VersionInterface} from "../../Models/Version.Interface";
import {UrlStringsUtil} from "../../Constants/url.strings.util";
import {take} from "rxjs";

@Injectable({
  providedIn: 'root'
})

export class VersionEndpointService implements  OnInit{

  private version!: VersionInterface;
  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.version = this.CheckForCurrentRelease();
  }

  CheckForNewRelease() {
    this.http.get<VersionInterface>(UrlStringsUtil.VersionLatestUrl).pipe(take(1)).subscribe(
      data => this.version = data
    );
    return this.version;
  }

  CheckForCurrentRelease() {
    this.http.get<VersionInterface>(UrlStringsUtil.VersionCurrentUrl).pipe(take(1)).subscribe(
      data => this.version = data
    );
    return this.version;
  }

  HasNewRelease() {
    if(this.version === undefined || this.version === null) {
      return false;
    }
    return this.version.newRelease;
  }

  GetVersionName() {
    if(this.version === undefined || this.version === null) {
      return '';
    }
    return this.version.name;
  }

}
