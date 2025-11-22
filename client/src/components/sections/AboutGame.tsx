import {Wallet, Grid, RefreshCw, Trophy} from "lucide-react";

export function AboutGame(){
    return (
        <section id="about" className="w-full bg-slate-100 py-20 h-200 items-center text-center flex">
            <div className="max-w-6xl mx-auto px-4 flex flex-col items-center text-center gap-12">

                <div className="space-y-2">
                    <h2 className="text-6xl font-bold text-slate-800">
                        How It Works
                    </h2>
                    <p className="text-slate-600 text-lg max-w-2xl">
                        Four simple steps to support Jerne IF and win prizes
                    </p>
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-8 w-full">

                    <div
                        className="relative bg-white rounded-2xl shadow-md border border-slate-200 py-14 px-8 flex flex-col items-center text-center">
                        <Wallet size={64} className="text-green-600 mb-6"/>
                        <h3 className="text-2xl font-semibold mb-3">Add Balance</h3>
                        <p className="text-slate-500 text-base max-w-[240px]">
                            Top up your account balance via MobilePay to get started
                        </p>
                        <span
                            className="absolute top-4 right-4 bg-green-600 text-white font-bold text-base rounded-full w-10 h-10 flex items-center justify-center">
                            1
                        </span>
                    </div>

                    <div
                        className="relative bg-white rounded-2xl shadow-md border border-slate-200 py-14 px-8 flex flex-col items-center text-center">
                        <Grid size={64} className="text-green-600 mb-6"/>
                        <h3 className="text-2xl font-semibold mb-3">Pick Numbers</h3>
                        <p className="text-slate-500 text-base max-w-[240px]">
                            Select 5–8 numbers from 1–16 for your board
                        </p>
                        <span
                            className="absolute top-4 right-4 bg-green-600 text-white font-bold text-base rounded-full w-10 h-10 flex items-center justify-center">
                            2
                        </span>
                    </div>

                    <div
                        className="relative bg-white rounded-2xl shadow-md border border-slate-200 py-14 px-8 flex flex-col items-center text-center">
                        <RefreshCw size={64} className="text-green-600 mb-6"/>
                        <h3 className="text-2xl font-semibold mb-3">Play Weekly</h3>
                        <p className="text-slate-500 text-base max-w-[240px]">
                            Set your boards to repeat or play week by week
                        </p>
                        <span
                            className="absolute top-4 right-4 bg-green-600 text-white font-bold text-base rounded-full w-10 h-10 flex items-center justify-center">
                            3
                        </span>
                    </div>

                    <div
                        className="relative bg-white rounded-2xl shadow-md border border-slate-200 py-14 px-8 flex flex-col items-center text-center">
                        <Trophy size={64} className="text-green-600 mb-6"/>
                        <h3 className="text-2xl font-semibold mb-3">Win Prizes</h3>
                        <p className="text-slate-500 text-base max-w-[240px]">
                            Match the 3 winning numbers and share in 70% of the pot
                        </p>
                        <span
                            className="absolute top-4 right-4 bg-green-600 text-white font-bold text-base rounded-full w-10 h-10 flex items-center justify-center">
                            4
                        </span>
                    </div>
                </div>
            </div>
        </section>
    );
};
