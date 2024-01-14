export enum DownloadState {
  New = 0,
  InProgress,
  Failed = 2,
  Completed = 3,
}

export type DownloadStateTuple = [string, DownloadState];
