export enum ReportReason {
  Spam = 0,
  Inappropriate,
  Duplicate,
  Outdated,
  IncorrectInformation,
  Other,
}

export enum SaveType {
  SavedFromListing = 0,
  ManuallyAdded,
}

export enum ApplicationStatus {
  NotApplied = 0,
  Applied,
  Interviewing,
  WaitingForResponse,
  Offered,
  Rejected,
  Accepted,
  Declined,
  Ghosted,
}
