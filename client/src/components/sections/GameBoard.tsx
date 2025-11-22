import "./Game.css";
import { useState } from "react";
import {toast, Toaster} from "react-hot-toast";

const MAX_SELECTION = 8;
const COST_MAP: Record<number, number> = {
    5: 20,
    6: 40,
    7: 80,
    8: 120,
};

export function GameBoard() {
    const [selected, setSelected] = useState<number[]>([]);

    const toggle = (num: number) => {

        // unselect if already selected
        if (selected.includes(num)) {
            setSelected(prev => prev.filter(x => x !== num));
            return;
        }

        // maximum selection reached
        if (selected.length >= MAX_SELECTION) {
            toast.error(`Maximum is ${MAX_SELECTION}!`);
            return;
        }

        // add to selection
        setSelected(prev => [...prev, num]);
    };

    const cost = COST_MAP[selected.length] ?? 0;
    const isNextButtonEnabled = selected.length > 4;

    return (
        <div className="gameboard-container">
            <Toaster position="top-center" />

            <h1>Test: Uge 47 2025</h1>

            <div className="gameboard-grid">
                {[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16].map(n => (
                    <button
                        key={n}
                        className={`gameboard-box ${selected.includes(n) ? "selected" : ""}`}
                        onClick={() => toggle(n)}
                    >
                        {n}
                    </button>
                ))}
            </div>

            <div className="gameboard-pris">
                Pris: <strong>{cost} kr</strong>
                {selected.length > 0 && ` (${selected.length} selected)`}
            </div>

            <button
                className="gameboard-next"
                disabled={!isNextButtonEnabled}
                style={{ opacity: isNextButtonEnabled ? 1 : 0.5 }}
            >
                Næste!
            </button>
        </div>
    );
}
