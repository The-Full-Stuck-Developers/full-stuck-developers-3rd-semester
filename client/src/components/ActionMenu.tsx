import { type JSX, useEffect, useRef, useState } from "react";
import { EllipsisVertical, AlertTriangle } from "lucide-react";

type ActionItem =
    | {
    label: string;
    color?: string;
    onClick: () => void;
    icon?: JSX.Element;
    requiresConfirmation?: boolean;
    confirmTitle?: string;
    confirmMessage?: string;
}
    | { separator: true };

export function ActionMenu({ actions }: { actions: ActionItem[] }) {
    const [open, setOpen] = useState(false);
    const [flipUp, setFlipUp] = useState(false);
    const [confirmAction, setConfirmAction] = useState<ActionItem | null>(null);
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
            const menuHeight = actions.length * 40;
            setFlipUp(spaceBelow < menuHeight);
        }
    }, [open, actions.length]);

    const handleActionClick = (action: ActionItem) => {
        if ("separator" in action) return;

        setOpen(false);

        if (action.requiresConfirmation) {
            setConfirmAction(action);
        } else {
            action.onClick();
        }
    };

    const handleConfirm = () => {
        if (confirmAction && "onClick" in confirmAction) {
            confirmAction.onClick();
        }
        setConfirmAction(null);
    };

    const handleCancel = () => {
        setConfirmAction(null);
    };

    return (
        <>
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
                                    onClick={() => handleActionClick(action)}
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

            {confirmAction && "onClick" in confirmAction && (
                <div
                    className="fixed inset-0 bg-slate-700/65 bg-opacity-50 flex items-center justify-center z-50"
                    onClick={handleCancel}
                >
                    <div
                        className="bg-white rounded-lg shadow-2xl max-w-md w-full mx-4 p-6"
                        onClick={(e) => e.stopPropagation()}
                    >
                        <div className="flex items-start mb-4">
                            <div className="flex-shrink-0 w-10 h-10 rounded-full bg-amber-100 flex items-center justify-center mr-3">
                                <AlertTriangle className="text-amber-600" size={20} />
                            </div>
                            <div className="flex-1">
                                <h3 className="text-lg font-semibold text-gray-900 mb-1">
                                    {confirmAction.confirmTitle || "Confirm Action"}
                                </h3>
                                <p className="text-sm text-gray-600">
                                    {confirmAction.confirmMessage || `Are you sure you want to ${confirmAction.label.toLowerCase()}?`}
                                </p>
                            </div>
                        </div>

                        <div className="flex justify-end gap-3 mt-6">
                            <button
                                onClick={handleCancel}
                                className="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50 cursor-pointer"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleConfirm}
                                className="px-4 py-2 text-sm font-medium text-white bg-amber-600 rounded-md hover:bg-amber-700 cursor-pointer"
                                style={{ backgroundColor: confirmAction.color || "#dc2626" }}
                            >
                                Confirm
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </>
    );
}

// Demo usage
export default function Demo() {
    return (
        <div className="p-8 bg-gray-50 min-h-screen">
            <h1 className="text-2xl font-bold mb-6">Action Menu Demo</h1>

            <ActionMenu
                actions={[
                    {
                        label: "Edit",
                        onClick: () => alert("Edit clicked"),
                        icon: <span>✏️</span>
                    },
                    {
                        label: "Duplicate",
                        onClick: () => alert("Duplicated"),
                        icon: <span>📋</span>
                    },
                    { separator: true },
                    {
                        label: "Archive",
                        onClick: () => alert("Archived"),
                        color: "#f59e0b",
                        requiresConfirmation: true,
                        confirmTitle: "Archive Item?",
                        confirmMessage: "This item will be moved to the archive. You can restore it later."
                    },
                    {
                        label: "Delete",
                        onClick: () => alert("Deleted!"),
                        color: "#ef4444",
                        requiresConfirmation: true,
                        confirmTitle: "Delete Item?",
                        confirmMessage: "This action cannot be undone. The item will be permanently deleted."
                    }
                ]}
            />
        </div>
    );
}