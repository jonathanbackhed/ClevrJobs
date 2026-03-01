import type { UUID } from "node:crypto";
import { JobListingMiniDto } from "./job";

export interface SavedJobResponse {
  id: UUID;
  jobListingMini: JobListingMiniDto;
  savedAt: Date;
}
