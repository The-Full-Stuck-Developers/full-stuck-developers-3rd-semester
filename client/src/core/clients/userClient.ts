import {baseUrl} from "@core/baseUrl.ts";
import {UsersClient} from "@core/generated-client.ts";

const getUserClient = () => {
    return new UsersClient(baseUrl, {
        fetch: async (url, init) => {
            init = init ?? {};
            init.headers = {
                ...(init.headers ?? {}),
                Authorization: `Bearer ${localStorage.getItem("token")}`,
            };
            return fetch(url, init);
        },
    });
};

export default getUserClient;
