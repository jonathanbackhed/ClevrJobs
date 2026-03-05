import DateInput from "@/components/ui/form/DateInput";
import SelectInput from "@/components/ui/form/SelectInput";
import { getApplicationStatusLabel } from "@/lib/displayNameHelpers";
import { ApplicationStatus } from "@/types/enum";
import type { FilterOptions } from "@/types";
import React from "react";
import CheckboxInput from "@/components/ui/form/CheckboxInput";

interface Props {
  filterOptions: FilterOptions;
  onFilterChange: React.Dispatch<React.SetStateAction<FilterOptions>>;
}

export default function TrackedJobFilter({ filterOptions, onFilterChange }: Props) {
  return (
    <>
      <h3 className="mb-2 text-xl font-bold">Filter</h3>
      <div className="flex flex-col gap-2">
        <div className="flex flex-col">
          <label htmlFor="from">Från</label>
          <DateInput
            name="from"
            type="month"
            max={filterOptions.to || undefined}
            value={filterOptions.from}
            onChange={(e: any) => onFilterChange((prev) => ({ ...prev, from: e.target.value }))}
            customStyles="py-2"
          />
        </div>
        <div className="flex flex-col">
          <label htmlFor="to">Till</label>
          <DateInput
            name="to"
            type="month"
            min={filterOptions.from || undefined}
            value={filterOptions.to}
            onChange={(e: any) => onFilterChange((prev) => ({ ...prev, to: e.target.value }))}
            customStyles="py-2"
          />
        </div>
        <div className="flex flex-col">
          <label htmlFor="applicationStatus">Status</label>
          <SelectInput
            name="applicationStatus"
            value={filterOptions.applicationStatus}
            onChange={(e: any) => onFilterChange((prev) => ({ ...prev, applicationStatus: e.target.value }))}
            customStyles="py-2"
          >
            <option value={""}></option>
            {Object.values(ApplicationStatus)
              .filter((key) => typeof key === "number")
              .map((status: number) => {
                const name = getApplicationStatusLabel(Number(status));
                return (
                  <option key={status} value={status}>
                    {name}
                  </option>
                );
              })}
          </SelectInput>
        </div>
        <div className="flex items-center justify-between">
          <label htmlFor="haveCalled">Har ringt:</label>
          <CheckboxInput
            name="haveCalled"
            checked={filterOptions.haveCalled}
            onChange={(e: any) => onFilterChange((prev) => ({ ...prev, haveCalled: e.target.checked }))}
          />
        </div>
        <div className="flex items-center justify-between">
          <label htmlFor="spontaneousApplication">Spontanansökan:</label>
          <CheckboxInput
            name="spontaneousApplication"
            checked={filterOptions.spontaneousApplication}
            onChange={(e: any) => onFilterChange((prev) => ({ ...prev, spontaneousApplication: e.target.checked }))}
          />
        </div>
        <button
          onClick={() =>
            onFilterChange({
              from: "",
              to: "",
              applicationStatus: "",
              haveCalled: false,
              spontaneousApplication: false,
            })
          }
          className="mt-2 hover:cursor-pointer hover:opacity-80"
        >
          Återställ
        </button>
      </div>
    </>
  );
}
