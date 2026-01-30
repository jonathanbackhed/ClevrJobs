import React from "react";

interface Props {
  totalPages: number;
  totalCount: number;
  pageSize: number;
  currentPage: number;
  onPageChange: (page: number) => void;
}

export default function Pagination({ totalPages, totalCount, pageSize, currentPage, onPageChange }: Props) {
  const startItem = (currentPage - 1) * pageSize + 1;
  const endItem = Math.min(currentPage * pageSize, totalCount);

  return (
    <div className="mt-4 flex flex-col items-center justify-center">
      <div className="flex gap-2">
        {Array.from({ length: totalPages }, (_, i) => i + 1).map((page) => (
          <button
            key={page}
            onClick={() => onPageChange(page)}
            disabled={currentPage === page}
            className={`border-accent/40 hover:bg-accent rounded-xl border px-4 py-2 hover:text-white ${currentPage === page ? "bg-accent-light cursor-default text-white" : "bg-cream-light cursor-pointer text-stone-500"}`}
          >
            {page}
          </button>
        ))}
        {totalPages > 1 && currentPage < totalPages && (
          <button
            onClick={() => onPageChange(currentPage + 1)}
            className="border-accent/40 bg-cream-light hover:bg-accent cursor-pointer rounded-xl border px-4 py-2 text-stone-500 hover:text-white"
          >
            Nästa
          </button>
        )}
      </div>
      <span className="mt-2 text-sm font-semibold">
        Visar {startItem}-{endItem} av {totalCount} annonser
      </span>
    </div>
  );
}
