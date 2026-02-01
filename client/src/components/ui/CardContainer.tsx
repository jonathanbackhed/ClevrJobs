import { cn } from "@/lib/utils/helpers";
import React from "react";

interface Props {
  children: React.ReactNode;
  customStyles?: string;
  group?: boolean;
  hoverEffects?: boolean;
}

export default function CardContainer({ children, customStyles = "", group, hoverEffects }: Props) {
  return (
    <main
      className={cn(
        group && "group",
        "animate-fade-in-up bg-cream-light border-accent/15 relative cursor-default rounded-2xl border p-6 shadow-sm/5 shadow-stone-800 transition-all duration-250 ease-out dark:shadow-stone-200",
        hoverEffects && "hover:border-accent-light/40 hover:-translate-y-0.5 hover:shadow-md/10",
        customStyles,
      )}
    >
      {children}
    </main>
  );
}
