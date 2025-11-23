import {ArrowRight, Trophy, Users, TrendingUp, RefreshCw} from "lucide-react";
import { Link } from "react-router-dom";

export function Hero() {
    return (
        <section className="relative w-full bg-gradient-to-br from-[#0f2b5b] via-[#0a1f44] to-[#1e3a6b] overflow-hidden">

            <div className="pt-28 pb-20 lg:pt-36 lg:pb-28">
                <div className="max-w-7xl mx-auto px-6 lg:px-8 text-center">

                    <div className="space-y-6">
                        <h1 className="text-5xl sm:text-6xl md:text-7xl lg:text-8xl font-black tracking-tighter text-white">
                            <span className="inline-block animate-fade-up">Dead</span>{" "}
                            <span className="inline-block text-[#e30613] animate-fade-up animation-delay-200">
                Pigeons
              </span>
                        </h1>

                        <p className="text-2xl md:text-4xl font-bold text-white/90 animate-fade-up animation-delay-400">
                            Support <span className="text-[#e30613]">Jerne IF Esbjerg</span>
                        </p>

                        <p className="mt-6 text-lg md:text-xl text-white/70 max-w-3xl mx-auto font-medium animate-fade-up animation-delay-600">
                            Weekly lottery • Support your local sports club • Win exciting prizes every week

                        </p>
                    </div>

                    <div className="flex flex-col sm:flex-row gap-5 justify-center items-center mt-12">
                        <Link
                            to="/login"
                            className="group inline-flex items-center gap-3 px-10 py-5 rounded-full bg-[#e30613] hover:bg-[#ff1a2b] text-white font-bold text-lg shadow-2xl hover:shadow-[#e30613]/60 transition-all duration-300 transform hover:scale-105 animate-fade-up animation-delay-800"
                        >
                            Start Playing Now
                            <ArrowRight className="w-6 h-6 group-hover:translate-x-2 transition" />
                        </Link>

                        <a
                            href="#how-it-works"
                            className="px-10 py-5 rounded-full border-2 border-white/50 bg-white/5 backdrop-blur-md text-white font-semibold hover:bg-white/10 hover:border-white transition-all animate-fade-up animation-delay-1000"
                        >
                            How It Works
                        </a>
                    </div>

                    <div className="grid grid-cols-1 sm:grid-cols-4 gap-6 mt-20 max-w-5xl mx-auto">
                        <div className="bg-white/10 backdrop-blur-md border border-white/20 rounded-2xl p-8 hover:bg-white/15 hover:border-[#e30613]/50 transition-all duration-500 hover:scale-105 hover:-translate-y-2 animate-slide-up animation-delay-1200">
                            <Trophy className="w-14 h-14 mx-auto mb-4 text-[#e30613]" />
                            <p className="text-5xl font-black text-white">70%</p>
                            <p className="text-white/80 mt-2">Prize Pool</p>
                        </div>

                        <div className="bg-white/10 backdrop-blur-md border border-white/20 rounded-2xl p-8 hover:bg-white/15 hover:border-[#e30613]/50 transition-all duration-500 hover:scale-105 hover:-translate-y-2 animate-slide-up animation-delay-1400">
                            <Users className="w-14 h-14 mx-auto mb-4 text-[#e30613]" />
                            <p className="text-5xl font-black text-white">30%</p>
                            <p className="text-white/80 mt-2">To Jerne IF</p>
                        </div>

                        <div className="bg-white/10 backdrop-blur-md border border-white/20 rounded-2xl p-8 hover:bg-white/15 hover:border-[#e30613]/50 transition-all duration-500 hover:scale-105 hover:-translate-y-2 animate-slide-up animation-delay-1600">
                            <RefreshCw className="w-14 h-14 mx-auto mb-4 text-[#e30613]"/>
                            <p className="text-5xl font-black text-white">Sunday</p>
                            <p className="text-white/80 mt-2">Every Sunday Draw</p>
                        </div>


                        <div className="bg-white/10 backdrop-blur-md border border-white/20 rounded-2xl p-8 hover:bg-white/15 hover:border-[#e30613]/50 transition-all duration-500 hover:scale-105 hover:-translate-y-2 animate-slide-up animation-delay-1800">
                            <TrendingUp className="w-14 h-14 mx-auto mb-4 text-[#e30613]" />
                            <p className="text-5xl font-black text-white">3</p>
                            <p className="text-white/80 mt-2">Winning Numbers</p>
                        </div>

                    </div>
                </div>
            </div>
        </section>
    );
}

