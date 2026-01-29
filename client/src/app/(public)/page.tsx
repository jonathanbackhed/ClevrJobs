import JobList from "@/components/layout/JobList";
import { Search } from "lucide-react";

export default function Home() {
  return (
    <div className="bg-cream relative min-h-screen font-sans text-stone-800">
      <div
        className="pointer-events-none fixed inset-0 z-0 opacity-10"
        style={{
          backgroundImage: `url("data:image/svg+xml,%3Csvg viewBox='0 0 400 400' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.9' numOctaves='3' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)'/%3E%3C/svg%3E")`,
        }}
      />
      <div className="relative z-10 mx-auto max-w-3xl px-4 py-12 pb-20 sm:px-6 sm:py-16">
        <header className="animate-fade-in-down mb-12 text-center">
          <h1 className="text-accent relative inline-block font-serif text-4xl font-bold tracking-tight sm:text-6xl">
            ClevrJobs
            <span className="via-accent absolute -bottom-1 left-1/2 h-1 w-2/5 -translate-x-1/2 rounded-full bg-linear-to-r from-transparent to-transparent" />
          </h1>
          <p className="my-5 text-stone-500">AI-summerade jobb från platsbanken</p>
          <div className="border-accent-light/30 inline-flex items-center gap-1.5 rounded-full border bg-white/90 px-4 py-2 text-sm text-stone-500 shadow-sm/5 shadow-stone-800">
            <Search height={16} />
            Sökning: <strong className="text-accent font-semibold">"C#"</strong>
          </div>
        </header>
        <JobList />
      </div>
    </div>
  );
}
