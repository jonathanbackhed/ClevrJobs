export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface JobListingDto {
  title: string;
  companyName: string;
  roleName: string;
  location: string;
  extent?: string;
  duration?: string;
  applicationDeadline: string;
  published: string;
  listingId: string;
  listingUrl: string;
  source: Source;

  // processed fields
  id: number;
  description: string;
  requiredTechnologies: string;
  niceTohaveTechnologies: string;
  competenceRank: CompetenceRank;
  keywordsCV: string;
  keywordsCL: string;
  customCoverLetterFocus: string;
  motivation: string;
}

export interface JobListingMiniDto {
  title: string;
  companyName: string;
  roleName: string;
  location: string;
  extent?: string;
  duration?: string;
  applicationDeadline: string;
  source: Source;

  // processed fields
  id: number;
  description: string;
  requiredTechnologies: string;
  niceTohaveTechnologies: string;
  competenceRank: CompetenceRank;
}

export enum CompetenceRank {
  NewGrad = 0,
  Junior,
  MidLevel,
  Senior,
  Lead,
  Unknown,
}

export enum Source {
  Platsbanken = 0,
}
