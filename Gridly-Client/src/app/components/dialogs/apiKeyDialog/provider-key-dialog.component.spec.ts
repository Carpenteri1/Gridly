import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AsyncPipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { of, throwError } from 'rxjs';
import { ProviderKeysService } from '../../../services/provider_key_services/provider-keys.service';
import { ThirdPartyProvider } from '../../../enums/third-party-provider.enum';
import { StubTranslatePipe } from '../../../testing/stub-translate.pipe';
import { ProviderKeyDialogComponent } from './provider-key-dialog.component';

describe('ProviderKeyDialogComponent', () => {
  let fixture: ComponentFixture<ProviderKeyDialogComponent>;
  let component: ProviderKeyDialogComponent;

  const providerKeysServiceMock = {
    save: jest.fn(),
    onKeySaved: jest.fn(),
  };

  const translateServiceMock = {
    instant: (key: string) => key,
  };

  beforeEach(() => {
    jest.clearAllMocks();
    providerKeysServiceMock.onKeySaved.mockResolvedValue(undefined);

    TestBed.configureTestingModule({
      imports: [ProviderKeyDialogComponent],
      providers: [
        { provide: ProviderKeysService, useValue: providerKeysServiceMock },
        { provide: TranslateService, useValue: translateServiceMock },
      ],
    }).overrideComponent(ProviderKeyDialogComponent, {
      remove: { imports: [TranslatePipe] },
      add: { imports: [AsyncPipe, StubTranslatePipe] },
    });

    fixture = TestBed.createComponent(ProviderKeyDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    jest.spyOn(component, 'close').mockImplementation(() => undefined);
  });

  it('does nothing when the raw key is empty or whitespace', () => {
    component.rawKey = '   ';

    component.onSubmit();

    expect(providerKeysServiceMock.save).not.toHaveBeenCalled();
  });

  it('saves the trimmed key and closes on success', async () => {
    providerKeysServiceMock.save.mockReturnValue(of(undefined));
    const keySavedSpy = jest.spyOn(component.keySaved, 'emit');
    component.rawKey = '  my-key  ';

    component.onSubmit();
    await fixture.whenStable();

    expect(providerKeysServiceMock.save).toHaveBeenCalledWith('my-key', ThirdPartyProvider.VisualCrossing);
    expect(component.saving).toBe(false);
    expect(component.rawKey).toBe('');
    expect(providerKeysServiceMock.onKeySaved).toHaveBeenCalledWith(ThirdPartyProvider.VisualCrossing);
    expect(component.close).toHaveBeenCalled();
    expect(keySavedSpy).toHaveBeenCalled();
  });

  it('sets a translated error message when saving fails', () => {
    providerKeysServiceMock.save.mockReturnValue(throwError(() => new Error('boom')));
    component.rawKey = 'my-key';

    component.onSubmit();

    expect(component.saving).toBe(false);
    expect(component.errorMessage).toBe('apiKeyDialog.text.saveFailedMessage');
  });
});
