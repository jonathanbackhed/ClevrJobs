"use client";

import Pagination from "@/components/layout/Pagination";
import BackButton from "@/components/ui/BackButton";
import CardContainer from "@/components/ui/CardContainer";
import CustomButton from "@/components/ui/CustomButton";
import PulsatingText from "@/components/ui/PulsatingText";
import { useSavedJobs } from "@/hooks/useProfile";
import { cn, formatDateTime, getApplicationStatusName, getSaveTypeName } from "@/lib/utils/helpers";
import { SavedJobResponse } from "@/types/user";
import { Clock, Heart, MapPin } from "lucide-react";
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
        <BackButton text="Gå tillbaka" backFunction={() => router.push("/profile")} />
        {data?.items && data.items.length < 1 && (
          <span className="text-center text-xl font-bold">Inga sparade jobb hittades</span>
        )}
        {data.items.map((savedJob: SavedJobResponse) => (
          <CardContainer key={savedJob.id}>
            <div className="relative">
              <div className="flex flex-row items-start justify-between gap-3">
                <button>
                  <Heart
                    size={22}
                    className="fill-red-500 text-red-500 transition-colors duration-200 hover:cursor-pointer hover:opacity-70"
                  />
                </button>
                <div className="flex flex-col items-end gap-0 text-right">
                  <span className="text-sm leading-tight font-medium text-stone-700 dark:text-stone-400">
                    {getApplicationStatusName(savedJob.applicationStatus)}
                  </span>
                  <span className="text-sm leading-tight text-stone-500">{getSaveTypeName(savedJob.saveType)}</span>
                </div>
              </div>
              <h2 className="mb-1 font-serif text-2xl leading-tight font-bold tracking-tight text-stone-800 dark:text-stone-300">
                {savedJob.title}
              </h2>
              <p className="mb-4 flex items-center gap-1 text-sm text-stone-500">
                <MapPin size={14} opacity={0.6} />
                {savedJob.companyName} – {savedJob.location}
              </p>
              <div className="mb-4">
                <p className="mb-2 text-xs font-semibold tracking-wide text-stone-500 uppercase">Anteckningar</p>
                <p className="mb-4 text-[0.925rem] leading-relaxed text-stone-700 dark:text-stone-400">
                  {savedJob.notes}
                </p>
                {savedJob.rejectReason && (
                  <>
                    <p className="mb-2 text-xs font-semibold tracking-wide text-stone-500 uppercase">
                      Anledning för nekad
                    </p>
                    <p className="mb-4 text-[0.925rem] leading-relaxed text-stone-700 dark:text-stone-400">
                      {savedJob.rejectReason}
                    </p>
                  </>
                )}
              </div>
              <div className="border-accent-light/30 flex flex-col gap-3 border-t pt-4 sm:flex-row sm:items-center sm:justify-between">
                <div>
                  <span className="flex items-center gap-1.5 text-sm text-stone-500">
                    <Clock size={14} opacity={0.6} />
                    Sista ansökningsdag:{" "}
                    <strong className="font-semibold text-stone-700 dark:text-stone-400">
                      {savedJob.applicationDeadline}
                    </strong>
                  </span>
                  <span className="flex items-center gap-1.5 text-sm text-stone-500">
                    Sparat för
                    <strong className="font-semibold text-stone-700 dark:text-stone-400">
                      {formatDateTime(savedJob.savedAt)}
                    </strong>
                  </span>
                </div>
                <div className="flex gap-2">
                  <CustomButton
                    type="button"
                    action={() => console.log("")}
                    children="Redigera"
                    variant="borderHoverFill"
                    size="sm"
                    customStyles="text-center"
                  />
                  <CustomButton
                    type="link"
                    action={`asd`}
                    // onClick={saveScroll}
                    children="Visa mer"
                    variant="borderHoverFill"
                    size="sm"
                    customStyles="text-center"
                    scroll={false}
                  />
                </div>
              </div>
            </div>
          </CardContainer>
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
