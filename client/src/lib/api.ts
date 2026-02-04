import { JobListingDto, JobListingMiniDto, PagedResult, ReportJobRequest } from "@/types/job";

const getApiUrl = (): string => {
  if (typeof window === "undefined") {
    return process.env.API_URL || "http://localhost:5075";
  }

  return process.env.NEXT_PUBLIC_BASE_API_URL || "http://localhost:5075";
};

const apiFetch = async (path: string, body?: {}) => {
  const url = getApiUrl() + path;
  console.log("Req url:", url);
  const res = await fetch(
    getApiUrl() + path,
    body ? { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(body) } : undefined,
  );
  if (!res.ok) {
    throw new Error(`API error: ${res.status}`);
  }

  return res.json();
};

export const api = {
  getAllJobs: async (page: number): Promise<PagedResult<JobListingMiniDto>> =>
    await apiFetch(`/api/v1/jobs/all?page=${page}`),
  getSingleJob: async (id: number): Promise<JobListingDto> => await apiFetch(`/api/v1/jobs/${id}`),
  reportJob: async (id: number, reportJobRequest: ReportJobRequest) =>
    await apiFetch(`/api/v1/jobs/${id}/report`, reportJobRequest),
};
