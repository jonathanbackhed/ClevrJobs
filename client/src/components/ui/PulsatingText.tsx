import { cn } from "@/lib/utils";

interface Props {
  text: string;
  customStyles?: string;
}

export default function PulsatingText({ text, customStyles }: Props) {
  return <div className={cn("animate-pulse text-center text-xl font-bold", customStyles)}>{text}</div>;
}
