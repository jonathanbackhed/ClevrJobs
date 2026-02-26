"use client";

import TrackedJobModal from "@/components/features/tracker/TrackedJobModal";
import TrackedJob from "@/components/features/tracker/TrackedJob";
import Pagination from "@/components/layout/Pagination";
import CustomButton from "@/components/ui/CustomButton";
import PulsatingText from "@/components/ui/PulsatingText";
import Toast from "@/components/ui/Toast";
import { useTrackedJobs } from "@/hooks/useTracked";
import { SCROLL_KEY } from "@/lib/constants";
import { TrackedJobResponse } from "@/types/tracked";
import { notFound, useSearchParams } from "next/navigation";
import { useEffect, useMemo, useState } from "react";
import { Plus, SlidersHorizontal } from "lucide-react";
import TrackedJobFilter from "@/components/features/tracker/TrackedJobFilter";
import { FilterOptions } from "@/types/filter";

export default function Tracked() {
  const params = useSearchParams();
  const page = Number(params.get("page")) || 1;

  const [currentPage, setCurrentPage] = useState(page);
  const [showModal, setShowModal] = useState(false);
  const [showFilter, setShowFilter] = useState(false);
  const [selectedJob, setSelectedJob] = useState<TrackedJobResponse | undefined>(undefined);
  const [filterOptions, setFilterOptions] = useState<FilterOptions>({
    from: "",
    to: "",
    applicationStatus: undefined,
  });

  const { data, isLoading, error } = useTrackedJobs(page);

  const handleEdit = (job: TrackedJobResponse) => {
    setSelectedJob(job);
    setShowModal(true);
  };

  const handleClose = () => {
    setShowModal(false);
    setSelectedJob(undefined);
  };

  const filteredJobs = useMemo(() => {
    return data?.items.filter((job: TrackedJobResponse) => {
      const date = new Date(job.createdAt);
      const month = date.getMonth() + 1;
      const year = date.getFullYear();

      const matchesFrom =
        filterOptions.from && filterOptions.from !== ""
          ? year > Number(filterOptions.from.split("-")[0]) ||
            (year === Number(filterOptions.from.split("-")[0]) && month >= Number(filterOptions.from.split("-")[1]))
          : true;

      const matchesTo =
        filterOptions.to && filterOptions.to !== ""
          ? year < Number(filterOptions.to.split("-")[0]) ||
            (year === Number(filterOptions.to.split("-")[0]) && month <= Number(filterOptions.to.split("-")[1]))
          : true;

      const matchesStatus =
        filterOptions.applicationStatus && filterOptions.applicationStatus !== undefined
          ? job.applicationStatus === Number(filterOptions.applicationStatus)
          : true;

      return matchesFrom && matchesTo && matchesStatus;
    });
  }, [data, filterOptions]);

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
      <div className="mx-auto flex max-w-4xl flex-col px-4 py-12 pb-20 sm:px-6 sm:py-16">
        <header className="animate-fade-in-down mb-12 text-center">
          <span className="text-accent relative inline-block font-serif text-4xl font-bold tracking-tight sm:text-6xl">
            Tracker
          </span>
        </header>
        <div className="mb-4 flex items-center justify-between">
          <CustomButton type="button" action={() => setShowModal(true)} variant="filled" customStyles="w-auto">
            Skapa nytt jobb
          </CustomButton>
          <button
            onClick={() => setShowFilter((prev) => !prev)}
            className="block hover:cursor-pointer hover:opacity-70 md:hidden"
          >
            <SlidersHorizontal size={24} />
          </button>
        </div>
        {showFilter && (
          <div className="mb-4 block md:hidden">
            <TrackedJobFilter filterOptions={filterOptions} onFilterChange={setFilterOptions} />
          </div>
        )}
        <div className="flex flex-1 gap-4">
          <div className="flex w-full flex-col gap-4">
            {filteredJobs && filteredJobs.length > 0 ? (
              filteredJobs.map((trackedJob: TrackedJobResponse, index: number) => (
                <TrackedJob key={trackedJob.id} job={trackedJob} index={index} onEdit={() => handleEdit(trackedJob)} />
              ))
            ) : (
              <span className="text-center text-xl font-bold">Inga följda jobb hittades</span>
            )}
          </div>
          <div className="bg-cream-light border-accent/15 sticky top-6 hidden w-52 shrink-0 self-start rounded-2xl p-4 shadow-stone-800 md:block">
            <TrackedJobFilter filterOptions={filterOptions} onFilterChange={setFilterOptions} />
          </div>
        </div>

        <button
          onClick={() => setShowModal(true)}
          className="bg-cream-light border-accent/30 hover:bg-accent fixed right-4 bottom-4 z-40 rounded-full border p-4 transition-colors duration-150 hover:cursor-pointer"
        >
          <Plus size={22} />
        </button>

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
