const isProduction = import.meta.env.PROD;

 const prod = "https://projectsolutionserver.fly.dev";
 const dev = "http://localhost:5284";
export const baseUrl = isProduction ? prod : dev;