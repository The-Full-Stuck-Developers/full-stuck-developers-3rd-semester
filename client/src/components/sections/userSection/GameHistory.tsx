import { CheckCircle, Calendar } from "lucide-react";

const gameHistory = [
  {
    week: 23,
    date: "8.–15. jun. 2024",
    winning: [2, 7, 14],
    boards: 2,
    won: true,
    prize: "1.250,00 kr.",
  },
  {
    week: 22,
    date: "1.–8. jun. 2024",
    winning: [5, 11, 3],
    boards: 1,
    won: false,
  },
  {
    week: 20,
    date: "18.–25. maj 2024",
    winning: [1, 8, 13],
    boards: 2,
    won: false,
  },
  {
    week: 20,
    date: "18.–25. maj 2024",
    winning: [1, 8, 13],
    boards: 2,
    won: false,
  },
];

export function GameHistory() {
  return (
    <div>
      <h1 className="text-4xl font-black mb-8 bg-gradient-to-r from-white to-red-400 bg-clip-text text-transparent">
        Game History
      </h1>

      <div className="space-y-6">
        {gameHistory.map((game) => (
          <div
            key={game.week}
            className={`rounded-2xl border p-6 transition-all ${
              game.won
                ? "bg-green-900/20 border-green-600/50 shadow-lg shadow-green-900/20"
                : "bg-gray-800 border-gray-700"
            }`}
          >
            <div className="flex items-center justify-between mb-4">
              <div className="flex items-center gap-4">
                <div
                  className={`p-3 rounded-xl ${game.won ? "bg-green-900/40" : "bg-gray-700"}`}
                >
                  {game.won ? (
                    <CheckCircle className="w-7 h-7 text-green-400" />
                  ) : (
                    <Calendar className="w-7 h-7 text-gray-400" />
                  )}
                </div>
                <div>
                  <div className="text-xl font-bold">Week {game.week}</div>
                  <div className="text-sm text-gray-400">{game.date}</div>
                </div>
              </div>

              <div className="text-right">
                <div className="text-2xl font-black">
                  {game.boards} board{game.boards > 1 && "s"}
                </div>
                {game.won && (
                  <div className="text-green-400 font-bold text-lg mt-1">
                    Won {game.prize}
                  </div>
                )}
              </div>
            </div>

            <div className="flex items-center gap-3">
              <span className="text-sm text-gray-400">Winning numbers:</span>
              <div className="flex gap-2">
                {game.winning.map((num) => (
                  <div
                    key={num}
                    className={`w-12 h-12 rounded-lg flex items-center justify-center font-mono text-lg font-bold ${
                      game.won
                        ? "bg-green-600 text-white"
                        : "bg-gray-700 text-gray-300"
                    }`}
                  >
                    {num.toString().padStart(2, "0")}
                  </div>
                ))}
              </div>
            </div>

            {game.won && (
              <div className="mt-4 p-4 bg-green-900/30 border border-green-700/50 rounded-xl">
                <p className="text-green-300 text-sm">
                  Congratulations! Your board(s) matched all 3 winning numbers.
                  <br />
                  Prize money has been distributed by the club.
                </p>
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}
