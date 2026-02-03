import { api } from "@/lib/api";
import { times } from "@/lib/constants";
import { JobListingDto, JobListingMiniDto, PagedResult, ReportJobRequest } from "@/types/job";
import { useMutation, useQuery } from "@tanstack/react-query";

export function useJobs(page: number) {
  return useQuery({
    queryKey: ["jobs", "all", page],
    queryFn: async (): Promise<PagedResult<JobListingMiniDto>> => await api.getAllJobs(page),
    staleTime: times.five,
    gcTime: times.ten,
  });
}

export function useJob(id: number) {
  return useQuery({
    queryKey: ["jobs", "single", id],
    queryFn: async (): Promise<JobListingDto> => await api.getSingleJob(id),
    staleTime: times.thirty,
    gcTime: times.thirty,
    enabled: !!id,
  });
}

export function useReportJob(id: number) {
  return useMutation({
    mutationFn: async (reportJobRequest: ReportJobRequest) => await api.reportJob(id, reportJobRequest),
  });
}
