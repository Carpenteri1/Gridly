export class UrlStringsUtil {
  static readonly VersionUrl = '/api/version/';
  static readonly GetVersionUrl = this.VersionUrl;

  static readonly CardUrl = '/api/component/';
  static readonly CardUrlDelete = this.CardUrl+'delete/';
  static readonly CardUrlGet = this.CardUrl+'get';
  static readonly CardUrlGetById = this.CardUrl+'getbyid/';
  static readonly CardUrlSave = this.CardUrl+'save';
  static readonly CardUrlEdit = this.CardUrl+'edit';
  static readonly CardsBatchUrlEdit = this.CardUrl+'batch/edit';

  static readonly IconUrl = '/api/icon/';
  static readonly IconUrlSearch = this.IconUrl+'search?value=';
  static readonly IconGet = this.IconUrl+'get';

  static readonly GitHubReleaseURL = 'https://github.com/Carpenteri1/Gridly/releases/tag/';
}
