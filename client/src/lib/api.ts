import { JobListingDto, JobListingMiniDto, PagedResult, ReportJobRequest } from "@/types/job";
import { SavedJobRequest, SavedJobResponse } from "@/types/user";

const fetchOptions = {
  GET: (token: string) => {
    return {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    };
  },
  POST: (token: string, body?: {}) => {
    return {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: body && JSON.stringify(body),
    };
  },
  PUT: (token: string, body: {}) => {
    return {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(body),
    };
  },
  DELETE: (token: string) => {
    return {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    };
  },
};

type FetchOptions =
  | ReturnType<typeof fetchOptions.GET>
  | ReturnType<typeof fetchOptions.POST>
  | ReturnType<typeof fetchOptions.PUT>
  | ReturnType<typeof fetchOptions.DELETE>;

const getApiUrl = (): string => {
  if (typeof window === "undefined") {
    return process.env.API_URL || "http://localhost:5075";
  }

  return process.env.NEXT_PUBLIC_BASE_API_URL || "http://localhost:5075";
};

const apiFetchAuth = async (path: string, fetchOptions: FetchOptions) => {
  const res = await fetch(getApiUrl() + path, fetchOptions);
  if (!res.ok) {
    throw new Error(`API error: ${res.status}`);
  }

  return res.json();
};

const apiFetch = async (path: string, body?: {}) => {
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

  // auth
  getAllSavedJobs: async (page: number, token: string): Promise<PagedResult<SavedJobResponse>> =>
    await apiFetchAuth(`/api/v1/user/saved?page=${page}`, fetchOptions.GET(token)),
  saveCustomJob: async (savedJobRequest: SavedJobRequest, token: string): Promise<SavedJobResponse> =>
    await apiFetchAuth(`/api/v1/user/saved`, fetchOptions.POST(token, savedJobRequest)),
  saveExistingJob: async (id: string, token: string): Promise<SavedJobResponse> =>
    await apiFetchAuth(`/api/v1/user/saved/${id}`, fetchOptions.POST(token)),
};
