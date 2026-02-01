import { cn } from "@/lib/utils/helpers";
import Link from "next/link";
import { tv } from "tailwind-variants";

type ButtonProps = React.ButtonHTMLAttributes<HTMLButtonElement>;
type LinkProps = React.ComponentProps<typeof Link>;
type AnchorProps = React.AnchorHTMLAttributes<HTMLAnchorElement>;

type CommonProps = {
  children: React.ReactNode;
  variant: "filled" | "borderHoverFill" | "border" | "none";
  size?: "sm" | "md" | "lg" | "none";
  customStyles?: string;
};

type Props =
  | (CommonProps & {
      type: "button";
      action: () => void;
    } & ButtonProps)
  | (CommonProps & {
      type: "link";
      action: LinkProps["href"];
    } & Omit<LinkProps, "href">)
  | (CommonProps & {
      type: "a";
      action: AnchorProps["href"];
    } & Omit<AnchorProps, "href">);

export default function CustomButton({ children, type, action, variant, size, customStyles, ...rest }: Props) {
  const bs = buttonStyles({ type: variant, size });

  if (type === "button") {
    return (
      <button onClick={action} className={cn(bs, customStyles)} {...(rest as ButtonProps)}>
        {children}
      </button>
    );
  }

  if (type === "link") {
    const { href, ...linkRest } = rest as LinkProps;
    return (
      <Link href={action} className={cn(bs, customStyles)} {...linkRest}>
        {children}
      </Link>
    );
  }

  if (type === "a") {
    return (
      <a href={typeof action === "string" ? action : "#"} className={cn(bs, customStyles)} {...(rest as AnchorProps)}>
        {children}
      </a>
    );
  }
}

const buttonStyles = tv({
  base: "w-full sm:w-auto rounded-full cursor-pointer font-semibold disabled:cursor-default",
  variants: {
    type: {
      filled: "bg-accent shadow-accent rounded-full text-white transition-all duration-200 hover:shadow-md/50",
      borderHoverFill:
        "border border-accent text-accent transition-colors duration-200 hover:bg-accent active:bg-accent hover:text-white active:text-white",
      border:
        "hover:border-accent hover:text-accent border border-stone-500/30 bg-transparent text-stone-500 transition-all duration-200",
      none: "",
    },
    size: {
      sm: "text-sm px-5 py-2",
      md: "text-[0.925rem] px-5 py-2",
      lg: "text-lg px-6 py-3",
      none: "",
    },
  },
  defaultVariants: {
    size: "md",
  },
});
