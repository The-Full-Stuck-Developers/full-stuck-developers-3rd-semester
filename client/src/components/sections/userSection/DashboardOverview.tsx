import { Gamepad2, DollarSign, Clock, TrendingUp, Plus } from "lucide-react";
import { Link } from "react-router-dom";

export function DashboardOverview() {
  return (
    <>
      <div className="flex justify-between items-start mb-10">
        <div>
          <h1 className="text-4xl font-black mb-2">Welcome back, Jens!</h1>
          <p className="text-xl text-gray-400">
            Week 24 ends in{" "}
            <span className="text-red-400 font-bold">3 days</span>
          </p>
        </div>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">
        {[
          {
            value: "3",
            title: "Active Boards",
            subtitle: "Week 24",
            icon: Gamepad2,
            color: "blue",
          },
          {
            value: "285,00 kr.",
            title: "Balance",
            subtitle: "Available",
            icon: DollarSign,
            color: "green",
          },
          {
            value: "2",
            title: "Pending",
            subtitle: "Deposits & boards",
            icon: Clock,
            color: "yellow",
          },
          {
            value: "2.150,00 kr.",
            title: "Total Won",
            subtitle: "This season",
            icon: TrendingUp,
            color: "purple",
          },
        ].map((stat, i) => (
          <div
            key={i}
            className="bg-gray-800 rounded-2xl p-6 border border-gray-700"
          >
            <div className="flex items-start justify-between mb-4">
              <div className="text-4xl font-bold">{stat.value}</div>
              <div className={`p-3 rounded-xl bg-${stat.color}-900/30`}>
                <stat.icon className={`w-6 h-6 text-${stat.color}-400`} />
              </div>
            </div>
            <p className="text-sm text-gray-400">{stat.title}</p>
            <p className="text-xs text-gray-500 mt-1">{stat.subtitle}</p>
          </div>
        ))}
      </div>

      <div className="grid lg:grid-cols-2 gap-8">
        <div className="bg-gray-800 rounded-2xl p-8 border border-gray-700">
          <h2 className="text-2xl font-bold mb-6">Current Game – Week 24</h2>
          <div className="text-center mb-8">
            <div className="inline-block p-6 bg-red-900/20 rounded-full mb-4">
              <Gamepad2 className="w-12 h-12 text-red-400" />
            </div>
            <div className="text-3xl font-black">2.850,00 kr. Pot</div>
            <p className="text-gray-400">Ends Saturday 17:00</p>
          </div>
          <Link
            to="/game/current"
            className="w-full bg-red-600 hover:bg-red-700 text-white py-4 rounded-xl font-bold flex items-center justify-center gap-3 transition"
          >
            <Plus className="w-6 h-6" />
            Buy New Board
          </Link>
        </div>

        <div className="bg-gray-800 rounded-2xl p-8 border border-gray-700">
          <h2 className="text-2xl font-bold mb-6">Quick Actions</h2>
          <div className="space-y-4">
            <Link
              to="/player/deposit"
              className="block p-5 bg-gray-700/50 rounded-xl hover:bg-gray-700 transition"
            >
              <div className="font-semibold">Add Funds</div>
              <div className="text-sm text-gray-400">Top up via MobilePay</div>
            </Link>
            <Link
              to="/player/boards"
              className="block p-5 bg-gray-700/50 rounded-xl hover:bg-gray-700 transition"
            >
              <div className="font-semibold">Manage Boards</div>
              <div className="text-sm text-gray-400">
                Repeat or cancel subscriptions
              </div>
            </Link>
          </div>
        </div>
      </div>
    </>
  );
}
