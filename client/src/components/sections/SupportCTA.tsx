import { MessageCircle } from "lucide-react";

export function SupportCTA() {
    return (
        <section className="w-full bg-[#0f2b5b] py-24 lg:py-32 overflow-hidden relative">

            <div className="absolute inset-0 opacity-5">
                <div className="absolute inset-0 bg-gradient-to-br from-[#e30613]/20 via-transparent to-[#e30613]/20" />
            </div>

            <div className="max-w-6xl mx-auto px-6 relative z-10 text-center">

                <h2 className="text-5xl sm:text-6xl lg:text-7xl font-black text-white tracking-tighter">
                    READY TO SUPPORT <br className="hidden sm:block" />
                    <span className="text-[#e30613]">JERNE IF?</span>
                </h2>

                <p className="mt-6 text-xl sm:text-2xl text-white/90 font-medium max-w-3xl mx-auto">
                    Join hundreds of supporters. Play Dead Pigeons, win big, and help our club grow — every week.
                </p>

                <div className="mt-12 flex flex-col sm:flex-row gap-6 justify-center items-center">

                    <button className="group px-10 py-5 rounded-full bg-[#e30613] hover:bg-[#c20510] text-white font-bold text-xl shadow-xl hover:shadow-2xl transition-all duration-300 flex items-center gap-3">
                        <MessageCircle size={28} />
                        Contact Admin Team
                    </button>

                    {/*<button className="px-10 py-5 rounded-full border-4 border-white text-white font-bold text-xl hover:bg-white hover:text-[#0f2b5b] transition-all duration-300">*/}
                    {/*    How It Works*/}
                    {/*</button>*/}
                </div>

                <p className="mt-10 text-lg text-white/80">
                    Questions? Deposits? Rules? Our admins reply within hours
                </p>

            </div>
        </section>
    );
}