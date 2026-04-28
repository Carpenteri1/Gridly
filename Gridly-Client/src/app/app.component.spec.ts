import { AppComponent } from './app.component';
import { TextStringsUtil } from './Constants/text.strings.util';

describe('AppComponent', () => {
  afterEach(() => {
    jest.restoreAllMocks();
  });

  it('uses the shared client title constant', () => {
    const app = new AppComponent();

    expect(app.title).toBe(TextStringsUtil.ClientTitle);
  });

  it('starts with edit mode disabled', () => {
    const app = new AppComponent();

    expect(app.isEditMode).toBe(false);
  });

  it('logs when adding a widget', () => {
    const logSpy = jest.spyOn(console, 'log').mockImplementation();
    const app = new AppComponent();

    app.onAddWidget();

    expect(logSpy).toHaveBeenCalledWith('Add Widget');
  });

  it('logs when saving', () => {
    const logSpy = jest.spyOn(console, 'log').mockImplementation();
    const app = new AppComponent();

    app.onSave();

    expect(logSpy).toHaveBeenCalledWith('Save');
  });
});
