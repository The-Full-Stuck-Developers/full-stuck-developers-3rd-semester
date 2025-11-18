import {Outlet, useNavigate} from "react-router";
import {Navbar} from "@components/sections/NavBar.tsx";
import {Hero} from "@components/sections/Hero.tsx";
import {AboutGame} from "@components/sections/AboutGame.tsx";
import {WhyPlay} from "@components/sections/WhyPlay.tsx";
import {BoardsPricing} from "@components/sections/BoardsPricing.tsx";
import {SupportCTA} from "@components/sections/SupportCTA.tsx";
import {Footer} from "@components/sections/Footer.tsx";

export default function Home() {

    const navigate = useNavigate();

    return <>
        <Navbar/>
        <Hero/>
        <AboutGame/>
        <WhyPlay/>
        <BoardsPricing/>
        <SupportCTA/>
        <Footer/>
    </>
}