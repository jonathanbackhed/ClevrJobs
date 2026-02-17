import { UUID } from "crypto";
import { ApplicationStatus, SaveType } from "./enum";

export interface SavedJobRequest {
  id: UUID;
  haveApplied: boolean;
  applicationStatus: ApplicationStatus;
  rejectReason?: string;
  notes?: string;

  title?: string;
  description?: string;
  companyName?: string;
  location?: string;
  applicationDeadline?: string;
  listingUrl?: string;
}

export interface SavedJobResponse {
  id: UUID;
  saveType: SaveType;
  processedJobId?: number;

  haveApplied: boolean;
  applicationStatus: ApplicationStatus;
  rejectReason?: string;
  notes?: string;
  savedAt: string;

  title?: string;
  description?: string;
  companyName?: string;
  location?: string;
  applicationDeadline?: string;
  listingUrl?: string;
}
