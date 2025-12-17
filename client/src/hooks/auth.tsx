import { useNavigate } from "react-router-dom";
import { useAtom } from "jotai";
import { tokenAtom, userInfoAtom } from "../atoms/token";
import type { AuthUserInfo, LoginRequestDto } from "../core/generated-client";
import { authClient } from "../api-clients.ts";

type AuthHook = {
  user: AuthUserInfo | null;
  isAuthenticated: boolean;
  login: (request: LoginRequestDto) => Promise<void>;
  logout: () => void;
};

export const useAuth = () => {
  const [jwt, setJwt] = useAtom(tokenAtom);
  const [user] = useAtom(userInfoAtom);
  const navigate = useNavigate();

  const login = async (request: LoginRequestDto) => {
    try {
      const response = await authClient.login(request);
      setJwt(response.jwt!);

      const userInfo = await authClient.userInfo();

      if (userInfo.isAdmin) navigate("/admin/dashboard");
      else navigate("/player/dashboard");
    } catch (err) {
      console.log("login error (raw):", err);
      throw err;
    }
  };


  const logout = async () => {
    setJwt(null);
    navigate("/");
  };

  return {
    user,
    isAuthenticated: !!user,
    login,
    logout,
  } as AuthHook;
};
