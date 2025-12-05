import React, {type JSX, useEffect, useRef, useState} from "react";
import {Link, Outlet, useLocation, useNavigate} from "react-router-dom";
import Logo from "../jerneif-logo.png";
import {Backpack, ChevronsDown, Dice5, Menu, Users} from "lucide-react";
import {ThemeToggle} from "@components/ThemeToggle.tsx";
import {useTranslation} from "react-i18next";
import {t} from "i18next";
import {useAuth} from "../hooks/auth.tsx";

interface MenuItem {
    label: string;
    path: string;
    icon: JSX.Element;
}

export const SidebarLayout: React.FC = () => {
    const checkboxRef = useRef<HTMLInputElement>(null);
    const navigate = useNavigate();
    const location = useLocation();
    const [open, setOpen] = useState(true);
    const [userMenu, setUserMenu] = useState(false);
    const menuRef = useRef<HTMLDivElement>(null);
    const {i18n} = useTranslation();
    const [lang, setLang] = useState<"en" | "dk">(i18n.language.startsWith("dk") ? "dk" : "en");
    const { user, logout } = useAuth();

    // On mount, check localStorage for saved language
    useEffect(() => {
        const savedLang = localStorage.getItem("language") as "en" | "dk" | null;
        if (savedLang && savedLang !== lang) {
            setLang(savedLang);
            i18n.changeLanguage(savedLang);
        }
    }, [i18n, lang]);

    const changeLanguage = (lng: "en" | "dk") => {
        setLang(lng);
        i18n.changeLanguage(lng);
        localStorage.setItem("language", lng);
    };

    const handleLogout = () => {
        logout();
    };

    const menuItems: MenuItem[] = [
        {
            label: "Users",
            path: "/admin/users",
            icon: (
                <Users size={26}/>
            ),
        },
        {
            label: "Games",
            path: "/admin/games",
            icon: (
                <Dice5 size={26}/>
            ),
        },
        {
            label: "Something else",
            path: "",
            icon: (
                <Backpack size={26}/>
            ),
        },

    ];

    const handleToggleDrawer = () => {
        if (checkboxRef.current) {
            checkboxRef.current.checked = !checkboxRef.current.checked;
            setOpen(checkboxRef.current.checked);
        }
    };

    const handleCloseDrawer = () => {
        setUserMenu(false);

        if (checkboxRef.current) {
            checkboxRef.current.checked = false;
            setOpen(false);
        }
    };

    const handleMenuClick = (path: string) => {
        navigate(path);
        handleCloseDrawer();
    };

    const isActive = (path: string) => {
        return location.pathname === path;
    };

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
                setUserMenu(false);
            }
        };

        document.addEventListener("mousedown", handleClickOutside);
        return () => document.removeEventListener("mousedown", handleClickOutside);
    }, []);

    // Show loading if user not loaded yet
    if (!user) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="text-2xl">Loading...</div>
            </div>
        );
    }

    return (
        <div className={" overflow-x-hidden"}>
            <style>{`
                @keyframes fadeInScale {
                    from {
                        opacity: 0;
                        transform: scale(0.95);
                    }
                    to {
                        opacity: 1;
                        transform: scale(1);
                    }
                }
            `}</style>
            <div className="drawer">
                <input id="my-drawer" ref={checkboxRef} type="checkbox" className="drawer-toggle"/>

                <div className="">
                    {/*Top nav*/}
                    <nav
                        className="navbar w-screen top-0 left-0 right-0 z-50 bg-[#0f2b5b]/95 backdrop-blur-lg border-b border-white/10">
                        <div className="w-full px-6 py-5 flex flex-row items-center justify-between">
                            <label htmlFor="my-drawer" className="btn btn-square rounded-lg">
                                    <span
                                        className={`transition-transform duration-300 ${
                                            open ? "rotate-0" : "rotate-180"
                                        }`}
                                    >
                                        <Menu size={28}/>
                                    </span>
                            </label>

                            <div className="flex items-center gap-4 group">
                                <div
                                    className="w-12 h-12 rounded-full bg-white/10 border-2 border-dashed border-white/30 flex items-center justify-center overflow-hidden">
                                    <img
                                        src={Logo}
                                        alt="Jerne IF Logo"
                                        className="w-full h-full object-cover"
                                        onError={(e) => {
                                            e.currentTarget.style.display = "none";
                                        }}
                                    />
                                </div>
                                <span className="text-2xl font-black text-white tracking-tight">
                                        Jerne IF
                                    </span>
                            </div>
                            <div ref={menuRef} className="relative z-50">
                                <button
                                    onClick={() => setUserMenu(!userMenu)}
                                    className="flex flex-row items-center justify-evenly px-4 py-2 rounded-lg bg-[#e30613] hover:bg-[#c20510] text-white font-bold shadow-lg hover:shadow-xl transition-all duration-300 cursor-pointer"
                                >
                                    <ChevronsDown size={24}/>
                                    {user.name}
                                </button>

                                {userMenu && (
                                    <div
                                        className="absolute right-0 mt-2 w-40 bg-white rounded-lg shadow-lg overflow-hidden z-50 opacity-0 scale-95 animate-[fadeInScale_200ms_ease-out_forwards]"
                                        style={{
                                            animation: 'fadeInScale 200ms ease-out forwards',
                                            transformOrigin: 'top right'
                                        }}
                                    >
                                        <div className={"px-4 space-y-3 py-3 border-b"}>

                                            <ThemeToggle/>
                                            <div
                                                className="relative bg-gray-200 rounded-full flex items-center px-1 py-1 gap-2 w-24 h-9 mx-auto">
                                                <div
                                                    className={`absolute h-7 w-10 bg-white rounded-full shadow transition-all duration-300 ${
                                                        lang === "en" ? "left-1" : "left-[calc(100%-2.5rem-0.25rem)]"
                                                    }`}
                                                />
                                                <button
                                                    onClick={() => changeLanguage("en")}
                                                    className={`z-10 mx-auto transition ${
                                                        lang === "en" ? "opacity-100" : "opacity-50 cursor-pointer"
                                                    }`}
                                                >
                                                    <span className="fi fi-gb text-xl rounded-full mt-1"></span>
                                                </button>
                                                <button
                                                    onClick={() => changeLanguage("dk")}
                                                    className={`z-10 mx-auto transition ${
                                                        lang === "dk" ? "opacity-100" : "opacity-50 cursor-pointer"
                                                    }`}
                                                >
                                                    <span className="fi fi-dk text-xl rounded-full mt-1"></span>
                                                </button>
                                            </div>
                                        </div>
                                        <Link to="/my-profile"
                                              className={"w-full block text-left px-4 py-2 text-gray-700 hover:bg-gray-100 transition-colors cursor-pointer"}>
                                            {t("my_profile")}
                                        </Link>
                                        <button
                                            onClick={() => {
                                                handleLogout();
                                                setUserMenu(false);
                                            }}
                                            className="w-full text-left px-4 py-2 text-gray-700 hover:bg-gray-100 transition-colors cursor-pointer"
                                        >
                                            {t("logout")}
                                        </button>
                                    </div>
                                )}
                            </div>
                        </div>
                    </nav>

                    <div className={"p-14 mx-0"}>
                        <Outlet/>
                    </div>
                </div>

                {/*Side Drawer*/}
                <div className="drawer-side z-50">
                    <label htmlFor="my-drawer" aria-label="close sidebar" className="drawer-overlay"></label>
                    <ul className="menu min-h-full w-80 bg-base-200 m-0 p-0">
                        <div className={"ps-3 pt-5 mb-3"}>
                            <span className={"text-2xl text-center"}>Admin panel</span>
                        </div>
                        {menuItems.map((item, index) => (
                            <li key={`${item.label}-${index}`}>
                                <button
                                    onClick={() => handleMenuClick(item.path)}
                                    className={`py-5 ${isActive(item.path) ? "bg-[#0f2b5b] text-white" : "hover:bg-[#576a8c] hover:text-white"}`}>
                                    {item.icon}
                                    <span>{item.label}</span>
                                </button>
                            </li>
                        ))}
                    </ul>
                </div>
            </div>
        </div>
    );
};