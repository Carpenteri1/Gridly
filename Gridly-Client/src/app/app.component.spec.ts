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
});
