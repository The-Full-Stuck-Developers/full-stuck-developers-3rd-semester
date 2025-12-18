import { Shield, Clock3, Users, TrendingUp } from "lucide-react";
import { useTranslation } from "react-i18next";

export function WhyPlay() {
  const { t } = useTranslation();

  return (
    <section className="w-full bg-gray-50 py-16 lg:py-20">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="text-center mb-12 lg:mb-16">
          <h2 className="text-4xl sm:text-5xl lg:text-6xl font-black text-[#0f2b5b] tracking-tighter">
            {t("home:why_play_dead_pigeons")}
          </h2>
          <p className="mt-4 text-lg sm:text-xl text-gray-700 max-w-3xl mx-auto font-semibold">
            {t("home:fun_secure_game")}
          </p>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-6 lg:gap-8">
          <div className="group relative bg-white rounded-2xl border-2 border-[#0f2b5b]/10 shadow-lg hover:shadow-2xl hover:border-[#e30613] transition-all duration-300 overflow-hidden">
            <div className="p-8 text-left">
              <div className="flex items-center justify-center w-16 h-16 rounded-full bg-[#e30613]/10 mb-6 group-hover:bg-[#e30613] group-hover:text-white transition-colors">
                <Shield className="w-9 h-9 text-[#e30613] group-hover:text-white" />
              </div>
              <h3 className="text-xl font-bold text-[#0f2b5b] mb-3">
                {t("home:secure_balance_system")}
              </h3>
              <p className="text-gray-600 leading-relaxed">
                {t("home:deposits_are_tracked")}
              </p>
            </div>
          </div>

          <div className="group relative bg-white rounded-2xl border-2 border-[#0f2b5b]/10 shadow-lg hover:shadow-2xl hover:border-[#e30613] transition-all duration-300 overflow-hidden">
            <div className="p-8 text-left">
              <div className="flex items-center justify-center w-16 h-16 rounded-full bg-[#e30613]/10 mb-6 group-hover:bg-[#e30613] group-hover:text-white transition-colors">
                <Clock3 className="w-9 h-9 text-[#e30613] group-hover:text-white" />
              </div>
              <h3 className="text-xl font-bold text-[#0f2b5b] mb-3">
                {t("home:weekly_drawings")}
              </h3>
              <p className="text-gray-600 leading-relaxed">
                {t("home:new_numbers_every_week")}
              </p>
            </div>
          </div>

          <div className="group relative bg-white rounded-2xl border-2 border-[#0f2b5b]/10 shadow-lg hover:shadow-2xl hover:border-[#e30613] transition-all duration-300 overflow-hidden">
            <div className="p-8 text-left">
              <div className="flex items-center justify-center w-16 h-16 rounded-full bg-[#e30613]/10 mb-6 group-hover:bg-[#e30613] group-hover:text-white transition-colors">
                <Users className="w-9 h-9 text-[#e30613] group-hover:text-white" />
              </div>
              <h3 className="text-xl font-bold text-[#0f2b5b] mb-3">
                {t("home:community_first")}
              </h3>
              <p className="text-gray-600 leading-relaxed">
                {t("home:70_percent_goes_to_players")}
              </p>
            </div>
          </div>

          <div className="group relative bg-white rounded-2xl border-2 border-[#0f2b5b]/10 shadow-lg hover:shadow-2xl hover:border-[#e30613] transition-all duration-300 overflow-hidden">
            <div className="p-8 text-left">
              <div className="flex items-center justify-center w-16 h-16 rounded-full bg-[#e30613]/10 mb-6 group-hover:bg-[#e30613] group-hover:text-white transition-colors">
                <TrendingUp className="w-9 h-9 text-[#e30613] group-hover:text-white" />
              </div>
              <h3 className="text-xl font-bold text-[#0f2b5b] mb-3">
                {t("home:flexible_and_fair_pricing")}
              </h3>
              <p className="text-gray-600 leading-relaxed">
                {t("home:pick_5_to_9_numbers")}
              </p>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
