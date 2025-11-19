import { Link } from "react-router-dom";

export function Navbar() {
    return (
        <nav className="absolute top-3 left-0 w-full z-50 backdrop-blur-sm">
            <div className="max-w-8xl mx-auto flex items-center justify-between px-4 py-4">

                <Link to="/" className="text-xl font-bold text-verde text-white">
                    DeadPigeons
                </Link>

                <div className="flex-1 flex justify-center">
                    <div className="flex items-center gap-8 text-lg text-white font-medium">
                        <a href="#about" className="hover:text-verde transition">
                            About
                        </a>
                        <a href="#pricing" className="hover:text-verde transition">
                            Pricing
                        </a>
                        <a href="#contact" className="hover:text-verde transition">
                            Contact
                        </a>
                    </div>
                </div>
                <Link to="/login" className="hover:text-verde text-lg text-white border-1 rounded-md p-1 w-20 text-center">
                    Login
                </Link>

            </div>
        </nav>
    );
};
