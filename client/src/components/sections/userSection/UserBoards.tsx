import { Link, useNavigate } from "react-router-dom";
import Logo from "../../../jerneif-logo.png";

export function UserBoards() {
    const navigate = useNavigate();

    return (
        <>
            <nav className="fixed top-0 left-0 right-0 z-50 bg-[#0f2b5b]/95 backdrop-blur-lg border-b border-white/10">
                <div className="max-w-7xl mx-auto px-6 py-5 flex items-center justify-between">
                    <Link to="/user/dashboard" className="flex items-center gap-4">
                        <div className="w-12 h-12 rounded-full bg-white/10 border-2 border-dashed border-white/30 overflow-hidden">
                            <img src={Logo} alt="Jerne IF" className="w-full h-full object-cover" />
                        </div>
                        <span className="text-2xl font-black text-white tracking-tight">Jerne IF</span>
                    </Link>
                    <button onClick={() => navigate("/login")} className="px-6 py-3 rounded-full bg-[#e30613] hover:bg-[#c20510] text-white font-bold shadow-lg transition">
                        Logout
                    </button>
                </div>
            </nav>

            <div className="min-h-screen bg-gray-50 pt-32 pb-20 px-4">
                <div className="max-w-5xl mx-auto text-center">
                    <h1 className="text-6xl font-black text-[#0f2b5b] mb-6">My Boards</h1>
                    <p className="text-2xl text-gray-600 mb-12">Your tickets and results will appear here</p>
                    <div className="bg-white/80 backdrop-blur rounded-3xl p-20 border-4 border-dashed border-[#0f2b5b]/20">
                        <p className="text-3xl font-bold text-[#0f2b5b]">No boards yet</p>
                        <Link to="/game/current" className="mt-8 inline-block px-10 py-5 bg-[#e30613] hover:bg-[#c20510] text-white font-black rounded-full text-xl shadow-xl transition">
                            Play Now
                        </Link>
                    </div>
                </div>
            </div>
        </>
    );
}