import { Shield, Clock3, Users, TrendingUp } from "lucide-react";

export const WhyPlay = () => {
    return (
        <section className="w-full bg-white py-24 h-200 items-center text-center flex">
            <div className="max-w-6xl mx-auto px-4 flex flex-col items-center text-center gap-12">

                <div className="space-y-3">
                    <h2 className="text-5xl font-bold text-slate-900">
                        Why Play Dead Pigeons?
                    </h2>
                    <p className="text-slate-600 text-xl max-w-2xl">
                        A fun, secure, and community-focused way to support local sports
                    </p>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-8 w-full">

                    <div className="flex items-center gap-6 bg-slate-50 rounded-2xl border border-slate-200 shadow-sm px-8 py-10 text-left">
                        <div className="flex h-16 w-16 items-center justify-center rounded-full bg-blue-50">
                            <Shield className="text-blue-600" size={40} />
                        </div>
                        <div>
                            <h3 className="text-xl font-semibold text-slate-900">
                                Secure Balance System
                            </h3>
                            <p className="text-base text-slate-500 max-w-sm">
                                Your deposits are tracked and verified by admins before use.
                            </p>
                        </div>
                    </div>

                    <div className="flex items-center gap-6 bg-slate-50 rounded-2xl border border-slate-200 shadow-sm px-8 py-10 text-left">
                        <div className="flex h-16 w-16 items-center justify-center rounded-full bg-blue-50">
                            <Clock3 className="text-blue-600" size={40} />
                        </div>
                        <div>
                            <h3 className="text-xl font-semibold text-slate-900">
                                Weekly Drawings
                            </h3>
                            <p className="text-base text-slate-500 max-w-sm">
                                New winning numbers drawn every week – play until 5 PM Saturday.
                            </p>
                        </div>
                    </div>

                    <div className="flex items-center gap-6 bg-slate-50 rounded-2xl border border-slate-200 shadow-sm px-8 py-10 text-left">
                        <div className="flex h-16 w-16 items-center justify-center rounded-full bg-blue-50">
                            <Users className="text-blue-600" size={40} />
                        </div>
                        <div>
                            <h3 className="text-xl font-semibold text-slate-900">
                                Community Support
                            </h3>
                            <p className="text-base text-slate-500 max-w-sm">
                                30% of all proceeds go directly to supporting Jerne IF.
                            </p>
                        </div>
                    </div>

                    <div className="flex items-center gap-6 bg-slate-50 rounded-2xl border border-slate-200 shadow-sm px-8 py-10 text-left">
                        <div className="flex h-16 w-16 items-center justify-center rounded-full bg-blue-50">
                            <TrendingUp className="text-blue-600" size={40} />
                        </div>
                        <div>
                            <h3 className="text-xl font-semibold text-slate-900">
                                Flexible Pricing
                            </h3>
                            <p className="text-base text-slate-500 max-w-sm">
                                Choose 5–8 numbers with prices from 20 to 160 DKK.
                            </p>
                        </div>
                    </div>

                </div>
            </div>
        </section>
    );
};
