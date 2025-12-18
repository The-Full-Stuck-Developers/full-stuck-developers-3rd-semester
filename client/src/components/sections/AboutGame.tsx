import { Wallet, Grid, RefreshCw, Trophy } from "lucide-react";
import {useTranslation} from "react-i18next";

export function AboutGame() {
  const { t } = useTranslation();

  return (
    <section id="about" className="w-full bg-gray-50 py-24 lg:py-32">
      <div className="max-w-7xl mx-auto px-6">
        <div className="text-center mb-16">
          <h2 className="text-5xl sm:text-6xl lg:text-7xl font-black text-[#0f2b5b] tracking-tighter uppercase">
            {t('home:how_it_works')}
          </h2>
          <p className="mt-4 text-xl sm:text-2xl text-gray-700 font-semibold max-w-3xl mx-auto">
            {t('home:how_it_works_description')}
          </p>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-8">
          <div className="group relative bg-white rounded-3xl border-2 border-[#0f2b5b]/10 shadow-lg hover:shadow-2xl hover:border-[#e30613] transition-all duration-300 p-10 text-center">
            <div className="absolute -top-6 left-1/2 -translate-x-1/2 w-14 h-14 rounded-full bg-[#e30613] text-white font-black text-2xl flex items-center justify-center shadow-xl">
              1
            </div>
            <Wallet
              size={68}
              className="text-[#e30613] mx-auto mb-6 group-hover:scale-110 transition-transform"
            />
            <h3 className="text-2xl font-bold text-[#0f2b5b] mb-4">
              {t('home:add_balance')}
            </h3>
            <p className="text-gray-600 leading-relaxed">
              {t('home:top_up_account')}
            </p>
          </div>

          <div className="group relative bg-white rounded-3xl border-2 border-[#0f2b5b]/10 shadow-lg hover:shadow-2xl hover:border-[#e30613] transition-all duration-300 p-10 text-center">
            <div className="absolute -top-6 left-1/2 -translate-x-1/2 w-14 h-14 rounded-full bg-[#e30613] text-white font-black text-2xl flex items-center justify-center shadow-xl">
              2
            </div>
            <Grid
              size={68}
              className="text-[#e30613] mx-auto mb-6 group-hover:scale-110 transition-transform"
            />
            <h3 className="text-2xl font-bold text-[#0f2b5b] mb-4">
              {t('home:pick_numbers')}
            </h3>
            <p className="text-gray-600 leading-relaxed">
              {t('home:select_numbers_for_board')}
            </p>
          </div>

          <div className="group relative bg-white rounded-3xl border-2 border-[#0f2b5b]/10 shadow-lg hover:shadow-2xl hover:border-[#e30613] transition-all duration-300 p-10 text-center">
            <div className="absolute -top-6 left-1/2 -translate-x-1/2 w-14 h-14 rounded-full bg-[#e30613] text-white font-black text-2xl flex items-center justify-center shadow-xl">
              3
            </div>
            <RefreshCw
              size={68}
              className="text-[#e30613] mx-auto mb-6 group-hover:scale-110 transition-transform"
            />
            <h3 className="text-2xl font-bold text-[#0f2b5b] mb-4">
              {t('home:play_weekly')}
            </h3>
            <p className="text-gray-600 leading-relaxed">
              {t('home:set_up_boards_or_play_weekly')}
            </p>
          </div>

          <div className="group relative bg-white rounded-3xl border-2 border-[#0f2b5b]/10 shadow-lg hover:shadow-2xl hover:border-[#e30613] transition-all duration-300 p-10 text-center">
            <div className="absolute -top-6 left-1/2 -translate-x-1/2 w-14 h-14 rounded-full bg-[#e30613] text-white font-black text-2xl flex items-center justify-center shadow-xl">
              4
            </div>
            <Trophy
              size={68}
              className="text-[#e30613] mx-auto mb-6 group-hover:scale-110 transition-transform"
            />
            <h3 className="text-2xl font-bold text-[#0f2b5b] mb-4">
              {t('home:win_prizes')}
            </h3>
            <p className="text-gray-600 leading-relaxed">
              {t('home:match_numbers_and_win')}
            </p>
          </div>
        </div>
      </div>
    </section>
  );
}
