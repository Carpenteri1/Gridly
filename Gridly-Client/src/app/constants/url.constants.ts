export const urlConstants = {
  card: { get: '/api/card/get' },
  icon: { search: '/api/icon/search?value=', get: '/api/icon/get' },
  rowColumn: { get: '/api/row/get', batchSave: '/api/row/batchSave' },
  version: { get: '/api/version/' },
  widget: { get: '/api/widget/get' },
  weather: { get: '/api/weather/get', getStoredWeatherData: '/api/weather/getstoredweatherdata',getVisualCrossingData: '/api/weather/getvisualcrossingdata', save: '/api/weather/save' },
  providerKey: { getLocalStatus: '/api/providerkeys/local/provider/status', getRemoteStatus: '/api/providerkeys/remote/provider/status', save: '/api/providerkeys/save' },
  header: { githubReleaseUrl: 'https://github.com/Carpenteri1/Gridly/releases/tag/' },
} as const;
