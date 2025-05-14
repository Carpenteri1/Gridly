export class RegexStringsUtil {
  static readonly urlPattern = /^(https?:\/\/)(www\.)?(?!www\.)[A-Za-z0-9.]+(\.|\:)[a-zA-Z0-9]{2,}$/;
  static readonly imageUrlPattern = /^(https?:\/\/)(www\.)?(?!www\.)[A-Za-z0-9-.]+(\.|\:)[a-zA-Z0-9?=:\/&]{2,}$|^(data:image\/)(png|svg|jpeg|jpg|ico)(;base64,).*/;
  static readonly namePattern = /^[A-Za-z]+$/;
}
