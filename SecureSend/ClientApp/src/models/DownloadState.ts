export enum DownloadState {
  NewFile = 0,
  InProgress = 1,
  Completed = 2,
  Failed = 3,
}

export type DownloadStateTuple = [string, DownloadState];
