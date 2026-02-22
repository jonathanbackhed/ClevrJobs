import React from "react";
import { FieldError } from "react-hook-form";
import { cn } from "tailwind-variants";

interface Props extends React.InputHTMLAttributes<HTMLInputElement> {
  customStyles?: string;
  errors?: FieldError;
  placeholder?: string;
  type?: "url" | "email" | "number" | "tel";
}

export default function TextInput({ customStyles, errors, placeholder, type, ...rest }: Props) {
  return (
    <input
      type={type ? type : "text"}
      placeholder={placeholder}
      className={cn(
        "bg-cream-light rounded-lg border border-stone-600 p-2.5 ring-stone-800 focus:ring-offset-0 focus-visible:ring-1",
        errors && "border-red-600 ring-red-600 dark:border-red-800 dark:ring-red-800",
        customStyles,
      )}
      {...rest}
    />
  );
}
