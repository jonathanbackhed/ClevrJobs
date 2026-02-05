"use client";

import { useJobs } from "@/hooks/useJobs";
import { JobListingMiniDto } from "@/types/job";
import JobListItem from "./JobListItem";
import { useState } from "react";
import Pagination from "./Pagination";
import { useSearchParams } from "next/navigation";
import PulsatingText from "../ui/PulsatingText";

export default function JobList() {
  const params = useSearchParams();
  const page = Number(params.get("page")) || 1;

  const [currentPage, setCurrentPage] = useState(page);

  const { data, isLoading, error } = useJobs(currentPage);

  if (isLoading) return <PulsatingText text="Loading..." />;
  if (error) return <PulsatingText text={`Error: \n${error.message}`} />;

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
