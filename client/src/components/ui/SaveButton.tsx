"use client";

import { useDeleteSavedJob, useSavedIds, useSaveJob } from "@/hooks/useSaved";
import { cn } from "@/lib/utils/helpers";
import { useUser } from "@clerk/nextjs";
import { Heart } from "lucide-react";

interface Props {
  id: number;
}

export default function SaveButton({ id }: Props) {
  const { isSignedIn } = useUser();

  const saveMutation = useSaveJob();
  const deleteMutation = useDeleteSavedJob();

  const { data: savedIds } = useSavedIds(isSignedIn);

  const savedJobId = savedIds?.get(id);

  const handleClick = () => {
    if (savedJobId) {
      if (!confirm("Är du säker på att du vill ta bort jobb?")) return;
      deleteMutation.mutate(savedJobId);
    } else {
      saveMutation.mutate(id);
    }
  };

  return (
    <button onClick={handleClick}>
      <Heart
        size={22}
        className={cn(
          "text-accent hover:text-accent-light transition-colors duration-200 hover:cursor-pointer",
          savedJobId && "fill-accent hover:fill-accent-light",
        )}
      />
    </button>
  );
}
