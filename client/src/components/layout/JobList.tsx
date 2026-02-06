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

  if (isLoading) return <PulsatingText text="Loading..." customStyles="flex-1" />;
  if (error) return <PulsatingText text={`Error: \n${error.message}`} customStyles="flex-1" />;

  return (
    <>
      <div className="flex flex-1 flex-col gap-4">
        {data?.items && data.items.length < 1 && (
          <span className="text-center text-xl font-bold">Inga job hittades</span>
        )}
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
