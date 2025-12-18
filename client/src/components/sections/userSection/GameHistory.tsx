import { useState, useEffect } from "react";
import { toast } from "react-hot-toast";
import getBetsClient from "@core/clients/betsClient.ts";
import { useTranslation } from "react-i18next";
import { Trophy, Sparkles } from "lucide-react";
import Pagination from "../../Pagination";

interface BetHistoryItem {
  id: string;
  numbers: string;
  count: number;
  price: number;
  date: string;
  isWinning: boolean;
  betSeriesId?: string | null;
  seriesTotal?: number | null;
  seriesIndex?: number | null;
}

export function GameHistory() {
  const { t } = useTranslation("player");
  const [bets, setBets] = useState<BetHistoryItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalCount, setTotalCount] = useState(0);

  useEffect(() => {
    const loadHistory = async () => {
      try {
        setLoading(true);
        const client = getBetsClient();
        const response = await client.getUserHistory(currentPage, pageSize);
        setBets(response.bets as BetHistoryItem[]);
        setTotalCount(response.totalCount);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    loadHistory();
  }, [currentPage, pageSize]);

  if (loading) return <p className="text-center">{t("loading_history")}</p>;

  return (
    <div>
      <div className="flex items-center justify-between mb-8">
        <h1 className="text-4xl font-black bg-gradient-to-r from-white to-red-400 bg-clip-text text-transparent">
          {t("game_history")}
        </h1>
        <div className="flex items-center gap-4">
          <label className="text-sm text-gray-400">{t("items_per_page")}:</label>
          <select
            value={pageSize}
            onChange={(e) => {
              setPageSize(Number(e.target.value));
              setCurrentPage(1);
            }}
            className="bg-gray-800 border border-gray-700 rounded-lg px-3 py-2 text-white"
          >
            <option value={5}>5</option>
            <option value={10}>10</option>
          </select>
        </div>
      </div>

      <div className="space-y-6">
        {bets.map((bet) => (
          <div
            key={bet.id}
            className={`rounded-2xl border-2 p-6 transition-all ${
              bet.isWinning
                ? "border-yellow-400 bg-gradient-to-br from-yellow-900/40 via-yellow-800/30 to-yellow-900/40 shadow-lg shadow-yellow-500/20"
                : "border-gray-700 bg-gray-800"
            }`}
          >
            {bet.isWinning && (
              <div className="absolute -top-2 -right-2 w-16 h-16 bg-yellow-500 rounded-full opacity-20 blur-xl"></div>
            )}
            <div className="relative">
              <div className="flex items-center justify-between mb-4">
                <div className="flex-1">
                  <div className="flex items-center gap-3 flex-wrap">
                    <div className={`text-xl font-bold ${bet.isWinning ? "text-yellow-200" : "text-white"}`}>
                      {bet.count} numbers • {bet.price} kr
                      {bet.seriesTotal && bet.seriesTotal > 1 && bet.seriesIndex && (
                        <span className="ml-2 text-sm font-normal text-blue-400">
                          ({t("week")} {bet.seriesIndex} {t("of")} {bet.seriesTotal})
                        </span>
                      )}
                    </div>
                    {bet.isWinning && (
                      <div className="flex items-center gap-2 px-4 py-2 bg-gradient-to-r from-yellow-500 to-yellow-600 rounded-full text-white text-sm font-bold shadow-lg shadow-yellow-500/50 animate-pulse">
                        <Trophy className="w-5 h-5" />
                        <span>{t("winning_board")}</span>
                        <Sparkles className="w-4 h-4" />
                      </div>
                    )}
                  </div>
                  <div className={`text-sm mt-2 ${bet.isWinning ? "text-yellow-300/80" : "text-gray-400"}`}>
                    {new Date(bet.date).toLocaleDateString("en-US", {
                      day: "2-digit",
                      month: "long",
                      year: "numeric",
                      hour: "2-digit",
                      minute: "2-digit",
                    })}
                  </div>
                </div>
              </div>

              <div className="flex items-center gap-3 mt-4">
                <span className={`text-sm font-medium ${bet.isWinning ? "text-yellow-200" : "text-gray-400"}`}>
                  {t("your_numbers")}
                </span>
                <div className="flex gap-2 flex-wrap">
                  {bet.numbers.split(",").map((num) => (
                    <div
                      key={num}
                      className={`w-14 h-14 rounded-xl text-white flex items-center justify-center font-bold text-lg shadow-lg transition-transform hover:scale-110 ${
                        bet.isWinning
                          ? "bg-gradient-to-br from-yellow-500 to-yellow-600 border-2 border-yellow-300 shadow-yellow-500/50"
                          : "bg-red-600"
                      }`}
                    >
                      {num.trim().padStart(2, "0")}
                    </div>
                  ))}
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>

      {bets.length === 0 && !loading && (
        <p className="text-center text-gray-400 mt-10">{t("no_coupons")}</p>
      )}

      {totalCount > 0 && Math.ceil(totalCount / pageSize) > 1 && (
        <div className="mt-8">
          <Pagination
            currentPage={currentPage}
            totalPages={Math.ceil(totalCount / pageSize)}
            onPageChange={setCurrentPage}
            perPage={pageSize}
            totalItems={totalCount}
          />
        </div>
      )}
    </div>
  );
}
