export function SupportCTA()  {
    return (
        <section className="w-full bg-gradient-to-r from-emerald-600 via-emerald-500 to-sky-600 text-white py-32">
            <div className="max-w-6xl mx-auto px-6 flex flex-col items-center text-center gap-12">

                <div className="space-y-4">
                    <h2 className="text-5xl md:text-6xl font-extrabold">
                        Ready to Support Jerne IF?
                    </h2>

                    <p className="text-xl md:text-2xl text-white/90 max-w-3xl mx-auto">
                        Join our community of supporters and get your chance to win while helping local sports thrive
                    </p>
                </div>

                <div className="flex flex-col sm:flex-row gap-6 mt-2">
                    <button className="px-10 py-4 rounded-full border-2 border-white text-white font-semibold text-lg hover:bg-white/10 transition">
                        Contact Admin
                    </button>
                </div>

                <p className="text-lg text-white/85 mt-2">
                    Questions? Reach out to our admin team for assistance
                </p>
            </div>
        </section>
    );
};
