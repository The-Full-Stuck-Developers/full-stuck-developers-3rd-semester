import { Check } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";

const boards = [
  { numbers: 5, price: 20, popular: false },
  { numbers: 6, price: 40, popular: false },
  { numbers: 7, price: 80, popular: true },
  { numbers: 8, price: 160, popular: false },
];

export function BoardsPricing() {
  const { t } = useTranslation();
  const navigate = useNavigate();

  const handleSelectBoard = () => {
    navigate("/login");
  };

  return (
    <section id="pricing" className="w-full bg-gray-50 py-20 lg:py-24">
      <div className="max-w-6xl mx-auto px-6">
        <div className="text-center mb-12">
          <h2 className="text-4xl sm:text-5xl lg:text-6xl font-black text-[#0f2b5b] tracking-tighter uppercase">
            {t("home:choose_your_board")}
          </h2>
          <p className="mt-3 text-lg text-gray-700 font-medium">
            {t("home:more_numbers_bigger_chances")}
          </p>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
          {boards.map((board) => (
            <div
              key={board.numbers}
              className={`group relative bg-white rounded-2xl border-2 shadow-lg transition-all duration-300
                                ${
                                  board.popular
                                    ? "border-[#e30613] shadow-xl scale-105 -translate-y-3"
                                    : "border-[#0f2b5b]/10 hover:border-[#e30613] hover:shadow-xl"
                                }`}
            >
              {board.popular && (
                <div className="absolute -top-4 left-1/2 -translate-x-1/2 bg-[#e30613] text-white font-black text-xs px-5 py-1.5 rounded-full shadow-lg uppercase tracking-wider">
                  {t("home:most_popular")}
                </div>
              )}

              <div className="pt-10 pb-8 px-6 text-center">
                <div
                  className={`text-5xl font-black mb-2 ${board.popular ? "text-[#e30613]" : "text-[#0f2b5b]"}`}
                >
                  {board.numbers}
                </div>
                <p className="text-sm uppercase tracking-wider text-gray-600 font-bold mb-6">
                  {t("home:numbers")}
                </p>

                <div className="mb-8">
                  <span className="text-4xl font-black text-[#0f2b5b]">
                    {board.price}
                  </span>
                  <span className="text-xl font-medium text-gray-600 ml-1">
                    kr
                  </span>
                </div>

                <ul className="space-y-3 text-left text-gray-700 text-sm font-medium mb-8">
                  <li className="flex items-center gap-2">
                    <Check size={18} className="text-[#e30613]" />
                    {t("home:from_1_to_16")}
                  </li>
                  <li className="flex items-center gap-2">
                    <Check size={18} className="text-[#e30613]" />
                    {t("home:weekly_drawings")}
                  </li>
                  <li className="flex items-center gap-2">
                    <Check size={18} className="text-[#e30613]" />
                    {t("home:auto_repeat_option")}
                  </li>
                </ul>

                {/* React Router version */}
                <button
                  onClick={handleSelectBoard}
                  className={`w-full py-3.5 rounded-full font-bold text-base transition-all shadow-md
                                        ${
                                          board.popular
                                            ? "bg-[#e30613] hover:bg-[#c20510] text-white"
                                            : "bg-[#0f2b5b] hover:bg-[#0a1e3f] text-white"
                                        }`}
                >
                  {t("home:select_board")}
                </button>
              </div>
            </div>
          ))}
        </div>

        <div className="text-center mt-12">
          <p className="text-lg font-semibold text-gray-700">
            <span className="text-[#e30613]">70%</span> {t("home:to_prizes")} ·{" "}
            <span className="text-[#0f2b5b]">30%</span> {t("home:supports")}{" "}
            <span className="text-[#e30613]">Jerne IF</span>
          </p>
        </div>
      </div>
    </section>
  );
}
