"use client";

import AddNewJobModal from "@/components/features/tracker/AddNewJobModal";
import TrackedJob from "@/components/features/tracker/TrackedJob";
import Pagination from "@/components/layout/Pagination";
import BackButton from "@/components/ui/BackButton";
import CustomButton from "@/components/ui/CustomButton";
import PulsatingText from "@/components/ui/PulsatingText";
import { useCreateTrackedJob, useTrackedJobs } from "@/hooks/useTracked";
import { TrackedJobResponse } from "@/types/tracked";
import { notFound, useRouter, useSearchParams } from "next/navigation";
import React, { useState } from "react";

export default function Tracked() {
  const router = useRouter();
  const params = useSearchParams();
  const page = Number(params.get("page")) || 1;

  const [currentPage, setCurrentPage] = useState(page);
  const [showModal, setShowModal] = useState(false);

  const { data, isLoading, error } = useTrackedJobs(page);
  const createMutation = useCreateTrackedJob();

  if (isLoading) return <PulsatingText text="Loading..." customStyles="h-screen flex items-center justify-center" />;
  if (error) return <PulsatingText text={`Error: ${error.message}`} />;
  if (!data) notFound();

  return (
    <>
      <AddNewJobModal showModal={showModal} setShowModal={setShowModal} />
      <div className="mx-auto flex min-h-screen max-w-3xl flex-col px-4 py-12 pb-20 sm:px-6 sm:py-16">
        <div className="flex flex-1 flex-col gap-4">
          <div className="flex items-center justify-between">
            <BackButton text="Gå tillbaka" backFunction={() => router.push("/profile")} />
            <CustomButton type="button" action={() => setShowModal(true)} variant="filled">
              Skapa nytt jobb
            </CustomButton>
          </div>
          {data?.items && data.items.length < 1 && (
            <span className="text-center text-xl font-bold">Inga följda jobb hittades</span>
          )}
          {data.items.map((trackedJob: TrackedJobResponse, index: number) => (
            <TrackedJob key={trackedJob.id} job={trackedJob} />
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
    </>
  );
}
