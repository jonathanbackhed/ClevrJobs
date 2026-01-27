import JobList from "@/components/layout/JobList";

export default function Home() {
  return (
    <div className="flex h-screen items-center justify-center bg-zinc-50 font-mono">
      <div className="flex flex-col">
        <h1 className="text-center text-4xl">ClevrJobs</h1>
        <p>Here are the latest processed jobs for with query C# from Platsbanken.</p>
        <div>
          <JobList />
        </div>
      </div>
    </div>
  );
}
