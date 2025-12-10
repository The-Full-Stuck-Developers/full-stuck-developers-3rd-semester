import { useState } from "react";
import { Toaster, toast } from "react-hot-toast";
import { Navbar } from "@components/sections/NavBar.tsx";

const QUICK_AMOUNTS = [20, 40, 80, 160];

type UserDepositProps = {
    userId: string;
};

type StatusState =
    | null
    | {
    type: "success" | "error";
    message: string;
};

export function UserDeposit({ userId }: UserDepositProps) {
    const [amount, setAmount] = useState<string>("");
    const [loading, setLoading] = useState(false);
    const [status, setStatus] = useState<StatusState>(null);

    const handleQuickAmountClick = (value: number) => {
        setAmount(String(value));
        setStatus(null);
    };

    const handleAddBalance = async () => {
        const parsedAmount = Number(amount);

        if (!parsedAmount || parsedAmount <= 0) {
            const msg = "Please enter a valid amount.";
            toast.error(msg);
            setStatus({ type: "error", message: msg });
            return;
        }

        if (!userId) {
            const msg = "No user id – are you logged in?";
            toast.error(msg);
            setStatus({ type: "error", message: msg });
            return;
        }

        // generate fake MobilePay transaction number
        const generatedMobilePayNumber =
            Math.floor(100000000 + Math.random() * 900000000);

        try {
            setLoading(true);
            setStatus(null);

            const res = await fetch("http://localhost:5284/api/Transactions", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    userId,
                    amount: parsedAmount,
                    mobilePayTransactionNumber: generatedMobilePayNumber,
                }),
            });

            if (!res.ok) {
                let message = "Failed to create deposit.";
                try {
                    const data = await res.json();
                    if (data?.message) message = data.message;
                } catch {
                    // ignore
                }
                throw new Error(message);
            }

            const successMessage = `Deposit of ${parsedAmount} kr was created successfully. Reference: ${generatedMobilePayNumber}`;
            toast.success(successMessage);
            setStatus({
                type: "success",
                message: successMessage,
            });
            setAmount("");
        } catch (err: any) {
            const errorMessage = err?.message ?? "Something went wrong while creating your deposit.";
            toast.error(errorMessage);
            setStatus({
                type: "error",
                message: errorMessage,
            });
        } finally {
            setLoading(false);
        }
    };

    const isDisabled = loading || !amount.trim() || Number(amount) <= 0;

    return (
        <div className="min-h-screen bg-white flex justify-center items-start py-16 px-4">
            <Navbar onLoginClick={console.log} />

            <Toaster position="top-center" />

            <div className="w-full max-w-xl mt-20">
                <h1 className="text-center text-[#213965] text-3xl md:text-4xl font-extrabold mb-6">
                    Add Balance
                </h1>

                <div className="bg-white border border-[#213965] rounded-3xl shadow-xl px-6 sm:px-8 py-8">
                    <div className="mb-6">
                        <label className="block text-sm font-semibold text-[#0C244B] mb-1">
                            Amount (kr)
                        </label>
                        <input
                            type="number"
                            min={1}
                            value={amount}
                            onChange={(e) => {
                                setAmount(e.target.value);
                                setStatus(null);
                            }}
                            className="w-full rounded-2xl border border-slate-200 px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-[#FF2B3A]/30 focus:border-[#FF2B3A]"
                            placeholder="Enter amount in kr"
                        />
                    </div>

                    <div className="mb-6">
                        <p className="text-xs font-semibold text-[#6071A0] mb-2">
                            Quick amounts
                        </p>
                        <div className="flex flex-wrap gap-3">
                            {QUICK_AMOUNTS.map((value) => {
                                const isActive = Number(amount) === value;
                                return (
                                    <button
                                        key={value}
                                        type="button"
                                        onClick={() => handleQuickAmountClick(value)}
                                        className={[
                                            "px-4 py-2 rounded-full text-sm font-semibold transition-all border",
                                            isActive
                                                ? "bg-[#FF2B3A] text-white border-[#FF2B3A] shadow-md"
                                                : "bg-[#F5F7FB] text-[#04153A] border-[#FF2B3A]/40 hover:bg-[#E3EBFF]",
                                        ].join(" ")}
                                    >
                                        {value} kr
                                    </button>
                                );
                            })}
                        </div>
                    </div>

                    {/* Big status message box */}
                    {status && (
                        <div
                            className={[
                                "mb-6 rounded-2xl border px-4 py-3 text-sm leading-relaxed flex gap-3",
                                status.type === "success"
                                    ? "bg-[#E8FFF2] border-emerald-400 text-emerald-800"
                                    : "bg-[#FFECEC] border-[#FF2B3A]/70 text-[#4B0C0C]",
                            ].join(" ")}
                        >
                            <span className="mt-0.5 text-lg">
                                {status.type === "success" ? "✅" : "⚠️"}
                            </span>
                            <div>
                                <p className="font-semibold mb-1">
                                    {status.type === "success"
                                        ? "Deposit created"
                                        : "Something went wrong"}
                                </p>
                                <p>{status.message}</p>
                            </div>
                        </div>
                    )}

                    <button
                        type="button"
                        onClick={handleAddBalance}
                        disabled={isDisabled}
                        className={[
                            "w-full rounded-full py-3 text-sm font-semibold transition-all shadow-md",
                            isDisabled
                                ? "bg-slate-300 text-slate-500 cursor-not-allowed"
                                : "bg-[#FF2B3A] text-white hover:-translate-y-0.5 hover:shadow-lg",
                        ].join(" ")}
                    >
                        {loading ? "Processing..." : "Add Balance"}
                    </button>

                    <p className="mt-4 text-center text-xs text-slate-500">
                        70% to prizes · 30% supports{" "}
                        <span className="font-semibold">Jerne IF</span>
                    </p>

                    {/* Back button */}
                    <button
                        type="button"
                        onClick={() => window.history.back()}
                        className="mt-6 w-full rounded-full border border-[#213965] py-2.5 text-sm font-semibold text-[#213965] hover:bg-[#213965] hover:text-white transition-all"
                    >
                        Back to user page
                    </button>
                </div>
            </div>
        </div>
    );
}
