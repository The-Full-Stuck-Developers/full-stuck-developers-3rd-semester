import { useAtom } from "jotai";
import { loadable } from "jotai/utils";
import { userInfoAtom } from "../atoms/token";

const userInfoLoadableAtom = loadable(userInfoAtom);

export const useAuthInfo = () => {
  const [user] = useAtom(userInfoLoadableAtom);

  return {
    user: user.state === "hasData" ? user.data : null,
    isLoading: user.state === "loading",
    isAuthenticated: user.state === "hasData" && !!user.data,
    error: user.state === "hasError" ? user.error : null,
  };
};
