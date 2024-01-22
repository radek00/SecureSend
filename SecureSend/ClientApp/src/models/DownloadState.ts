export enum DownloadState {
  NewFile = 0,
  InProgress,
  Completed = 2,
  Failed = 3,
}

export type DownloadStateTuple = [string, DownloadState];
