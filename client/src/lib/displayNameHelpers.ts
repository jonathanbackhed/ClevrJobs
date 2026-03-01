import { ApplicationStatus, CompetenceRank, ReportReason, SaveType, Source } from "@/types/enum";

const CompetenceRankLabels: Record<number, string> = {
  [CompetenceRank.NewGrad]: "New-grad",
  [CompetenceRank.Junior]: "Junior",
  [CompetenceRank.MidLevel]: "Mid-level",
  [CompetenceRank.Senior]: "Senior",
  [CompetenceRank.Lead]: "Lead",
  [CompetenceRank.Unknown]: "Unknown",
};
export function getCompetenceRankLabel(value: number): string {
  return CompetenceRankLabels[value] ?? CompetenceRank[CompetenceRank.Unknown];
}

const SourceLabels: Record<Source, string> = {
  [Source.Platsbanken]: "Platsbanken",
};
export function getSourceLabel(value: number): string {
  return SourceLabels[value as Source] ?? "N/A";
}

const ReportReasonLabels: Record<ReportReason, string> = {
  [ReportReason.Spam]: "Spam",
  [ReportReason.Inappropriate]: "Olämplig",
  [ReportReason.Duplicate]: "Duplicerad",
  [ReportReason.Outdated]: "Föråldrad",
  [ReportReason.IncorrectInformation]: "Felaktig information",
  [ReportReason.Other]: "Annat (beskriv gärna nedan)",
};
export function getReasonLabel(value: number): string {
  return ReportReasonLabels[value as ReportReason] ?? "N/A";
}

const SaveTypeLabels: Record<SaveType, string> = {
  [SaveType.SavedFromListing]: "Sparat från ClevrJobs",
  [SaveType.ManuallyAdded]: "Tillagt själv",
};
export function getSaveTypeLabel(value: number): string {
  return SaveTypeLabels[value as SaveType] ?? "N/A";
}

const ApplicationStatusLabels: Record<ApplicationStatus, string> = {
  [ApplicationStatus.NotApplied]: "Inte ansökt",
  [ApplicationStatus.Applied]: "Ansökt",
  [ApplicationStatus.Interviewing]: "Intervjuprocess",
  [ApplicationStatus.WaitingForResponse]: "Väntar på svar",
  [ApplicationStatus.Offered]: "Fått erbjudande",
  [ApplicationStatus.Rejected]: "Inget erbjudande",
  [ApplicationStatus.Accepted]: "Accepterat erbjudande",
  [ApplicationStatus.Declined]: "Nekat erbjudande",
  [ApplicationStatus.Ghosted]: "Ghostad",
};
export function getApplicationStatusLabel(value: number): string {
  return ApplicationStatusLabels[value as ApplicationStatus] ?? "N/A";
}
