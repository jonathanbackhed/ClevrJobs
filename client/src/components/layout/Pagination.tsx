"use client";

import { cn } from "@/lib/utils/helpers";
import { useRouter } from "next/navigation";

interface Props {
  totalPages: number;
  totalCount: number;
  pageSize: number;
  currentPage: number;
  onPageChange: (page: number) => void;
}

export default function Pagination({ totalPages, totalCount, pageSize, currentPage, onPageChange }: Props) {
  const router = useRouter();

  const startItem = (currentPage - 1) * pageSize + 1;
  const endItem = Math.min(currentPage * pageSize, totalCount);

  const handlePageChange = (pageChange: number) => {
    const params = new URLSearchParams();
    params.set("page", pageChange.toString());
    router.push(`?${params.toString()}`, { scroll: false });

    window.scrollTo({ top: 0, behavior: "instant" });
    onPageChange(pageChange);
  };

  const getPageNumbers = () => {
    const pages: number[] = [];
    const showMax = 7;

    if (totalPages <= showMax) {
      return Array.from({ length: totalPages }, (_, i) => i + 1);
    }

    pages.push(1);

    const middlePages = showMax - 2;
    const halfMiddle = Math.floor(middlePages / 2);

    let startPage = Math.max(2, currentPage - halfMiddle);
    let endPage = Math.min(totalPages - 1, currentPage + halfMiddle);

    if (currentPage <= halfMiddle + 1) {
      endPage = Math.min(showMax - 1, totalPages - 1);
    }

    if (currentPage >= totalPages - halfMiddle) {
      startPage = Math.max(2, totalPages - showMax + 2);
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    pages.push(totalPages);

    return pages;
  };

  const pages = getPageNumbers();

  return (
    <div className="mt-4 flex flex-col items-center justify-between">
      <div className="flex flex-wrap justify-center gap-1.5">
        <button
          onClick={() => handlePageChange(currentPage - 1)}
          disabled={currentPage === 1}
          className="active:bg-accent border-accent/40 bg-cream-light hover:bg-accent order-8 flex-1 cursor-pointer rounded-xl border px-4 py-2 text-stone-500 hover:text-white active:text-white disabled:cursor-default sm:order-first sm:w-auto"
        >
          Föregående
        </button>
        <div className="flex basis-full justify-center gap-1.5 sm:basis-0">
          {pages.map((page) => (
            <button
              key={page}
              onClick={() => handlePageChange(page)}
              disabled={currentPage === page}
              className={cn(
                "border-accent/40 hover:bg-accent size-10 flex-1 rounded-xl border hover:text-white",
                currentPage === page
                  ? "bg-accent-light cursor-default text-white"
                  : "bg-cream-light cursor-pointer text-stone-500",
              )}
            >
              {page}
            </button>
          ))}
        </div>
        <button
          onClick={() => handlePageChange(currentPage + 1)}
          disabled={currentPage === totalPages}
          className="active:bg-accent border-accent/40 bg-cream-light hover:bg-accent order-last flex-1 cursor-pointer rounded-xl border px-4 py-2 text-stone-500 hover:text-white active:text-white disabled:cursor-default sm:w-auto"
        >
          Nästa
        </button>
      </div>
      <span className="mt-2 text-sm font-semibold">
        Visar {startItem}-{endItem} av {totalCount} annonser
      </span>
    </div>
  );
}
