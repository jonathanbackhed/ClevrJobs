import { ApplicationStatus } from "./enum";

export interface FilterOptions {
  from?: string;
  to?: string;
  applicationStatus?: ApplicationStatus | "";
  haveCalled: boolean;
  spontaneousApplication: boolean;
}
