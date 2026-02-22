"use client";

import CheckboxInput from "@/components/ui/form/CheckboxInput";
import DateInput from "@/components/ui/form/DateInput";
import SelectInput from "@/components/ui/form/SelectInput";
import TextAreaInput from "@/components/ui/form/TextAreaInput";
import TextInput from "@/components/ui/form/TextInput";
import Modal from "@/components/ui/Modal";
import { useCreateTrackedJob, useUpdateTrackedJob } from "@/hooks/useTracked";
import { cn, getApplicationStatusName, toUndefinedIfEmpty } from "@/lib/utils/helpers";
import { ApplicationStatus } from "@/types/enum";
import { TrackedJobRequest, TrackedJobResponse } from "@/types/tracked";
import { useEffect } from "react";
import { useForm } from "react-hook-form";

const emptyValues: TrackedJobRequest = {
  applicationStatus: ApplicationStatus.NotApplied,
  rejectReason: undefined,
  notes: undefined,
  applyDate: undefined,
  haveCalled: false,
  spontaneousApplication: false,
  title: "",
  companyName: "",
  location: "",
  applicationDeadline: "",
  listingUrl: "",
};

interface Props {
  showModal: boolean;
  onClose: () => void;
  defaultValues?: TrackedJobResponse;
}

export default function TrackedJobModal({ showModal, onClose, defaultValues }: Props) {
  const isEdit = !!defaultValues;

  const createMutation = useCreateTrackedJob();
  const updateMutation = useUpdateTrackedJob(defaultValues?.id);
  const {
    register,
    handleSubmit,
    watch,
    formState: { errors },
    reset,
  } = useForm<TrackedJobRequest>({
    defaultValues: defaultValues ?? emptyValues,
  });

  const statusAsNumber = Number(watch("applicationStatus"));
  const showNotes = statusAsNumber > 0;
  const showRejectReason = statusAsNumber === ApplicationStatus.Rejected;

  const createTrackedJob = (data: TrackedJobRequest) => {
    const trackedJob: TrackedJobRequest = {
      ...data,
      applicationStatus: Number(data.applicationStatus),
      rejectReason: showRejectReason ? data.rejectReason : undefined,
      notes: showNotes ? data.notes : undefined,
    };

    if (isEdit) {
      updateMutation.mutate(trackedJob, {
        onSuccess: () => onClose(),
      });
    } else {
      createMutation.mutate(trackedJob, {
        onSuccess: () => onClose(),
      });
    }
  };

  useEffect(() => {
    reset(defaultValues ?? emptyValues);
  }, [defaultValues]);

  useEffect(() => {
    if (!showModal) {
      reset(emptyValues);
      return;
    }

    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        onClose();
      }
    };

    document.addEventListener("keydown", handleEscape);

    return () => document.removeEventListener("keydown", handleEscape);
  }, [showModal]);

  return (
    <Modal isOpen={showModal} close={onClose} customStyles="overflow-y-auto w-full h-full sm:w-auto sm:h-auto">
      <form onSubmit={handleSubmit(createTrackedJob)} className="flex flex-col gap-4">
        <h3 className="font-serif text-3xl">{isEdit ? "Uppdatera jobb" : "Skapa nytt jobb"}</h3>
        <div className="flex flex-col gap-4 sm:flex-row sm:gap-10">
          <div className="flex w-full flex-col">
            <label htmlFor="title" className="ml-1">
              Titel:
            </label>
            <TextInput
              placeholder="Titel"
              errors={errors.title}
              {...register("title", {
                required: "Titel får inte vara tomt",
                maxLength: { value: 150, message: "Titel får max vara 150 karaktärer lång" },
                minLength: { value: 1, message: "Titel får inte vara tomt" },
              })}
            />
            {errors.title && <span className="ml-1 text-red-600 dark:text-red-800">{errors.title.message}</span>}
          </div>
          <div className="flex w-full flex-col">
            <label htmlFor="companyName" className="ml-1">
              Företagets namn:
            </label>
            <TextInput
              placeholder="Företagets namn"
              errors={errors.companyName}
              {...register("companyName", {
                required: "Företagsnamn får inte vara tomt",
                maxLength: { value: 100, message: "Företagsnamn får max vara 100 karaktärer lång" },
                minLength: { value: 1, message: "Företagsnamn får inte vara tomt" },
              })}
            />
            {errors.companyName && (
              <span className="ml-1 text-red-600 dark:text-red-800">{errors.companyName.message}</span>
            )}
          </div>
        </div>
        <div className="flex flex-col gap-4 sm:flex-row sm:gap-10">
          <div className="flex w-full flex-col">
            <label htmlFor="location" className="ml-1">
              Plats:
            </label>
            <TextInput
              placeholder="Plats"
              errors={errors.location}
              {...register("location", {
                required: "Plats får inte vara tomt",
                maxLength: { value: 100, message: "Plats får max vara 100 karaktärer lång" },
                minLength: { value: 1, message: "Plats får inte vara tomt" },
              })}
            />
            {errors.location && <span className="ml-1 text-red-600 dark:text-red-800">{errors.location.message}</span>}
          </div>
          <div className="flex w-full flex-col">
            <label htmlFor="applicationDeadline" className="ml-1">
              Sista ansökningsdag:
            </label>
            <DateInput
              {...register("applicationDeadline", {
                required: false,
                setValueAs: toUndefinedIfEmpty,
              })}
            />
          </div>
        </div>
        <div className="flex w-full flex-col">
          <label htmlFor="listingUrl" className="ml-1">
            Länk till annons:
          </label>
          <TextInput
            type="url"
            placeholder="T.ex. https://www.linkedin.com/annons/12345"
            errors={errors.listingUrl}
            {...register("listingUrl", {
              required: false,
              maxLength: { value: 100, message: "Länk får max vara 100 karaktärer lång" },
              setValueAs: toUndefinedIfEmpty,
            })}
          />
          {errors.listingUrl && (
            <span className="ml-1 text-red-600 dark:text-red-800">{errors.listingUrl.message}</span>
          )}
        </div>
        <div className="flex flex-col gap-4 sm:flex-row sm:gap-10">
          <div className="flex w-full flex-col">
            <label htmlFor="applicationStatus" className="ml-1">
              Status:
            </label>
            <SelectInput
              {...register("applicationStatus", {
                required: false,
              })}
            >
              {Object.values(ApplicationStatus)
                .filter((key) => typeof key === "number")
                .map((status: number) => {
                  const name = getApplicationStatusName(Number(status));
                  return (
                    <option key={status} value={status}>
                      {name}
                    </option>
                  );
                })}
            </SelectInput>
          </div>
          {showNotes && (
            <div className="flex w-full flex-col">
              <label htmlFor="applyDate" className="ml-1">
                Ansökt:
              </label>
              <DateInput
                errors={errors.applyDate}
                {...register("applyDate", {
                  required: "Ansökt datum får inte vara tomt",
                  setValueAs: toUndefinedIfEmpty,
                })}
              />
              {errors.applyDate && (
                <span className="ml-1 text-red-600 dark:text-red-800">{errors.applyDate.message}</span>
              )}
            </div>
          )}
        </div>
        <div className="flex flex-col gap-4 sm:flex-row sm:gap-10">
          {showNotes && (
            <>
              <div className="flex w-full items-center gap-1">
                <label htmlFor="spontaneousApplication" className="ml-1">
                  Spontanansökan:
                </label>
                <CheckboxInput {...register("spontaneousApplication")} />
              </div>
              <div className="flex w-full items-center gap-1">
                <label htmlFor="haveCalled" className="ml-1">
                  Har ringt:
                </label>
                <CheckboxInput {...register("haveCalled")} />
              </div>
            </>
          )}
        </div>
        {showRejectReason && (
          <div className="flex w-full flex-col">
            <label htmlFor="rejectReason" className="ml-1">
              Anledning för nekad ansökan:
            </label>
            <TextAreaInput
              placeholder="T.ex. 'Hade för lite erfarenhet'"
              errors={errors.rejectReason}
              {...register("rejectReason", {
                required: false,
                maxLength: { value: 500, message: "Anledning får max vara 500 karaktärer lång" },
                setValueAs: toUndefinedIfEmpty,
              })}
            />
            {errors.rejectReason && (
              <span className="ml-1 text-red-600 dark:text-red-800">{errors.rejectReason.message}</span>
            )}
          </div>
        )}
        {showNotes && (
          <div className="flex w-full flex-col">
            <label htmlFor="notes" className="ml-1">
              Anteckningar:
            </label>
            <TextAreaInput
              placeholder="T.ex. 'Ringde rekryterare men fick inget svar'"
              rows={4}
              errors={errors.notes}
              {...register("notes", {
                required: false,
                maxLength: { value: 1000, message: "Anteckningar får max vara 1000 karaktärer lång" },
                setValueAs: toUndefinedIfEmpty,
              })}
            />
            {errors.notes && <span className="ml-1 text-red-600 dark:text-red-800">{errors.notes.message}</span>}
          </div>
        )}
        {isEdit ? (
          <input
            type="submit"
            value={updateMutation.isPending ? "Uppdaterar..." : "Uppdatera"}
            disabled={updateMutation.isPending}
            className="bg-accent outline-accent cursor-pointer rounded-2xl px-3 py-2 text-white outline-0 focus:outline-1 disabled:opacity-25"
          />
        ) : (
          <input
            type="submit"
            value={createMutation.isPending ? "Skapar..." : "Lägg till"}
            disabled={createMutation.isPending}
            className="bg-accent outline-accent cursor-pointer rounded-2xl px-3 py-2 text-white outline-0 focus:outline-1 disabled:opacity-25"
          />
        )}
      </form>
    </Modal>
  );
}
