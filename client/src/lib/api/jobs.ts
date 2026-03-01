import type { JobListingDto, JobListingMiniDto, PagedResult, ReportJobRequest } from "@/types";
import { apiFetch } from "./fetch";

export const jobsApi = {
  getAllJobs: async (page: number): Promise<PagedResult<JobListingMiniDto>> =>
    await apiFetch(`/api/v1/jobs/all?page=${page}`),

  getSingleJob: async (id: number): Promise<JobListingDto> => await apiFetch(`/api/v1/jobs/${id}`),

  reportJob: async (id: number, reportJobRequest: ReportJobRequest) =>
    await apiFetch(`/api/v1/jobs/${id}/report`, reportJobRequest),
};
