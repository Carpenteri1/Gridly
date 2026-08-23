import { TestBed } from '@angular/core/testing';
import { DialogService } from './dialog.service';

describe('DialogService', () => {
  let service: DialogService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [DialogService],
    });

    service = TestBed.inject(DialogService);
  });

  it('emits a reset event when image data is cleared', () => {
    const resetSpy = jest.fn();
    const subscription = service.resetFile$.subscribe(resetSpy);

    service.resetImageData();

    expect(resetSpy).toHaveBeenCalledTimes(1);
    subscription.unsubscribe();
  });

  it('returns undefined for unsupported uploads', async () => {
    const file = new File(['text'], 'notes.txt', { type: 'text/plain' });
    const event = {
      target: {
        files: {
          item: () => file,
        },
      },
    } as unknown as Event;

    await expect(service.onFileUpload(event)).resolves.toBeUndefined();
  });

  it('maps supported uploads into icon data', async () => {
    const file = new File(['<svg></svg>'], 'weather.svg', { type: 'image/svg+xml' });
    const readFileAsBase64Spy = jest.spyOn(
      service as unknown as { readFileAsBase64(file: File): Promise<string> },
      'readFileAsBase64'
    );
    readFileAsBase64Spy.mockResolvedValue('encoded-image');
    const event = {
      target: {
        files: {
          item: () => file,
        },
      },
    } as unknown as Event;

    await expect(service.onFileUpload(event)).resolves.toEqual({
      base64Data: 'encoded-image',
      materialIcon: '',
      name: 'weather',
      type: 'svg',
    });

    expect(readFileAsBase64Spy).toHaveBeenCalledWith(file);
  });

  it('accepts a supported image extension even without an image/* mime type', async () => {
    const file = new File(['<svg></svg>'], 'weather.svg', { type: 'application/octet-stream' });
    const event = {
      target: { files: { item: () => file } },
    } as unknown as Event;

    const result = await service.onFileUpload(event);

    expect(result).toBeDefined();
    expect(result?.type).toBe('svg');
  });

  it('keeps the whole file name when it has no extension', async () => {
    const file = new File(['data'], 'noextension', { type: 'image/png' });
    const event = {
      target: { files: { item: () => file } },
    } as unknown as Event;

    const result = await service.onFileUpload(event);

    expect(result?.name).toBe('noextension');
  });

  it('returns default settings', () => {
    expect(service.settings()).toEqual({
      width: 250,
      height: 250,
      imageHidden: false,
      titleHidden: false,
    });
  });

  it('builds icon data for a given material icon name', () => {
    expect(service.setIcon('box')).toEqual({
      id: undefined,
      type: '',
      name: '',
      base64Data: '',
      materialIcon: 'box',
    });
  });

  it('toggles the add-provider-key dialog open and closed', () => {
    service.openProviderKeyDialog(5);
    expect(service.isAddProviderDialogOpen()).toBe(5);

    service.closeAddProviderKeyDialog();
    expect(service.isAddProviderDialogOpen()).toBeNull();
  });

  it('toggles the set-provider-location dialog open and closed', () => {
    service.openSetProviderLocationDialog(6);
    expect(service.isSetProviderLocationDialogOpen()).toBe(6);

    service.closeSetProviderLocationDialog();
    expect(service.isSetProviderLocationDialogOpen()).toBeNull();
  });

  it('toggles the edit-card dialog open and closed', () => {
    service.openEditDialog(7);
    expect(service.isEditDialogOpen()).toBe(7);

    service.closeEditDialog();
    expect(service.isEditDialogOpen()).toBeNull();
  });

  it('toggles the delete-card dialog open and closed', () => {
    service.openDeleteDialog(8);
    expect(service.isDeleteDialogOpen()).toBe(8);

    service.closeDeleteDialog();
    expect(service.isDeleteDialogOpen()).toBeNull();
  });
});
