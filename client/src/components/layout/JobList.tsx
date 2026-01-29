"use client";

import { useJobs } from "@/hooks/useJobs";
import { JobListingMiniDto } from "@/types/Job";
import JobListItem from "./JobListItem";

export default function JobList() {
  const { data, isLoading, error } = useJobs();

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <div className="flex flex-col gap-4">
      {data?.items.map((job: JobListingMiniDto, i: number) => (
        <JobListItem key={job.id} job={job} index={i} />
      ))}
    </div>
  );
}
