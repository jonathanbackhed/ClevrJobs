import { cn } from "@/lib/utils";

interface Props extends React.InputHTMLAttributes<HTMLInputElement> {
  value: string;
  disabled?: boolean;
  customStyles?: string;
}

export default function SubmitInput({ value, disabled, customStyles, ...rest }: Props) {
  return (
    <input
      type="submit"
      value={value}
      disabled={disabled}
      className={cn(
        "bg-accent outline-accent cursor-pointer rounded-2xl px-3 py-2 text-white ring-stone-800 outline-0 transition-opacity duration-150 hover:opacity-90 focus:ring-offset-0 focus-visible:ring-1 active:opacity-100 disabled:opacity-25",
        customStyles,
      )}
    />
  );
}
