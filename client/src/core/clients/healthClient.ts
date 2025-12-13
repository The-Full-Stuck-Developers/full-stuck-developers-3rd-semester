import {HealthClient} from "@core/generated-client.ts";
import {baseUrl} from "@core/baseUrl.ts";

const getHealthClient = () => {
    return new HealthClient(baseUrl, {
        fetch: async (url, init) => {
            init = init ?? {};
            init.headers = {
                ...(init.headers ?? {}),
                Authorization: `Bearer ${localStorage.getItem("token")}`,
            };
            return fetch(url, init);
        },
    });
}
export default getHealthClient;
