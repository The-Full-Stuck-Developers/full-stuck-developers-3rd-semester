import { useState, useEffect } from "react";
import { Gamepad2, DollarSign, Clock, TrendingUp, Plus } from "lucide-react";
import { Link } from "react-router-dom";
import { useAuth } from "../../../hooks/auth.tsx";
import getTransactionsClient from "@core/clients/transactionClient.ts";
import getBetsClient from "@core/clients/betsClient.ts";

export function DashboardOverview() {
  const { user } = useAuth();
  const [balance, setBalance] = useState<number | null>(null);
  const [activeBoards, setActiveBoards] = useState<number>(0);
  const [totalDeposits, setTotalDeposits] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadDashboardData = async () => {
      if (!user?.id) return;

      try {
        const transactionsClient = getTransactionsClient();
        const betsClient = getBetsClient();

        // Fetch balance
        const userBalance = await transactionsClient.getUserBalance(user.id);
        setBalance(userBalance);

        // Fetch total deposits
        const deposits = await transactionsClient.getUserDepositTotal(user.id);
        setTotalDeposits(deposits);

        // Fetch boards count
        const betsResponse = await betsClient.getUserHistory(1, 100);
        setActiveBoards(betsResponse.totalCount);
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

  return (
    <>
      <div className="flex justify-between items-start mb-10">
        <div>
          <h1 className="text-4xl font-black mb-2">
            Welcome back, {user?.name ?? "Loading..."}!
          </h1>
          <p className="text-xl text-gray-400">Ready to play?</p>
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
          <p className="text-sm text-gray-400">Active Boards</p>
          <p className="text-xs text-gray-500 mt-1">Total bets placed</p>
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
          <p className="text-sm text-gray-400">Balance</p>
          <p className="text-xs text-gray-500 mt-1">Available</p>
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
          <p className="text-sm text-gray-400">Total Deposits</p>
          <p className="text-xs text-gray-500 mt-1">All time</p>
        </div>

        <div className="bg-gray-800 rounded-2xl p-6 border border-gray-700">
          <div className="flex items-start justify-between mb-4">
            <div className="text-4xl font-bold">{loading ? "..." : "0 kr"}</div>
            <div className="p-3 rounded-xl bg-purple-900/30">
              <TrendingUp className="w-6 h-6 text-purple-400" />
            </div>
          </div>
          <p className="text-sm text-gray-400">Total Won</p>
          <p className="text-xs text-gray-500 mt-1">This season</p>
        </div>
      </div>

      <div className="grid lg:grid-cols-2 gap-8">
        <div className="bg-gray-800 rounded-2xl p-8 border border-gray-700">
          <h2 className="text-2xl font-bold mb-6">Current Game</h2>
          <div className="text-center mb-8">
            <div className="inline-block p-6 bg-red-900/20 rounded-full mb-4">
              <Gamepad2 className="w-12 h-12 text-red-400" />
            </div>
            <div className="text-3xl font-black">Place Your Bet</div>
            <p className="text-gray-400">Pick 5-8 numbers to play</p>
          </div>
          <Link
            to="/game/current"
            className="w-full bg-red-600 hover:bg-red-700 text-white py-4 rounded-xl font-bold flex items-center justify-center gap-3 transition"
          >
            <Plus className="w-6 h-6" />
            Buy New Board
          </Link>
        </div>

        <div className="bg-gray-800 rounded-2xl p-8 border border-gray-700">
          <h2 className="text-2xl font-bold mb-6">Quick Actions</h2>
          <div className="space-y-4">
            <Link
              to="/player/deposit"
              className="block p-5 bg-gray-700/50 rounded-xl hover:bg-gray-700 transition"
            >
              <div className="font-semibold">Add Funds</div>
              <div className="text-sm text-gray-400">Top up via MobilePay</div>
            </Link>
            <Link
              to="/player/boards"
              className="block p-5 bg-gray-700/50 rounded-xl hover:bg-gray-700 transition"
            >
              <div className="font-semibold">Manage Boards</div>
              <div className="text-sm text-gray-400">
                View your submitted boards
              </div>
            </Link>
          </div>
        </div>
      </div>
    </>
  );
}
