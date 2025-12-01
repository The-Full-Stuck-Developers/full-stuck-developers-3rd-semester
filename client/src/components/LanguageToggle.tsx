import {useState} from "react";

export const LanguageToggle = () => {
    const [lang, setLang] = useState("gb");

    return (
        <div className="relative bg-gray-200 rounded-full flex items-center px-1 py-1 gap-2 w-24 h-9 mx-auto">
            <div
                className={`absolute h-7 w-10 bg-white rounded-full shadow transition-all duration-300 ${
                    lang === "gb" ? "left-1" : "left-[calc(100%-2.5rem-0.25rem)]"
                }`}
            />
            <button
                onClick={() => setLang("gb")}
                className={`z-10 mx-auto transition ${
                    lang === "gb" ? "opacity-100" : "opacity-50 cursor-pointer"
                }`}
            >
                <span className="fi fi-gb text-xl rounded-full mt-1"></span>
            </button>
            <button
                onClick={() => setLang("dk")}
                className={`z-10 mx-auto transition ${
                    lang === "dk" ? "opacity-100" : "opacity-50 cursor-pointer"
                }`}
            >
                <span className="fi fi-dk text-xl rounded-full mt-1"></span>
            </button>
        </div>
    );
};
