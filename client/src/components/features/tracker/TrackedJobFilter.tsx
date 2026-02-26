import DateInput from "@/components/ui/form/DateInput";
import SelectInput from "@/components/ui/form/SelectInput";
import { getApplicationStatusName } from "@/lib/utils/helpers";
import { ApplicationStatus } from "@/types/enum";
import { FilterOptions } from "@/types/filter";
import React from "react";

interface Props {
  filterOptions: FilterOptions;
  onFilterChange: React.Dispatch<React.SetStateAction<FilterOptions>>;
}

export default function TrackedJobFilter({ filterOptions, onFilterChange }: Props) {
  return (
    <div className="bg-cream-light border-accent/15 sticky top-6 hidden w-52 shrink-0 self-start rounded-2xl p-4 shadow-stone-800 sm:block">
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
                const name = getApplicationStatusName(Number(status));
                return (
                  <option key={status} value={status}>
                    {name}
                  </option>
                );
              })}
          </SelectInput>
        </div>
        <button
          onClick={() => onFilterChange({ from: "", to: "", applicationStatus: undefined })}
          className="mt-2 hover:cursor-pointer hover:opacity-80"
        >
          Återställ
        </button>
      </div>
    </div>
  );
}
