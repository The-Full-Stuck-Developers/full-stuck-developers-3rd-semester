import "../../../gameBoard.css";
import { useState, useEffect } from "react";
import { toast } from "react-hot-toast";
import { Link, useNavigate } from "react-router-dom";
import { ArrowLeft } from "lucide-react";
import getBetsClient from "@core/clients/betsClient.ts";

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
    const [selected, setSelected] = useState<number[]>([]);
    const [repeatWeeks, setRepeatWeeks] = useState(1);
    const navigate = useNavigate();

    const DRAFT_KEY = "deadPigeonDraft";

    // Load draft (silently - no toast)
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

    // Auto save
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
            toast.error(`Maximum is ${MAX_SELECTION}!`);
        } else {
            setSelected((prev) => [...prev, num].sort((a, b) => a - b));
        }
    };

    const cost = COST_MAP[selected.length] ?? 0;
    const isNextButtonEnabled = selected.length >= 5;

    const submitBoard = async () => {
        if (!isNextButtonEnabled) return;

        const payload = {
            numbers: selected,
            count: selected.length,
            price: cost,
            repeatWeeks,
        };

        try {
            const client = getBetsClient();
            const result = await client.placeBet(payload);

            if (!result.success) {
                toast.error(result.message || "Error placing a bet");
                return;
            }

            toast.success(result.message);

            // Clear
            localStorage.removeItem(DRAFT_KEY);
            setSelected([]);
            setRepeatWeeks(1);

            navigate("/player/dashboard");
        } catch (err: any) {
            console.error("Error placing bet:", err);
            const errorMessage = err?.message || err?.response?.data?.message || "Failed to submit board. Please try again.";
            toast.error(errorMessage);
        }
    };

    const clearDraft = () => {
        if (confirm("Are you sure you want to clear your board?")) {
            setSelected([]);
            setRepeatWeeks(1);
            localStorage.removeItem(DRAFT_KEY);
            toast("Draft cleared");
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
                    <span className="font-bold">Back to My Boards</span>
                </Link>
            </div>

            <h1 className="text-center text-4xl font-black mb-8 text-white">
                Pick Your Numbers
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
                    Price:{" "}
                    <strong className="text-3xl text-red-400">
                        {cost * repeatWeeks} kr
                    </strong>
                    {selected.length > 0 &&
                        ` (${selected.length} numbers × ${repeatWeeks} week${repeatWeeks > 1 ? "s" : ""})`}
                </div>

                <div className="max-w-xs mx-auto">
                    <label className="block text-sm text-gray-400 mb-2 text-center">
                        Repeat for <strong>{repeatWeeks}</strong> week
                        {repeatWeeks > 1 && "s"}
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
                        <span>1 week</span>
                        <span>20 weeks</span>
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
                        <>Submit Board ({cost * repeatWeeks} kr)</>
                    ) : (
                        "Pick at least 5 numbers"
                    )}
                </button>

                {selected.length > 0 && (
                    <button
                        onClick={clearDraft}
                        className="w-full py-4 bg-gray-800 hover:bg-gray-700 text-white rounded-2xl font-bold transition"
                    >
                        Clear Board
                    </button>
                )}

                <p className="text-center text-sm text-gray-400">
                    Your board is <strong>automatically saved</strong> as you play
                </p>
            </div>
        </div>
    );
}