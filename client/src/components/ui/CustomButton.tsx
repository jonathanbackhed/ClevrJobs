import Link from "next/link";

interface Props {
  text: string;
  href?: string;
}

export default function CustomButton({ text, href }: Props) {
  return href ? (
    <Link
      href={href}
      className="border-accent text-accent hover:bg-accent active:bg-accent w-full cursor-pointer rounded-full border px-5 py-2 text-sm font-semibold transition-colors duration-200 hover:text-white active:text-white sm:w-auto"
    >
      {text}
    </Link>
  ) : (
    <button className="border-accent text-accent hover:bg-accent active:bg-accent w-full cursor-pointer rounded-full border px-5 py-2 text-sm font-semibold transition-colors duration-200 hover:text-white active:text-white sm:w-auto">
      {text}
    </button>
  );
}
