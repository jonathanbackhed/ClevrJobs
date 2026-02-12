import Link from "next/link";

export default function Logo() {
  return (
    <Link href={"/"}>
      <h1 className="text-accent relative inline-block font-serif text-4xl font-bold tracking-tight sm:text-6xl">
        ClevrJobs
        <span className="via-accent absolute -bottom-1 left-1/2 h-1 w-2/5 -translate-x-1/2 rounded-full bg-linear-to-r from-transparent to-transparent" />
      </h1>
    </Link>
  );
}
