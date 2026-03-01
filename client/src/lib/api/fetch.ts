export const fetchOptions = {
  GET: (token: string) => {
    return {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    };
  },
  POST: (token: string, body?: {}) => {
    return {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: body && JSON.stringify(body),
    };
  },
  PUT: (token: string, body: {}) => {
    return {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(body),
    };
  },
  DELETE: (token: string) => {
    return {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    };
  },
};

export type FetchOptions =
  | ReturnType<typeof fetchOptions.GET>
  | ReturnType<typeof fetchOptions.POST>
  | ReturnType<typeof fetchOptions.PUT>
  | ReturnType<typeof fetchOptions.DELETE>;

const getApiUrl = (): string => {
  if (typeof window === "undefined") {
    return process.env.API_URL || "http://localhost:5075";
  }

  return process.env.NEXT_PUBLIC_BASE_API_URL || "http://localhost:5075";
};

export const apiFetchAuth = async (path: string, fetchOptions: FetchOptions) => {
  const res = await fetch(getApiUrl() + path, fetchOptions);
  if (!res.ok) {
    throw new Error(`API error: ${res.status}`);
  }

  return res.json();
};

export const apiFetch = async (path: string, body?: {}) => {
  const res = await fetch(
    getApiUrl() + path,
    body ? { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(body) } : undefined,
  );
  if (!res.ok) {
    throw new Error(`API error: ${res.status}`);
  }

  return res.json();
};
