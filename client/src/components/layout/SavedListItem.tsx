import { formatDateTime, getApplicationStatusName, getSaveTypeName } from "@/lib/utils/helpers";
import { SavedJobResponse } from "@/types/user";
import CardContainer from "../ui/CardContainer";
import SaveButton from "../ui/SaveButton";
import { Clock, MapPin } from "lucide-react";
import CustomButton from "../ui/CustomButton";

interface Props {
  savedJob: SavedJobResponse;
}

export default function SavedListItem({ savedJob }: Props) {
  return (
    <CardContainer key={savedJob.id}>
      <div className="relative">
        <div className="flex flex-row items-start justify-between gap-3">
          <SaveButton id={savedJob.id} />
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
          <p className="mb-4 text-[0.925rem] leading-relaxed text-stone-700 dark:text-stone-400">{savedJob.notes}</p>
          {savedJob.rejectReason && (
            <>
              <p className="mb-2 text-xs font-semibold tracking-wide text-stone-500 uppercase">Anledning för nekad</p>
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
  );
}
