import { useState } from "react";
import { Moon, Sun } from "lucide-react";

export const ThemeToggle = () => {
  const [theme, setTheme] = useState("light");

  return (
    <div
      title={"Not available"}
      className="relative bg-gray-900 rounded-full flex items-center px-1 py-1 gap-2 w-24 h-9 mx-auto disabled cursor-not-allowed select-none pointer-events-none"
    >
      <div
        className={`absolute h-7 w-10 bg-gray-700 rounded-full shadow transition-all duration-300 ${
          theme === "light" ? "left-1" : "left-[calc(100%-2.5rem-0.25rem)]"
        }`}
      />
      <button
        onClick={() => setTheme("light")}
        className={`z-10 mx-auto transition ${
          theme === "light" ? "opacity-100" : "opacity-50 cursor-pointer"
        }`}
      >
        <Sun className="w-5 h-5" color={"#ffffff"} />
      </button>
      <button
        onClick={() => setTheme("dark")}
        className={`z-10 mx-auto transition ${
          theme === "dark" ? "opacity-100" : "opacity-50 cursor-pointer"
        }`}
      >
        <Moon className="w-5 h-5" color={"#ffffff"} />
      </button>
    </div>
  );
};
