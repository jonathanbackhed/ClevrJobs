"use client";

import JobDetails from "@/components/layout/JobDetails";
import Logo from "@/components/ui/Logo";
import PulsatingText from "@/components/ui/PulsatingText";
import { useJob } from "@/hooks/useJobs";
import { notFound, useParams } from "next/navigation";

export default function Details() {
  const params = useParams<{ id: string }>();

  const jobId = Number(params.id);
  if (isNaN(jobId) || jobId <= 0) {
    notFound();
  }

  const { data, isLoading, error } = useJob(jobId);

  if (isLoading) return <PulsatingText text="Loading..." customStyles="h-screen flex items-center justify-center" />;
  if (error) return <PulsatingText text={`Error: ${error.message}`} />;
  if (!data) notFound();

  return (
    <div className="mx-auto max-w-3xl px-4 py-12 pb-20 sm:px-6 sm:py-16">
      <header className="animate-fade-in-down mb-12 text-center">
        <Logo />
      </header>
      <JobDetails job={data} />
    </div>
  );
}
