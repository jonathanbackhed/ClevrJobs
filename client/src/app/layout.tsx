import type { Metadata } from "next";
import { DM_Serif_Display, Source_Sans_3 } from "next/font/google";
import "./globals.css";
import Providers from "./providers";

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

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="sv">
      <body className={`${dmSerif.variable} ${sourceSans.variable} antialiased`}>
        <div className="bg-cream relative min-h-screen font-sans text-stone-800 dark:text-stone-300">
          <div
            className="pointer-events-none fixed inset-0 z-0 opacity-10 dark:opacity-5"
            style={{
              backgroundImage: `url("data:image/svg+xml,%3Csvg viewBox='0 0 400 400' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.9' numOctaves='3' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)'/%3E%3C/svg%3E")`,
            }}
          />
          <Providers>{children}</Providers>
        </div>
      </body>
    </html>
  );
}
