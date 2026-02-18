import { api } from "@/lib/api";
import { times } from "@/lib/constants";
import { PagedResult } from "@/types/job";
import { SavedJobRequest, SavedJobResponse } from "@/types/user";
import { useAuth } from "@clerk/nextjs";
import { useMutation, useQuery } from "@tanstack/react-query";

export function useSavedJobs(page: number) {
  const { getToken } = useAuth();

  return useQuery({
    queryKey: ["saved", "all", page],
    queryFn: async (): Promise<PagedResult<SavedJobResponse>> => {
      const token = await getToken({ template: "cs-api" });
      return await api.getAllSavedJobs(page, token!);
    },
    staleTime: times.fifteen,
    gcTime: times.thirty,
  });
}

export function useExistingSavedIds(isSignedIn: boolean | undefined) {
  const { getToken } = useAuth();

  return useQuery({
    queryKey: ["saved", "ids"],
    queryFn: async (): Promise<{ processedJobId: number; savedJobId: string }[]> => {
      const token = await getToken({ template: "cs-api" });
      return await api.getExistingSavedIds(token!);
    },
    staleTime: times.hour,
    gcTime: times.hour,
    enabled: isSignedIn === true,
    select: (ids: { processedJobId: number; savedJobId: string }[]) =>
      new Map(ids.map((i) => [i.processedJobId, i.savedJobId])),
  });
}

export function useSaveCustomJob() {
  const { getToken } = useAuth();

  return useMutation({
    mutationFn: async (savedJobRequest: SavedJobRequest) => {
      const token = await getToken({ template: "cs-api" });
      return await api.saveCustomJob(savedJobRequest, token!);
    },
  });
}

export function useSaveExistingJob() {
  const { getToken } = useAuth();

  return useMutation({
    mutationFn: async (id: number) => {
      const token = await getToken({ template: "cs-api" });
      return await api.saveExistingJob(id, token!);
    },
  });
}

export function useUpdateSavedJob() {
  const { getToken } = useAuth();

  return useMutation({
    mutationFn: async (saveJob: SavedJobRequest) => {
      const token = await getToken({ template: "cs-api" });
      return await api.updateSavedJob(saveJob, token!);
    },
  });
}

export function useDeleteSavedJob() {
  const { getToken } = useAuth();

  return useMutation({
    mutationFn: async (id: string) => {
      const token = await getToken({ template: "cs-api" });
      return await api.deleteSavedJob(id, token!);
    },
  });
}
