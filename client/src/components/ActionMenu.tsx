import { type JSX, useEffect, useRef, useState } from "react";
import { EllipsisVertical } from "lucide-react";

type ActionItem =
    | { label: string; color?: string; onClick: () => void; icon?: JSX.Element }
    | { separator: true };

export function ActionMenu({ actions }: { actions: ActionItem[] }) {
    const [open, setOpen] = useState(false);
    const [flipUp, setFlipUp] = useState(false);
    const ref = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        const handleClick = (e: MouseEvent) => {
            if (ref.current && !ref.current.contains(e.target as Node)) setOpen(false);
        };
        document.addEventListener("mousedown", handleClick);
        return () => document.removeEventListener("mousedown", handleClick);
    }, []);

    useEffect(() => {
        if (open && ref.current) {
            const rect = ref.current.getBoundingClientRect();
            const spaceBelow = window.innerHeight - rect.bottom;
            const menuHeight = actions.length * 40; // approx height of menu
            setFlipUp(spaceBelow < menuHeight);
        }
    }, [open, actions.length]);

    return (
        <div className="relative" ref={ref}>
            <button
                onClick={() => setOpen(!open)}
                className="px-3 py-1.5 bg-slate-300 rounded-md hover:bg-slate-400 flex flex-row items-center outline cursor-pointer"
            >
                <EllipsisVertical size={14} className="me-1" />
                Actions
            </button>

            {open && (
                <div
                    className={`absolute -left-14 min-w-36 bg-white border rounded-md shadow-xl shadow-gray-200 z-10 animate-fade overflow-hidden ${
                        flipUp ? "bottom-full mb-2" : "mt-2"
                    }`}
                >
                    {actions.map((action, i) =>
                        "separator" in action ? (
                            <div key={i} className="border-t"></div>
                        ) : (
                            <button
                                key={i}
                                onClick={() => {
                                    setOpen(false);
                                    action.onClick();
                                }}
                                className="flex items-center w-full text-left px-4 py-2 hover:bg-slate-100 cursor-pointer"
                            >
                                <div className="w-4 h-4 flex items-center justify-center me-2">{action.icon}</div>
                                <span style={{ color: action.color ?? "#000000" }}>{action.label}</span>
                            </button>
                        )
                    )}
                </div>
            )}
        </div>
    );
}
