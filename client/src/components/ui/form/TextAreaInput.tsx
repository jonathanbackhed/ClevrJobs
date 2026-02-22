import React from "react";
import { FieldError } from "react-hook-form";
import { cn } from "tailwind-variants";

interface Props extends React.SelectHTMLAttributes<HTMLTextAreaElement> {
  customStyles?: string;
  errors?: FieldError;
  placeholder?: string;
  rows?: number;
  cols?: number;
  resize?: boolean;
}

export default function TextAreaInput({ customStyles, errors, placeholder, rows, cols, resize, ...rest }: Props) {
  return (
    <textarea
      placeholder={placeholder}
      className={cn(
        "bg-cream-light rounded-lg border border-stone-600 p-2.5 ring-stone-800 focus:ring-offset-0 focus-visible:ring-1",
        errors && "border-red-600 ring-red-600 dark:border-red-800 dark:ring-red-800",
        !resize && "resize-none",
        customStyles,
      )}
      rows={rows ?? 2}
      cols={cols}
      {...rest}
    />
  );
}
