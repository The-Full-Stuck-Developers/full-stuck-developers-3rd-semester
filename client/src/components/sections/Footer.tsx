import { Trophy, Clock, Dice5, Target } from "lucide-react";
import { useTranslation } from "react-i18next";

export function Footer() {
  const { t } = useTranslation();

  return (
    <footer id="contact" className="w-full bg-[#1a1f25] text-white py-20">
      <div className="max-w-6xl mx-auto px-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-16 mb-16">
          <div className="space-y-4">
            <div className="flex items-center gap-3 text-xl font-semibold">
              <span className="text-3xl">🕊️</span>
              Dead Pigeons
            </div>

            <p className="text-lg text-gray-400 leading-relaxed">
              {t("home:supporting_through_community")}
            </p>
          </div>

          <div className="space-y-4">
            <h4 className="text-2xl font-semibold">{t("home:quick_links")}</h4>
            <ul className="space-y-3 text-lg text-gray-300">
              <li className="hover:text-white cursor-pointer">
                <a href={"#about"}>{t("home:how_to_play")}</a>
              </li>
              <li className="hover:text-white cursor-pointer">
                <a href={"#about"}>{t("home:game_rules")}</a>
              </li>
              <li className="hover:text-white cursor-pointer">
                <a href={"#pricing"}>{t("home:pricing")}</a>
              </li>
            </ul>
          </div>

          <div className="space-y-4">
            <h4 className="text-2xl font-semibold">Game Info</h4>
            <ul className="space-y-4 text-lg text-gray-300">
              <li className="flex items-center gap-3">
                <Target className="text-emerald-400" size={26} />
                {t("home:numbers")}: 1–16
              </li>

              <li className="flex items-center gap-3">
                <Trophy className="text-yellow-400" size={26} />
                {t("home:prize_pool")}: 70%
              </li>

              <li className="flex items-center gap-3">
                <Clock className="text-red-400" size={26} />
                {t("home:deadline")}: {t("home:saturday")} 17:00
              </li>

              <li className="flex items-center gap-3">
                <Dice5 className="text-blue-400" size={26} />
                {t("home:weekly_drawings")}
              </li>
            </ul>
          </div>
        </div>

        <div className="w-full h-px bg-gray-700 mb-6"></div>

        <p className="text-center text-gray-500 text-lg">
          © {new Date().getFullYear()} Dead Pigeons · Jerne IF. All rights
          reserved.
        </p>
      </div>
    </footer>
  );
}
