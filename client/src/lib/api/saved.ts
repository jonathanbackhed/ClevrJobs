import { PagedResult, SavedJobResponse } from "@/types";
import { apiFetchAuth, fetchOptions } from "./fetch";

export const savedApi = {
  getAllSavedJobs: async (page: number, token: string): Promise<PagedResult<SavedJobResponse>> =>
    await apiFetchAuth(`/api/v1/saved?page=${page}`, fetchOptions.GET(token)),

  getSavedIds: async (token: string): Promise<{ processedJobId: number; savedJobId: string }[]> =>
    await apiFetchAuth(`/api/v1/saved/ids`, fetchOptions.GET(token)),

  saveJob: async (id: number, token: string): Promise<{}> =>
    await apiFetchAuth(`/api/v1/saved/${id}`, fetchOptions.POST(token)),

  deleteSavedJob: async (id: string, token: string): Promise<{}> =>
    await apiFetchAuth(`/api/v1/saved/${id}`, fetchOptions.DELETE(token)),
};
