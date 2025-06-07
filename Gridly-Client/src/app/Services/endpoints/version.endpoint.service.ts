import { Injectable, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { VersionModel } from "../../Models/Version.Model";
import { UrlStringsUtil } from "../../Constants/url.strings.util";

@Injectable({
  providedIn: 'root'
})

export class VersionEndpointService implements  OnInit{

  private version!: VersionModel;
  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.version = this.CheckForCurrentRelease();
  }

  CheckForNewRelease() {
    this.http.get<VersionModel>(UrlStringsUtil.VersionLatestUrl).subscribe(
      (versionData) => {
        this.version = versionData;
      },
      error => {
        console.error('Error checking for new release:', error);
      }
    );
    return this.version;
  }

  CheckForCurrentRelease() {
    this.http.get<VersionModel>(UrlStringsUtil.VersionCurrentUrl).subscribe(
      (versionData) => {
        this.version = versionData;
      },
      error => {
        console.error('Error checking for current release:', error);
      }
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
