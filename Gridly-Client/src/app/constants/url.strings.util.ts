export class UrlStringsUtil {
  static readonly VersionUrl = '/api/version/';
  static readonly GetVersionUrl = this.VersionUrl;

  static readonly CardUrl = '/api/card/';
  static readonly CardUrlDelete = this.CardUrl+'delete/';
  static readonly CardUrlGet = this.CardUrl+'get';
  static readonly CardUrlGetById = this.CardUrl+'getbyid/';

  static readonly RowColumnUrl = '/api/row/';
  static readonly RowColumnUrlGet = this.RowColumnUrl+'get';
  static readonly RowColumnUrlBatchSave = this.RowColumnUrl+'batchSave';

  static readonly IconUrl = '/api/icon/';
  static readonly IconUrlSearch = this.IconUrl+'search?value=';
  static readonly IconGet = this.IconUrl+'get';

  static readonly GitHubReleaseURL = 'https://github.com/Carpenteri1/Gridly/releases/tag/';
}
