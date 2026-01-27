import { api } from "@/lib/api";
import { times } from "@/lib/constants";
import { JobListingDto, JobListingMiniDto, PagedResult } from "@/types/Job";
import { useQuery } from "@tanstack/react-query";

export function useJobs() {
  return useQuery({
    queryKey: ["jobs"],
    queryFn: async (): Promise<PagedResult<JobListingMiniDto>> => await api.getAllJobs(),
    staleTime: times.ten,
    gcTime: times.fifteen,
  });
}

export function useJob(id: number) {
  return useQuery({
    queryKey: ["jobs", id],
    queryFn: async (): Promise<JobListingDto> => await api.getSingleJob(id),
    staleTime: times.five,
    gcTime: times.ten,
    enabled: !!id,
  });
}
