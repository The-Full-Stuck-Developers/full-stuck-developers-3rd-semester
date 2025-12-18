import { useState, useEffect } from "react";
import { Link, useLocation } from "react-router-dom";
import { Gamepad2, Trash2, Plus, FileEdit } from "lucide-react";
import getBetsClient from "@core/clients/betsClient.ts";
import { toast } from "react-hot-toast";
import Pagination from "../../Pagination";
import { useTranslation } from "react-i18next";

type Draft = {
  selected: number[];
  repeatWeeks: number;
};

type SubmittedBoard = {
  id: string;
  numbers: number[];
  fieldCount: 5 | 6 | 7 | 8;
  price: number;
  repeatWeeks: number;
  submittedAt: string;
  betSeriesId?: string | null;
  seriesTotal?: number | null;
  seriesIndex?: number | null;
  gameWeek?: number | null;
  gameYear?: number | null;
  gameStartTime?: string | null;
};

export function UserBoards() {
  const { t } = useTranslation("player");
  const location = useLocation();
  const [draft, setDraft] = useState<Draft | null>(null);
  const [submittedBoards, setSubmittedBoards] = useState<SubmittedBoard[]>([]);
  const [loading, setLoading] = useState(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const pageSize = 3;

  const loadBoards = async () => {
    try {
      setLoading(true);
      const client = getBetsClient();
      const response = await client.getUserActiveBoards(currentPage, pageSize);

      const boards: SubmittedBoard[] = response.bets.map((bet: any) => ({
        id: bet.id,
        numbers: bet.numbers
          .split(",")
          .map((n: string) => parseInt(n.trim(), 10)),
        fieldCount: bet.count as 5 | 6 | 7 | 8,
        price: bet.price,
        repeatWeeks: bet.seriesTotal || 1,
        submittedAt: bet.date,
        betSeriesId: bet.betSeriesId,
        seriesTotal: bet.seriesTotal,
        seriesIndex: bet.seriesIndex,
        gameWeek: bet.gameWeek,
        gameYear: bet.gameYear,
        gameStartTime: bet.gameStartTime,
      }));

      setSubmittedBoards(boards);
      setTotalCount(response.totalCount);
    } catch (error) {
      console.error("Failed to load boards:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const draftData = localStorage.getItem("deadPigeonDraft");
    if (draftData) {
      setDraft(JSON.parse(draftData));
    }

    loadBoards();
  }, [currentPage, location.pathname]);

  const clearDraft = () => {
    localStorage.removeItem("deadPigeonDraft");
    setDraft(null);
  };

  const handleDeleteBoard = async (boardId: string) => {
    if (!confirm(t("delete_board_confirm"))) {
      return;
    }

    try {
      const client = getBetsClient();
      await client.deleteBet(boardId);

      toast.success(t("board_deleted"));

      await loadBoards();
    } catch (error: any) {
      console.error("Error deleting board:", error);
      const errorMessage =
        error?.response ||
        error?.message ||
        "Failed to delete board. Please try again.";
      toast.error(errorMessage);
    }
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-8">
        <h1 className="text-4xl font-black bg-gradient-to-r from-white to-red-400 bg-clip-text text-transparent">
          {t("my_boards")}
        </h1>
        <Link
          to="/game/current"
          className="px-6 py-3 bg-red-600 hover:bg-red-700 rounded-xl font-bold flex items-center gap-2 transition"
        >
          <Plus className="w-5 h-5" />
          {t("new_board")}
        </Link>
      </div>

      {draft && (
        <Link
          to="/game/current"
          className="block mb-8 p-6 bg-yellow-900/20 border-2 border-dashed border-yellow-600/60 rounded-2xl hover:bg-yellow-900/30 transition-all"
        >
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-4">
              <div className="p-3 bg-yellow-900/40 rounded-xl">
                <FileEdit className="w-7 h-7 text-yellow-400" />
              </div>
              <div>
                <div className="font-bold text-xl text-yellow-300">
                  {t("continue_your_board")}
                </div>
                <div className="text-sm text-gray-300">
                  {t("numbers_selected", { count: draft.selected.length })} •{" "}
                  {draft.repeatWeeks}{" "}
                  {draft.repeatWeeks > 1 ? t("weeks") : t("week")}
                </div>
              </div>
            </div>
            <div className="flex items-center gap-3">
              <div className="flex gap-2">
                {draft.selected.map((n) => (
                  <div
                    key={n}
                    className="w-14 h-14 bg-yellow-600 text-white rounded-xl flex items-center justify-center text-xl font-bold shadow-lg"
                  >
                    {n.toString().padStart(2, "0")}
                  </div>
                ))}
              </div>
              <button
                onClick={(e) => {
                  e.preventDefault();
                  e.stopPropagation();
                  clearDraft();
                }}
                className="p-2 hover:bg-red-900/50 rounded-lg transition"
              >
                <Trash2 className="w-5 h-5 text-red-400" />
              </button>
            </div>
          </div>
        </Link>
      )}

      {loading ? (
        <div className="text-center py-20">
          <p className="text-gray-400">{t("loading_boards")}</p>
        </div>
      ) : submittedBoards.length > 0 ? (
        <>
          <div className="space-y-6">
            {submittedBoards.map((board, i) => (
              <div
                key={board.id}
                className="bg-gray-800 rounded-2xl p-6 border border-gray-700"
              >
                <div className="flex items-center justify-between mb-4">
                  <div className="flex items-center gap-4">
                    <div className="p-3 bg-green-900/30 rounded-xl">
                      <Gamepad2 className="w-6 h-6 text-green-400" />
                    </div>
                    <div>
                      <div className="font-bold text-lg">
                        {board.fieldCount} {t("numbers")} • {board.price} kr
                        {board.seriesTotal &&
                          board.seriesTotal > 1 &&
                          board.seriesIndex && (
                            <span className="ml-2 text-sm font-normal text-blue-400">
                              ({t("week")} {board.seriesIndex} {t("of")}{" "}
                              {board.seriesTotal})
                            </span>
                          )}
                      </div>
                      <div className="text-sm text-gray-400">
                        {board.gameWeek && board.gameYear ? (
                          <>
                            {t("week")} {board.gameWeek}, {board.gameYear}
                            {board.gameStartTime && (
                              <span className="ml-2">
                                •{" "}
                                {new Date(
                                  board.gameStartTime,
                                ).toLocaleDateString("en-US", {
                                  month: "short",
                                  day: "numeric",
                                })}
                              </span>
                            )}
                          </>
                        ) : (
                          new Date(board.submittedAt).toLocaleDateString()
                        )}
                      </div>
                    </div>
                  </div>
                  <div className="flex gap-2">
                    <button
                      className="p-2 hover:bg-red-900/50 rounded-lg transition"
                      onClick={() => handleDeleteBoard(board.id)}
                      title={t("delete_board")}
                    >
                      <Trash2 className="w-5 h-5 text-red-400" />
                    </button>
                  </div>
                </div>

                <div className="grid grid-cols-8 gap-3">
                  {board.numbers.map((n) => (
                    <div
                      key={n}
                      className="aspect-square bg-gray-700 rounded-lg flex items-center justify-center font-mono font-bold text-lg"
                    >
                      {n.toString().padStart(2, "0")}
                    </div>
                  ))}
                </div>
              </div>
            ))}
          </div>

          {Math.ceil(totalCount / pageSize) > 1 && (
            <div className="mt-6">
              <Pagination
                currentPage={currentPage}
                totalPages={Math.ceil(totalCount / pageSize)}
                onPageChange={setCurrentPage}
                perPage={pageSize}
                totalItems={totalCount}
              />
            </div>
          )}
        </>
      ) : !draft ? (
        <div className="text-center py-20">
          <Gamepad2 className="w-24 h-24 text-gray-600 mx-auto mb-6" />
          <p className="text-2xl font-bold text-gray-400 mb-4">
            {t("no_boards_yet")}
          </p>
          <Link
            to="/game/current"
            className="inline-block px-10 py-5 bg-red-600 hover:bg-red-700 text-white font-black text-xl rounded-full"
          >
            {t("play_now")}
          </Link>
        </div>
      ) : null}
    </div>
  );
}
