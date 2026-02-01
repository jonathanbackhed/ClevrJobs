import { JobListingDto, JobListingMiniDto, PagedResult } from "@/types/job";

const getApiUrl = (): string => {
  if (typeof window === "undefined") {
    return process.env.API_URL || "http://localhost:5075";
  }

  return process.env.NEXT_PUBLIC_BASE_API_URL || "http://localhost:5075";
};

const apiFetch = async (path: string) => {
  const url = getApiUrl() + path;
  console.log("Req url:", url);
  const res = await fetch(getApiUrl() + path);
  if (!res.ok) {
    throw new Error(`API error: ${res.status}`);
  }

  return await res.json();
};

export const api = {
  getAllJobs: async (page: number): Promise<PagedResult<JobListingMiniDto>> => await apiFetch(`/job/all?page=${page}`),
  getSingleJob: async (id: number): Promise<JobListingDto> => await apiFetch(`/job/${id}`),
};
