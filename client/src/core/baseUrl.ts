export const baseUrl =
  import.meta.env.VITE_API_URL ||
  (import.meta.env.DEV
    ? "http://localhost:5284"
    : "https://deadpigeonsapp.fly.dev");
