import { Check } from "lucide-react";

const boards = [
    { numbers: 5, price: 20, popular: false },
    { numbers: 6, price: 40, popular: false },
    { numbers: 7, price: 80, popular: true },
    { numbers: 8, price: 160, popular: false },
];

export const BoardsPricing = () => {
    return (
        <section id="pricing" className="w-full bg-slate-100 py-28">
            <div className="max-w-7xl mx-auto px-6 flex flex-col items-center text-center gap-16">

                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-12 w-full">
                    {boards.map((board) => (
                        <div
                            key={board.numbers}
                            className={`relative bg-white rounded-3xl border shadow-md px-10 py-16 flex flex-col items-center text-center
                                ${board.popular ? "border-blue-400 shadow-lg scale-[1.05]" : "border-slate-200"}
                            `}
                        >
                            {board.popular && (
                                <span className="absolute -top-4 right-5 bg-blue-600 text-white text-sm font-semibold px-4 py-1.5 rounded-full shadow">
                                    Popular
                                </span>
                            )}

                            <p className="text-6xl font-extrabold text-emerald-600 mb-2">
                                {board.numbers}
                            </p>
                            <p className="text-lg uppercase tracking-wide text-slate-500 mb-10">
                                Numbers
                            </p>

                            <div className="mb-10">
                                <span className="text-4xl font-bold text-slate-900">{board.price}</span>
                                <span className="text-xl text-slate-500 ml-1">DKK</span>
                            </div>

                            <ul className="space-y-4 text-lg text-slate-600 mb-12">
                                <li className="flex items-center gap-3">
                                    <Check size={22} className="text-emerald-600" />
                                    Pick from 1–16
                                </li>
                                <li className="flex items-center gap-3">
                                    <Check size={22} className="text-emerald-600" />
                                    Weekly drawings
                                </li>
                                <li className="flex items-center gap-3">
                                    <Check size={22} className="text-emerald-600" />
                                    Auto-repeat option
                                </li>
                            </ul>

                            <button
                                className={`w-full rounded-full py-4 text-lg font-semibold text-white transition shadow-md
                                    ${board.popular
                                    ? "bg-gradient-to-r from-emerald-500 to-blue-500 hover:opacity-90"
                                    : "bg-emerald-600 hover:bg-emerald-500"}
                                `}
                            >
                                Select Board
                            </button>
                        </div>
                    ))}
                </div>

                <p className="text-lg text-slate-600 mt-4">
                    <span className="font-semibold">70%</span> of proceeds go to prizes ·{" "}
                    <span className="font-semibold">30%</span> supports Jerne IF
                </p>
            </div>
        </section>
    );
};
