import {TOKEN_KEY, tokenStorage} from "./atoms/token";
import{AuthClient} from "./core/generated-client";


const customFetch = async (url: RequestInfo, init?: RequestInit) => {
    const token = tokenStorage.getItem(TOKEN_KEY, null);

    if (token) {
        // Copy of existing init or new object, with copy of existing headers or
        // new object including Bearer token.
        init = {
            ...(init ?? {}),
            headers: {
                ...(init?.headers ?? {}),
                Authorization: `Bearer ${token}`,
            },
        };
    }
    return await fetch(url, init);
};

const baseUrl = undefined;
export const authClient = new AuthClient(baseUrl, { fetch: customFetch });