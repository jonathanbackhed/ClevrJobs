"use client";

import Modal from "@/components/ui/Modal";
import { useCreateTrackedJob } from "@/hooks/useTracked";
import { cn, getApplicationStatusName, toUndefinedIfEmpty } from "@/lib/utils/helpers";
import { ApplicationStatus } from "@/types/enum";
import { TrackedJobRequest } from "@/types/tracked";
import React, { useEffect, useRef, useState } from "react";
import { useForm } from "react-hook-form";

interface Props {
  showModal: boolean;
  setShowModal: (state: boolean) => void;
}

export default function AddNewJobModal({ showModal, setShowModal }: Props) {
  const createMutation = useCreateTrackedJob();
  const {
    register,
    handleSubmit,
    watch,
    formState: { errors },
    reset,
  } = useForm<TrackedJobRequest>({
    defaultValues: {
      applicationStatus: ApplicationStatus.NotApplied,
    },
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

    createMutation.mutate(trackedJob);
  };

  useEffect(() => {
    if (!showModal) {
      reset();
      return;
    }

    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        setShowModal(false);
      }
    };

    document.addEventListener("keydown", handleEscape);

    return () => document.removeEventListener("keydown", handleEscape);
  }, [showModal]);

  return (
    <Modal isOpen={showModal} close={setShowModal}>
      <form onSubmit={handleSubmit(createTrackedJob)} className="flex flex-col gap-4">
        <h3 className="font-serif text-3xl">Skapa nytt jobb</h3>
        <div className="flex gap-10">
          <div className="flex w-full flex-col">
            <label htmlFor="title" className="ml-1">
              Titel:
            </label>
            <input
              type="text"
              placeholder="Titel"
              className={cn(
                "bg-cream-light outline-accent resize-none rounded-lg p-2.5 outline-0 focus:outline-2",
                errors.title && "outline outline-red-600 dark:outline-red-800",
              )}
              {...register("title", {
                required: "Titel får inte vara tomt",
                maxLength: { value: 150, message: "Titel får max vara 150 karaktärer lång" },
              })}
            />
            {errors.title && <span className="ml-1 text-red-600 dark:text-red-800">{errors.title.message}</span>}
          </div>
          <div className="flex w-full flex-col">
            <label htmlFor="companyName">Företagets namn:</label>
            <input
              type="text"
              placeholder="Företagets namn"
              className={cn(
                "bg-cream-light outline-accent resize-none rounded-lg p-2.5 outline-0 focus:outline-2",
                errors.companyName && "outline outline-red-600 dark:outline-red-800",
              )}
              {...register("companyName", {
                required: "Företagsnamn får inte vara tomt",
                maxLength: { value: 100, message: "Företagsnamn får max vara 100 karaktärer lång" },
              })}
            />
            {errors.companyName && (
              <span className="ml-1 text-red-600 dark:text-red-800">{errors.companyName.message}</span>
            )}
          </div>
        </div>
        <div className="flex gap-10">
          <div className="flex w-full flex-col">
            <label htmlFor="location">Plats:</label>
            <input
              type="text"
              placeholder="Plats"
              className={cn(
                "bg-cream-light outline-accent resize-none rounded-lg p-2.5 outline-0 focus:outline-2",
                errors.location && "outline outline-red-600 dark:outline-red-800",
              )}
              {...register("location", {
                required: "Plats får inte vara tomt",
                maxLength: { value: 100, message: "Plats får max vara 100 karaktärer lång" },
              })}
            />
            {errors.location && <span className="ml-1 text-red-600 dark:text-red-800">{errors.location.message}</span>}
          </div>
          <div className="flex w-full flex-col">
            <label htmlFor="applicationDeadline">Sista ansökningsdag:</label>
            <input
              type="date"
              className="bg-cream-light outline-accent resize-none rounded-lg p-2.5 outline-0 focus:outline-2"
              {...register("applicationDeadline", { required: false, setValueAs: toUndefinedIfEmpty })}
            />
          </div>
        </div>
        <div className="flex w-full flex-col">
          <label htmlFor="listingUrl">Länk till annons:</label>
          <input
            type="url"
            placeholder="Länk till annons eller företagets hemsida"
            className={cn(
              "bg-cream-light outline-accent resize-none rounded-lg p-2.5 outline-0 focus:outline-2",
              errors.listingUrl && "outline outline-red-600 dark:outline-red-800",
            )}
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
        <div className="flex w-full flex-col">
          <label htmlFor="applicationStatus">Status:</label>
          <select
            required
            className="bg-cream-light outline-accent resize-none rounded-lg p-2.5 outline-0 focus:outline-2"
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
          </select>
        </div>
        {showRejectReason && (
          <div className="flex w-full flex-col">
            <label htmlFor="rejectReason">Anledning för nekad ansökan:</label>
            <textarea
              placeholder="T.ex. 'Hade för lite erfarenhet'"
              className={cn(
                "bg-cream-light outline-accent resize-none rounded-lg p-2.5 outline-0 focus:outline-2",
                errors.rejectReason && "outline outline-red-600 dark:outline-red-800",
              )}
              {...register("rejectReason", {
                required: false,
                maxLength: { value: 500, message: "Anledning får max vara 500 karaktärer lång" },
                setValueAs: toUndefinedIfEmpty,
              })}
              rows={2}
            />
            {errors.rejectReason && (
              <span className="ml-1 text-red-600 dark:text-red-800">{errors.rejectReason.message}</span>
            )}
          </div>
        )}
        {showNotes && (
          <div className="flex w-full flex-col">
            <label htmlFor="notes">Anteckningar:</label>
            <textarea
              placeholder="T.ex. 'Ringde rekryterare men fick inget svar'"
              className={cn(
                "bg-cream-light outline-accent resize-none rounded-lg p-2.5 outline-0 focus:outline-2",
                errors.notes && "outline outline-red-600 dark:outline-red-800",
              )}
              {...register("notes", {
                required: false,
                maxLength: { value: 1000, message: "Anteckningar får max vara 1000 karaktärer lång" },
                setValueAs: toUndefinedIfEmpty,
              })}
              rows={4}
            />
            {errors.notes && <span className="ml-1 text-red-600 dark:text-red-800">{errors.notes.message}</span>}
          </div>
        )}
        <input
          type="submit"
          value={createMutation.isPending ? "Skapar..." : "Lägg till"}
          disabled={createMutation.isPending}
          className="bg-accent outline-accent cursor-pointer rounded-2xl px-3 py-2 text-white outline-0 focus:outline-1 disabled:opacity-25"
        />
      </form>
    </Modal>
  );
}
