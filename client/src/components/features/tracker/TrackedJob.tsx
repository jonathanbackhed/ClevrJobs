"use client";

import CardContainer from "@/components/ui/CardContainer";
import CustomButton from "@/components/ui/CustomButton";
import { useDeleteTrackedJob, useUpdateTrackedJob } from "@/hooks/useTracked";
import { CAME_FROM_LISTING, SCROLL_KEY } from "@/lib/constants";
import { formatDateTime, getApplicationStatusName } from "@/lib/utils/helpers";
import { SaveType } from "@/types/enum";
import { TrackedJobResponse } from "@/types/tracked";
import { Clock, MapPin, Pencil, Trash } from "lucide-react";

interface Props {
  job: TrackedJobResponse;
  index: number;
  onEdit: () => void;
}

export default function TrackedJob({ job, index, onEdit }: Props) {
  const deleteMutation = useDeleteTrackedJob();

  const handleDelete = () => {
    if (!confirm("Är du säker på att du vill ta bort jobbet?")) return;

    deleteMutation.mutate(job.id);
  };

  const saveScroll = () => {
    sessionStorage.setItem(SCROLL_KEY, window.scrollY.toString());
    sessionStorage.setItem(CAME_FROM_LISTING, "true");
  };

  return (
    <CardContainer
      customStyles="animate-fade-in-up transition-all duration-250 ease-out overflow-hidden sm:p-6 p-4"
      style={{ animationDelay: `${index * 50 + 100}ms` }}
    >
      <div className="mb-1 flex items-center justify-between">
        <h2 className="font-serif text-2xl leading-tight font-bold tracking-tight text-stone-800 dark:text-stone-300">
          {job.title}
        </h2>
        <span className="text-sm leading-tight font-medium text-nowrap text-stone-700 dark:text-stone-400">
          {getApplicationStatusName(job.applicationStatus)}
        </span>
      </div>
      <div className="mb-4">
        <p className="mb-1 flex items-center gap-1 text-sm text-stone-500">
          <MapPin size={14} opacity={0.6} />
          {job.companyName} – {job.location}
        </p>
        {job.listingUrl && (
          <a href={job.listingUrl} target="_blank" className="text-accent block gap-1 truncate text-sm">
            {job.listingUrl}
          </a>
        )}
      </div>
      {job.notes && (
        <>
          <p className="text-xs font-semibold tracking-wide text-stone-500 uppercase">Anteckningar</p>
          <p className="mb-4 text-[0.925rem] leading-relaxed text-stone-700 dark:text-stone-400">{job.notes}</p>
        </>
      )}
      {job.rejectReason && (
        <>
          <p className="text-xs font-semibold tracking-wide text-stone-500 uppercase">Anledning till nekad tjänst</p>
          <p className="mb-4 text-[0.925rem] leading-relaxed text-stone-700 dark:text-stone-400">{job.rejectReason}</p>
        </>
      )}
      <div className="border-accent-light/30 flex flex-col gap-3 border-t pt-4 sm:flex-row sm:items-center sm:justify-between">
        <div className="flex flex-col gap-0.5">
          {job.applicationDeadline && (
            <span className="flex items-center gap-1.5 text-sm text-stone-500">
              <Clock size={14} opacity={0.6} />
              Sista ansökningsdag:{" "}
              <strong className="font-semibold text-stone-700 dark:text-stone-400">
                {formatDateTime(job.applicationDeadline)}{" "}
                <span className="text-stone-500">({job.applicationDeadline})</span>
              </strong>
            </span>
          )}
          <span className="flex items-center gap-1.5 text-sm text-stone-500">
            <Clock size={14} opacity={0.6} />
            Jobb skapat för
            <strong className="font-semibold text-stone-700 dark:text-stone-400">
              {formatDateTime(job.createdAt)}
            </strong>
          </span>
        </div>
        <div className="flex items-center justify-end gap-4">
          <button onClick={onEdit}>
            <Pencil
              size={22}
              className="text-accent hover:text-accent-light transition-colors duration-200 hover:cursor-pointer"
            />
          </button>
          <button onClick={handleDelete}>
            <Trash
              size={22}
              className="text-accent hover:text-accent-light transition-colors duration-200 hover:cursor-pointer"
            />
          </button>
          {job.saveType === SaveType.SavedFromListing && job.processedJobId && (
            <CustomButton
              type="link"
              action={`/job/${job.processedJobId}`}
              onClick={saveScroll}
              children="Gå till annons"
              variant="borderHoverFill"
              size="sm"
              customStyles="text-center"
              scroll={false}
            />
          )}
        </div>
      </div>
    </CardContainer>
  );
}
