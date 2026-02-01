"use client";

import { useJob } from "@/hooks/useJobs";
import { formatDateTime, getSourceName, isMoreThan24hAgo } from "@/lib/utils/helpers";
import { CompetenceRank, JobListingDto } from "@/types/job";
import {
  Bot,
  Briefcase,
  Calendar,
  CheckCheck,
  ChevronLeft,
  Clock,
  ExternalLink,
  Info,
  MapPin,
  ScrollText,
  Link as LinkIcon,
  Cpu,
  Flag,
  Pickaxe,
} from "lucide-react";
import Link from "next/link";
import CompetenceTag from "../ui/CompetenceTag";
import RequirementTag from "../ui/RequirementTag";
import Badge from "../ui/Badge";
import CardContainer from "../ui/CardContainer";
import SectionHeading from "../ui/SectionHeading";
import toast, { Toaster } from "react-hot-toast";
import CustomButton from "../ui/CustomButton";
import Toast from "../ui/Toast";

interface Props {
  job: JobListingDto;
}

export default function JobDetails({ job }: Props) {
  const requirementsList = job.requiredTechnologies.split(",");
  const niceToHavesList = job.niceTohaveTechnologies.split(",");
  const keywordsCvList = job.keywordsCV.split(",");
  const keywordsClList = job.keywordsCL.split(",");

  const isOld = isMoreThan24hAgo(job.processedAt);

  const copyToClipboard = async () => {
    try {
      await navigator.clipboard.writeText(job.listingUrl);
      toast.success("Länk kopierad", {
        id: "clipboard",
      });
    } catch (err) {
      console.log("Failed to copy url", err);
    }
  };

  const sendReport = async () => {
    console.log("Not implemented yet!");
  };

  return (
    <div className="flex flex-col gap-4">
      <Toast />

      <Link
        href="/"
        className="group hover:text-accent flex items-center gap-1 text-sm font-medium text-stone-500 transition-all duration-200 hover:-translate-x-0.5"
      >
        <ChevronLeft className="transition-transform duration-200 group-hover:-translate-x-1" /> Gå tillbaka
      </Link>

      <div className="relative mt-4">
        {!isOld && <Badge text="Ny" />}
        <CardContainer group={true} customStyles="overflow-hidden flex flex-col gap-8">
          <div className="from-accent via-accent-light absolute top-0 right-0 left-0 h-1 bg-linear-to-r to-transparent" />

          <div className="flex flex-col gap-3">
            <div className="flex gap-3">
              <CompetenceTag rank={job.competenceRank} />
              <CompetenceTag rank={CompetenceRank.Unknown} customText={job.extent} />
            </div>
            <h2 className="font-serif text-3xl leading-tight font-bold tracking-tight text-stone-800 dark:text-stone-300">
              {job.title}
            </h2>
            <p className="flex items-center gap-1 text-stone-500">
              <MapPin size={14} opacity={0.6} />
              {job.companyName} – {job.location}
            </p>
            <p className="flex items-center gap-1 text-stone-500">
              <Pickaxe size={14} opacity={0.6} />
              {job.roleName}
            </p>
          </div>

          <div className="border-accent-light/30 border-t" />

          <div className="flex flex-row items-center justify-around gap-4">
            <div>
              <p className="mb-1 text-xs font-semibold tracking-wide text-stone-500 uppercase">Anställning</p>
              <p className="text-[0.925rem] font-semibold text-stone-800 dark:text-stone-300">
                {!job.duration ? job.extent : job.extent + " - " + job.duration}
              </p>
            </div>
            <div>
              <p className="mb-1 text-xs font-semibold tracking-wide text-stone-500 uppercase">Plats</p>
              <p className="text-[0.925rem] font-semibold text-stone-800 dark:text-stone-300">{job.location}</p>
            </div>
            <div>
              <p className="mb-1 text-xs font-semibold tracking-wide text-stone-500 uppercase">Sista ansökningsdag</p>
              <p className="text-accent text-[0.925rem] font-semibold">{job.applicationDeadline}</p>
            </div>
          </div>
        </CardContainer>
      </div>

      <CardContainer>
        <SectionHeading icon={<ScrollText size={18} />} title="Om tjänsten" />
        <p className="text-[0.925rem] leading-[1.75] text-stone-600 dark:text-stone-400">{job.description}</p>
      </CardContainer>

      <CardContainer>
        <SectionHeading icon={<CheckCheck size={18} />} title="Krav & kompetenser" />
        <p className="mb-2 text-xs font-semibold tracking-wide text-stone-500 uppercase">Krav</p>
        <div className="mb-4 flex flex-wrap gap-1.5">
          {requirementsList.map((req) => (
            <RequirementTag key={req} requirement={req} />
          ))}
        </div>
        <p className="mb-2 text-xs font-semibold tracking-wide text-stone-500 uppercase">Meriterande</p>
        <div className="flex flex-wrap gap-1.5">
          {niceToHavesList.map((req) => (
            <RequirementTag key={req} requirement={req} />
          ))}
        </div>
      </CardContainer>

      <CardContainer>
        <SectionHeading icon={<Flag size={18} />} title="CV & personligt brev" />
        <p className="mb-2 text-xs font-semibold tracking-wide text-stone-500 uppercase">Fokus CV</p>
        <div className="mb-4 flex flex-wrap gap-1.5">
          {keywordsCvList.map((req) => (
            <RequirementTag key={req} requirement={req} />
          ))}
        </div>
        <p className="mb-2 text-xs font-semibold tracking-wide text-stone-500 uppercase">Fokus personligt brev</p>
        <p className="mb-2 text-[0.925rem] leading-[1.75] text-stone-600 dark:text-stone-400">
          {job.customCoverLetterFocus}
        </p>
        <div className="flex flex-wrap gap-1.5">
          {keywordsClList.map((req) => (
            <RequirementTag key={req} requirement={req} />
          ))}
        </div>
      </CardContainer>

      <CardContainer>
        <SectionHeading icon={<Info size={18} />} title="Detaljer" />
        <div className="flex flex-col gap-2">
          <div className="flex items-start gap-3">
            <Calendar size={20} className="text-accent-light" />
            <span className="text-[0.925rem] text-stone-600 dark:text-stone-400">
              <strong className="font-semibold text-stone-800 dark:text-stone-300">Publicerad:</strong>{" "}
              {job.published.split(",")[0]}
            </span>
          </div>
          <div className="flex items-start gap-3">
            <MapPin size={20} className="text-accent-light" />
            <span className="text-[0.925rem] text-stone-600 dark:text-stone-400">
              <strong className="font-semibold text-stone-800 dark:text-stone-300">Arbetsplats:</strong> {job.location}
            </span>
          </div>
          <div className="flex items-start gap-3">
            <Briefcase size={20} className="text-accent-light" />
            <span className="text-[0.925rem] text-stone-600 dark:text-stone-400">
              <strong className="font-semibold text-stone-800 dark:text-stone-300">Arbetsgivare:</strong>{" "}
              {job.companyName}
            </span>
          </div>
          <div className="flex items-start gap-3">
            <Clock size={20} className="text-accent-light" />
            <span className="text-[0.925rem] text-stone-600 dark:text-stone-400">
              <strong className="font-semibold text-stone-800 dark:text-stone-300">Sista ansökningsdag:</strong>{" "}
              {job.applicationDeadline}
            </span>
          </div>
          <div className="flex items-start gap-3">
            <Cpu size={20} className="text-accent-light" />
            <span className="text-[0.925rem] text-stone-600 dark:text-stone-400">
              <strong className="font-semibold text-stone-800 dark:text-stone-300">Annonsen bearbetad:</strong>{" "}
              {formatDateTime(job.processedAt)}
            </span>
          </div>
        </div>
      </CardContainer>

      <CardContainer>
        <SectionHeading icon={<Bot size={18} />} title="AI motivering" />
        <p className="text-[0.925rem] leading-[1.75] text-stone-600 dark:text-stone-400">{job.motivation}</p>
      </CardContainer>

      <CardContainer>
        <div className="flex flex-col items-center justify-center gap-3">
          <p className="text-[0.925rem] leading-[1.75] text-stone-600 dark:text-stone-400">
            Intresserad? Ansök direkt hos {job.companyName} via platsbanken.
          </p>
          <div className="flex justify-center gap-3">
            <CustomButton
              type="a"
              action={job.listingUrl}
              variant="filled"
              size="md"
              customStyles="flex items-center gap-2 hover:-translate-y-px"
              target="_blank"
            >
              <ExternalLink size={16} className="-translate-y-0.5" /> Öppna på {getSourceName(job.source)}
            </CustomButton>
            <CustomButton
              type="button"
              action={copyToClipboard}
              variant="border"
              size="md"
              customStyles="gap-2 flex items-center"
            >
              <LinkIcon size={16} className="-translate-y-0.5" />
              Kopiera länk
            </CustomButton>
          </div>
        </div>
      </CardContainer>

      <div className="flex justify-center">
        <span className="mt-2 text-sm font-semibold">
          Något som inte stämmer?{" "}
          <CustomButton type="button" action={sendReport} variant="none" size="none">
            <strong className="text-accent hover:text-accent/80">Rapportera annons</strong>
          </CustomButton>
        </span>
      </div>
    </div>
  );
}
