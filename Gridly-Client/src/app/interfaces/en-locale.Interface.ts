export interface AppStrings {
  text: { title: string };
}

export interface HeaderStrings {
  text: { newVersionAvailable: string };
  url: { githubReleaseUrl: string };
}

export interface GridCardStrings {
  text: {
    resizeTitle: string;
    cancelToolTip: string;
    movePositionTitle: string;
    movePositionSaveToolTip: string;
    saveButtonTitle: string;
    cancelButtonTitle: string;
    iconNotFoundMessage: string;
  };
}

export interface CardStrings {
  url: { get: string };
  text: { emptyTitle: string; emptyDescription: string };
  errors: {
    addFailed: string;
    getByIdFailed: string;
    deletionFailed: string;
    batchEditFailed: string;
    editFailed: string;
    getFailed: string;
  };
}

export interface IconStrings {
  url: { search: string; get: string };
}

export interface RowColumnStrings {
  url: { get: string; batchSave: string };
}

export interface VersionStrings {
  url: { get: string };
}

export interface WidgetStrings {
  url: { get: string };
}

export interface WeatherStrings {
  url: { get: string; getVisualCrossingData: string; save: string };
}

export interface ProviderKeyStrings {
  url: { getLocalStatus: string; getRemoteStatus: string; save: string };
}

export interface AddCardPickerStrings {
  text: { title: string; description: string };
}

export interface EditCardDialogStrings {
  text: {
    title: string;
    acceptBtnTitle: string;
    cancelBtnTitle: string;
    inputNameTitle: string;
    inputUrlTitle: string;
    inputSearchIconTitle: string;
    dropDownOptionUploadImage: string;
    dropDownOptionLinkToImage: string;
    linkToImageTitle: string;
  };
}

export interface AddCardDialogStrings {
  text: {
    title: string;
    acceptBtnTitle: string;
    cancelBtnTitle: string;
    inputNameTitle: string;
    inputUrlTitle: string;
    dropDownOptionUploadImage: string;
    dropDownOptionLinkToImage: string;
    linkToImageTitle: string;
  };
}

export interface DeleteCardDialogStrings {
  text: {
    title: string;
    headerTitle: string;
    acceptBtnTitle: string;
    cancelBtnTitle: string;
    description: string;
  };
}

export interface MenuStrings {
  text: {
    addCardButtonTitle: string;
    saveButtonTitle: string;
    editButtonTitle: string;
    exitEditButtonTitle: string;
    dropDownDragTitle: string;
    dropDownResizeTitle: string;
    addProviderKeysButtonTitle: string;
  };
}

export interface ApiKeyDialogStrings {
  text: {
    title: string;
    description: string;
    foundApiKeyAt: string;
    invalidMessage: string;
    inputLabel: string;
    saveBtnTitle: string;
    cancelBtnTitle: string;
    saveFailedMessage: string;
  };
}

export interface WeatherProviderLocationDialogStrings {
  text: {
    title: string;
    countryInputLabel: string;
    cityInputLabel: string;
    saveBtnTitle: string;
    cancelBtnTitle: string;
    saveFailedMessage: string;
    saveInvalidKeyFailedMessage: string;
    saveLocationNotFoundFailedMessage: string;
  };
}

export interface EnLocale {
  app: AppStrings;
  header: HeaderStrings;
  gridCard: GridCardStrings;
  card: CardStrings;
  icon: IconStrings;
  rowColumn: RowColumnStrings;
  version: VersionStrings;
  widget: WidgetStrings;
  weather: WeatherStrings;
  providerKey: ProviderKeyStrings;
  addCardPicker: AddCardPickerStrings;
  editCardDialog: EditCardDialogStrings;
  addCardDialog: AddCardDialogStrings;
  deleteCardDialog: DeleteCardDialogStrings;
  menu: MenuStrings;
  apiKeyDialog: ApiKeyDialogStrings;
  weatherProviderLocationDialog: WeatherProviderLocationDialogStrings;
}
