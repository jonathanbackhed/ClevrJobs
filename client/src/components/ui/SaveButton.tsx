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

  const savedJobId = typeof id === "string" ? id : savedIds?.get(id);
  const isSaved = Boolean(savedJobId);

  const handleClick = () => {
    if (isSaved) {
      if (!savedJobId) return;
      if (!confirm("Är du säker på att du vill ta bort jobb?")) return;

      deleteMutation.mutate(savedJobId, {
        onSuccess: () => {
          toast.success("Jobb borttaget");
          queryClient.invalidateQueries({ queryKey: ["saved"] });
        },
        onError: () => {
          console.log("Error deleting saving job");
        },
      });
      return;
    }

    if (typeof id !== "number") return;

    saveMutation.mutate(id, {
      onSuccess: () => {
        toast.success("Jobb sparat");
        queryClient.invalidateQueries({ queryKey: ["saved"] });
      },
      onError: () => {
        console.log("Error saving job");
      },
    });
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
