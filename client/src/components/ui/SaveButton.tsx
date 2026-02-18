"use client";

import { useDeleteSavedJob, useExistingSavedIds, useSaveExistingJob } from "@/hooks/useProfile";
import { cn } from "@/lib/utils/helpers";
import { useUser } from "@clerk/nextjs";
import { useQueryClient } from "@tanstack/react-query";
import { Heart } from "lucide-react";
import toast from "react-hot-toast";

interface Props {
  id: number | string;
}

export default function SaveButton({ id }: Props) {
  const { isSignedIn } = useUser();
  const queryClient = useQueryClient();

  const saveMutation = useSaveExistingJob();
  const deleteMutation = useDeleteSavedJob();

  const { data: savedIds } = useExistingSavedIds(isSignedIn);

  let isSaved = false;
  if (typeof id === "string" || savedIds?.has(id)) {
    isSaved = true;
  }

  const handleClick = () => {
    if (!savedIds) return;

    if (isSaved) {
      const savedId = typeof id === "string" ? id : savedIds.get(id);
      if (!savedId) return;

      if (!confirm("Är du säker på att du vill ta bort jobb?")) return;

      deleteMutation.mutate(savedId, {
        onSuccess: () => {
          toast.success("Jobb borttaget");
          queryClient.invalidateQueries({ queryKey: ["saved"] });
        },
        onError: () => {
          console.log("Error deleting saving job");
        },
      });
    } else {
      saveMutation.mutate(id as number, {
        onSuccess: () => {
          toast.success("Jobb sparat");
          queryClient.invalidateQueries({ queryKey: ["saved"] });
        },
        onError: () => {
          console.log("Error saving job");
        },
      });
    }
  };

  return (
    <button onClick={handleClick}>
      <Heart
        size={22}
        className={cn(
          "text-accent hover:text-accent-light transition-colors duration-200 hover:cursor-pointer",
          isSaved && "fill-accent hover:fill-accent-light",
        )}
      />
    </button>
  );
}
