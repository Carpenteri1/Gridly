export class RegexStringsUtil {
  static readonly urlPattern = /^(https?:\/\/)?(www\.)?((([A-Za-z0-9-]+\.)+[A-Za-z]{2,})|((\d{1,3}\.){3}\d{1,3}))(:\d{1,5})?$/;
  static readonly iconUrlPattern = /^(https?:\/\/(?:www\.)?[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}(?::\d+)?(?:[\/?#][^\s]*)?)$|^(data:image\/(png|svg|jpeg|jpg|ico);base64,[A-Za-z0-9+/=]+)$/;
  static readonly namePattern = /^[A-Za-z]+$/;
}
