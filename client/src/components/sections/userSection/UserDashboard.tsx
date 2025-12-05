import { Link, useNavigate } from "react-router-dom";
import Logo from "../../../jerneif-logo.png";
import { useAuth } from "../../../hooks/auth";
import { useEffect, useState } from "react";

export default function UserDashboard() {
    const navigate = useNavigate();
    const { user, logout } = useAuth();
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (user) {
            setLoading(false);
        }
    } , [user]);

    if (loading || !user) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                                <div className="text-2xl">Loading...</div>
            </div>
        );
    }
    const balanceDKK = (user.balance / 100).toFixed(2);
    const userName = user.name;

    const handleLogout = () => {
        navigate("/");
    };

    return (
        <>
            <nav className="fixed top-0 left-0 right-0 z-50 bg-[#0f2b5b]/95 backdrop-blur-lg border-b border-white/10">
                <div className="max-w-7xl mx-auto px-6 py-5">
                    <div className="flex items-center justify-between">
                        <Link to="/user/dashboard" className="flex items-center gap-4 group">
                            <div className="w-12 h-12 rounded-full bg-white/10 border-2 border-dashed border-white/30 flex items-center justify-center overflow-hidden">
                                <img
                                    src={Logo}
                                    alt="Jerne IF Logo"
                                    className="w-full h-full object-cover"
                                    onError={(e) => { e.currentTarget.style.display = "none"; }}
                                />
                            </div>
                            <span className="text-2xl font-black text-white tracking-tight">
                Jerne IF
              </span>
                        </Link>
                        <div className="flex items-center gap-6">
                            <div className="text-right">
                                <p className="text-lg font-bold text-white">{user.name}</p>
                            </div>

                            <button
                                onClick={handleLogout}
                                className="px-6 py-3 rounded-full bg-[#e30613] hover:bg-[#c20510] text-white font-bold shadow-lg hover:shadow-xl transition-all duration-300"
                            >
                                Logout
                            </button>
                        </div>
                    </div>
                </div>
            </nav>

            <div className="min-h-screen bg-gray-50 pt-32 pb-20 px-4">
                <div className="max-w-5xl mx-auto">

                    <div className="mb-10 text-center">
                        <h1 className="text-5xl font-bold text-gray-900">
                            Hello, {userName}!
                        </h1>
                        <p className="text-xl text-gray-600 mt-3">
                            Ready for a next week with Dead Pigeons?
                        </p>
                    </div>


                    <div className="bg-gradient-to-r from-[#ed1c24] to-white rounded-2xl p-10 text-white shadow-2xl mb-12">
                        <h2 className="text-3xl font-semibold mb-4">Balance</h2>
                        <div className="text-7xl font-extrabold tracking-tight">
                            {balanceDKK}
                            <span className="text-4xl font-normal ml-3">DKK</span>
                        </div>

                        <Link
                            to="/user/deposit"
                            className="inline-flex items-center mt-10 bg-white text-emerald-600 font-bold py-5 px-10 rounded-xl hover:bg-gray-100 transition shadow-lg text-lg"
                        >
                            <svg className="w-7 h-7 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M12 4v16m8-8H4" />
                            </svg>
                            Deposit Money
                        </Link>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                        <div
                            onClick={() => navigate("/game/current")}
                            className="bg-white rounded-2xl p-10 shadow-xl hover:shadow-2xl hover:border-4 hover:border-emerald-500 transition-all cursor-pointer text-center"
                        >
                            <div className="w-20 h-20 bg-emerald-100 rounded-2xl flex items-center justify-center mb-6 mx-auto">
                                <svg className="w-12 h-12 text-emerald-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M12 4v16m8-8H4" />
                                </svg>
                            </div>
                            <h3 className="text-3xl font-bold text-gray-900 mb-3">Play Now</h3>
                            <p className="text-gray-600 text-lg">Choose numbers for existing week</p>
                        </div>

                        <Link
                            to="/user/my-boards"
                            className="bg-white rounded-2xl p-10 shadow-xl hover:shadow-2xl hover:border-4 hover:border-blue-500 transition-all block text-center"
                        >
                            <div className="w-20 h-20 bg-blue-100 rounded-2xl flex items-center justify-center mb-6 mx-auto">
                                <svg className="w-12 h-12 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                                </svg>
                            </div>
                            <h3 className="text-3xl font-bold text-gray-900 mb-3">My Coupons</h3>
                            <p className="text-gray-600 text-lg">All coupons and scores</p>
                        </Link>

                        <Link
                            to="/user/deposit"
                            className="bg-white rounded-2xl p-10 shadow-xl hover:shadow-2xl hover:border-4 hover:border-purple-500 transition-all block text-center"
                        >
                            <div className="w-20 h-20 bg-purple-100 rounded-2xl flex items-center justify-center mb-6 mx-auto">
                                <svg className="w-12 h-12 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1" />
                                </svg>
                            </div>
                            <h3 className="text-3xl font-bold text-gray-900 mb-3">Deposits and history</h3>
                            <p className="text-gray-600 text-lg">MobilePay – status and history</p>
                        </Link>
                    </div>
                </div>
            </div>
        </>
    );
}