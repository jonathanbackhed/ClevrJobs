import { UUID } from "crypto";
import { ApplicationStatus, SaveType } from "./enum";

export interface TrackedJobRequest {
  applicationStatus: ApplicationStatus;
  rejectReason?: string;
  notes?: string;
  applyDate?: Date;
  haveCalled: boolean;
  spontaneousApplication: boolean;

  title: string;
  companyName: string;
  location: string;
  applicationDeadline?: string;
  listingUrl?: string;
}

export interface TrackedJobResponse {
  id: UUID;
  saveType: SaveType;
  processedJobId?: number;
  applicationStatus: ApplicationStatus;
  rejectReason?: string;
  notes?: string;
  createdAt: Date;
  updatedAt?: Date;
  applyDate?: Date;
  haveCalled: boolean;
  spontaneousApplication: boolean;

  title: string;
  companyName: string;
  location: string;
  applicationDeadline?: string;
  listingUrl?: string;
}
