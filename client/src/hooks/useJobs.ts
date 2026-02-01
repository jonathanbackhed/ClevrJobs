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

export function useJob(initData: JobListingDto) {
  return useQuery({
    queryKey: ["jobs", "single", initData.id],
    queryFn: async (): Promise<JobListingDto> => await api.getSingleJob(initData.id),
    initialData: initData,
    staleTime: times.ten,
    gcTime: times.thirty,
    enabled: !!initData.id,
  });
}
