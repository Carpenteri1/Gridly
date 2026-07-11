export class UrlStringsUtil {
  static readonly CardUrl = '/api/card/';
  static readonly IconUrl = '/api/icon/';
  static readonly RowColumnUrl = '/api/row/';
  static readonly VersionUrl = '/api/version/';
  static readonly WidgetUrl = '/api/widget/';

  static readonly CardUrlGet = this.CardUrl+'get';

  static readonly IconUrlSearch = this.IconUrl+'search?value=';
  static readonly IconGet = this.IconUrl+'get';

  static readonly RowColumnUrlGet = this.RowColumnUrl+'get';
  static readonly RowColumnUrlBatchSave = this.RowColumnUrl+'batchSave';

  static readonly GetVersionUrl = this.VersionUrl;

  static readonly GetWidgetUrl = this.WidgetUrl+'get';

  static readonly GitHubReleaseURL = 'https://github.com/Carpenteri1/Gridly/releases/tag/';
}
