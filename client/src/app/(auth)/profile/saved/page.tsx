"use client";

import JobListItem from "@/components/layout/JobListItem";
import Pagination from "@/components/layout/Pagination";
import BackButton from "@/components/ui/BackButton";
import PulsatingText from "@/components/ui/PulsatingText";
import { useSavedJobs } from "@/hooks/useSaved";
import { SavedJobResponse } from "@/types/saved";
import { notFound, useRouter, useSearchParams } from "next/navigation";
import { useState } from "react";

export default function Saved() {
  const router = useRouter();
  const params = useSearchParams();
  const page = Number(params.get("page")) || 1;

  const [currentPage, setCurrentPage] = useState(page);

  const { data, isLoading, error } = useSavedJobs(page);

  if (isLoading) return <PulsatingText text="Loading..." customStyles="h-screen flex items-center justify-center" />;
  if (error) return <PulsatingText text={`Error: ${error.message}`} />;
  if (!data) notFound();

  return (
    <div className="mx-auto flex min-h-screen max-w-3xl flex-col px-4 py-12 pb-20 sm:px-6 sm:py-16">
      <div className="flex flex-1 flex-col gap-4">
        {/* <BackButton text="Gå tillbaka" backFunction={() => router.push("/profile")} /> */}
        {data?.items && data.items.length < 1 && (
          <span className="text-center text-xl font-bold">Inga sparade jobb hittades</span>
        )}
        {data.items.map((savedJob: SavedJobResponse, index: number) => (
          <JobListItem key={savedJob.id} job={savedJob.jobListingMini} index={index} />
        ))}
      </div>

      {data.items.length > data.pageSize && (
        <Pagination
          totalPages={data?.totalPages ?? 0}
          totalCount={data?.totalCount ?? 0}
          pageSize={data?.pageSize ?? 0}
          currentPage={currentPage}
          onPageChange={setCurrentPage}
        />
      )}
    </div>
  );
}
