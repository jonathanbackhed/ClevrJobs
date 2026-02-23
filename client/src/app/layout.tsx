import type { Metadata, Viewport } from "next";
import { DM_Serif_Display, Source_Sans_3 } from "next/font/google";
import "./globals.css";
import Providers from "./providers";
import { ClerkProvider } from "@clerk/nextjs";
import Navbar from "@/components/layout/Navbar";

const dmSerif = DM_Serif_Display({
  variable: "--font-dm-serif",
  subsets: ["latin"],
  weight: "400",
});

const sourceSans = Source_Sans_3({
  variable: "--font-source-sans",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "ClevrJobs - AI-summerade jobb",
  description: "Hitta relevanta tjänster enklare med hjälp av smarta AI summeringar",
};

export const viewport: Viewport = {
  width: "device-width",
  initialScale: 1,
  viewportFit: "cover",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <ClerkProvider>
      <html lang="sv">
        <body className={`${dmSerif.variable} ${sourceSans.variable} bg-cream min-h-dvh antialiased`}>
          <div className="relative min-h-svh font-sans text-stone-800 dark:text-stone-300">
            <Navbar />
            <Providers>{children}</Providers>
          </div>
        </body>
      </html>
    </ClerkProvider>
  );
}
