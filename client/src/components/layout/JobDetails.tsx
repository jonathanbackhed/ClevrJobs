"use client";

import { useReportJob } from "@/hooks/useJobs";
import { formatDateTime, getReasonName, getSourceName, isMoreThan24hAgo } from "@/lib/utils/helpers";
import { CompetenceRank, JobListingDto, ReportJobRequest } from "@/types/job";
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
import CompetenceTag from "../ui/CompetenceTag";
import RequirementTag from "../ui/RequirementTag";
import Badge from "../ui/Badge";
import CardContainer from "../ui/CardContainer";
import SectionHeading from "../ui/SectionHeading";
import toast from "react-hot-toast";
import CustomButton from "../ui/CustomButton";
import Toast from "../ui/Toast";
import { useRouter } from "next/navigation";
import { useEffect, useRef, useState } from "react";
import { ReportReason } from "@/types/enum";
import Modal from "../ui/Modal";
import { CAME_FROM_LISTING } from "@/lib/constants";
import SaveButton from "../ui/SaveButton";

interface Props {
  job: JobListingDto;
}

export default function JobDetails({ job }: Props) {
  const [showModal, setShowModal] = useState(false);

  const formRef = useRef<HTMLFormElement>(null);

  const reportMutation = useReportJob(job.id);
  const router = useRouter();

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

  const sendReport = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    const formData = new FormData(e.currentTarget);
    const reasonValue = formData.get("reportReason") as string;
    const testData: ReportJobRequest = {
      reason: Number(reasonValue) as ReportReason,
      description: (formData.get("reportDescription") as string)?.trim() || undefined,
    };

    reportMutation.mutate(testData, {
      onSuccess: () => {
        toast.success("Rapport skickad");
        setShowModal(false);
        formRef.current?.reset();
      },
      onError: () => {
        toast.error("Rapport skickades inte");
      },
    });
  };

  const handleBack = () => {
    if (sessionStorage.getItem(CAME_FROM_LISTING) === "true") {
      sessionStorage.removeItem(CAME_FROM_LISTING);
      router.back();
    } else {
      router.push("/");
    }
  };

  useEffect(() => {
    if (!showModal) return;

    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        setShowModal(false);
      }
    };

    document.addEventListener("keydown", handleEscape);

    return () => {
      document.removeEventListener("keydown", handleEscape);
    };
  }, [showModal]);

  return (
    <div className="flex flex-col gap-4">
      <Modal isOpen={showModal} close={setShowModal}>
        <form ref={formRef} onSubmit={sendReport} className="flex flex-col gap-4">
          <h3 className="font-serif text-3xl">Rapportera annons</h3>
          <div className="flex flex-col">
            <label htmlFor="reportReason">Anledning:</label>
            <select
              name="reportReason"
              defaultValue=""
              required
              className="bg-cream-light outline-accent rounded-lg px-3 py-2 outline-0 focus:outline-2"
            >
              <option value="" disabled>
                - Välj nedan -
              </option>
              {Object.values(ReportReason)
                .filter((key) => typeof key === "number")
                .map((reason: number) => {
                  const name = getReasonName(Number(reason));
                  return (
                    <option key={`reason-${reason}`} value={reason}>
                      {name}
                    </option>
                  );
                })}
            </select>
          </div>
          <div className="flex flex-col">
            <label htmlFor="reportDescription">Beskrivning:</label>
            <textarea
              name="reportDescription"
              placeholder="Beskriv gärna ditt problem i detalj."
              maxLength={300}
              className="bg-cream-light outline-accent resize-none rounded-2xl p-2.5 outline-0 focus:outline-2"
              rows={4}
              cols={60}
            />
          </div>
          <input
            type="submit"
            value={reportMutation.isPending ? "Skickar..." : "Skicka"}
            disabled={reportMutation.isPending}
            className="bg-accent outline-accent cursor-pointer rounded-2xl px-3 py-2 text-white outline-0 focus:outline-1 disabled:opacity-25"
          />
        </form>
      </Modal>
      <Toast />

      <button
        onClick={handleBack}
        className="group hover:text-accent flex w-fit cursor-pointer items-center gap-1 text-sm font-medium text-stone-500 transition-all duration-200 hover:-translate-x-0.5"
      >
        <ChevronLeft className="transition-transform duration-200 group-hover:-translate-x-1" /> Gå tillbaka
      </button>

      <div className="relative mt-4">
        {!isOld && <Badge text="Ny" />}
        <CardContainer group={true} customStyles="overflow-hidden flex flex-col gap-8">
          <div className="from-accent via-accent-light absolute top-0 right-0 left-0 h-1 bg-linear-to-r to-transparent" />

          <div className="flex flex-col gap-3">
            <div className="flex justify-between">
              <div className="flex gap-3">
                <CompetenceTag rank={job.competenceRank} />
                <CompetenceTag rank={CompetenceRank.Unknown} customText={job.extent} />
              </div>
              <SaveButton id={job.id} />
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

          <div className="flex flex-row items-start justify-around gap-4 sm:items-center">
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
              customStyles="flex items-center justify-center gap-2 hover:-translate-y-px"
              target="_blank"
            >
              <ExternalLink size={16} className="-translate-y-0.5" /> Öppna på {getSourceName(job.source)}
            </CustomButton>
            <CustomButton
              type="button"
              action={copyToClipboard}
              variant="border"
              size="md"
              customStyles="gap-2 flex items-center justify-center"
            >
              <LinkIcon size={16} className="-translate-y-0.5" />
              Kopiera länk
            </CustomButton>
          </div>
        </div>
      </CardContainer>

      <div className="flex justify-center">
        <span className="mt-2 text-center text-sm font-semibold">
          Något som inte stämmer?{" "}
          <CustomButton
            type="button"
            action={() => setShowModal((prev) => !prev)}
            disabled={reportMutation.isPending}
            variant="none"
            size="none"
          >
            <strong className="text-accent hover:text-accent/80">Rapportera annons</strong>
          </CustomButton>
        </span>
      </div>
    </div>
  );
}
