import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { Gamepad2, Edit2, Trash2, Plus } from "lucide-react";
import getBetsClient from "@core/clients/betsClient.ts";
import { toast } from "react-hot-toast";
import Pagination from "../../Pagination";

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
};

export function UserBoards() {
  const [draft, setDraft] = useState<Draft | null>(null);
  const [submittedBoards, setSubmittedBoards] = useState<SubmittedBoard[]>([]);
  const [loading, setLoading] = useState(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const pageSize = 3;

  useEffect(() => {
    // Load draft
    const draftData = localStorage.getItem("deadPigeonDraft");
    if (draftData) {
      setDraft(JSON.parse(draftData));
    }

    // Load submitted boards from API
    const loadBoards = async () => {
      try {
        setLoading(true);
        const client = getBetsClient();
        const response = await client.getUserHistory(currentPage, pageSize);
        
        const boards: SubmittedBoard[] = response.bets.map((bet) => ({
          id: bet.id,
          numbers: bet.numbers.split(",").map((n) => parseInt(n.trim(), 10)),
          fieldCount: bet.count as 5 | 6 | 7 | 8,
          price: bet.price,
          repeatWeeks: 1, // This isn't stored in the bet history, defaulting to 1
          submittedAt: bet.date,
        }));
        
        setSubmittedBoards(boards);
        setTotalCount(response.totalCount);
      } catch (error) {
        console.error("Failed to load boards:", error);
        // Don't show error toast for empty boards - it's normal for new users
      } finally {
        setLoading(false);
      }
    };

    loadBoards();
  }, [currentPage]);

  const clearDraft = () => {
    localStorage.removeItem("deadPigeonDraft");
    setDraft(null);
  };

  const handleDeleteBoard = async (boardId: string) => {
    if (!confirm("Are you sure you want to delete this board? The money will be refunded to your account.")) {
      return;
    }

    try {
      const client = getBetsClient();
      await client.deleteBet(boardId);
      
      toast.success("Board deleted successfully. Money has been refunded to your account.");
      
      // Reload boards
      const historyResponse = await client.getUserHistory(currentPage, pageSize);
      const boards: SubmittedBoard[] = historyResponse.bets.map((bet) => ({
        id: bet.id,
        numbers: bet.numbers.split(",").map((n) => parseInt(n.trim(), 10)),
        fieldCount: bet.count as 5 | 6 | 7 | 8,
        price: bet.price,
        repeatWeeks: 1,
        submittedAt: bet.date,
      }));
      setSubmittedBoards(boards);
      setTotalCount(historyResponse.totalCount);
    } catch (error: any) {
      console.error("Error deleting board:", error);
      const errorMessage = error?.response || error?.message || "Failed to delete board. Please try again.";
      toast.error(errorMessage);
    }
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-8">
        <h1 className="text-4xl font-black bg-gradient-to-r from-white to-red-400 bg-clip-text text-transparent">
          My Boards
        </h1>
        <Link
          to="/game/current"
          className="px-6 py-3 bg-red-600 hover:bg-red-700 rounded-xl font-bold flex items-center gap-2 transition"
        >
          <Plus className="w-5 h-5" />
          New Board
        </Link>
      </div>

      {/* DRAFT BOARD – appears first */}
      {draft && (
        <Link
          to="/game/current"
          className="block mb-8 p-6 bg-yellow-900/20 border-2 border-dashed border-yellow-600/60 rounded-2xl hover:bg-yellow-900/30 transition-all"
        >
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-4">
              <div className="p-3 bg-yellow-900/40 rounded-xl">
                <Edit2 className="w-7 h-7 text-yellow-400" />
              </div>
              <div>
                <div className="font-bold text-xl text-yellow-300">
                  Continue Your Board
                </div>
                <div className="text-sm text-gray-300">
                  {draft.selected.length} numbers selected • {draft.repeatWeeks}{" "}
                  week{draft.repeatWeeks > 1 && "s"}
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

      {/* SUBMITTED BOARDS */}
      {loading ? (
        <div className="text-center py-20">
          <p className="text-gray-400">Loading your boards...</p>
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
                        {board.fieldCount} numbers • {board.price} kr
                      </div>
                      <div className="text-sm text-gray-400">
                        {new Date(board.submittedAt).toLocaleDateString()}
                      </div>
                    </div>
                  </div>
                  <div className="flex gap-2">
                    <button 
                      className="p-2 hover:bg-red-900/50 rounded-lg transition"
                      onClick={() => handleDeleteBoard(board.id)}
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
          <p className="text-2xl font-bold text-gray-400 mb-4">No boards yet</p>
          <Link
            to="/game/current"
            className="inline-block px-10 py-5 bg-red-600 hover:bg-red-700 text-white font-black text-xl rounded-full"
          >
            Play Now
          </Link>
        </div>
      ) : null}
    </div>
  );
}
