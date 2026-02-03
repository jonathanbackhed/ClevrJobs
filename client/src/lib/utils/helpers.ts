import { ReportReason } from "@/types/enum";
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

export function formatDateTime(dateTime: string): string {
  const formattedDateTime = formatDistanceToNowStrict(new Date(dateTime), {
    addSuffix: true,
    locale: sv,
  });

  return formattedDateTime;
}

export function isMoreThan24hAgo(dateTime: string): boolean {
  const date = new Date(dateTime);
  const now = new Date();

  const hoursDifference = (now.getTime() - date.getTime()) / (1000 * 60 * 60);

  return hoursDifference > 24;
}

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}
