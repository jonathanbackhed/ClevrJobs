import React from "react";
import { SignInButton, SignOutButton, SignUpButton, SignedIn, SignedOut, UserButton } from "@clerk/nextjs";

export default function Navbar() {
  return (
    <div className="absolute top-0 right-0 left-0 z-40 flex flex-row items-center justify-end gap-4 p-4">
      <SignedIn>
        <UserButton />
      </SignedIn>
      <SignedOut>
        <div>
          <SignInButton />
        </div>
        <div>
          <SignUpButton />
        </div>
      </SignedOut>
    </div>
  );
}
