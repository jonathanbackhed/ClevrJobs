import { CompetenceRank } from "@/types/job";
import { formatDistanceToNow, formatDistanceToNowStrict } from "date-fns";
import { sv } from "date-fns/locale";

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
