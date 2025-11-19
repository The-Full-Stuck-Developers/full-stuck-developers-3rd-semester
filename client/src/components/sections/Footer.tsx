import { Trophy, Clock, Dice5, Target } from "lucide-react";

export function Footer() {
    return (
        <footer id="contact" className="w-full bg-[#1a1f25] text-white py-20">
            <div className="max-w-6xl mx-auto px-6">

                <div className="grid grid-cols-1 md:grid-cols-3 gap-16 mb-16">

                    <div className="space-y-4">
                        <div className="flex items-center gap-3 text-xl font-semibold">
                            <span className="text-3xl">🕊️</span>
                            Dead Pigeons
                        </div>

                        <p className="text-lg text-gray-400 leading-relaxed">
                            Supporting Jerne IF through community lottery gaming
                        </p>
                    </div>

                    <div className="space-y-4">
                        <h4 className="text-2xl font-semibold">Quick Links</h4>
                        <ul className="space-y-3 text-lg text-gray-300">
                            <li className="hover:text-white cursor-pointer">How to Play</li>
                            <li className="hover:text-white cursor-pointer">Game Rules</li>
                            <li className="hover:text-white cursor-pointer">Pricing</li>
                            <li className="hover:text-white cursor-pointer">Contact</li>
                        </ul>
                    </div>

                    <div className="space-y-4">
                        <h4 className="text-2xl font-semibold">Game Info</h4>
                        <ul className="space-y-4 text-lg text-gray-300">

                            <li className="flex items-center gap-3">
                                <Target className="text-emerald-400" size={26} />
                                Numbers: 1–16
                            </li>

                            <li className="flex items-center gap-3">
                                <Trophy className="text-yellow-400" size={26} />
                                Prize Pool: 70%
                            </li>

                            <li className="flex items-center gap-3">
                                <Clock className="text-red-400" size={26} />
                                Deadline: Saturday 5 PM
                            </li>

                            <li className="flex items-center gap-3">
                                <Dice5 className="text-blue-400" size={26} />
                                Weekly Drawings
                            </li>

                        </ul>
                    </div>
                </div>

                <div className="w-full h-px bg-gray-700 mb-6"></div>

                <p className="text-center text-gray-500 text-lg">
                    © 2024 Dead Pigeons · Jerne IF. All rights reserved.
                </p>
            </div>
        </footer>
    );
};
