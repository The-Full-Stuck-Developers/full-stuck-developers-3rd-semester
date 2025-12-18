import React, { type JSX, useEffect, useRef, useState } from "react";
import { Link, Outlet, useLocation, useNavigate } from "react-router-dom";
import Logo from "../jerneif-logo.png";
import {
  ChevronsDown,
  GamepadIcon,
  LayoutDashboard,
  Users,
  Wallet,
} from "lucide-react";
import { ThemeToggle } from "@components/ThemeToggle.tsx";
import { useTranslation } from "react-i18next";
import { useAuth } from "../hooks/auth.tsx";
import { useAuthInfo } from "../hooks/authInfo.tsx";

interface MenuItem {
  label: string;
  path: string;
  icon: JSX.Element;
}

export const SidebarLayout: React.FC = () => {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const location = useLocation();
  const [userMenu, setUserMenu] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);
  const { i18n } = useTranslation();
  const [lang, setLang] = useState<"en" | "dk">(
    i18n.language.startsWith("dk") ? "dk" : "en",
  );
  const { logout } = useAuth();
  const { user } = useAuthInfo();

  // load saved language
  useEffect(() => {
    const savedLang = localStorage.getItem("language") as "en" | "dk" | null;
    if (savedLang && savedLang !== lang) {
      setLang(savedLang);
      i18n.changeLanguage(savedLang);
    }
  }, []);

  const changeLanguage = (lng: "en" | "dk") => {
    setLang(lng);
    i18n.changeLanguage(lng);
    localStorage.setItem("language", lng);
  };

  const handleLogout = () => logout();

  const menuItems: MenuItem[] = [
    {
      label: t("dashboard"),
      path: "/admin/dashboard",
      icon: <LayoutDashboard size={26} />,
    },
    {
      label: t("users"),
      path: "/admin/users",
      icon: <Users size={26} />,
    },
    {
      label: t("upcoming_games"),
      path: "/admin/upcoming-games",
      icon: <GamepadIcon size={26} />,
    },
    {
      label: t("past_games"),
      path: "/admin/past-games",
      icon: <GamepadIcon size={26} />,
    },
    {
      label: t("transactions"),
      path: "/admin/transactions",
      icon: <Wallet size={26} />,
    },
  ];

  const isActive = (path: string) => location.pathname === path;

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
        setUserMenu(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  if (!user) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-2xl">Loading...</div>
      </div>
    );
  }

  return (
    <div className="w-full h-screen overflow-hidden bg-gray-900">
      {/*Header*/}
      <nav className="navbar fixed top-0 left-0 right-0 z-50 bg-gray-800 backdrop-blur-lg h-[72px] flex items-center">
        <div className="w-full px-6 flex flex-row items-center justify-between">
          {/* Logo and title */}
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 rounded-full bg-white/10 border-2 border-dashed border-white/30 flex items-center justify-center overflow-hidden">
              <img
                src={Logo}
                alt="Jerne IF Logo"
                className="w-full h-full object-cover"
                onError={(e) => (e.currentTarget.style.display = "none")}
              />
            </div>
            <span className="text-2xl font-black text-white tracking-tight">
              Jerne IF
            </span>
          </div>

          {/* User Menu with language and theme */}
          <div ref={menuRef} className="relative z-50">
            <button
              onClick={() => setUserMenu(!userMenu)}
              className="flex flex-row items-center px-3 py-1.5 rounded-lg bg-[#e30613] hover:bg-[#c20510] text-white font-bold shadow-lg transition cursor-pointer"
            >
              <ChevronsDown size={24} />
              {user.name}
            </button>

            {userMenu && (
              <div
                className="absolute right-0 mt-2 w-40 bg-gray-800 rounded-lg shadow-lg overflow-hidden animate-[fadeInScale_200ms_ease-out_forwards] border border-gray-500"
                style={{ transformOrigin: "top right" }}
              >
                <div className="px-4 space-y-3 py-3 border-b border-gray-400">
                  <ThemeToggle />

                  {/* Language switch */}
                  <div className="relative bg-gray-900 rounded-full flex items-center px-1 py-1 gap-2 w-24 h-9 mx-auto">
                    <div
                      className={`absolute h-7 w-10 bg-gray-700 rounded-full shadow transition-all duration-300 ${
                        lang === "en"
                          ? "left-1"
                          : "left-[calc(100%-2.5rem-0.25rem)]"
                      }`}
                    />
                    <button
                      onClick={() => changeLanguage("en")}
                      className={`z-10 mx-auto transition ${
                        lang === "en"
                          ? "opacity-100"
                          : "opacity-50 cursor-pointer"
                      }`}
                    >
                      <span className="fi fi-gb text-xl rounded-full mt-1"></span>
                    </button>
                    <button
                      onClick={() => changeLanguage("dk")}
                      className={`z-10 mx-auto transition ${
                        lang === "dk"
                          ? "opacity-100"
                          : "opacity-50 cursor-pointer"
                      }`}
                    >
                      <span className="fi fi-dk text-xl rounded-full mt-1"></span>
                    </button>
                  </div>
                </div>

                {/*<Link*/}
                {/*  to="/admin/my-profile"*/}
                {/*  className="block px-4 py-2 text-white hover:bg-gray-700"*/}
                {/*>*/}
                {/*  {t("my_profile")}*/}
                {/*</Link>*/}

                <button
                  onClick={() => {
                    handleLogout();
                    setUserMenu(false);
                  }}
                  className="w-full text-left px-4 py-2 text-white hover:bg-gray-700 cursor-pointer"
                >
                  {t("logout")}
                </button>
              </div>
            )}
          </div>
        </div>
      </nav>

      {/* SIDEBAR */}
      <div className="fixed top-[72px] left-0 h-[calc(100vh-72px)] w-64 bg-gray-800 overflow-y-auto z-40">
        <ul className="menu p-0 m-0 w-full">
          {menuItems.map((item, index) => (
            <li key={`${item.label}-${index}`}>
              <button
                onClick={() => navigate(item.path)}
                className={`flex items-center gap-3 px-6 py-3 w-full text-left font-medium ${
                  isActive(item.path)
                    ? "bg-red-900/20 text-white"
                    : "hover:bg-gray-700 text-white"
                }`}
              >
                {item.icon}
                <span>{item.label}</span>
              </button>
            </li>
          ))}
        </ul>
      </div>

      {/* MAIN CONTENT */}
      <div className="ml-64 mt-[72px] h-[calc(100vh-72px)] overflow-y-auto p-14">
        <Outlet />
      </div>
    </div>
  );
};
