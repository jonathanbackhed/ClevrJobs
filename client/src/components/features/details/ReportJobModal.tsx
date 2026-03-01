import SelectInput from "@/components/ui/form/SelectInput";
import SubmitInput from "@/components/ui/form/SubmitInput";
import TextAreaInput from "@/components/ui/form/TextAreaInput";
import Modal from "@/components/ui/Modal";
import { getReasonLabel } from "@/lib/displayNameHelpers";
import type { ReportJobRequest } from "@/types";
import { ReportReason } from "@/types/enum";
import React, { useRef } from "react";
import toast from "react-hot-toast";

interface Props {
  showModal: boolean;
  setShowModal: React.Dispatch<React.SetStateAction<boolean>>;
  reportMutation: any;
}

export default function ReportJobModal({ showModal, setShowModal, reportMutation }: Props) {
  const formRef = useRef<HTMLFormElement>(null);

  const sendReport = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    const formData = new FormData(e.currentTarget);
    const reasonValue = formData.get("reportReason") as string;
    const testData: ReportJobRequest = {
      reason: Number(reasonValue) as ReportReason,
      description: (formData.get("reportDescription") as string)?.trim() || undefined,
    };

    reportMutation.mutate(testData, {
      onSuccess: () => {
        toast.success("Rapport skickad");
        setShowModal(false);
        formRef.current?.reset();
      },
      onError: () => {
        toast.error("Rapport skickades inte");
      },
    });
  };
  return (
    <Modal isOpen={showModal} close={() => setShowModal(false)}>
      <form ref={formRef} onSubmit={sendReport} className="flex flex-col gap-4">
        <h3 className="font-serif text-3xl">Rapportera annons</h3>
        <div className="flex flex-col">
          <label htmlFor="reportReason">Anledning:</label>
          <SelectInput name="reportReason" defaultValue="" required>
            <option value="" disabled>
              - Välj nedan -
            </option>
            {Object.values(ReportReason)
              .filter((key) => typeof key === "number")
              .map((reason: number) => {
                const name = getReasonLabel(Number(reason));
                return (
                  <option key={`reason-${reason}`} value={reason}>
                    {name}
                  </option>
                );
              })}
          </SelectInput>
        </div>
        <div className="flex flex-col">
          <label htmlFor="reportDescription">Beskrivning:</label>
          <TextAreaInput
            name="reportDescription"
            placeholder="Beskriv gärna ditt problem i detalj."
            maxLength={300}
            rows={4}
            cols={60}
          />
        </div>
        <SubmitInput value={reportMutation.isPending ? "Skickar..." : "Skicka"} disabled={reportMutation.isPending} />
      </form>
    </Modal>
  );
}
