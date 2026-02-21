"use client";

import { SignInButton, SignUpButton, SignedIn, SignedOut, UserButton } from "@clerk/nextjs";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { cn } from "tailwind-variants";

const links = [
  { label: "Hem", href: "/" },
  { label: "Tracker", href: "/profile/tracker" },
  { label: "Sparade jobb", href: "/profile/saved" },
];

export default function Navbar() {
  const pathname = usePathname();

  return (
    <nav className="top-0 right-0 left-0 z-40 flex flex-row items-center gap-4 p-4">
      <div className="flex flex-1 items-center justify-between gap-3 sm:justify-end">
        <SignedIn>
          <div className="flex gap-3">
            {links.map(({ label, href }) => (
              <Link
                key={href}
                href={href}
                className={cn("font-semibold hover:opacity-80", pathname === href && "text-accent font-bold underline")}
              >
                {label}
              </Link>
            ))}
          </div>
          <UserButton />
        </SignedIn>
      </div>
      <SignedOut>
        <SignInButton>
          <button className="font-semibold hover:cursor-pointer hover:opacity-80">Logga in</button>
        </SignInButton>
        <SignUpButton>
          <button className="font-semibold hover:cursor-pointer hover:opacity-80">Skapa konto</button>
        </SignUpButton>
      </SignedOut>
    </nav>
  );
}
