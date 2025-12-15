import { useState } from "react";
import { Toaster, toast } from "react-hot-toast";
import getTransactionsClient from "@core/clients/transactionClient.ts";
import { useAuth } from "../../../hooks/auth.tsx";

const QUICK_AMOUNTS = [20, 40, 80, 160];

export function UserDeposit() {
  const { user } = useAuth(); // ✅ get logged in user (contains id)
  const [amount, setAmount] = useState("");
  const [loading, setLoading] = useState(false);
  const [status, setStatus] = useState<{
    type: "success" | "error";
    message: string;
  } | null>(null);

  const handleQuick = (val: number) => {
    setAmount(String(val));
    setStatus(null);
  };

  const handleSubmit = async () => {
    const num = Number(amount);

    if (!num || num <= 0) {
      setStatus({ type: "error", message: "Enter valid amount" });
      return;
    }

    // ✅ userInfoAtom is async, so user can be null for a moment
    if (!user?.id) {
      toast.error("User not loaded yet, try again.");
      setStatus({ type: "error", message: "User not loaded yet" });
      return;
    }

    const ref = Math.floor(100000000 + Math.random() * 900000000);

    try {
      setLoading(true);

      const client = getTransactionsClient();

      // ✅ IMPORTANT: method name depends on generated-client
      // If your method name is different, Ctrl+click TransactionsClient and replace it here.
      await client.createTransaction({
        userId: user.id,
        amount: num,
        mobilePayTransactionNumber: ref,
      });

      toast.success("Deposit created!");
      setStatus({ type: "success", message: `Success! Ref: ${ref}` });
      setAmount("");
    } catch (e) {
      console.error(e);
      toast.error("Failed");
      setStatus({ type: "error", message: "Something went wrong" });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <Toaster
        position="top-center"
        toastOptions={{ style: { background: "#1f2937", color: "#fff" } }}
      />

      <h1 className="text-4xl font-black mb-10">Add Balance</h1>

      <div className="bg-gray-800 rounded-3xl border border-gray-700 shadow-2xl p-8 max-w-2xl">
        <div className="mb-8">
          <label className="block text-sm font-bold text-gray-300 mb-3">
            Amount (kr)
          </label>
          <input
            type="number"
            value={amount}
            onChange={(e) => {
              setAmount(e.target.value);
              setStatus(null);
            }}
            className="w-full bg-gray-700 border border-gray-600 rounded-2xl px-5 py-4 text-lg focus:outline-none focus:ring-2 focus:ring-red-500"
            placeholder="e.g. 200"
          />
        </div>

        <div className="mb-8">
          <p className="text-sm font-bold text-gray-400 mb-4">Quick amounts</p>
          <div className="grid grid-cols-4 gap-4">
            {QUICK_AMOUNTS.map((v) => (
              <button
                key={v}
                onClick={() => handleQuick(v)}
                className={`py-4 rounded-xl font-bold transition-all border ${
                  Number(amount) === v
                    ? "bg-red-600 text-white border-red-600 shadow-lg"
                    : "bg-gray-700 text-gray-300 border-gray-600 hover:bg-gray-600"
                }`}
              >
                {v} kr
              </button>
            ))}
          </div>
        </div>

        {status && (
          <div
            className={`mb-6 p-5 rounded-2xl border flex gap-4 ${
              status.type === "success"
                ? "bg-emerald-900/30 border-emerald-700 text-emerald-300"
                : "bg-red-900/30 border-red-700 text-red-300"
            }`}
          >
            <span className="text-2xl">
              {status.type === "success" ? "Success" : "Warning"}
            </span>
            <div>
              <p className="font-bold">
                {status.type === "success" ? "Success!" : "Error"}
              </p>
              <p className="text-sm">{status.message}</p>
            </div>
          </div>
        )}

        <button
          onClick={handleSubmit}
          disabled={loading || !amount || Number(amount) <= 0 || !user?.id}
          className={`w-full py-5 rounded-2xl font-bold text-lg transition-all shadow-lg ${
            loading || !amount || Number(amount) <= 0 || !user?.id
              ? "bg-gray-600 text-gray-400"
              : "bg-red-600 hover:bg-red-700 text-white hover:shadow-red-600/50"
          }`}
        >
          {loading ? "Processing..." : "Add Balance"}
        </button>

        <p className="mt-6 text-center text-sm text-gray-400">
          70% to prizes • 30% supports{" "}
          <span className="font-bold text-red-400">Jerne IF</span>
        </p>
      </div>
    </div>
  );
}
