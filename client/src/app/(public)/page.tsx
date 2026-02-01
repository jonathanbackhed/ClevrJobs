import JobList from "@/components/layout/JobList";
import Logo from "@/components/ui/Logo";
import { Search } from "lucide-react";

export default function Home() {
  return (
    <div className="mx-auto max-w-3xl px-4 py-12 pb-20 sm:px-6 sm:py-16">
      <header className="animate-fade-in-down mb-12 text-center">
        <Logo />
        <p className="my-5 text-stone-500">AI-summerade jobb från platsbanken</p>
        <div className="border-accent-light/30 dark:bg-cream-light inline-flex items-center gap-1.5 rounded-full border bg-white/90 px-4 py-2 text-sm text-stone-500 shadow-sm/5 shadow-stone-800 dark:shadow-stone-200">
          <Search height={16} />
          Sökning: <strong className="text-accent font-semibold">"C#"</strong>
        </div>
      </header>
      <JobList />
    </div>
  );
}
