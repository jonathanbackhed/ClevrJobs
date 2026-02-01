import { LucideProps } from "lucide-react";
import React, { ForwardRefExoticComponent, RefAttributes } from "react";

interface Props {
  icon: React.ReactNode;
  title?: string;
}

export default function SectionHeading({ icon, title }: Props) {
  return (
    <h2 className="mb-4 flex items-center gap-2.5 font-serif text-xl font-semibold text-stone-800">
      <span className="bg-accent-light/15 text-accent flex size-8 shrink-0 items-center justify-center rounded-full">
        {icon}
      </span>
      {title}
    </h2>
  );
}
