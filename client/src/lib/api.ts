import { JobListingDto, JobListingMiniDto, PagedResult } from "@/types/job";

const apiUrl = process.env.NEXT_PUBLIC_BASE_API_URL;

const apiFetch = async (path: string) => {
  const res = await fetch(apiUrl + path);
  if (!res.ok) {
    throw new Error(`API error: ${res.status}`);
  }

  return res.json();
};

export const api = {
  getAllJobs: async (page: number): Promise<PagedResult<JobListingMiniDto>> => await apiFetch(`/job/all?page=${page}`),
  getSingleJob: async (id: number): Promise<JobListingDto> => await apiFetch(`/job/${id}`),
};
