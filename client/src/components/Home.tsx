import {Navbar} from "@components/sections/NavBar.tsx";
import {Hero} from "@components/sections/Hero.tsx";
import {AboutGame} from "@components/sections/AboutGame.tsx";
import {WhyPlay} from "@components/sections/WhyPlay.tsx";
import {BoardsPricing} from "@components/sections/BoardsPricing.tsx";
import {SupportCTA} from "@components/sections/SupportCTA.tsx";
import {Footer} from "@components/sections/Footer.tsx";
import Login from "@components/sections/LoginSection/Login.tsx";
import {useState} from "react";
import {GameBoard} from "@components/sections/userSection/GameBoard.tsx";

export function Home() {
    const [isLoginOpen, setIsLoginOpen] = useState(false);
    return (
        <>
            <Navbar onLoginClick={() => setIsLoginOpen(true)}/>
            <Hero/>
            <AboutGame/>
            {/*<GameBoard/>*/}
            <WhyPlay/>
            <BoardsPricing/>
            <SupportCTA/>
            <Footer/>

            {/* Login Modal */}
            <Login
                isOpen={isLoginOpen}
                onClose={() => setIsLoginOpen(false)}
            />
        </>

    )
}
