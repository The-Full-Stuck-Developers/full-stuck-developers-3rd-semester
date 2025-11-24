import React, {type JSX, useRef, useState} from "react";
import {Outlet, useNavigate, useLocation} from "react-router-dom";
import GroupIcon from '@mui/icons-material/Group';
import KeyboardDoubleArrowLeftIcon from '@mui/icons-material/KeyboardDoubleArrowLeft';

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

    const menuItems: MenuItem[] = [
        {
            label: "Users",
            path: "/admin/users",
            icon: (
                <GroupIcon/>
            ),
        },
        {
            label: "Games",
            path: "",
            icon: (
                <GroupIcon/>
            ),
        },
        {
            label: "Something else",
            path: "",
            icon: (
                <GroupIcon/>
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

    return (
        <div className="drawer lg:drawer-open">
            <input
                ref={checkboxRef}
                id="my-drawer-4"
                type="checkbox"
                className="drawer-toggle"
                defaultChecked={true}
            />
            <div className="drawer-content flex flex-col">
                {/* Navbar */}
                <nav className="navbar w-full bg-base-200">
                    <button
                        onClick={handleToggleDrawer}
                        aria-label="open sidebar"
                        className="btn btn-square btn-ghost"
                    >
                        <span
                            className={`transition-transform duration-300 ${
                                open ? "rotate-0" : "rotate-180"
                            }`}
                        >
                            <KeyboardDoubleArrowLeftIcon/>
                        </span>
                    </button>
                </nav>
                {/* Page content */}
                <div className="p-4 flex-1 overflow-y-auto">
                    <Outlet/>
                </div>
            </div>

            {/* Sidebar Container */}
            <div className="drawer-side z-50">
                <label
                    onClick={handleCloseDrawer}
                    htmlFor="my-drawer-4"
                    aria-label="close sidebar"
                    className="drawer-overlay"
                ></label>

                {/* Sidebar Content */}
                <ul className="menu min-h-full w-80 bg-base-200 m-0 p-0">
                    <div className={"ps-3 pt-5 mb-3"}>
                        <span className={"text-2xl text-center"}>Admin panel</span>
                    </div>
                    {menuItems.map((item) => (
                        <li key={item.path}>
                            <button
                                onClick={() => handleMenuClick(item.path)}
                                className={`py-5 ${isActive(item.path) ? "bg-base-300 text-primary" : "hover:bg-base-200"}
`}                            >
                                {item.icon}
                                <span>{item.label}</span>
                            </button>
                        </li>
                    ))}
                </ul>
            </div>
        </div>
    );
};