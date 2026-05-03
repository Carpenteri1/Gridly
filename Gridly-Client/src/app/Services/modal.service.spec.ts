import { TestBed } from '@angular/core/testing';
import { ModalService } from './modal.service';

describe('ModalService', () => {
  let service: ModalService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ModalService],
    });

    service = TestBed.inject(ModalService);
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
});
