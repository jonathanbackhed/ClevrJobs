import { CompetenceRank } from "@/types/job";

export function GetCompetenceRankOrDefault(value: number): string {
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
