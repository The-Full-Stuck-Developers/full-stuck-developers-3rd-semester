import React, {type JSX, useEffect, useRef, useState} from "react";
import {Outlet, useLocation, useNavigate} from "react-router-dom";
import Logo from "../jerneif-logo.png";
import {Backpack, ChevronsDown, Dice5, Menu, Users} from "lucide-react";
import { Link } from "react-router-dom";

interface MenuItem {
    label: string;
    path: string;
    icon: JSX.Element;
}

const tempUser = {
    fullName: "Jan Kowalski",
    balanceCents: 124500,
};

export const SidebarLayout: React.FC = () => {
    const checkboxRef = useRef<HTMLInputElement>(null);
    const navigate = useNavigate();
    const location = useLocation();
    const [open, setOpen] = useState(true);
    const balanceDKK = (tempUser.balanceCents / 100).toFixed(2);
    const [userMenu, setUserMenu] = useState(false);
    const menuRef = useRef<HTMLDivElement>(null);

    const handleLogout = () => {
        // Implement later token/localStorage etc
        navigate("/login");
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
            path: "",
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
        if (checkboxRef.current) {
            checkboxRef.current.checked = false;
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
                <input id="my-drawer" type="checkbox" className="drawer-toggle"/>
                <div className="">
                    {/*Top nav*/}
                    <nav
                        className="navbar w-screen top-0 left-0 right-0 z-50 bg-[#0f2b5b]/95 backdrop-blur-lg border-b border-white/10">
                        <div className="w-full px-6 py-5 flex flex-row items-center justify-between">
                            <label htmlFor="my-drawer" className="btn btn-square">
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
                                    className="flex flex-row items-center justify-evenly px-6 py-3 rounded-full bg-[#e30613] hover:bg-[#c20510] text-white font-bold shadow-lg hover:shadow-xl transition-all duration-300 cursor-pointer"
                                >
                                    <ChevronsDown size={24}/>
                                    {tempUser.fullName}
                                </button>

                                {userMenu && (
                                    <div
                                        className="absolute right-0 mt-2 w-48 bg-white rounded-lg shadow-lg overflow-hidden z-50 opacity-0 scale-95 animate-[fadeInScale_200ms_ease-out_forwards]"
                                        style={{
                                            animation: 'fadeInScale 200ms ease-out forwards',
                                            transformOrigin: 'top right'
                                        }}
                                    >
                                        <Link to="/" className={"w-full block text-left px-4 py-2 text-gray-700 hover:bg-gray-100 transition-colors cursor-pointer"}>
                                            My profile
                                        </Link>
                                        <button
                                            onClick={() => {
                                                handleLogout();
                                                setUserMenu(false);
                                            }}
                                            className="w-full text-left px-4 py-2 text-gray-700 hover:bg-gray-100 transition-colors cursor-pointer"
                                        >
                                            Logout
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
                <div className="drawer-side">
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