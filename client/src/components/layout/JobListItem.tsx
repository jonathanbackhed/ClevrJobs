import { GetCompetenceRankOrDefault } from "@/lib/utils/helpers";
import { JobListingMiniDto } from "@/types/Job";
import Link from "next/link";

interface Props {
  job: JobListingMiniDto;
}

export default function JobListItem({ job }: Props) {
  const competenceRank = GetCompetenceRankOrDefault(job.competenceRank);

  return (
    <li className="rounded-lg bg-zinc-200 p-2">
      <Link href="#" className="space-y-4">
        <div className="flex justify-between">
          <div className="flex flex-col">
            <p className="text-xs">{competenceRank}</p>
            <h3 className="text-2xl leading-6 font-bold">{job.title}</h3>
            <p className="text-sm leading-4.5">{job.companyName}</p>
            <p className="text-sm leading-4.5">
              {job.roleName} - {job.location}
            </p>
          </div>
          <div className="flex flex-col">
            <p className="text-end text-xs">{job.extent}</p>
            <p className="text-xs">{job.duration}</p>
          </div>
        </div>
        <div>
          <p className="leading-5 font-bold">{job.description}</p>
        </div>
        <div>
          <p className="overflow-hidden text-sm text-ellipsis whitespace-nowrap">
            <span className="font-bold">Krav:</span> {job.requiredTechnologies}
          </p>
          <p className="text-sm">
            <span className="font-bold">Meriterande:</span> {job.niceTohaveTechnologies}
          </p>
          <p className="text-xs">
            Sista ansökningsdag: <span className="font-bold">{job.applicationDeadline}</span>
          </p>
        </div>
      </Link>
    </li>
  );
}
