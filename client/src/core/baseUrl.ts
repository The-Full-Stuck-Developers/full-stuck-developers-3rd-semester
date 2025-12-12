const isProduction = import.meta.env.VITE_PROD;

const prod = "https://deadpigeonsapp.fly.dev";
const dev = "http://localhost:5284";
export const baseUrl = isProduction ? prod : dev;
