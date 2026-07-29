export class UrlStringsUtil {
  static readonly CardUrl = '/api/card/';
  static readonly IconUrl = '/api/icon/';
  static readonly RowColumnUrl = '/api/row/';
  static readonly VersionUrl = '/api/version/';
  static readonly WidgetUrl = '/api/widget/';
  static readonly WeatherUrl = '/api/weather/';
  static readonly ProviderKeyUrl = '/api/providerkeys/';

  static readonly CardUrlGet = this.CardUrl+'get';

  static readonly IconUrlSearch = this.IconUrl+'search?value=';
  static readonly IconGet = this.IconUrl+'get';

  static readonly RowColumnUrlGet = this.RowColumnUrl+'get';
  static readonly RowColumnUrlBatchSave = this.RowColumnUrl+'batchSave';

  static readonly GetVersionUrl = this.VersionUrl;

  static readonly GetWidgetUrl = this.WidgetUrl+'get';

  static readonly GetWeatherUrl = this.WeatherUrl+'get';
  static readonly GetVisualCrossingDataURL = this.WeatherUrl+'getvisualcrossingdata';
  static readonly GetUrl = this.WeatherUrl+'get';

  static readonly GetLocalProviderKeyStatusUrl = this.ProviderKeyUrl+'local/provider/status';
  static readonly GetRemoteProviderKeyStatusUrl = this.ProviderKeyUrl+'remote/provider/status';
  static readonly SaveProviderKeyUrl = this.ProviderKeyUrl+'save';

  static readonly GitHubReleaseURL = 'https://github.com/Carpenteri1/Gridly/releases/tag/';
}
