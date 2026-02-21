import { api } from "@/lib/api";
import { times } from "@/lib/constants";
import { TrackedJobRequest } from "@/types/tracked";
import { useAuth } from "@clerk/nextjs";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { UUID } from "crypto";
import toast from "react-hot-toast";

export function useTrackedJobs(page: number) {
  const { getToken } = useAuth();

  return useQuery({
    queryKey: ["tracked", "all", page],
    queryFn: async () => {
      const token = await getToken({ template: "cs-api" });
      return await api.getAllTrackedJobs(page, token!);
    },
    staleTime: times.fifteen,
    gcTime: times.thirty,
  });
}

export function useCreateTrackedJob() {
  const { getToken } = useAuth();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (trackedJobReq: TrackedJobRequest) => {
      const token = await getToken({ template: "cs-api" });
      return await api.createTrackedJob(trackedJobReq, token!);
    },
    onSuccess: () => {
      toast.success("Jobb skapat");
      queryClient.invalidateQueries({ queryKey: ["tracked"] });
    },
    onError: () => {
      toast.error("Något gick fel, försök igen senare");
    },
  });
}

export function useCreateTrackedJobFromExisting() {
  const { getToken } = useAuth();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (processedJobId: number) => {
      const token = await getToken({ template: "cs-api" });
      return await api.createTrackedJobFromExisting(processedJobId, token!);
    },
    onSuccess: () => {
      toast.success("Jobb tillagt i tracker");
      queryClient.invalidateQueries({ queryKey: ["tracked"] });
    },
    onError: () => {
      toast.error("Något gick fel, försök igen senare");
    },
  });
}

export function useUpdateTrackedJob(trackedJobId?: UUID) {
  const { getToken } = useAuth();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (trackedJobReq: TrackedJobRequest) => {
      if (!trackedJobId) throw new Error("No trackedJobId provided");
      const token = await getToken({ template: "cs-api" });
      return await api.updateTrackedJob(trackedJobId, trackedJobReq, token!);
    },
    onSuccess: () => {
      toast.success("Jobb uppdaterat");
      queryClient.invalidateQueries({ queryKey: ["tracked"] });
    },
    onError: () => {
      toast.error("Något gick fel, försök igen senare");
    },
  });
}

export function useDeleteTrackedJob() {
  const { getToken } = useAuth();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (trackedJobId: UUID) => {
      const token = await getToken({ template: "cs-api" });
      return await api.deleteTrackedJob(trackedJobId, token!);
    },
    onSuccess: () => {
      toast.success("Jobb borttaget");
      queryClient.invalidateQueries({ queryKey: ["tracked"] });
    },
    onError: () => {
      toast.error("Något gick fel, försök igen senare");
    },
  });
}
