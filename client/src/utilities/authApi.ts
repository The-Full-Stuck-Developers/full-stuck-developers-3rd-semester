import {AuthClient} from "@core/generated-client.ts";
import {baseUrl} from "@core/baseUrl.ts";
import {customFetch} from "@utilities/customFetch.ts";


export const authApi = new AuthClient(baseUrl, customFetch);
