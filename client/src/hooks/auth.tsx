import {useNavigate} from "react-router-dom";
import {useAtom} from "jotai";
import {tokenAtom, userInfoAtom} from "../atoms/token";
import type {AuthUserInfo, LoginRequest} from "../core/generated-client";
import {authClient} from "../api-clients.ts";

type AuthHook = {
    user: AuthUserInfo | null;
    login: (request: LoginRequest) => Promise<void>;
    logout: () => void;
};

export const useAuth = () => {
    const [_, setJwt] = useAtom(tokenAtom);
    const [user] = useAtom(userInfoAtom);
    const navigate = useNavigate();

    const login = async (request: LoginRequest) => {
        const response = await authClient.login(request);
        setJwt(response.jwt!);
        // Fetch user info to check if admin
        const userInfo = await authClient.userInfo();

        // Redirect based on admin status
        if (userInfo.isAdmin) {
            navigate("/admin/users");
        } else {
            navigate("/user/dashboard");
        }
    }

    const logout = async () => {
        setJwt(null);
        navigate("/");
    }

    return {
        user,
        login,
        logout,
    } as AuthHook;
};
