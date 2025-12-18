import { useState, useEffect } from "react";
import { toast } from "react-hot-toast";
import getBetsClient from "@core/clients/betsClient.ts";

interface BetHistoryDto {
  id: string;
  numbers: string;
  count: number;
  price: number;
  date: string;
}

interface BetHistoryResponse {
  bets: BetHistoryDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export function GameHistory() {
  const [bets, setBets] = useState<BetHistoryDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadHistory = async () => {
      try {
        const client = getBetsClient();
        const response = await client.getUserHistory(1, 100);
        setBets(response.bets);
      } catch (err) {
        console.error(err);
        // Don't show error toast for empty history - it's normal for new users
      } finally {
        setLoading(false);
      }
    };

    loadHistory();
  }, []);

  if (loading) return <p className="text-center">Loading history...</p>;

  return (
    <div>
      <h1 className="text-4xl font-black mb-8 bg-gradient-to-r from-white to-red-400 bg-clip-text text-transparent">
        Game History
      </h1>

      <div className="space-y-6">
        {bets.map((bet) => (
          <div
            key={bet.id}
            className="rounded-2xl border border-gray-700 bg-gray-800 p-6"
          >
            <div className="flex items-center justify-between mb-4">
              <div>
                <div className="text-xl font-bold">
                  {bet.count} numbers • {bet.price} kr
                </div>
                <div className="text-sm text-gray-400">
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

            <div className="flex items-center gap-3">
              <span className="text-sm text-gray-400">Your numbers:</span>
              <div className="flex gap-2 flex-wrap">
                {bet.numbers.split(",").map((num) => (
                  <div
                    key={num}
                    className="w-12 h-12 rounded-lg bg-red-600 text-white flex items-center justify-center font-bold text-lg"
                  >
                    {num.trim().padStart(2, "0")}
                  </div>
                ))}
              </div>
            </div>
          </div>
        ))}
      </div>

      {bets.length === 0 && !loading && (
        <p className="text-center text-gray-400 mt-10">No coupons available.</p>
      )}
    </div>
  );
}
