import { useState } from "react";
import {
  BarChart3,
  Gamepad2,
  DollarSign,
  TrendingUp,
  Menu,
  X,
  Settings,
  LogOut,
  Moon,
  Languages,
} from "lucide-react";
import { Link, Outlet, useNavigate } from "react-router-dom";
import Logo from "../../../jerneif-logo.png";
import { useAuth } from "../../../hooks/auth.tsx";

export function PlayerDashboard() {
  const { user } = useAuth();
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const navigate = useNavigate();

  const navItems = [
    { to: "/player/dashboard", icon: BarChart3, label: "Overview" },
    { to: "/player/boards", icon: Gamepad2, label: "My Boards" },
    { to: "/player/deposit", icon: DollarSign, label: "Add Balance" },
    { to: "/player/history", icon: TrendingUp, label: "Game History" },
  ];

  const initials = user?.name ? user.name.charAt(0).toUpperCase() : "U";

  return (
    <div className="min-h-screen bg-gray-900 text-white">
      {/* Mobile Header */}
      <div className="lg:hidden flex items-center justify-between p-4 border-b border-gray-800">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-full bg-red-600 flex items-center justify-center text-white font-bold">
            DP
          </div>
          <div>
            <div className="font-semibold">Dead Pigeons</div>
            <div className="text-xs text-gray-400">PLAYER PORTAL</div>
          </div>
        </div>
        <button onClick={() => setSidebarOpen(!sidebarOpen)}>
          {sidebarOpen ? (
            <X className="w-6 h-6" />
          ) : (
            <Menu className="w-6 h-6" />
          )}
        </button>
      </div>

      <div className="flex">
        {/* Sidebar */}
        <aside
          className={`${
            sidebarOpen ? "translate-x-0" : "-translate-x-full"
          } lg:translate-x-0 fixed lg:static inset-y-0 left-0 z-50 w-64 bg-gray-800 border-r border-gray-700 transition-transform duration-300 ease-in-out`}
        >
          <div className="p-6 flex flex-col h-full">
            {/* Logo */}
            <div className="hidden lg:flex items-center gap-3 mb-10">
              <div className="w-12 h-12 rounded-full bg-white/10 border-2 border-dashed border-white/30 overflow-hidden">
                <img
                  src={Logo}
                  alt="Jerne IF"
                  className="w-full h-full object-cover"
                />
              </div>
              <div>
                <div className="font-bold text-lg">Dead Pigeons</div>
                <div className="text-xs text-gray-400">JERNE IF</div>
              </div>
            </div>

            {/* Navigation */}
            <nav className="space-y-1 flex-1">
              <p className="text-xs uppercase text-gray-500 font-medium mb-3">
                My Games
              </p>
              {navItems.map((item) => (
                <Link
                  key={item.to}
                  to={item.to}
                  onClick={() => setSidebarOpen(false)}
                  className="flex items-center gap-3 px-4 py-3 rounded-lg hover:bg-gray-700 transition text-white"
                >
                  <item.icon className="w-5 h-5" />
                  {item.label}
                </Link>
              ))}
            </nav>

            {/* User Info */}
            <div className="mt-auto space-y-6">
              <div className="flex items-center gap-3 p-4 bg-gray-700/50 rounded-xl">
                <div className="w-10 h-10 rounded-full bg-gray-600 flex items-center justify-center text-sm font-bold">
                  {initials}
                </div>

                <div className="flex-1 min-w-0">
                  <div className="font-medium truncate">
                    {user?.name ?? "Loading..."}
                  </div>
                  <div className="text-xs text-gray-400 truncate">
                    {user?.email ?? ""}
                  </div>
                </div>

                <button className="p-2 hover:bg-gray-600 rounded-lg transition">
                  <Settings className="w-4 h-4" />
                </button>
              </div>

              <div className="pt-4 border-t border-gray-700">
                <button
                  onClick={() => navigate("/login")}
                  className="w-full flex items-center gap-3 px-4 py-3 rounded-lg hover:bg-gray-700 transition text-left"
                >
                  <LogOut className="w-5 h-5" />
                  Sign Out
                </button>
              </div>
            </div>
          </div>
        </aside>

        {/* Main Content Area */}
        <main className="flex-1 p-4 lg:p-8 overflow-auto">
          <div className="max-w-6xl mx-auto">
            <div className="flex justify-end gap-2 mb-6">
              <button className="p-3 bg-gray-700 rounded-xl hover:bg-gray-600 transition">
                <Moon className="w-5 h-5 text-gray-300" />
              </button>
              <button className="p-3 bg-gray-700 rounded-xl hover:bg-gray-600 transition">
                <Languages className="w-5 h-5 text-gray-300" />
              </button>
            </div>

            <Outlet />
          </div>
        </main>
      </div>

      {sidebarOpen && (
        <div
          className="lg:hidden fixed inset-0 bg-black/50 z-40"
          onClick={() => setSidebarOpen(false)}
        />
      )}
    </div>
  );
}
