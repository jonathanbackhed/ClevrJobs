import CardContainer from "@/components/ui/CardContainer";
import { TrackedJobResponse } from "@/types/tracked";
import React from "react";

interface Props {
  job: TrackedJobResponse;
}

export default function TrackedJob({ job }: Props) {
  return (
    <CardContainer>
      <div></div>
    </CardContainer>
  );
}
