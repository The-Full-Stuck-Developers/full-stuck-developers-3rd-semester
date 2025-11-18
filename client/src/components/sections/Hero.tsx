export const Hero = () => {
    return (
        <section className="w-full h-300 bg-gradient-to-b from-emerald-700 via-sky-700 to-slate-900 text-white py-20 items-center text-center flex">
            <div className="max-w-5xl mx-auto px-4 flex flex-col items-center text-center gap-10 ">

                <div className="space-y-4">
                    <p className="text-6xl md:text-7xl lg:text-8xl font-extrabold tracking-tight">
                        <span className="mr-3">🕊️</span>
                        Dead Pigeons
                    </p>
                    <p className="text-xl md:text-3xl text-white/80 font-medium">
                        Support Jerne IF through our exciting lottery game
                    </p>
                    <p className="text-md md:text-base md:text-lg text-white/70 max-w-2xl mx-auto">
                        Pick your numbers, support your local sports club, and win exciting prizes every week!
                    </p>
                </div>

                <div className="flex flex-col sm:flex-row gap-4">
                    <button
                        className="px-11 py-4 rounded-full text-base font-semibold bg-emerald-400 hover:bg-emerald-300 text-slate-900 shadow-lg transition"
                    >
                        Start Playing
                    </button>
                    <button
                        className="px-11 py-4 rounded-full text-base font-semibold border border-white/60 bg-white/10 hover:bg-white/20 backdrop-blur-sm transition"
                    >
                        Learn More
                    </button>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-3 gap-4 w-full mt-6">
                    <div className="rounded-3xl bg-white/10 border border-white/10 py-10 px-8 backdrop-blur-sm">
                        <p className="text-4xl font-bold">70%</p>
                        <p className="mt-1 text-sm text-white/80">Goes to Prizes</p>
                    </div>
                    <div className="rounded-3xl bg-white/10 border border-white/10 py-10 px-8 backdrop-blur-sm">
                        <p className="text-4xl font-bold">1–16</p>
                        <p className="mt-1 text-sm text-white/80">Number Range</p>
                    </div>
                    <div className="rounded-3xl bg-white/10 border border-white/10 py-10 px-8 backdrop-blur-sm">
                        <p className="text-4xl font-bold">3</p>
                        <p className="mt-1 text-sm text-white/80">Winning Numbers</p>
                    </div>
                </div>
            </div>
        </section>
    );
};
