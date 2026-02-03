import React from "react";

interface Props {
  text: string;
  color?: string;
}

export default function Badge({ text, color }: Props) {
  return (
    <span className="bg-accent absolute -top-3 -left-2 z-30 w-fit rounded-md px-2.5 py-1 text-[0.7rem] font-semibold tracking-wide text-white">
      {text}
    </span>
  );
}
