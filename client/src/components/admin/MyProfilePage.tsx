import { useNavigate } from "react-router-dom";

//deprecated
export default function MyProfilePage() {
  const navigate = useNavigate();

  const tempUser = {
    fullName: "Jan Kowalski",
  };

  // Fake dashboard data – replace with real API data later
  const overview = {
    totalPlayers: 132,
    activePlayers: 97,
    upcomingGame: {
      week: 49,
      start: "Sun 23 Mar 2025",
      closes: "Sat 29 Mar • 17:00",
    },
    currentPool: {
      digitalRevenueCents: 184500, // 1,845.00 DKK
      estimatedPrizeCents: Math.round(184500 * 0.7),
    },
    openTransactions: 5,
  };

  const formatCurrency = (cents: number) =>
    (cents / 100).toLocaleString("da-DK", {
      style: "currency",
      currency: "DKK",
      minimumFractionDigits: 2,
    });

  return (
    <div className="min-h-screen bg-slate-100">
      <div className="mx-auto max-w-6xl px-4 py-8 space-y-8">
        {/* HERO */}
        <section className="rounded-2xl bg-gradient-to-r from-red-500 to-rose-400 px-6 py-6 text-white shadow">
          <div className="flex flex-col justify-between gap-4 md:flex-row md:items-center">
            <div>
              <p className="text-sm uppercase tracking-wide opacity-80">
                Admin profile
              </p>
              <h1 className="mt-1 text-3xl font-semibold">
                Hello, {tempUser.fullName.split(" ")[0]}!
              </h1>
              <p className="mt-2 text-sm md:text-base opacity-90">
                Manage your personal details and admin tools for Jerne IF.
              </p>
            </div>

            <div className="flex flex-col items-start gap-3 md:items-end">
              <span className="inline-flex items-center rounded-full bg-white/20 px-3 py-1 text-xs font-medium">
                Admin • Since 2024
              </span>
              <div className="flex gap-3">
                <button className="rounded-full bg-white px-4 py-2 text-sm font-medium text-red-600 shadow-sm hover:bg-slate-100">
                  Edit profile
                </button>
                <button className="rounded-full border border-white/70 px-4 py-2 text-sm font-medium text-white hover:bg-white/10">
                  Change password
                </button>
              </div>
            </div>
          </div>
        </section>

        {/* OVERVIEW WIDGETS */}
        <section className="grid gap-4 md:grid-cols-4">
          {/* Players */}
          <div className="rounded-2xl bg-white p-4 shadow">
            <p className="text-xs font-semibold uppercase text-slate-500">
              Players
            </p>
            <p className="mt-2 text-2xl font-semibold text-slate-900">
              {overview.totalPlayers}
            </p>
            <p className="mt-1 text-xs text-slate-500">
              {overview.activePlayers} active this week
            </p>
          </div>

          {/* Upcoming game */}
          <div className="rounded-2xl bg-white p-4 shadow">
            <p className="text-xs font-semibold uppercase text-slate-500">
              Upcoming game
            </p>
            <p className="mt-2 text-lg font-semibold text-slate-900">
              Week {overview.upcomingGame.week}
            </p>
            <p className="mt-1 text-xs text-slate-500">
              Starts: {overview.upcomingGame.start}
              <br />
              Closes: {overview.upcomingGame.closes}
            </p>
          </div>

          {/* Pool / revenue */}
          <div className="rounded-2xl bg-white p-4 shadow">
            <p className="text-xs font-semibold uppercase text-slate-500">
              Digital pool
            </p>
            <p className="mt-2 text-lg font-semibold text-slate-900">
              {formatCurrency(overview.currentPool.estimatedPrizeCents)}
            </p>
            <p className="mt-1 text-xs text-slate-500">
              of {formatCurrency(overview.currentPool.digitalRevenueCents)}{" "}
              revenue (70% prize pot)
            </p>
          </div>

          {/* Open transactions */}
          <div className="rounded-2xl bg-white p-4 shadow">
            <p className="text-xs font-semibold uppercase text-slate-500">
              Pending deposits
            </p>
            <p className="mt-2 text-2xl font-semibold text-slate-900">
              {overview.openTransactions}
            </p>
            <p className="mt-1 text-xs text-slate-500">
              Waiting for admin approval
            </p>
          </div>
        </section>

        {/* PERSONAL DETAILS */}
        <section className="grid gap-6 md:grid-cols-1">
          <div className="space-y-4 rounded-2xl bg-white p-6 shadow">
            <div className="flex items-center justify-between">
              <h2 className="text-lg font-semibold">Personal details</h2>
              <button className="text-sm font-medium text-red-600 hover:underline">
                Edit
              </button>
            </div>

            <div className="space-y-3 text-sm">
              <div>
                <p className="text-xs font-medium uppercase text-slate-500">
                  Full name
                </p>
                <p className="mt-1 text-slate-800">Jan Kowalski</p>
              </div>
              <div>
                <p className="text-xs font-medium uppercase text-slate-500">
                  Email
                </p>
                <p className="mt-1 text-slate-800">jan.kowalski@example.com</p>
              </div>
              <div>
                <p className="text-xs font-medium uppercase text-slate-500">
                  Phone
                </p>
                <p className="mt-1 text-slate-800">+45 12 34 56 78</p>
              </div>
              <div className="space-y-4 text-sm">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="font-medium text-slate-800">Password</p>
                    <p className="text-xs text-slate-500">
                      Last changed 3 months ago
                    </p>
                  </div>
                  <button className="text-sm font-medium text-red-600 hover:underline">
                    Change
                  </button>
                </div>
              </div>
            </div>
          </div>
        </section>

        {/* SHORTCUTS */}
        <section className="space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-semibold">Admin shortcuts</h2>
          </div>

          <div className="grid gap-4 md:grid-cols-4">
            {[
              {
                title: "Players",
                desc: "Manage players & status",
                path: "/admin/players",
              },
              {
                title: "Games",
                desc: "View weeks & winners",
                path: "/admin/games",
              },
              {
                title: "Transactions",
                desc: "Approve deposits",
                path: "/admin/transactions",
              },
              {
                title: "Reports",
                desc: "Export data (CSV)",
                path: "/admin/reports",
              },
            ].map((card) => (
              <button
                key={card.title}
                onClick={() => navigate(card.path)}
                className="flex flex-col items-start rounded-2xl bg-white p-5 text-left shadow transition hover:-translate-y-0.5 hover:shadow-md"
              >
                <span className="mb-2 inline-flex h-9 w-9 items-center justify-center rounded-xl bg-slate-100 text-sm font-semibold">
                  {card.title[0]}
                </span>
                <span className="text-sm font-semibold text-slate-900">
                  {card.title}
                </span>
                <span className="mt-1 text-xs text-slate-500">{card.desc}</span>
              </button>
            ))}
          </div>
        </section>

        {/* RECENT ACTIVITY */}
        <section className="space-y-4 rounded-2xl bg-white p-6 shadow">
          <h2 className="text-lg font-semibold">Recent admin activity</h2>
          <div className="overflow-x-auto text-sm">
            <table className="min-w-full text-left">
              <thead>
                <tr className="border-b text-xs uppercase tracking-wide text-slate-500">
                  <th className="py-2 pr-4">Time</th>
                  <th className="py-2 pr-4">Action</th>
                </tr>
              </thead>
              <tbody className="divide-y text-slate-800">
                <tr>
                  <td className="py-2 pr-4">15 Mar 2025, 18:02</td>
                  <td className="py-2 pr-4">
                    Approved transaction{" "}
                    <span className="font-mono">#123456</span>
                  </td>
                </tr>
                <tr>
                  <td className="py-2 pr-4">15 Mar 2025, 17:45</td>
                  <td className="py-2 pr-4">Added player “Anna Jensen”</td>
                </tr>
                <tr>
                  <td className="py-2 pr-4">15 Mar 2025, 17:30</td>
                  <td className="py-2 pr-4">Closed Game Week #49</td>
                </tr>
              </tbody>
            </table>
          </div>
        </section>
      </div>
    </div>
  );
}
