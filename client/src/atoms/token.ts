import { atom } from "jotai";
import { atomWithStorage, createJSONStorage } from "jotai/utils";
import { authClient } from "../api-clients";
import { AuthClient } from "../core/generated-client";
import { useAtom } from "jotai";

// Storage key for JWT
export const TOKEN_KEY = "token";
export const tokenStorage = createJSONStorage<string | null>(
  () => sessionStorage,
);

export const tokenAtom = atomWithStorage<string | null>(
  TOKEN_KEY,
  null,
  tokenStorage,
);

export const userInfoAtom = atom(async (get) => {
  // Create a dependency on 'token' atom
  const token = get(tokenAtom);
  if (!token) return null;
  // Fetch user-info
  return await authClient.userInfo();
});

export const useToken = () => {
  const [token, setToken] = useAtom(tokenAtom);

  const clearToken = () => setToken(null);

  return { token, setToken, clearToken };
};
