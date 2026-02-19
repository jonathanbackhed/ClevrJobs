import { api } from "@/lib/api";
import { times } from "@/lib/constants";
import { useAuth } from "@clerk/nextjs";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import toast from "react-hot-toast";

export function useSavedJobs(page: number) {
  const { getToken } = useAuth();

  return useQuery({
    queryKey: ["saved", "all", page],
    queryFn: async () => {
      const token = await getToken({ template: "cs-api" });
      return await api.getAllSavedJobs(page, token!);
    },
    staleTime: times.fifteen,
    gcTime: times.thirty,
  });
}

export function useSavedIds(isSignedIn: boolean | undefined) {
  const { getToken } = useAuth();

  return useQuery({
    queryKey: ["saved", "ids"],
    queryFn: async () => {
      const token = await getToken({ template: "cs-api" });
      return await api.getSavedIds(token!);
    },
    staleTime: times.hour,
    gcTime: times.hour,
    enabled: isSignedIn === true,
    select: (ids: { processedJobId: number; savedJobId: string }[]) =>
      new Map(ids.map((i) => [i.processedJobId, i.savedJobId])),
  });
}

export function useSaveJob() {
  const { getToken } = useAuth();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: number) => {
      const token = await getToken({ template: "cs-api" });
      return await api.saveJob(id, token!);
    },
    onSuccess: () => {
      toast.success("Jobb sparat");
      queryClient.invalidateQueries({ queryKey: ["saved"] });
    },
    onError: () => {
      toast.error("Något gick fel, försök igen senare");
    },
  });
}

export function useDeleteSavedJob() {
  const { getToken } = useAuth();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      const token = await getToken({ template: "cs-api" });
      return await api.deleteSavedJob(id, token!);
    },
    onSuccess: () => {
      toast.success("Jobb borttaget");
      queryClient.invalidateQueries({ queryKey: ["saved"] });
    },
    onError: () => {
      toast.error("Något gick fel, försök igen senare");
    },
  });
}
