import { Link } from "react-router-dom";
import { MessageCircle } from "lucide-react";
import Logo from "../../jerneif-logo.png";

interface NavbarProps {
  onLoginClick: () => void;
}

export function Navbar({ onLoginClick }: NavbarProps) {
  return (
    <nav className="fixed top-0 left-0 right-0 z-50 bg-[#0f2b5b]/95 backdrop-blur-lg border-b border-white/10">
      <div className="max-w-7xl mx-auto px-6 py-5">
        <div className="flex items-center justify-between">
          <Link to="/" className="flex items-center gap-4 group">
            <div className="w-12 h-12 rounded-full bg-white/10 border-2 border-dashed border-white/30 flex items-center justify-center overflow-hidden">
              <img
                src={Logo}
                alt="Jerne IF Logo"
                className="w-full h-full object-cover"
                onError={(e) => {
                  e.currentTarget.style.display = "none";
                }}
              />
              <span className="text-2xl hidden">Dead Pigeon</span>
            </div>

            <span className="text-2xl font-black text-white tracking-tight">
              Jerne IF
            </span>
          </Link>

          <div className="hidden lg:flex items-center gap-10 text-white font-semibold text-lg">
            <a href="#about" className="hover:text-[#e30613] transition">
              About
            </a>
            <a href="#pricing" className="hover:text-[#e30613] transition">
              Pricing
            </a>
            <a href="#contact" className="hover:text-[#e30613] transition">
              Contact
            </a>
          </div>

          <div className="flex items-center gap-4">
            <button
              onClick={onLoginClick}
              className="px-6 py-3 rounded-full border-2 border-white/30 text-white font-semibold hover:bg-white/10 hover:border-white transition-all"
            >
              Login
            </button>

            <a
              href="#contact"
              className="flex items-center gap-2 px-6 py-3 rounded-full bg-[#e30613] hover:bg-[#c20510] text-white font-bold shadow-lg hover:shadow-xl transition-all duration-300"
            >
              <MessageCircle size={20} />
              <span className="hidden sm:inline">Contact Admin</span>
              <span className="sm:hidden">Chat</span>
            </a>
          </div>
        </div>
      </div>
    </nav>
  );
}
