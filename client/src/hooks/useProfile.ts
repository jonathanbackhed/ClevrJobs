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

export function useSaveCustomJob() {
  const { getToken } = useAuth();

  return useMutation({
    mutationFn: async (savedJobRequest: SavedJobRequest) => {
      const token = await getToken({ template: "cs-api" });
      return await api.saveCustomJob(savedJobRequest, token!);
    },
  });
}

export function useSaveExistingJob(id: string) {
  const { getToken } = useAuth();

  return useMutation({
    mutationFn: async () => {
      const token = await getToken({ template: "cs-api" });
      return await api.saveExistingJob(id, token!);
    },
  });
}
