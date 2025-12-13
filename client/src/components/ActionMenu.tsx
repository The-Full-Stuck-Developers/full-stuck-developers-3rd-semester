import {type JSX, useEffect, useRef, useState} from "react";
import {AlertTriangle, EllipsisVertical} from "lucide-react";
import {useTranslation} from "react-i18next";

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

export function ActionMenu({actions, dropdown = true}: { actions: ActionItem[]; dropdown?: boolean }) {
    const {t} = useTranslation();
    const [open, setOpen] = useState(false);
    const [flipUp, setFlipUp] = useState(false);
    const [confirmAction, setConfirmAction] = useState<ActionItem | null>(null);
    const [hoveredIndex, setHoveredIndex] = useState<number | null>(null);
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
                {dropdown ? (
                    <>
                        <button
                            onClick={() => setOpen(!open)}
                            className="px-3 py-1.5 bg-gray-800 border border-gray-700 rounded-md hover:bg-gray-900 hover:border-gray-900 flex flex-row items-center cursor-pointer"
                        >
                            <EllipsisVertical size={14} className="me-1"/>
                            {t("actions")}
                        </button>

                        {open && (
                            <div
                                className={`absolute -left-14 min-w-36 bg-gray-800 border border-gray-700 rounded-md shadow-xl z-10 animate-fade overflow-hidden ${
                                    flipUp ? "bottom-full mb-2" : "mt-2"
                                }`}
                            >
                                {actions.map((action, i) =>
                                    "separator" in action ? (
                                        <div key={i} className="border-t brder-gray-700"></div>
                                    ) : (
                                        <button
                                            key={i}
                                            onClick={() => handleActionClick(action)}
                                            className="flex items-center w-full text-left px-4 py-2 hover:bg-gray-700 cursor-pointer"
                                        >
                                            <div className="w-4 h-4 flex items-center justify-center me-2">{action.icon}</div>
                                            <span style={{color: action.color ?? "#000000"}}>{action.label}</span>
                                        </button>
                                    )
                                )}
                            </div>
                        )}
                    </>
                ) : (
                    <div className="flex flex-row gap-2">
                        {actions.map((action, i) =>
                            "separator" in action ? (
                                <div key={i} className="w-px bg-gray-700 mx-1"></div>
                            ) : (
                                <div key={i} className="relative">
                                    <button
                                        onClick={() => handleActionClick(action)}
                                        onMouseEnter={() => setHoveredIndex(i)}
                                        onMouseLeave={() => setHoveredIndex(null)}
                                        className="p-2 rounded-md hover:bg-gray-700 cursor-pointer"
                                        style={{color: action.color ?? "#000000"}}
                                    >
                                        <div className="w-4 h-4 flex items-center justify-center">{action.icon}</div>
                                    </button>
                                    {hoveredIndex === i && (
                                        <div className="absolute bottom-full left-1/2 -translate-x-1/2 mb-2 px-2 py-1 bg-gray-900 text-white text-xs rounded whitespace-nowrap z-20">
                                            {action.label}
                                            <div className="absolute top-full left-1/2 -translate-x-1/2 -mt-1 border-4 border-transparent border-t-gray-900"></div>
                                        </div>
                                    )}
                                </div>
                            )
                        )}
                    </div>
                )}
            </div>

            {confirmAction && "onClick" in confirmAction && (
                <div
                    className="fixed inset-0 bg-gray-700/65 bg-opacity-50 flex items-center justify-center z-50"
                    onClick={handleCancel}
                >
                    <div
                        className="bg-gray-800 rounded-lg shadow-2xl max-w-md w-full mx-4 p-6"
                        onClick={(e) => e.stopPropagation()}
                    >
                        <div className="flex items-start mb-4">
                            <div
                                className="shrink-0 w-10 h-10 rounded-full bg-amber-100 flex items-center justify-center mr-3">
                                <AlertTriangle className="text-amber-600" size={20}/>
                            </div>
                            <div className="flex-1">
                                <h3 className="text-lg font-semibold text-white mb-1">
                                    {confirmAction.confirmTitle || "Confirm Action"}
                                </h3>
                                <p className="text-sm text-gray-100">
                                    {confirmAction.confirmMessage || `Are you sure you want to ${confirmAction.label.toLowerCase()}?`}
                                </p>
                            </div>
                        </div>

                        <div className="flex justify-end gap-3 mt-6">
                            <button
                                onClick={handleCancel}
                                className="px-4 py-2 border border-gray-700 rounded-lg bg-gray-700 hover:bg-gray-600 cursor-pointer"
                            >
                                {t("cancel")}
                            </button>
                            <button
                                onClick={handleConfirm}
                                className="px-4 py-2 text-sm font-medium text-white bg-amber-600 rounded-md hover:bg-amber-700 cursor-pointer"
                                style={{backgroundColor: confirmAction.color || "#dc2626"}}
                            >
                                {t("confirm")}
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </>
    );
}
