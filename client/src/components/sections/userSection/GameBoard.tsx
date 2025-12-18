import "../../../gameBoard.css";
import { useState, useEffect } from "react";
import { toast } from "react-hot-toast";
import { Link, useNavigate } from "react-router-dom";
import { ArrowLeft } from "lucide-react";
import getBetsClient from "@core/clients/betsClient.ts";
import { useTranslation } from "react-i18next";

const MAX_SELECTION = 8;
const COST_MAP: Record<number, number> = {
  5: 20,
  6: 40,
  7: 80,
  8: 160,
};

interface PlaceBetResponse {
  success: boolean;
  message: string;
  betId: string;
  sortedNumbers: string;
  count: number;
  price: number;
  createdAt: string;
}

export function GameBoard() {
  const { t } = useTranslation("player");
  const [selected, setSelected] = useState<number[]>([]);
  const [repeatWeeks, setRepeatWeeks] = useState(1);
  const navigate = useNavigate();

  const DRAFT_KEY = "deadPigeonDraft";

  useEffect(() => {
    const draft = localStorage.getItem(DRAFT_KEY);
    if (draft) {
      try {
        const data = JSON.parse(draft);
        if (data.selected?.length > 0) {
          setSelected(data.selected);
          setRepeatWeeks(data.repeatWeeks || 1);
        }
      } catch (e) {
        localStorage.removeItem(DRAFT_KEY);
      }
    }
  }, []);

  useEffect(() => {
    if (selected.length === 0) {
      localStorage.removeItem(DRAFT_KEY);
      return;
    }

    const draft = {
      selected,
      repeatWeeks,
      savedAt: new Date().toISOString(),
    };
    localStorage.setItem(DRAFT_KEY, JSON.stringify(draft));
  }, [selected, repeatWeeks]);

  const toggle = (num: number) => {
    if (selected.includes(num)) {
      setSelected((prev) => prev.filter((x) => x !== num));
    } else if (selected.length >= MAX_SELECTION) {
      toast.error(t("maximum_selection", { max: MAX_SELECTION }));
    } else {
      setSelected((prev) => [...prev, num].sort((a, b) => a - b));
    }
  };

  const cost = COST_MAP[selected.length] ?? 0;
  const isNextButtonEnabled = selected.length >= 5;

  const submitBoard = async () => {
    if (!isNextButtonEnabled) return;

    const totalCost = cost * repeatWeeks;
    if (!confirm(t("submit_board_confirm", { cost: totalCost }))) {
      return;
    }

    const payload = {
      numbers: selected,
      count: selected.length,
      price: cost,
      repeatWeeks,
    };

    try {
      const client = getBetsClient();
      const result = await client.placeBet(payload);

      if (result && !result.success) {
        toast.error(result.message || t("not_enough_money"));
        return;
      }

      if (result && result.success) {
        toast.success(result.message);
        localStorage.removeItem(DRAFT_KEY);
        setSelected([]);
        setRepeatWeeks(1);
        navigate("/player/dashboard");
      }
    } catch (err: any) {
      console.error("Error placing bet:", err);
      let errorMessage = t("not_enough_money");
      if (err?.response) {
        try {
          const errorData =
            typeof err.response === "string"
              ? JSON.parse(err.response)
              : err.response;
          errorMessage = errorData?.message || errorMessage;
        } catch {
          errorMessage = err?.message || errorMessage;
        }
      } else if (err?.message) {
        errorMessage = err.message;
      }
      toast.error(errorMessage);
    }
  };

  const clearDraft = () => {
    if (confirm(t("clear_board_confirm"))) {
      setSelected([]);
      setRepeatWeeks(1);
      localStorage.removeItem(DRAFT_KEY);
      toast(t("draft_cleared"));
    }
  };

  return (
    <div className="gameboard-container">
      <div className="flex items-center justify-between mb-6">
        <Link
          to="/player/boards"
          className="flex items-center gap-3 text-white hover:text-red-400 transition"
        >
          <ArrowLeft className="w-6 h-6" />
          <span className="font-bold">{t("back_to_my_boards")}</span>
        </Link>
      </div>

      <h1 className="text-center text-4xl font-black mb-8 text-white">
        {t("pick_your_numbers")}
      </h1>

      <div className="gameboard-grid">
        {[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16].map((n) => (
          <button
            key={n}
            className={`gameboard-box ${selected.includes(n) ? "selected" : ""}`}
            onClick={() => toggle(n)}
          >
            {n}
          </button>
        ))}
      </div>

      {selected.length > 0 && (
        <div className="mt-8 flex justify-center gap-3 flex-wrap">
          {selected.map((n) => (
            <div
              key={n}
              className="w-16 h-16 bg-red-600 text-white rounded-2xl flex items-center justify-center text-2xl font-bold shadow-lg"
            >
              {n.toString().padStart(2, "0")}
            </div>
          ))}
        </div>
      )}

      <div className="mt-8 space-y-6">
        <div className="gameboard-pris text-center">
          {t("price")}:{" "}
          <strong className="text-3xl text-red-400">
            {cost * repeatWeeks} kr
          </strong>
          {selected.length > 0 &&
            ` (${selected.length} ${t("numbers")} × ${repeatWeeks} ${repeatWeeks > 1 ? t("weeks") : t("week")})`}
        </div>

        <div className="max-w-xs mx-auto">
          <label className="block text-sm text-gray-400 mb-2 text-center">
            {t("repeat_for")} <strong>{repeatWeeks}</strong>{" "}
            {repeatWeeks > 1 ? t("weeks") : t("week")}
          </label>
          <input
            type="range"
            min="1"
            max="20"
            value={repeatWeeks}
            onChange={(e) => setRepeatWeeks(Number(e.target.value))}
            className="w-full h-3 bg-gray-700 rounded-lg appearance-none cursor-pointer slider"
          />
          <div className="flex justify-between text-xs text-gray-500 mt-1">
            <span>1 {t("week")}</span>
            <span>20 {t("weeks")}</span>
          </div>
        </div>
      </div>

      <div className="mt-10 space-y-4">
        <button
          className="gameboard-next w-full"
          disabled={!isNextButtonEnabled}
          style={{ opacity: isNextButtonEnabled ? 1 : 0.5 }}
          onClick={submitBoard}
        >
          {isNextButtonEnabled ? (
            <>
              {t("submit_board")} ({cost * repeatWeeks} kr)
            </>
          ) : (
            t("pick_at_least_5")
          )}
        </button>

        {selected.length > 0 && (
          <button
            onClick={clearDraft}
            className="w-full py-4 bg-gray-800 hover:bg-gray-700 text-white rounded-2xl font-bold transition"
          >
            {t("clear_board")}
          </button>
        )}

        <p className="text-center text-sm text-gray-400">{t("auto_saved")}</p>
      </div>
    </div>
  );
}
