export class UploadExpiredError extends Error {
  constructor(msg: string) {
    super(msg);
    Object.setPrototypeOf(this, UploadExpiredError.prototype);
  }
}

export class UploadDoesNotExistError extends Error {
  constructor(msg: string) {
    super(msg);
    Object.setPrototypeOf(this, UploadDoesNotExistError.prototype);
  }
}
