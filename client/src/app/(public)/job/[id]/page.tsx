import JobDetails from "@/components/layout/JobDetails";
import Logo from "@/components/ui/Logo";
import { api } from "@/lib/api";
import { notFound } from "next/navigation";

export default async function Details({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;

  const jobId = Number(id);
  if (isNaN(jobId) || jobId <= 0) {
    notFound();
  }

  const jobData = await api.getSingleJob(jobId); // next error boundary handles error
  if (!jobData) {
    notFound();
  }

  return (
    <div className="mx-auto max-w-3xl px-4 py-12 pb-20 sm:px-6 sm:py-16">
      <header className="animate-fade-in-down mb-12 text-center">
        <Logo />
      </header>
      <JobDetails initData={jobData} />
    </div>
  );
}
