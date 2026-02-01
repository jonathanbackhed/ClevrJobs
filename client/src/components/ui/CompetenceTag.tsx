import { cn, getCompetenceRankOrDefault } from "@/lib/utils/helpers";
import { CompetenceRank } from "@/types/job";

interface Props {
  rank: CompetenceRank;
  customText?: string;
}

export default function CompetenceTag({ rank, customText }: Props) {
  const competenceRankText = getCompetenceRankOrDefault(rank);

  const styles = {
    0: "from-cyan-50 to-cyan-100 text-cyan-700", // Newgrad
    1: "from-emerald-50 to-emerald-100 text-emerald-700", // Junior
    2: "from-lime-50 to-lime-100 text-lime-700", // Mid
    3: "from-orange-50 to-orange-100 text-orange-700", // Senior
    4: "from-red-50 to-red-100 text-red-700", // Lead
    5: "from-stone-50 to-stone-100 text-stone-700", // Unknown
  };

  return (
    <span
      className={cn(
        "w-fit rounded-md bg-linear-to-br px-2.5 py-1 text-[0.7rem] font-semibold tracking-wide uppercase",
        styles[rank],
      )}
    >
      {customText ? customText : competenceRankText}
    </span>
  );
}
