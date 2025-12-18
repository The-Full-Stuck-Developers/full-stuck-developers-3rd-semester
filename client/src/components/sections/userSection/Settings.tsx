import { useState, useEffect } from "react";
import { X, Languages } from "lucide-react";
import { useTranslation } from "react-i18next";

export function Settings({
  isOpen,
  onClose,
}: {
  isOpen: boolean;
  onClose: () => void;
}) {
  const { t, i18n } = useTranslation("player");
  const [currentLang, setCurrentLang] = useState<"en" | "dk">("en");

  useEffect(() => {
    if (isOpen) {
      const savedLang = localStorage.getItem("language") as "en" | "dk" | null;
      const lang = savedLang || (i18n.language.startsWith("dk") ? "dk" : "en");
      setCurrentLang(lang);
    }
  }, [isOpen, i18n.language]);

  const changeLanguage = (lng: "en" | "dk") => {
    setCurrentLang(lng);
    i18n.changeLanguage(lng);
    localStorage.setItem("language", lng);
  };

  if (!isOpen) return null;

  return (
    <>
      <div className="fixed inset-0 bg-black/50 z-50" onClick={onClose} />
      <div className="fixed inset-0 flex items-center justify-center z-50 p-4">
        <div
          className="bg-gray-800 rounded-2xl p-6 max-w-md w-full border border-gray-700"
          onClick={(e) => e.stopPropagation()}
        >
          <div className="flex items-center justify-between mb-6">
            <div className="flex items-center gap-3">
              <Languages className="w-6 h-6 text-red-400" />
              <h2 className="text-2xl font-bold">{t("language")}</h2>
            </div>
            <button
              onClick={onClose}
              className="p-2 hover:bg-gray-700 rounded-lg transition"
            >
              <X className="w-5 h-5" />
            </button>
          </div>

          <div className="space-y-4">
            <button
              onClick={() => changeLanguage("en")}
              className={`w-full py-4 px-4 rounded-lg border-2 transition flex items-center justify-center gap-3 ${
                currentLang === "en"
                  ? "border-red-600 bg-red-900/20 text-red-400"
                  : "border-gray-600 bg-gray-700 hover:bg-gray-600"
              }`}
            >
              <span className="text-2xl">🇬🇧</span>
              <div className="font-bold">{t("english")}</div>
            </button>
            <button
              onClick={() => changeLanguage("dk")}
              className={`w-full py-4 px-4 rounded-lg border-2 transition flex items-center justify-center gap-3 ${
                currentLang === "dk"
                  ? "border-red-600 bg-red-900/20 text-red-400"
                  : "border-gray-600 bg-gray-700 hover:bg-gray-600"
              }`}
            >
              <span className="text-2xl">🇩🇰</span>
              <div className="font-bold">{t("danish")}</div>
            </button>
          </div>

          <div className="mt-6 pt-4 border-t border-gray-700">
            <button
              onClick={onClose}
              className="w-full py-3 px-4 bg-gray-700 hover:bg-gray-600 rounded-lg transition"
            >
              {t("cancel")}
            </button>
          </div>
        </div>
      </div>
    </>
  );
}
