import { atom } from "jotai";
import { atomWithStorage, createJSONStorage } from "jotai/utils";
import { authClient } from "../api-clients";

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
    const userInfo = await authClient.userInfo();
    return userInfo;
});