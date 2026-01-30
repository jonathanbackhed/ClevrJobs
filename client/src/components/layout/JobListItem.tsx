import { JobListingMiniDto } from "@/types/job";
import { Clock, MapPin } from "lucide-react";
import CompetenceTag from "../ui/CompetenceTag";
import RequirementTag from "../ui/RequirementTag";
import CustomButton from "../ui/CustomButton";
import { isMoreThan24hAgo } from "@/lib/utils/helpers";
import Badge from "../ui/Badge";

interface Props {
  job: JobListingMiniDto;
  index: number;
}

export default function JobListItem({ job, index }: Props) {
  const requirementsList = job.requiredTechnologies.split(",");

  const isOld = isMoreThan24hAgo(job.processedAt);

  return (
    <article
      className="group animate-fade-in-up bg-cream-light border-accent/15 hover:border-accent-light/40 relative cursor-pointer rounded-2xl border p-6 shadow-sm/5 shadow-stone-800 transition-all duration-250 ease-out hover:-translate-y-0.5 hover:shadow-md/10"
      style={{ animationDelay: `${index * 50 + 100}ms` }}
    >
      {!isOld && <Badge text="Ny" />}

      <div className="from-accent to-accent-light absolute top-0 bottom-0 left-0 w-1 bg-linear-to-b opacity-0 transition-opacity duration-250 group-hover:opacity-100" />

      <div className="flex flex-row items-start justify-between gap-3">
        <CompetenceTag rank={job.competenceRank} />
        <div className="flex flex-col items-end gap-0 text-right">
          <span className="text-sm leading-tight font-medium text-stone-700">{job.extent}</span>
          <span className="text-sm leading-tight text-stone-500">{job.duration}</span>
        </div>
      </div>
      <h2 className="mb-1 font-serif text-2xl leading-tight font-bold tracking-tight text-stone-800">{job.title}</h2>
      <p className="mb-4 flex items-center gap-1 text-sm text-stone-500">
        <MapPin size={14} opacity={0.6} />
        {job.companyName} – {job.location}
      </p>
      <p className="mb-4 text-[0.925rem] leading-relaxed text-stone-700">{job.description}</p>
      <div className="mb-4">
        <p className="mb-2 text-xs font-semibold tracking-wide text-stone-500 uppercase">Krav</p>
        <div className="flex flex-wrap gap-1.5">
          {requirementsList.map((req) => (
            <RequirementTag key={req} requirement={req} />
          ))}
        </div>
      </div>
      <div className="border-accent-light/30 flex flex-col gap-3 border-t pt-4 sm:flex-row sm:items-center sm:justify-between">
        <span className="flex items-center gap-1.5 text-sm text-stone-500">
          <Clock size={14} opacity={0.6} />
          Sista ansökningsdag: <strong className="font-semibold text-stone-700">{job.applicationDeadline}</strong>
        </span>
        <CustomButton text="Visa mer" />
      </div>
    </article>
  );
}
