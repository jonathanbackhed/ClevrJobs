import { ChevronLeft } from "lucide-react";

interface Props {
  text: string;
  backFunction: () => void;
}

export default function BackButton({ text, backFunction }: Props) {
  return (
    <button
      onClick={backFunction}
      className="group hover:text-accent flex w-fit cursor-pointer items-center gap-1 text-sm font-medium text-stone-500 transition-all duration-200 hover:-translate-x-0.5"
    >
      <ChevronLeft className="transition-transform duration-200 group-hover:-translate-x-1" /> {text}
    </button>
  );
}
