import { baseUrl } from "@core/baseUrl.ts";
import { UsersClient } from "@core/generated-client.ts";
import { TOKEN_KEY, tokenStorage, useToken } from "../../atoms/token.ts";

const getUserClient = () => {
  return new UsersClient(baseUrl, {
    fetch: async (url, init) => {
      const token = tokenStorage.getItem(TOKEN_KEY, null);

      init = init ?? {};
      init.headers = {
        ...(init.headers ?? {}),
        Authorization: `Bearer ${token}`,
      };
      return fetch(url, init);
    },
  });
};

export default getUserClient;
