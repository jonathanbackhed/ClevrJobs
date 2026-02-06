"use client";

import { cn } from "@/lib/utils/helpers";
import { X } from "lucide-react";
import React, { useEffect } from "react";
import CustomButton from "./CustomButton";

interface Props {
  children: React.ReactNode;
  isOpen: boolean;
  close: (state: boolean) => void;
}

export default function Modal({ children, isOpen, close }: Props) {
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
      <div className="bg-cream animate-slide-in-from-top relative max-h-svh max-w-svw rounded-2xl p-10 sm:max-w-3xl">
        <div className="absolute top-3 right-3">
          <CustomButton type="button" action={() => close(false)} variant="none" customStyles="p-0 hover:opacity-60">
            <X size={22} />
          </CustomButton>
        </div>
        {children}
      </div>
    </div>
  );
}
