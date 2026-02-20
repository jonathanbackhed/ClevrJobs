import { JobListingDto, JobListingMiniDto, PagedResult, ReportJobRequest } from "@/types/job";
import { SavedJobResponse } from "@/types/saved";
import { TrackedJobRequest, TrackedJobResponse } from "@/types/tracked";
import { UUID } from "crypto";

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

  // saved
  getAllSavedJobs: async (page: number, token: string): Promise<PagedResult<SavedJobResponse>> =>
    await apiFetchAuth(`/api/v1/saved?page=${page}`, fetchOptions.GET(token)),

  getSavedIds: async (token: string): Promise<{ processedJobId: number; savedJobId: string }[]> =>
    await apiFetchAuth(`/api/v1/saved/ids`, fetchOptions.GET(token)),

  saveJob: async (id: number, token: string): Promise<{}> =>
    await apiFetchAuth(`/api/v1/saved/${id}`, fetchOptions.POST(token)),

  deleteSavedJob: async (id: string, token: string): Promise<{}> =>
    await apiFetchAuth(`/api/v1/saved/${id}`, fetchOptions.DELETE(token)),

  // tracked
  getAllTrackedJobs: async (page: number, token: string): Promise<PagedResult<TrackedJobResponse>> =>
    await apiFetchAuth(`/api/v1/tracked?page=${page}`, fetchOptions.GET(token)),

  createTrackedJob: async (trackedJobReq: TrackedJobRequest, token: string): Promise<TrackedJobResponse> =>
    await apiFetchAuth(`/api/v1/tracked`, fetchOptions.POST(token)),

  createTrackedJobFromExisting: async (processedJobId: number, token: string): Promise<TrackedJobResponse> =>
    await apiFetchAuth(`/api/v1/tracked/${processedJobId}`, fetchOptions.POST(token)),

  updateTrackedJob: async (
    trackedJobId: UUID,
    trackedJobReq: TrackedJobRequest,
    token: string,
  ): Promise<TrackedJobResponse> =>
    await apiFetchAuth(`/api/v1/tracked/${trackedJobId}`, fetchOptions.PUT(token, trackedJobReq)),

  deleteTrackedJob: async (trackedJobId: string, token: string): Promise<{}> =>
    await apiFetchAuth(`/api/v1/tracked/${trackedJobId}`, fetchOptions.DELETE(token)),
};
