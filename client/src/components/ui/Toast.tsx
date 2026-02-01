import React from "react";
import { Toaster } from "react-hot-toast";

export default function Toast() {
  return (
    <Toaster
      toastOptions={{
        style: {
          background: "var(--bg-warm)",
          color: "var(--toast-text)",
        },
      }}
    />
  );
}
