import { ApplicationStatus, ReportReason, SaveType } from "@/types/enum";
import { CompetenceRank, Source } from "@/types/job";
import clsx, { ClassValue } from "clsx";
import { formatDistanceToNowStrict } from "date-fns";
import { sv } from "date-fns/locale";
import { twMerge } from "tailwind-merge";

export function getCompetenceRankOrDefault(value: number): string {
  const displayMap: Record<number, string> = {
    [CompetenceRank.NewGrad]: "New-grad",
    [CompetenceRank.Junior]: "Junior",
    [CompetenceRank.MidLevel]: "Mid-level",
    [CompetenceRank.Senior]: "Senior",
    [CompetenceRank.Lead]: "Lead",
    [CompetenceRank.Unknown]: "Unknown",
  };

  if (value in CompetenceRank) {
    return displayMap[value];
  }

  return CompetenceRank[CompetenceRank.Unknown];
}

export function getSourceName(value: number): string {
  const displayMap: Record<number, string> = {
    [Source.Platsbanken]: "Platsbanken",
  };

  if (value in Source) {
    return displayMap[value];
  }

  return "N/A";
}

export function getReasonName(value: number): string {
  const displayMap: Record<number, string> = {
    [ReportReason.Spam]: "Spam",
    [ReportReason.Inappropriate]: "Olämplig",
    [ReportReason.Duplicate]: "Duplicerad",
    [ReportReason.Outdated]: "Föråldrad",
    [ReportReason.IncorrectInformation]: "Felaktig information",
    [ReportReason.Other]: "Annat (beskriv gärna nedan)",
  };

  if (value in ReportReason) {
    return displayMap[value];
  }

  return "N/A";
}

export function getSaveTypeName(value: number): string {
  const displayMap: Record<number, string> = {
    [SaveType.SavedFromListing]: "Sparat från ClevrJobs",
    [SaveType.ManuallyAdded]: "Tillagt själv",
  };

  if (value in SaveType) {
    return displayMap[value];
  }

  return "N/A";
}

export function getApplicationStatusName(value: number): string {
  const displayMap: Record<number, string> = {
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

  if (value in ApplicationStatus) {
    return displayMap[value];
  }

  return "N/A";
}

export function formatDateTime(dateTime: string | Date): string {
  var date = typeof dateTime === "string" ? new Date(dateTime) : dateTime;
  const formattedDateTime = formatDistanceToNowStrict(date, {
    addSuffix: true,
    locale: sv,
  });

  return formattedDateTime;
}

export function isMoreThan24hAgo(dateTime: string | Date): boolean {
  const date = typeof dateTime === "string" ? new Date(dateTime) : dateTime;
  const now = new Date();

  const hoursDifference = (now.getTime() - date.getTime()) / (1000 * 60 * 60);

  return hoursDifference > 24;
}

export const toUndefinedIfEmpty = (value: string) => (value && value.trim() === "" ? undefined : value);

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}
