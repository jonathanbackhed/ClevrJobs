"use client";

import { useJobs } from "@/hooks/useJobs";
import { JobListingMiniDto } from "@/types/Job";
import React from "react";
import JobListItem from "./JobListItem";

export default function JobList() {
  const { data, isLoading, error } = useJobs();

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <div className="max-h-175 w-5xl overflow-y-scroll">
      <ol className="flex flex-col space-y-2">
        {data?.items.map((job: JobListingMiniDto) => (
          <JobListItem key={job.id} job={job} />
        ))}
      </ol>
    </div>
  );
}
