export enum UploadState {
  NewFile = 0,
  InProgress = 1,
  Completed = 2,
  Failed = 3,
  Paused = 4,
  Cancelled = 5,
  Merging = 6,
}

export type UploadStateTuple = [string, UploadState];
