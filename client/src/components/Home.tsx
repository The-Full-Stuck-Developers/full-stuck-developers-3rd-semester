import {Outlet, useNavigate} from "react-router";

export default function Home() {

    const navigate = useNavigate();

    return <>
        
        <Outlet/>

    </>
}