export class UrlStringsUtil {
  static readonly VersionUrl = '/api/version/';
  static readonly VersionLatestUrl = this.VersionUrl+'latest';
  static readonly VersionCurrentUrl = this.VersionUrl+'current';
  static readonly VersionSaveUrl = this.VersionUrl+'save';

  static readonly ComponentUrl = '/api/component/';
  static readonly ComponentUrlDelete = this.ComponentUrl+'delete/';
  static readonly ComponentUrlGet = this.ComponentUrl+'get';
  static readonly ComponentUrlGetById = this.ComponentUrl+'getbyid/';
  static readonly ComponentUrlSave = this.ComponentUrl+'save';
  static readonly ComponentUrlEdit = this.ComponentUrl+'edit';
  static readonly ComponentsBatchUrlEdit = this.ComponentUrl+'batch/edit';

  static readonly GitHubReleaseURL = 'https://github.com/Carpenteri1/Gridly/releases/tag/';
}
