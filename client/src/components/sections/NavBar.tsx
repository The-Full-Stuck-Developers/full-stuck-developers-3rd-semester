import {Link, useNavigate} from "react-router-dom";
import {MessageCircle} from "lucide-react";
import Logo from "../../jerneif-logo.png";
import {useTranslation} from "react-i18next";
import React, {useEffect, useState} from "react";
import i18n from "../../i18n.ts";
import {useAuthInfo} from "../../hooks/authInfo.tsx";

interface NavbarProps {
    onLoginClick: () => void;
}

export function Navbar({onLoginClick}: NavbarProps) {
    const {t} = useTranslation();
    const {user} = useAuthInfo();
    console.log(user);
    const navigate = useNavigate();
    const [lang, setLang] = useState<"en" | "dk">(
        i18n.language.startsWith("dk") ? "dk" : "en",
    );

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
    return (
        <nav className="fixed top-0 left-0 right-0 z-50 bg-[#0f2b5b]/95 backdrop-blur-lg border-b border-white/10">
            <div className="max-w-7xl mx-auto px-6 py-5">
                <div className="flex items-center justify-between">
                    <Link to="/" className="flex items-center gap-4 group">
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
                            <span className="text-2xl hidden">Dead Pigeons</span>
                        </div>

                        <span className="text-2xl font-black text-white tracking-tight">
              Jerne IF
            </span>
                    </Link>

                    <div className="hidden lg:flex items-center gap-10 text-white font-semibold text-lg">
                        <a href="#about" className="hover:text-[#e30613] transition">
                            {t('home:about')}
                        </a>
                        <a href="#pricing" className="hover:text-[#e30613] transition">
                            {t('home:pricing')}
                        </a>
                        <a href="#contact" className="hover:text-[#e30613] transition">
                            {t('home:contact')}
                        </a>
                    </div>

                    <div className="flex items-center gap-4">
                        {!user ? (

                            <button
                                onClick={onLoginClick}
                                className="px-6 py-3 rounded-full border-2 border-white/30 text-white font-semibold hover:bg-white/10 hover:border-white transition-all"
                            >
                                {t('home:login')}
                            </button>
                        ) : (
                            <button
                                onClick={() => user.isAdmin ? navigate('/admin/dashboard') : navigate('/player/dashboard')}
                                className="px-6 py-3 rounded-full border-2 border-white/30 text-white font-semibold hover:bg-white/10 hover:border-white transition-all"
                            >
                                {t('home:dashboard')}
                            </button>

                        )}

                        <div
                            className="relative rounded-full flex items-center px-2 py-2 gap-2 w-28 h-13 mx-auto border-2 border-white/30 hover:bg-white/10 hover:border-white transition-all m-0">
                            <div
                                className={`absolute h-8 w-11 bg-white/20 rounded-full shadow-lg transition-all duration-300 ${
                                    lang === "en"
                                        ? "left-2"
                                        : "left-[calc(100%-2.75rem-0.5rem)]"
                                }`}
                            />
                            <button
                                onClick={() => changeLanguage("en")}
                                className={`z-10 mx-auto transition-opacity ${
                                    lang === "en"
                                        ? "opacity-100"
                                        : "opacity-50 cursor-pointer"
                                }`}
                            >
                                <span className="fi fi-gb text-xl rounded-full mt-1"></span>
                            </button>
                            <button
                                onClick={() => changeLanguage("dk")}
                                className={`z-10 mx-auto transition-opacity ${
                                    lang === "dk"
                                        ? "opacity-100"
                                        : "opacity-50 cursor-pointer"
                                }`}
                            >
                                <span className="fi fi-dk text-xl rounded-full mt-1"></span>
                            </button>
                        </div>


                        {/*<a*/}
                        {/*  href="#contact"*/}
                        {/*  className="flex items-center gap-2 px-6 py-3 rounded-full bg-[#e30613] hover:bg-[#c20510] text-white font-bold shadow-lg hover:shadow-xl transition-all duration-300"*/}
                        {/*>*/}
                        {/*  <MessageCircle size={20} />*/}
                        {/*  <span className="hidden sm:inline">Contact Admin</span>*/}
                        {/*  <span className="sm:hidden">Chat</span>*/}
                        {/*</a>*/}
                    </div>
                </div>
            </div>
        </nav>
    );
}
