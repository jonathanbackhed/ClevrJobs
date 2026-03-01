import clsx, { ClassValue } from "clsx";
import { formatDistanceToNowStrict } from "date-fns";
import { sv } from "date-fns/locale";
import { twMerge } from "tailwind-merge";

export function formatDateTime(dateTime: string | Date): string {
  var date = typeof dateTime === "string" ? new Date(dateTime + "Z") : dateTime;
  const formattedDateTime = formatDistanceToNowStrict(date, {
    addSuffix: true,
    locale: sv,
  });

  return formattedDateTime;
}

export function isMoreThan24hAgo(dateTime: string | Date): boolean {
  const date = typeof dateTime === "string" ? new Date(dateTime) : dateTime;
  const now = new Date();

  const hoursDifference = (now.getTime() - date.getTime()) / (1000 * 60 * 60);

  return hoursDifference > 24;
}

export const toUndefinedIfEmpty = (value: string) => (value && value.trim() === "" ? undefined : value);

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}
