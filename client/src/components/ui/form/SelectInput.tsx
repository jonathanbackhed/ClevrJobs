import { cn } from "@/lib/utils/helpers";
import React from "react";
import { FieldError } from "react-hook-form";

interface Props extends React.SelectHTMLAttributes<HTMLSelectElement> {
  children: React.ReactNode;
  customStyles?: string;
  errors?: FieldError;
  defaultValue?: string;
}

export default function SelectInput({ children, customStyles, errors, defaultValue, ...rest }: Props) {
  return (
    <select
      className={cn(
        "bg-cream-light rounded-lg border border-stone-600 p-2.5 ring-stone-800 focus:ring-offset-0 focus-visible:ring-1",
        errors && "border-red-600 ring-red-600 dark:border-red-800 dark:ring-red-800",
        customStyles,
      )}
      {...rest}
    >
      {children}
    </select>
  );
}
