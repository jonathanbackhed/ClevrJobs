import { api } from "@/lib/api";
import { times } from "@/lib/constants";
import { JobListingDto, JobListingMiniDto, PagedResult } from "@/types/job";
import { useQuery } from "@tanstack/react-query";

export function useJobs(page: number) {
  return useQuery({
    queryKey: ["jobs", "all", page],
    queryFn: async (): Promise<PagedResult<JobListingMiniDto>> => await api.getAllJobs(page),
    staleTime: times.fifteen,
    gcTime: times.hour,
  });
}

export function useJob(id: number) {
  return useQuery({
    queryKey: ["jobs", "single", id],
    queryFn: async (): Promise<JobListingDto> => await api.getSingleJob(id),
    staleTime: times.ten,
    gcTime: times.thirty,
    enabled: !!id,
  });
}
