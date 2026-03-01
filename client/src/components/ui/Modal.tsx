"use client";

import { cn } from "@/lib/utils";
import { X } from "lucide-react";
import React, { useEffect } from "react";
import CustomButton from "./CustomButton";

interface Props {
  children: React.ReactNode;
  isOpen: boolean;
  close: () => void;
  customStyles?: string;
}

export default function Modal({ children, isOpen, close, customStyles }: Props) {
  useEffect(() => {
    if (!isOpen) return;

    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        close();
      }
    };

    document.addEventListener("keydown", handleEscape);

    return () => {
      document.removeEventListener("keydown", handleEscape);
    };
  }, [isOpen]);

  useEffect(() => {
    if (isOpen) {
      document.body.classList.add("overflow-hidden");
    } else {
      document.body.classList.remove("overflow-hidden");
    }

    return () => {
      document.body.classList.remove("overflow-hidden");
    };
  }, [isOpen]);

  return (
    <div
      className={cn(
        "fixed top-0 right-0 bottom-0 left-0 z-50 h-screen w-screen items-center justify-center bg-black/30 backdrop-blur-sm",
        isOpen ? "flex" : "hidden",
      )}
    >
      <div
        className={cn(
          "bg-cream animate-slide-in-from-top relative h-full max-h-svh w-full max-w-svw overflow-y-auto rounded-none p-10 sm:h-auto sm:w-auto sm:max-w-3xl sm:rounded-2xl",
          customStyles,
        )}
      >
        <div className="absolute top-3 right-3">
          <CustomButton type="button" action={() => close()} variant="none" customStyles="p-0 hover:opacity-60">
            <X size={22} />
          </CustomButton>
        </div>
        {children}
      </div>
    </div>
  );
}
