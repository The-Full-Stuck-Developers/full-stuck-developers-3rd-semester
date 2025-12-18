import { useState } from "react";
import { Link, Outlet, useLocation } from "react-router-dom";
import { formatDkPhone } from "@utilities/formatDkPhone.ts";
import {
  Menu,
  X,
  BarChart3,
  Gamepad2,
  DollarSign,
  TrendingUp,
  LogOut,
  Languages,
} from "lucide-react";
import Logo from "../../../jerneif-logo.png";
import { useAuth } from "../../../hooks/auth.tsx";
import { Toaster } from "react-hot-toast";
import { useTranslation } from "react-i18next";
import { Settings as SettingsComponent } from "./Settings.tsx";

export function PlayerLayout() {
  const { user, logout } = useAuth();
  const { t } = useTranslation("player");
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [settingsOpen, setSettingsOpen] = useState(false);
  const location = useLocation();

  const navItems = [
    { to: "/player/dashboard", icon: BarChart3, label: t("overview") },
    { to: "/player/boards", icon: Gamepad2, label: t("my_boards_nav") },
    { to: "/player/deposit", icon: DollarSign, label: t("add_balance_nav") },
    { to: "/player/history", icon: TrendingUp, label: t("game_history_nav") },
  ];
  const isActive = (path: string) => location.pathname === path;

  const initials = user?.name
    ? user.name
        .split(" ")
        .filter(Boolean)
        .slice(0, 2)
        .map((p) => p[0]!.toUpperCase())
        .join("")
    : "U";

  return (
    <div className="min-h-screen bg-gray-900 text-white flex">
      <Toaster
        position="top-center"
        toastOptions={{ style: { background: "#1f2937", color: "#fff" } }}
      />
      {/* Mobile  */}
      {sidebarOpen && (
        <div
          className="fixed inset-0 bg-black/70 z-40 lg:hidden"
          onClick={() => setSidebarOpen(false)}
        />
      )}

      {/* Sidebar */}
      <aside
        className={`fixed lg:static inset-y-0 left-0 z-50 w-72 bg-gray-800 border-r border-gray-700 transform transition-transform duration-300 ease-in-out ${
          sidebarOpen ? "translate-x-0" : "-translate-x-full lg:translate-x-0"
        }`}
      >
        <div className="flex flex-col h-full">
          {/* Logo + Mobile Close */}
          <div className="p-6 flex items-center justify-between border-b border-gray-700">
            <div className="flex items-center gap-3">
              <div className="w-12 h-12 rounded-full bg-white/10 border-2 border-dashed border-white/30 overflow-hidden">
                <img
                  src={Logo}
                  alt="Jerne IF"
                  className="w-full h-full object-cover"
                />
              </div>
              <div>
                <div className="font-black text-lg">Dead Pigeons</div>
                <div className="text-xs text-gray-400">JERNE IF</div>
              </div>
            </div>
            <button
              onClick={() => setSidebarOpen(false)}
              className="lg:hidden p-2 hover:bg-gray-700 rounded-lg"
            >
              <X className="w-6 h-6" />
            </button>
          </div>

          <nav className="flex-1 p-4 space-y-1">
            {navItems.map((item) => (
              <Link
                key={item.to}
                to={item.to}
                onClick={() => setSidebarOpen(false)}
                className={`flex items-center gap-4 px-4 py-3.5 rounded-xl transition-all font-medium ${
                  isActive(item.to)
                    ? "bg-red-900/30 text-red-400 shadow-lg shadow-red-900/20"
                    : "hover:bg-gray-700 text-gray-300"
                }`}
              >
                <item.icon className="w-5 h-5" />
                {item.label}
              </Link>
            ))}
          </nav>

          {/* User Profile */}
          <div className="p-6 border-t border-gray-700">
            <div className="flex items-center gap-4 mb-4">
              <div className="w-12 h-12 bg-gray-600 rounded-full flex items-center justify-center font-bold text-lg">
                {initials}
              </div>
              <div className="min-w-0">
                <div className="font-semibold truncate">
                  {user?.name ?? "Loading..."}
                </div>
                <div className="text-xs text-gray-400 truncate">
                  {/* use whichever field exists on AuthUserInfo */}
                  {formatDkPhone(user?.phoneNumber)}
                </div>
              </div>
            </div>

            <div className="space-y-2">
              <button
                onClick={() => setSettingsOpen(true)}
                className="w-full flex items-center gap-3 px-4 py-3 rounded-xl hover:bg-gray-700 transition text-left text-sm"
                title={t("language")}
              >
                <Languages className="w-4 h-4" />
                {t("language")}
              </button>
              <button
                onClick={logout}
                className="w-full flex items-center gap-3 px-4 py-3 rounded-xl hover:bg-gray-700 transition text-left text-sm text-red-400"
              >
                <LogOut className="w-4 h-4" />
                {t("sign_out")}
              </button>
            </div>
          </div>
        </div>
      </aside>

      {/* Main Content Area */}
      <div className="flex-1 flex flex-col">
        {/* Mobile Header */}
        <header className="lg:hidden bg-gray-800 border-b border-gray-700 p-4 flex items-center justify-between">
          <button
            onClick={() => setSidebarOpen(true)}
            className="p-3 bg-gray-700 rounded-xl hover:bg-gray-600 transition"
          >
            <Menu className="w-6 h-6" />
          </button>
          <div className="font-black text-xl">Dead Pigeons</div>
          <div className="w-10" />
        </header>

        {/* Page Content */}
        <main className="flex-1 p-6 lg:p-10 overflow-y-auto">
          <div className="max-w-7xl mx-auto w-full">
            <Outlet />
          </div>
        </main>
      </div>

      <SettingsComponent
        isOpen={settingsOpen}
        onClose={() => setSettingsOpen(false)}
      />
    </div>
  );
}
