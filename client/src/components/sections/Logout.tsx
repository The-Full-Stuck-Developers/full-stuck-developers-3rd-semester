import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

export default function Logout() {
    const navigate = useNavigate();

    useEffect(() => {
        const doLogout = async () => {
            await fetch("/api/auth/logout", {
                method: "POST",
                credentials: "include"
            });

            localStorage.removeItem("token");
            navigate("/login", { replace: true });
        };

        doLogout();
    }, []);

    return null;
}
