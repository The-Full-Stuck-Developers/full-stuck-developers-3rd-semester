import { useState, useEffect } from "react";
import { Gamepad2, DollarSign, Clock, TrendingUp, Plus } from "lucide-react";
import { Link } from "react-router-dom";
import { useAuth } from "../../../hooks/auth.tsx";
import getTransactionsClient from "@core/clients/transactionClient.ts";
import getBetsClient from "@core/clients/betsClient.ts";
import { useTranslation } from "react-i18next";
import { baseUrl } from "@core/baseUrl.ts";
import { TOKEN_KEY, tokenStorage } from "../../../atoms/token.ts";

export function DashboardOverview() {
  const { t } = useTranslation("player");
  const { user } = useAuth();
  const [balance, setBalance] = useState<number | null>(null);
  const [activeBoards, setActiveBoards] = useState<number>(0);
  const [totalDeposits, setTotalDeposits] = useState<number | null>(null);
  const [betDeadline, setBetDeadline] = useState<Date | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadDashboardData = async () => {
      if (!user?.id) return;

      try {
        const transactionsClient = getTransactionsClient();
        const betsClient = getBetsClient();

        const userBalance = await transactionsClient.getUserBalance(user.id);
        setBalance(userBalance);

        const deposits = await transactionsClient.getUserDepositTotal(user.id);
        setTotalDeposits(deposits);

        const activeBoardsResponse = await betsClient.getUserActiveBoards(
          1,
          100,
        );
        setActiveBoards(activeBoardsResponse.totalCount);

        try {
          const token = tokenStorage.getItem(TOKEN_KEY, null);
          const response = await fetch(`${baseUrl}/api/Games/player/current`, {
            headers: {
              Authorization: token ? `Bearer ${token}` : "",
              Accept: "application/json",
            },
          });
          if (response.ok) {
            const currentGame = await response.json();

            if (currentGame && currentGame.betDeadline) {
              setBetDeadline(new Date(currentGame.betDeadline));
            }
          }
        } catch (gameError) {
          console.log("No current game available");
        }
      } catch (error) {
        console.error("Failed to load dashboard data:", error);
      } finally {
        setLoading(false);
      }
    };

    loadDashboardData();
  }, [user?.id]);

  const formatCurrency = (amount: number | null) => {
    if (amount === null) return "...";
    return `${amount.toLocaleString("da-DK")} kr`;
  };

  const formatBetDeadline = (deadline: Date | null) => {
    if (!deadline) return null;

    const now = new Date();
    const isPast = deadline < now;

    const localDeadline = new Date(deadline);
    const dayName = localDeadline.toLocaleDateString("en-US", {
      weekday: "long",
      timeZone: "Europe/Copenhagen",
    });
    const time = localDeadline.toLocaleTimeString("en-US", {
      hour: "2-digit",
      minute: "2-digit",
      hour12: false,
      timeZone: "Europe/Copenhagen",
    });

    return { dayName, time, isPast };
  };

  const deadlineInfo = formatBetDeadline(betDeadline);

  return (
    <>
      <div className="flex justify-between items-start mb-10">
        <div>
          <h1 className="text-4xl font-black mb-2">
            {t("welcome_back", { name: user?.name ?? "Loading..." })}
          </h1>
          <p className="text-xl text-gray-400">{t("ready_to_play")}</p>
        </div>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">
        <div className="bg-gray-800 rounded-2xl p-6 border border-gray-700">
          <div className="flex items-start justify-between mb-4">
            <div className="text-4xl font-bold">
              {loading ? "..." : activeBoards}
            </div>
            <div className="p-3 rounded-xl bg-blue-900/30">
              <Gamepad2 className="w-6 h-6 text-blue-400" />
            </div>
          </div>
          <p className="text-sm text-gray-400">{t("active_boards")}</p>
          <p className="text-xs text-gray-500 mt-1">{t("total_bets_placed")}</p>
        </div>

        <div className="bg-gray-800 rounded-2xl p-6 border border-gray-700">
          <div className="flex items-start justify-between mb-4">
            <div className="text-4xl font-bold">
              {loading ? "..." : formatCurrency(balance)}
            </div>
            <div className="p-3 rounded-xl bg-green-900/30">
              <DollarSign className="w-6 h-6 text-green-400" />
            </div>
          </div>
          <p className="text-sm text-gray-400">{t("balance")}</p>
          <p className="text-xs text-gray-500 mt-1">{t("available")}</p>
        </div>

        <div className="bg-gray-800 rounded-2xl p-6 border border-gray-700">
          <div className="flex items-start justify-between mb-4">
            <div className="text-4xl font-bold">
              {loading ? "..." : formatCurrency(totalDeposits)}
            </div>
            <div className="p-3 rounded-xl bg-yellow-900/30">
              <Clock className="w-6 h-6 text-yellow-400" />
            </div>
          </div>
          <p className="text-sm text-gray-400">{t("total_deposits")}</p>
          <p className="text-xs text-gray-500 mt-1">{t("all_time")}</p>
        </div>

        <div className="bg-gray-800 rounded-2xl p-6 border border-gray-700">
          <div className="flex items-start justify-between mb-4">
            <div className="text-4xl font-bold">{loading ? "..." : "0 kr"}</div>
            <div className="p-3 rounded-xl bg-purple-900/30">
              <TrendingUp className="w-6 h-6 text-purple-400" />
            </div>
          </div>
          <p className="text-sm text-gray-400">{t("total_won")}</p>
          <p className="text-xs text-gray-500 mt-1">{t("this_season")}</p>
        </div>
      </div>

      <div className="grid lg:grid-cols-2 gap-8">
        <div className="bg-gray-800 rounded-2xl p-8 border border-gray-700">
          <h2 className="text-2xl font-bold mb-6">{t("current_game")}</h2>
          <div className="text-center mb-8">
            <div className="inline-block p-6 bg-red-900/20 rounded-full mb-4">
              <Gamepad2 className="w-12 h-12 text-red-400" />
            </div>
            <div className="text-3xl font-black">{t("place_your_bet")}</div>
            <p className="text-gray-400">{t("pick_numbers")}</p>
            {deadlineInfo && (
              <div
                className={`mt-4 px-4 py-2 rounded-lg inline-block ${
                  deadlineInfo.isPast
                    ? "bg-red-900/30 border border-red-700 text-red-300"
                    : "bg-blue-900/30 border border-blue-700 text-blue-300"
                }`}
              >
                <div className="flex items-center gap-2 text-sm font-semibold">
                  <Clock className="w-4 h-4" />
                  <span>
                    {deadlineInfo.isPast
                      ? t("betting_closed")
                      : t("bets_until", {
                          day: deadlineInfo.dayName,
                          time: deadlineInfo.time,
                        })}
                  </span>
                </div>
              </div>
            )}
          </div>
          <Link
            to="/game/current"
            className={`w-full py-4 rounded-xl font-bold flex items-center justify-center gap-3 transition ${
              deadlineInfo?.isPast
                ? "bg-gray-600 text-gray-400 cursor-not-allowed"
                : "bg-red-600 hover:bg-red-700 text-white"
            }`}
            onClick={(e) => {
              if (deadlineInfo?.isPast) {
                e.preventDefault();
              }
            }}
          >
            <Plus className="w-6 h-6" />
            {deadlineInfo?.isPast ? t("betting_closed") : t("buy_new_board")}
          </Link>
        </div>

        <div className="bg-gray-800 rounded-2xl p-8 border border-gray-700">
          <h2 className="text-2xl font-bold mb-6">{t("quick_actions")}</h2>
          <div className="space-y-4">
            <Link
              to="/player/deposit"
              className="block p-5 bg-gray-700/50 rounded-xl hover:bg-gray-700 transition"
            >
              <div className="font-semibold">{t("add_funds")}</div>
              <div className="text-sm text-gray-400">
                {t("top_up_mobilepay")}
              </div>
            </Link>
            <Link
              to="/player/boards"
              className="block p-5 bg-gray-700/50 rounded-xl hover:bg-gray-700 transition"
            >
              <div className="font-semibold">{t("manage_boards")}</div>
              <div className="text-sm text-gray-400">
                {t("view_submitted_boards")}
              </div>
            </Link>
          </div>
        </div>
      </div>
    </>
  );
}
