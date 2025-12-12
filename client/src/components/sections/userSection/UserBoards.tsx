import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { Gamepad2, Edit2, Trash2, Copy, PauseCircle, Plus } from "lucide-react";

type Draft = {
    selected: number[];
    repeatWeeks: number;
};

type SubmittedBoard = {
    numbers: number[];
    fieldCount: 5 | 6 | 7 | 8;
    price: number;
    repeatWeeks: number;
    submittedAt: string;
};

export function UserBoards() {
    const [draft, setDraft] = useState<Draft | null>(null);
    const [submittedBoards, setSubmittedBoards] = useState<SubmittedBoard[]>([]);


    useEffect(() => {
        // Load draft
        const draftData = localStorage.getItem("deadPigeonDraft");
        if (draftData) {
            setDraft(JSON.parse(draftData));
        }

        // Load submitted boards
        const boards = localStorage.getItem("deadPigeonBoards");
        if (boards) {
            setSubmittedBoards(JSON.parse(boards));
        }
    }, []);

    const clearDraft = () => {
        localStorage.removeItem("deadPigeonDraft");
        setDraft(null);
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
                                <div className="font-bold text-xl text-yellow-300">Continue Your Board</div>
                                <div className="text-sm text-gray-300">
                                    {draft.selected.length} numbers selected • {draft.repeatWeeks} week{draft.repeatWeeks > 1 && "s"}
                                </div>
                            </div>
                        </div>
                        <div className="flex items-center gap-3">
                            <div className="flex gap-2">
                                {draft.selected.map(n => (
                                    <div key={n} className="w-14 h-14 bg-yellow-600 text-white rounded-xl flex items-center justify-center text-xl font-bold shadow-lg">
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
            {submittedBoards.length > 0 ? (
                <div className="space-y-6">
                    {submittedBoards.map((board, i) => (
                        <div key={i} className="bg-gray-800 rounded-2xl p-6 border border-gray-700">
                            <div className="flex items-center justify-between mb-4">
                                <div className="flex items-center gap-4">
                                    <div className="p-3 bg-green-900/30 rounded-xl">
                                        <Gamepad2 className="w-6 h-6 text-green-400" />
                                    </div>
                                    <div>
                                        <div className="font-bold text-lg">Board #{i + 1}</div>
                                        <div className="text-sm text-gray-400">
                                            {board.fieldCount} numbers • {board.repeatWeeks} week{board.repeatWeeks > 1 && "s"} • {board.price * board.repeatWeeks} kr
                                        </div>
                                    </div>
                                </div>
                                <div className="flex gap-2">
                                    <button className="p-2 hover:bg-gray-700 rounded-lg"><Copy className="w-5 h-5 text-gray-400" /></button>
                                    <button className="p-2 hover:bg-red-900/50 rounded-lg"><Trash2 className="w-5 h-5 text-red-400" /></button>
                                </div>
                            </div>

                            <div className="grid grid-cols-8 gap-3">
                                {board.numbers.map(n => (
                                    <div key={n} className="aspect-square bg-gray-700 rounded-lg flex items-center justify-center font-mono font-bold text-lg">
                                        {n.toString().padStart(2, "0")}
                                    </div>
                                ))}
                            </div>
                        </div>
                    ))}
                </div>
            ) : !draft ? (
                <div className="text-center py-20">
                    <Gamepad2 className="w-24 h-24 text-gray-600 mx-auto mb-6" />
                    <p className="text-2xl font-bold text-gray-400 mb-4">No boards yet</p>
                    <Link to="/game/current" className="inline-block px-10 py-5 bg-red-600 hover:bg-red-700 text-white font-black text-xl rounded-full">
                        Play Now
                    </Link>
                </div>
            ) : null}
        </div>
    );
}