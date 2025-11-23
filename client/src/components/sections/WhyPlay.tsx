import { Shield, Clock3, Users, TrendingUp } from "lucide-react";

export function WhyPlay() {
    return (
        <section className="w-full bg-gray-50 py-16 lg:py-20">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">

                <div className="text-center mb-12 lg:mb-16">
                    <h2 className="text-4xl sm:text-5xl lg:text-6xl font-black text-[#0f2b5b] tracking-tighter">
                        WHY PLAY DEAD PIGEONS?
                    </h2>
                    <p className="mt-4 text-lg sm:text-xl text-gray-700 max-w-3xl mx-auto font-semibold">
                        A fun, secure, and community-driven way to support Jerne IF Esbjerg
                    </p>
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-6 lg:gap-8">

                    <div className="group relative bg-white rounded-2xl border-2 border-[#0f2b5b]/10 shadow-lg hover:shadow-2xl hover:border-[#e30613] transition-all duration-300 overflow-hidden">
                        <div className="p-8 text-left">
                            <div className="flex items-center justify-center w-16 h-16 rounded-full bg-[#e30613]/10 mb-6 group-hover:bg-[#e30613] group-hover:text-white transition-colors">
                                <Shield className="w-9 h-9 text-[#e30613] group-hover:text-white" />
                            </div>
                            <h3 className="text-xl font-bold text-[#0f2b5b] mb-3">
                                Secure Balance System
                            </h3>
                            <p className="text-gray-600 leading-relaxed">
                                Your deposits are tracked and verified by admins before use.
                            </p>
                        </div>
                    </div>

                    <div className="group relative bg-white rounded-2xl border-2 border-[#0f2b5b]/10 shadow-lg hover:shadow-2xl hover:border-[#e30613] transition-all duration-300 overflow-hidden">
                        <div className="p-8 text-left">
                            <div className="flex items-center justify-center w-16 h-16 rounded-full bg-[#e30613]/10 mb-6 group-hover:bg-[#e30613] group-hover:text-white transition-colors">
                                <Clock3 className="w-9 h-9 text-[#e30613] group-hover:text-white" />
                            </div>
                            <h3 className="text-xl font-bold text-[#0f2b5b] mb-3">
                                Weekly Drawings
                            </h3>
                            <p className="text-gray-600 leading-relaxed">
                                New winning numbers every Saturday at 5 PM – never miss a draw.
                            </p>
                        </div>
                    </div>

                    <div className="group relative bg-white rounded-2xl border-2 border-[#0f2b5b]/10 shadow-lg hover:shadow-2xl hover:border-[#e30613] transition-all duration-300 overflow-hidden">
                        <div className="p-8 text-left">
                            <div className="flex items-center justify-center w-16 h-16 rounded-full bg-[#e30613]/10 mb-6 group-hover:bg-[#e30613] group-hover:text-white transition-colors">
                                <Users className="w-9 h-9 text-[#e30613] group-hover:text-white" />
                            </div>
                            <h3 className="text-xl font-bold text-[#0f2b5b] mb-3">
                                Community First
                            </h3>
                            <p className="text-gray-600 leading-relaxed">
                                30% of all proceeds go directly to Jerne IF youth and senior teams.
                            </p>
                        </div>
                    </div>

                    <div className="group relative bg-white rounded-2xl border-2 border-[#0f2b5b]/10 shadow-lg hover:shadow-2xl hover:border-[#e30613] transition-all duration-300 overflow-hidden">
                        <div className="p-8 text-left">
                            <div className="flex items-center justify-center w-16 h-16 rounded-full bg-[#e30613]/10 mb-6 group-hover:bg-[#e30613] group-hover:text-white transition-colors">
                                <TrendingUp className="w-9 h-9 text-[#e30613] group-hover:text-white" />
                            </div>
                            <h3 className="text-xl font-bold text-[#0f2b5b] mb-3">
                                Flexible & Fair Pricing
                            </h3>
                            <p className="text-gray-600 leading-relaxed">
                                Pick 5–8 numbers. Prices from just 20 kr up to 120 kr per row.
                            </p>
                        </div>
                    </div>

                </div>
            </div>
        </section>
    );
}