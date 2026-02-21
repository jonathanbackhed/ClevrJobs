"use client";

import TrackedJobModal from "@/components/features/tracker/TrackedJobModal";
import TrackedJob from "@/components/features/tracker/TrackedJob";
import Pagination from "@/components/layout/Pagination";
import BackButton from "@/components/ui/BackButton";
import CustomButton from "@/components/ui/CustomButton";
import PulsatingText from "@/components/ui/PulsatingText";
import Toast from "@/components/ui/Toast";
import { useTrackedJobs } from "@/hooks/useTracked";
import { SCROLL_KEY } from "@/lib/constants";
import { TrackedJobResponse } from "@/types/tracked";
import { notFound, useRouter, useSearchParams } from "next/navigation";
import { useEffect, useState } from "react";

export default function Tracked() {
  const router = useRouter();
  const params = useSearchParams();
  const page = Number(params.get("page")) || 1;

  const [currentPage, setCurrentPage] = useState(page);
  const [showModal, setShowModal] = useState(false);
  const [selectedJob, setSelectedJob] = useState<TrackedJobResponse | undefined>(undefined);

  const { data, isLoading, error } = useTrackedJobs(page);

  const handleEdit = (job: TrackedJobResponse) => {
    setSelectedJob(job);
    setShowModal(true);
  };

  const handleClose = () => {
    setShowModal(false);
    setSelectedJob(undefined);
  };

  useEffect(() => {
    const savedPos = sessionStorage.getItem(SCROLL_KEY);
    if (savedPos) {
      window.scrollTo(0, parseInt(savedPos));
      sessionStorage.removeItem(SCROLL_KEY);
    }
  }, []);

  if (isLoading) return <PulsatingText text="Loading..." customStyles="h-screen flex items-center justify-center" />;
  if (error) return <PulsatingText text={`Error: ${error.message}`} />;
  if (!data) notFound();

  return (
    <>
      <Toast />
      <TrackedJobModal showModal={showModal} onClose={handleClose} defaultValues={selectedJob} />
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
            <TrackedJob key={trackedJob.id} job={trackedJob} index={index} onEdit={() => handleEdit(trackedJob)} />
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
