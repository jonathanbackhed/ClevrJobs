"use client";

import { useJobs } from "@/hooks/useJobs";
import { JobListingMiniDto } from "@/types/job";
import JobListItem from "./JobListItem";
import { useState } from "react";
import Pagination from "./Pagination";

export default function JobList() {
  const [currentPage, setCurrentPage] = useState(1);

  const { data, isLoading, error } = useJobs(currentPage);

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <>
      <div className="flex flex-col gap-4">
        {data?.items.map((job: JobListingMiniDto, i: number) => (
          <JobListItem key={job.id} job={job} index={i} />
        ))}
      </div>

      <Pagination
        totalPages={data?.totalPages ?? 0}
        totalCount={data?.totalCount ?? 0}
        pageSize={data?.pageSize ?? 0}
        currentPage={currentPage}
        onPageChange={setCurrentPage}
      />
    </>
  );
}
