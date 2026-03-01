import type { PagedResult, TrackedJobRequest, TrackedJobResponse } from "@/types";
import { apiFetchAuth, fetchOptions } from "./fetch";
import type { UUID } from "node:crypto";

export const trackedApi = {
  getAllTrackedJobs: async (page: number, token: string): Promise<PagedResult<TrackedJobResponse>> =>
    await apiFetchAuth(`/api/v1/tracked?page=${page}`, fetchOptions.GET(token)),

  createTrackedJob: async (trackedJobReq: TrackedJobRequest, token: string): Promise<TrackedJobResponse> =>
    await apiFetchAuth(`/api/v1/tracked`, fetchOptions.POST(token, trackedJobReq)),

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
