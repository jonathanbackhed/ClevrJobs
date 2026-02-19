import { UUID } from "crypto";
import { JobListingMiniDto } from "./job";

export interface SavedJobResponse {
  id: UUID;
  jobListingMini: JobListingMiniDto;
  savedAt: Date;
}
